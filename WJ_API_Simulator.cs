/*
 * 项目名称：自动化设备运动控制上位机系统
 * 开发者：lqf
 * 说明：运动控制卡 API 接口（软件模拟版本）
 */
using System;

namespace LQF_PolishingSystem
{
    /// <summary>
    /// 运动控制卡 API 接口类（纯软件模拟，无需硬件）
    /// </summary>
    public class WJ_API
    {
        private static MotionControllerSimulator _simulator = MotionControllerSimulator.Instance;

        // 通用初始化指令
        public static Int32 WJ_Open(Int32 num_scom)
        {
            return _simulator.WJ_Open(num_scom);
        }

        public static Int32 WJ_Close()
        {
            return _simulator.WJ_Close();
        }

        // 查询指令
        public static Int32 WJ_Get_Axis_Acc(Int32 AxisNUM, ref Int32 pValue)
        {
            pValue = 10;
            return 0;
        }

        public static Int32 WJ_Get_Axis_Dec(Int32 AxisNUM, ref Int32 pValue)
        {
            pValue = 10;
            return 0;
        }

        public static Int32 WJ_Get_Axis_Vel(Int32 AxisNUM, ref Int32 pValue)
        {
            double vel = _simulator.GetAxisVelocity(AxisNUM);
            pValue = (Int32)vel;
            return 0;
        }

        public static Int32 WJ_Get_Axis_Subdivision(Int32 AxisNUM, ref Int32 pValue)
        {
            pValue = 1000;
            return 0;
        }

        public static Int32 WJ_Get_Axis_Status(Int32 AxisNUM, ref Int32 pValue)
        {
            return _simulator.WJ_Get_Axis_Status(AxisNUM, ref pValue);
        }

        public static Int32 WJ_Get_Axes_Status(Int32[] pValue)
        {
            for (int i = 0; i < pValue.Length; i++)
            {
                int status = 0;
                _simulator.WJ_Get_Axis_Status(i + 1, ref status);
                pValue[i] = status;
            }
            return 0;
        }

        public static Int32 WJ_Get_Axis_Pulses(Int32 AxisNUM, ref Int32 pValue)
        {
            return _simulator.WJ_Get_Axis_Pulses(AxisNUM, ref pValue);
        }

        public static Int32 WJ_Get_Axes_Pulses(Int32[] pValue)
        {
            int[] axisNums = { 1, 2, 4, 5, 6, 7 };
            for (int i = 0; i < axisNums.Length && i < pValue.Length; i++)
            {
                int pos = 0;
                _simulator.WJ_Get_Axis_Pulses(axisNums[i], ref pos);
                pValue[i] = pos;
            }
            return 0;
        }

        public static Int32 WJ_Get_Axes_Num(ref Int32 pValue)
        {
            pValue = 6;
            return 0;
        }

        // 运动指令
        public static Int32 WJ_Move_Axis_Pulses(Int32 AxisNUM, Int32 Value)
        {
            return _simulator.WJ_Move_Axis_Pulses(AxisNUM, Value);
        }

        public static Int32 WJ_Move_Axes_Pulses(Int32[] pValue)
        {
            int[] axisNums = { 1, 2, 4, 5, 6, 7 };
            for (int i = 0; i < axisNums.Length && i < pValue.Length; i++)
            {
                _simulator.WJ_Move_Axis_Pulses(axisNums[i], pValue[i]);
            }
            return 0;
        }

        public static Int32 WJ_Move_Axis_Vel(Int32 AxisNUM, Int32 Value)
        {
            return _simulator.WJ_Move_Axis_Vel(AxisNUM, Value);
        }

        public static Int32 WJ_Move_Axes_Vel(Int32[] pValue)
        {
            int[] axisNums = { 1, 2, 4, 5, 6, 7 };
            for (int i = 0; i < axisNums.Length && i < pValue.Length; i++)
            {
                _simulator.WJ_Move_Axis_Vel(axisNums[i], pValue[i]);
            }
            return 0;
        }

        public static Int32 WJ_Move_Axis_Emergency_Stop(Int32 AxisNUM)
        {
            return _simulator.WJ_Move_Axis_Emergency_Stop(AxisNUM);
        }

        public static Int32 WJ_Move_Axis_Slow_Stop(Int32 AxisNUM)
        {
            return _simulator.WJ_Move_Axis_Slow_Stop(AxisNUM);
        }

        public static Int32 WJ_Move_Axis_Home(Int32 AxisNUM, Int32 Value)
        {
            return 0;
        }

        // 设置指令
        public static Int32 WJ_Set_Axis_Acc(Int32 AxisNUM, Int32 Value)
        {
            return 0;
        }

        public static Int32 WJ_Set_Axis_Dec(Int32 AxisNUM, Int32 Value)
        {
            return 0;
        }

        public static Int32 WJ_Set_Axis_Vel(Int32 AxisNUM, Int32 Value)
        {
            return _simulator.WJ_Set_Axis_Vel(AxisNUM, Value);
        }

        public static Int32 WJ_Set_Axis_Subdivision(Int32 AxisNUM, Int32 Value)
        {
            return 0;
        }

        public static Int32 WJ_Set_Axis_Slow_Stop(Int32 AxisNUM, Int32 Value)
        {
            return 0;
        }

        public static Int32 WJ_Set_Led_Twinkle()
        {
            return 0;
        }

        public static Int32 WJ_Set_Axis_Pulses_Zero(Int32 AxisNUM)
        {
            return 0;
        }

        public static Int32 WJ_Set_Default()
        {
            return 0;
        }

        public static Int32 WJ_Set_Move_Axis_Vel_Acc(Int32 AxisNUM, Int32 Value)
        {
            return 0;
        }

        public static Int32 WJ_Set_Axis_Home_Pulses(Int32 AxisNUM, Int32 Value)
        {
            return 0;
        }

        // IO指令
        public static Int32 WJ_IO_Output(Int32 IONUM, Int32 Value)
        {
            return 0;
        }

        public static Int32 WJ_IO_Input(Int32 IONUM, ref Int32 pValue)
        {
            pValue = 0;
            return 0;
        }
    }
}
