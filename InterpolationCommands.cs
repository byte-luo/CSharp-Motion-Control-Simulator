using System;
using System.Collections.Generic;
using System.Linq;

namespace LQF_PolishingSystem
{
    /// <summary>
    /// 多轴插补命令抽象基类。
    /// 每个 tick 调用 Step(dt) 返回各轴的增量位移。
    /// </summary>
    public abstract class InterpolationCommand
    {
        public int[] AxisNumbers { get; protected set; } = Array.Empty<int>();
        public bool IsCompleted { get; protected set; }

        /// <summary>
        /// 同步轴：命令执行期间以恒定速度运行的轴（如 C 轴抛光轮）
        /// 轴号 → 速度（units/s）
        /// </summary>
        public Dictionary<int, double> SyncAxes { get; protected set; } = new();

        /// <summary>
        /// 每一步调用，返回各轴位移增量
        /// </summary>
        /// <param name="dt">时间步长（秒）</param>
        /// <returns>轴号 → 增量</returns>
        public abstract Dictionary<int, double> Step(double dt);

        public virtual void OnStart() { }
        public virtual void OnComplete() { }
    }

    /// <summary>
    /// 直线插补命令 —— DDA 比例法
    /// </summary>
    public class LinearCommand : InterpolationCommand
    {
        private readonly Dictionary<int, double> _targetDeltas;
        private readonly double _totalPathLength;
        private readonly double _totalTime;
        private double _elapsed;

        public LinearCommand(Dictionary<int, double> targetDeltas, double feedRate)
        {
            _targetDeltas = new Dictionary<int, double>(targetDeltas);
            AxisNumbers = _targetDeltas.Keys.ToArray();

            // 合成路径长度 = sqrt(Σ delta²)
            _totalPathLength = Math.Sqrt(_targetDeltas.Values.Sum(d => d * d));

            if (_totalPathLength < 0.001 || feedRate <= 0)
            {
                IsCompleted = true;
                _totalTime = 0;
            }
            else
            {
                _totalTime = _totalPathLength / feedRate;
            }
        }

        public override Dictionary<int, double> Step(double dt)
        {
            var result = new Dictionary<int, double>();
            if (IsCompleted) return result;

            _elapsed += dt;

            double progress;
            if (_elapsed >= _totalTime)
            {
                progress = 1.0;
                IsCompleted = true;
            }
            else
            {
                progress = _elapsed / _totalTime;
            }

            // 当前 tick 的进度增量
            double prevProgress = Math.Max(0, (_elapsed - dt) / _totalTime);
            double deltaRatio = progress - prevProgress;

            foreach (var kv in _targetDeltas)
            {
                result[kv.Key] = kv.Value * deltaRatio;
            }

            return result;
        }
    }

    /// <summary>
    /// 圆弧插补命令 —— 参数方程 + 微线段逼近
    /// </summary>
    public class ArcCommand : InterpolationCommand
    {
        private readonly double _cx, _cy, _radius;
        private readonly double _sweepAngle;   // 总扫描角（弧度，正数）
        private readonly int _angleSign;       // 1 = 逆时针, -1 = 顺时针
        private readonly double _targetAngle;  // 终点角度（弧度）
        private readonly double _totalTime;
        private double _currentAngle;          // 当前弧度
        private double _elapsed;
        private readonly int _xAxis, _yAxis;

        public ArcCommand(double cx, double cy, double radius,
                          double startAngleDeg, double endAngleDeg,
                          bool clockwise, double feedRate,
                          int xAxisNum = 1, int yAxisNum = 2)
        {
            _cx = cx;
            _cy = cy;
            _radius = radius;
            _xAxis = xAxisNum;
            _yAxis = yAxisNum;
            AxisNumbers = new[] { xAxisNum, yAxisNum };

            double startRad = startAngleDeg * Math.PI / 180.0;
            double endRad = endAngleDeg * Math.PI / 180.0;
            _currentAngle = startRad;

            if (clockwise)
            {
                _angleSign = -1;
                double diff = startRad - endRad;
                if (diff <= 0) diff += 2 * Math.PI;
                _sweepAngle = diff;
            }
            else
            {
                _angleSign = 1;
                double diff = endRad - startRad;
                if (diff <= 0) diff += 2 * Math.PI;
                _sweepAngle = diff;
            }

            _targetAngle = startRad + _angleSign * _sweepAngle;

            double pathLength = radius * _sweepAngle;

            if (pathLength < 0.001 || feedRate <= 0)
            {
                IsCompleted = true;
                _totalTime = 0;
            }
            else
            {
                _totalTime = pathLength / feedRate;
            }
        }

        public override Dictionary<int, double> Step(double dt)
        {
            var result = new Dictionary<int, double>();
            if (IsCompleted) return result;

            double prevAngle = _currentAngle;
            _elapsed += dt;

            if (_elapsed >= _totalTime)
            {
                IsCompleted = true;
                _currentAngle = _targetAngle;
            }
            else
            {
                _currentAngle += _angleSign * _sweepAngle * (dt / _totalTime);
            }

            double prevX = _cx + _radius * Math.Cos(prevAngle);
            double prevY = _cy + _radius * Math.Sin(prevAngle);
            double newX = _cx + _radius * Math.Cos(_currentAngle);
            double newY = _cy + _radius * Math.Sin(_currentAngle);

            result[_xAxis] = newX - prevX;
            result[_yAxis] = newY - prevY;

            return result;
        }
    }
}
