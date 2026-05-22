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
            lock (_axes)
            {
                foreach (var axis in _axes.Values)
                {
                    if (axis.Enabled && axis.Status == AxisStatus.Running)
                    {
                        UpdateAxisPosition(axis);
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
            foreach (var axis in _axes.Values)
            {
                axis.Enabled = false;
                axis.Status = AxisStatus.Idle;
            }
            return 0;
        }

        public int WJ_Get_Axis_Status(int axisNum, ref int pValue)
        {
            if (_axes.ContainsKey(axisNum))
            {
                pValue = (int)_axes[axisNum].Status;
                return 0;
            }
            return -1;
        }

        public int WJ_Get_Axis_Pulses(int axisNum, ref int pValue)
        {
            if (_axes.ContainsKey(axisNum))
            {
                pValue = (int)_axes[axisNum].CurrentPosition;
                return 0;
            }
            return -1;
        }

        public int WJ_Move_Axis_Vel(int axisNum, int velocity)
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
            
            return 0;
        }

        public int WJ_Move_Axis_Emergency_Stop(int axisNum)
        {
            if (_axes.ContainsKey(axisNum))
            {
                var axis = _axes[axisNum];
                axis.Status = AxisStatus.Idle;
                axis.Velocity = 0;
                axis.TargetPosition = axis.CurrentPosition;
            }
            return 0;
        }

        public int WJ_Move_Axis_Slow_Stop(int axisNum)
        {
            if (_axes.ContainsKey(axisNum))
            {
                var axis = _axes[axisNum];
                axis.Status = AxisStatus.Stopping;
            }
            return 0;
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
            if (!_axes.ContainsKey(axisNum))
                return -1;

            var axis = _axes[axisNum];
            axis.Enabled = true;
            axis.TargetPosition = axis.CurrentPosition + pulses;
            axis.Status = AxisStatus.Running;
            
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
    }
}
