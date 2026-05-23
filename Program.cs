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
            
            // 直接启动主程序（内置模拟运动控制卡功能）
            try
            {
                Application.Run(new Form1());
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "主程序启动失败！\n\n" +
                    "错误信息：" + ex.Message + "\n\n" +
                    "详细信息：\n" + ex.ToString(),
                    "启动错误",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
