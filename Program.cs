/*
 * 项目名称：自动化设备运动控制上位机系统
 * 开发者：lqf
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LQF_PolishingSystem
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // 询问用户选择启动模式
            DialogResult result = MessageBox.Show(
                "是否启动模拟器测试界面？\n\n" +
                "点击【是】- 启动运动控制模拟器测试\n" +
                "点击【否】- 启动主程序",
                "启动模式选择",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            
            if (result == DialogResult.Yes)
            {
                // 启动模拟器测试界面
                Application.Run(new SimulatorTestForm());
            }
            else
            {
                // 启动主程序
                Application.Run(new Form1());
            }
        }
    }
}
