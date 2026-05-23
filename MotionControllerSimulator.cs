using System;
using System.Collections.Generic;
using System.Threading;

namespace LQF_PolishingSystem
{
    public enum AxisStatus
    {
        Idle = 0,
        Running = 1,
        Stopping = 2,
        Error = 3
    }

    public class AxisData
    {
        public int AxisNum { get; set; }
        public double CurrentPosition { get; set; }
        public double TargetPosition { get; set; }
        public double Velocity { get; set; }
        public double MaxVelocity { get; set; }
        public double Acceleration { get; set; }
        public bool Enabled { get; set; }
        public AxisStatus Status { get; set; }
        
        public AxisData(int axisNum)
        {
            AxisNum = axisNum;
            CurrentPosition = 0;
            TargetPosition = 0;
            Velocity = 0;
            MaxVelocity = 100;
            Acceleration = 10;
            Enabled = false;
            Status = AxisStatus.Idle;
        }
    }

    public class MotionControllerSimulator
    {
        private static MotionControllerSimulator _instance;
        private Dictionary<int, AxisData> _axes;
        private Timer _motionTimer;
        private const int UPDATE_INTERVAL = 30;

        private Queue<InterpolationCommand> _commandQueue = new();
        private InterpolationCommand _activeCommand;
        private readonly object _commandLock = new();
        private HashSet<int> _disabledCommandAxes = new();

        public static MotionControllerSimulator Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new MotionControllerSimulator();
                return _instance;
            }
        }

        private MotionControllerSimulator()
        {
            _axes = new Dictionary<int, AxisData>();
            InitializeAxes();
            StartMotionTimer();
        }

        private void InitializeAxes()
        {
            int[] axisNumbers = { 1, 2, 4, 5, 6, 7 };
            foreach (int num in axisNumbers)
            {
                _axes[num] = new AxisData(num);
            }
        }

        private void StartMotionTimer()
        {
            _motionTimer = new Timer(MotionUpdateCallback, null, 0, UPDATE_INTERVAL);
        }

        private void MotionUpdateCallback(object state)
        {
            double dt = UPDATE_INTERVAL / 1000.0;
            var commandedAxes = new HashSet<int>();

            lock (_commandLock)
            {
                // 消费活跃插补命令
                if (_activeCommand != null)
                {
                    if (_activeCommand.IsCompleted)
                    {
                        _activeCommand.OnComplete();
                        _disabledCommandAxes.Clear();
                        _activeCommand = DequeueNext();
                        _activeCommand?.OnStart();
                    }
                }
                else
                {
                    _activeCommand = DequeueNext();
                    _activeCommand?.OnStart();
                }

                if (_activeCommand != null && !_activeCommand.IsCompleted)
                {
                    var deltas = _activeCommand.Step(dt);
                    lock (_axes)
                    {
                        foreach (var kv in deltas)
                        {
                            if (_disabledCommandAxes.Contains(kv.Key))
                                continue;

                            if (_axes.TryGetValue(kv.Key, out var axis))
                            {
                                axis.CurrentPosition += kv.Value;
                                axis.Velocity = kv.Value / dt;
                                axis.Status = AxisStatus.Running;
                                axis.Enabled = true;
                                commandedAxes.Add(kv.Key);
                            }
                        }

                        // 处理同步轴，跳过已摘除的
                        foreach (var sync in _activeCommand.SyncAxes)
                        {
                            if (_disabledCommandAxes.Contains(sync.Key))
                                continue;

                            if (_axes.TryGetValue(sync.Key, out var axis))
                            {
                                axis.CurrentPosition += sync.Value * dt;
                                axis.Velocity = sync.Value;
                                axis.Status = AxisStatus.Running;
                                axis.Enabled = true;
                                commandedAxes.Add(sync.Key);
                            }
                        }
                    }

                    if (_activeCommand.IsCompleted)
                    {
                        var completedCmd = _activeCommand;
                        completedCmd.OnComplete();
                        SetAxesIdle(completedCmd);
                        _disabledCommandAxes.Clear();
                        _activeCommand = DequeueNext();
                        _activeCommand?.OnStart();
                    }
                    else
                    {
                        // 所有轴都被摘除 → 取消命令
                        bool allDisabled = true;
                        foreach (var num in _activeCommand.AxisNumbers)
                        {
                            if (!_disabledCommandAxes.Contains(num))
                            { allDisabled = false; break; }
                        }
                        if (allDisabled)
                        {
                            foreach (var sync in _activeCommand.SyncAxes)
                            {
                                if (!_disabledCommandAxes.Contains(sync.Key))
                                { allDisabled = false; break; }
                            }
                        }
                        if (allDisabled)
                        {
                            var deadCmd = _activeCommand;
                            deadCmd.OnComplete();
                            _activeCommand = null;
                            _commandQueue.Clear();
                            _disabledCommandAxes.Clear();
                        }
                    }
                }
            }

            // 处理未被插补命令占用的、由单轴 API 启动的运动
            lock (_axes)
            {
                foreach (var axis in _axes.Values)
                {
                    if (!commandedAxes.Contains(axis.AxisNum)
                        && axis.Enabled
                        && axis.Status == AxisStatus.Running)
                    {
                        UpdateAxisPosition(axis);
                    }
                }
            }
        }

        private InterpolationCommand DequeueNext()
        {
            return _commandQueue.Count > 0 ? _commandQueue.Dequeue() : null;
        }

        private void SetAxesIdle(InterpolationCommand cmd)
        {
            if (cmd == null) return;
            lock (_axes)
            {
                foreach (var num in cmd.AxisNumbers)
                {
                    if (_axes.TryGetValue(num, out var ax))
                    {
                        ax.Status = AxisStatus.Idle;
                        ax.Velocity = 0;
                    }
                }
                foreach (var sync in cmd.SyncAxes)
                {
                    if (_axes.TryGetValue(sync.Key, out var ax))
                    {
                        ax.Status = AxisStatus.Idle;
                        ax.Velocity = 0;
                    }
                }
            }
        }

        private void UpdateAxisPosition(AxisData axis)
        {
            double distance = axis.TargetPosition - axis.CurrentPosition;
            
            if (Math.Abs(distance) < 0.1)
            {
                axis.CurrentPosition = axis.TargetPosition;
                axis.Velocity = 0;
                axis.Status = AxisStatus.Idle;
                return;
            }

            int direction = distance > 0 ? 1 : -1;
            double stepVelocity = axis.MaxVelocity * direction;
            double newPosition = axis.CurrentPosition + stepVelocity * (UPDATE_INTERVAL / 1000.0);
            
            if ((direction > 0 && newPosition >= axis.TargetPosition) ||
                (direction < 0 && newPosition <= axis.TargetPosition))
            {
                axis.CurrentPosition = axis.TargetPosition;
                axis.Velocity = 0;
                axis.Status = AxisStatus.Idle;
            }
            else
            {
                axis.CurrentPosition = newPosition;
                axis.Velocity = stepVelocity;
            }
        }

        public int WJ_Open(int num_scom)
        {
            return 0;
        }

        public int WJ_Close()
        {
            CancelAllCommands();
            lock (_axes)
            {
                foreach (var axis in _axes.Values)
                {
                    axis.Enabled = false;
                    axis.Status = AxisStatus.Idle;
                }
            }
            return 0;
        }

        public int WJ_Get_Axis_Status(int axisNum, ref int pValue)
        {
            lock (_axes)
            {
                if (_axes.ContainsKey(axisNum))
                {
                    pValue = (int)_axes[axisNum].Status;
                    return 0;
                }
            }
            return -1;
        }

        public int WJ_Get_Axis_Pulses(int axisNum, ref int pValue)
        {
            lock (_axes)
            {
                if (_axes.ContainsKey(axisNum))
                {
                    pValue = (int)_axes[axisNum].CurrentPosition;
                    return 0;
                }
            }
            return -1;
        }

        public int WJ_Move_Axis_Vel(int axisNum, int velocity)
        {
            lock (_axes)
            {
                if (!_axes.ContainsKey(axisNum))
                    return -1;

                var axis = _axes[axisNum];

                if (velocity == 0)
                {
                    axis.Status = AxisStatus.Idle;
                    axis.Velocity = 0;
                }
                else
                {
                    axis.Enabled = true;
                    axis.MaxVelocity = Math.Abs(velocity);
                    axis.TargetPosition = axis.CurrentPosition + velocity * 1000;
                    axis.Status = AxisStatus.Running;
                }
            }

            AbortCommandForAxis(axisNum);
            return 0;
        }

        public int WJ_Move_Axis_Emergency_Stop(int axisNum)
        {
            lock (_axes)
            {
                if (_axes.ContainsKey(axisNum))
                {
                    var axis = _axes[axisNum];
                    axis.Status = AxisStatus.Idle;
                    axis.Velocity = 0;
                    axis.TargetPosition = axis.CurrentPosition;
                }
            }

            DetachAxisFromActiveCommand(axisNum);
            return 0;
        }

        public int WJ_Move_Axis_Slow_Stop(int axisNum)
        {
            lock (_axes)
            {
                if (_axes.ContainsKey(axisNum))
                {
                    var axis = _axes[axisNum];
                    axis.Status = AxisStatus.Stopping;
                }
            }

            DetachAxisFromActiveCommand(axisNum);
            return 0;
        }

        /// <summary>
        /// 从活跃插补命令中摘除指定轴（用于急停/缓停），
        /// 不取消整个命令。只有当所有轴都被摘除时才取消命令。
        /// </summary>
        private void DetachAxisFromActiveCommand(int axisNum)
        {
            lock (_commandLock)
            {
                if (_activeCommand != null && !_activeCommand.IsCompleted)
                {
                    if (Array.IndexOf(_activeCommand.AxisNumbers, axisNum) >= 0 ||
                        _activeCommand.SyncAxes.ContainsKey(axisNum))
                    {
                        _disabledCommandAxes.Add(axisNum);
                    }
                }
            }
        }

        private void AbortCommandForAxis(int axisNum)
        {
            InterpolationCommand abortedCmd = null;
            lock (_commandLock)
            {
                if (_activeCommand != null && !_activeCommand.IsCompleted)
                {
                    if (Array.IndexOf(_activeCommand.AxisNumbers, axisNum) >= 0 ||
                        _activeCommand.SyncAxes.ContainsKey(axisNum))
                    {
                        abortedCmd = _activeCommand;
                        _activeCommand = null;
                        _commandQueue.Clear();
                        _disabledCommandAxes.Clear();
                    }
                }
            }

            if (abortedCmd != null)
            {
                abortedCmd.OnComplete();
                SetAxesIdle(abortedCmd);
            }
        }

        public int WJ_Set_Axis_Vel(int axisNum, int value)
        {
            if (_axes.ContainsKey(axisNum))
            {
                _axes[axisNum].MaxVelocity = Math.Abs(value);
                return 0;
            }
            return -1;
        }

        public int WJ_Move_Axis_Pulses(int axisNum, int pulses)
        {
            lock (_axes)
            {
                if (!_axes.ContainsKey(axisNum))
                    return -1;

                var axis = _axes[axisNum];
                axis.Enabled = true;
                axis.TargetPosition = axis.CurrentPosition + pulses;
                axis.Status = AxisStatus.Running;
            }

            AbortCommandForAxis(axisNum);
            return 0;
        }

        public double GetAxisPosition(int axisNum)
        {
            if (_axes.ContainsKey(axisNum))
                return _axes[axisNum].CurrentPosition;
            return 0;
        }

        public double GetAxisVelocity(int axisNum)
        {
            if (_axes.ContainsKey(axisNum))
                return _axes[axisNum].Velocity;
            return 0;
        }

        public bool IsAxisEnabled(int axisNum)
        {
            if (_axes.ContainsKey(axisNum))
                return _axes[axisNum].Enabled;
            return false;
        }

        public void SetAxisEnabled(int axisNum, bool enabled)
        {
            if (_axes.ContainsKey(axisNum))
                _axes[axisNum].Enabled = enabled;
        }

        /// <summary>
        /// 清空队列并立即执行新命令
        /// </summary>
        public void ExecuteCommand(InterpolationCommand command)
        {
            InterpolationCommand oldCmd = null;
            lock (_commandLock)
            {
                _commandQueue.Clear();
                if (_activeCommand != null && !_activeCommand.IsCompleted)
                {
                    oldCmd = _activeCommand;
                }
                _activeCommand = null;
                _disabledCommandAxes.Clear();
                _commandQueue.Enqueue(command);
            }

            oldCmd?.OnComplete();
            SetAxesIdle(oldCmd);
        }

        /// <summary>
        /// 追加命令到队列末尾
        /// </summary>
        public void EnqueueCommand(InterpolationCommand command)
        {
            lock (_commandLock)
            {
                _commandQueue.Enqueue(command);
            }
        }

        /// <summary>
        /// 取消当前及队列中所有命令
        /// </summary>
        public void CancelAllCommands()
        {
            InterpolationCommand oldCmd = null;
            lock (_commandLock)
            {
                if (_activeCommand != null && !_activeCommand.IsCompleted)
                {
                    oldCmd = _activeCommand;
                }
                _activeCommand = null;
                _commandQueue.Clear();
                _disabledCommandAxes.Clear();
            }

            oldCmd?.OnComplete();
            SetAxesIdle(oldCmd);
        }

        public int CommandQueueCount
        {
            get
            {
                lock (_commandLock)
                {
                    return _commandQueue.Count + (_activeCommand != null && !_activeCommand.IsCompleted ? 1 : 0);
                }
            }
        }
    }
}
