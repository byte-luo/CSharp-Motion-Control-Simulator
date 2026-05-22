using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LQF_PolishingSystem
{


    /// <summary>
    /// 多自由度磁流体抛光机上位机主窗体
    /// 支持点动、长动、自动轨迹三种控制模式
    /// </summary>
    public partial class Form1 : Form
    {

        public static int[] vel;
        public static double[] xtim;
        public static double[] ytim;
        public static double[] xdis;
        public static double[] ydis;
        public static int[] spe;
        public static int[] i ;
        public static int[] cishu;
        public static int[] zhijing;
        public static double[] rdis;

        public Form1()
        {
            InitializeComponent();
            combxsudu.Enabled = false;
            combysudu.Enabled = false;
            combzsudu.Enabled = false;
            combxzhuandong.Enabled = false;
            combzzhuandong.Enabled = false;
            combpaosu.Enabled = false;
            combmoshi.Enabled = false;
            combguiji.Enabled = false;
            combzouxiang.Enabled = false;

            //文本框初始化
            textBcishu.Enabled = false;
            textBxjuli.Enabled = false;
            textByjuli.Enabled = false;
            textBzhijing.Enabled = false;


            //点动按钮初始化
            butx1dian.Enabled = false;
            butx2dian.Enabled = false;
            buty1dian.Enabled = false;
            buty2dian.Enabled = false;
            butz1dian.Enabled = false;
            butz2dian.Enabled = false;
            pictx1dian.Enabled = false;
            pictx2dian.Enabled = false;
            pictz1dian.Enabled = false;
            pictz2dian.Enabled = false;
            butpao1dian.Enabled = false;
            butpao2dian.Enabled = false;

            //长动按钮初始化
            butx1chang.Enabled = false;
            butx2chang.Enabled = false;
            buty1chang.Enabled = false;
            buty2chang.Enabled = false;
            butz1chang.Enabled = false;
            butz2chang.Enabled = false;
            pictx1chang.Enabled = false;
            pictx2chang.Enabled = false;
            pictz1chang.Enabled = false;
            pictz2chang.Enabled = false;
            butpao1chang.Enabled = false;
            butpao2chang.Enabled = false;
        }

        private void butstart_Click(object sender, EventArgs e)
        {
            combxsudu.Enabled = true;
            combysudu.Enabled = true;
            combzsudu.Enabled = true;
            combxzhuandong.Enabled = true;
            combzzhuandong.Enabled = true;
            combpaosu.Enabled = true;
            combmoshi.Enabled = true;



            Form1.vel = new int[10];
            Form1.spe = new int[10];
            Form1.xdis = new double[10];
            Form1.ydis = new double[10];
            Form1.xtim = new double[10];
            Form1.ytim = new double[10];
            Form1.i = new int[10];
            Form1.cishu = new int[10];
            Form1.zhijing = new int[10];
            Form1.rdis = new double[10];




            if (0 == WJ_API.WJ_Open(0))//0表示打开的是USB
            {
                MessageBox.Show("USB连接成功");
            }
            else
            {
                MessageBox.Show("USB连接失败"); 
            }
        }

        private void butstop_Click(object sender, EventArgs e)
        {


            //选择框初始化
            combxsudu.Enabled = false;
            combysudu.Enabled = false;
            combzsudu.Enabled = false;
            combxzhuandong.Enabled = false;
            combzzhuandong.Enabled = false;
            combpaosu.Enabled = false;
            combmoshi.Enabled = false;
            combguiji.Enabled = false;
            combzouxiang.Enabled = false;

            //文本框初始化
            textBcishu.Enabled = false;
            textBxjuli.Enabled = false;
            textByjuli.Enabled = false;
            textBzhijing.Enabled = false;


            //点动按钮初始化
            butx1dian.Enabled = false;
            butx2dian.Enabled = false;
            buty1dian.Enabled = false;
            buty2dian.Enabled = false;
            butz1dian.Enabled = false;
            butz2dian.Enabled = false;
            pictx1dian.Enabled = false;
            pictx2dian.Enabled = false;
            pictz1dian.Enabled = false;
            pictz2dian.Enabled = false;
            butpao1dian.Enabled = false;
            butpao2dian.Enabled = false;

            //长动按钮初始化
            butx1chang.Enabled = false;
            butx2chang.Enabled = false;
            buty1chang.Enabled = false;
            buty2chang.Enabled = false;
            butz1chang.Enabled = false;
            butz2chang.Enabled = false;
            pictx1chang.Enabled = false;
            pictx2chang.Enabled = false;
            pictz1chang.Enabled = false;
            pictz2chang.Enabled = false;
            butpao1chang.Enabled = false;
            butpao2chang.Enabled = false;
            butxchangstop.Enabled = false;
            butychangstop.Enabled = false;
            butzchangstop.Enabled = false;
            butxzhuanstop.Enabled = false;
            butzzhuanstop.Enabled = false;
            butpaostop2.Enabled = false;


            if (0 == WJ_API.WJ_Open(0))
            {
                WJ_API.WJ_Move_Axis_Emergency_Stop(1);
                WJ_API.WJ_Move_Axis_Emergency_Stop(2);
                WJ_API.WJ_Move_Axis_Emergency_Stop(4);
                WJ_API.WJ_Move_Axis_Emergency_Stop(5);
                WJ_API.WJ_Move_Axis_Emergency_Stop(6);
                WJ_API.WJ_Move_Axis_Emergency_Stop(7);
                WJ_API.WJ_Close();
                MessageBox.Show("断开连接");

            }
        }

        private void combxsudu_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (combxsudu.SelectedIndex == 0)
            {
                Form1.vel[1] = 0;
                Form1.spe[1] = 0;
            }
            else if (combxsudu.SelectedIndex >= 1 )
            {
                if (combmoshi.SelectedIndex==0)
                {
                    butx1dian.Enabled = true;
                    butx2dian.Enabled = true;
                    butx1chang.Enabled = false;
                    butx2chang.Enabled = false;
                }

                if (combmoshi.SelectedIndex == 1)
                {
                    butx1dian.Enabled = false;
                    butx2dian.Enabled = false;
                    butx1chang.Enabled = true;
                    butx2chang.Enabled = true;
                }
                if (combxsudu.SelectedIndex == 1)
                {
                    Form1.vel[1] = 5;
                    Form1.spe[1] = 1200;
                }
                if (combxsudu.SelectedIndex == 2)
                {
                    Form1.vel[1] = 10;
                    Form1.spe[1] = 600;
                }
                if (combxsudu.SelectedIndex == 3)
                {
                    Form1.vel[1] = 15;
                    Form1.spe[1] = 400;
                }
                if (combxsudu.SelectedIndex == 4)
                {
                    Form1.vel[1] = 20;
                    Form1.spe[1] = 300;
                }
                if (combxsudu.SelectedIndex == 5)
                {
                    Form1.vel[1] = 25;
                    Form1.spe[1] = 240;
                }
                if (combxsudu.SelectedIndex == 6)
                {
                    Form1.vel[1] = 30;
                    Form1.spe[1] = 200;
                }
                if (combxsudu.SelectedIndex == 7)
                {
                    Form1.vel[1] = 35;
                    Form1.spe[1] = 171;
                }
                if (combxsudu.SelectedIndex == 8)
                {
                    Form1.vel[1] = 40;
                    Form1.spe[1] = 150;
                }
            }
        }
        
        private void combysudu_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (combysudu.SelectedIndex == 0)
            {
                Form1.vel[2] = 0;
                Form1.spe[2] = 0;
            }
            else if (combysudu.SelectedIndex >= 1 )
            {
                if (combmoshi.SelectedIndex == 0)
                {
                    buty1dian.Enabled = true;
                    buty2dian.Enabled = true;
                    buty1chang.Enabled = false;
                    buty2chang.Enabled = false;
                }

                if (combmoshi.SelectedIndex == 1)
                {
                    buty1dian.Enabled = false;
                    buty2dian.Enabled = false;
                    buty1chang.Enabled = true;
                    buty2chang.Enabled = true;
                }
                if (combysudu.SelectedIndex == 1)   //10r/min
                {
                    Form1.vel[2] = 5;
                    Form1.spe[2] = 1200;
                }
                if (combysudu.SelectedIndex == 2)   //20r/min
                {
                    Form1.vel[2] = 10;
                    Form1.spe[2] = 600;
                }
                if (combysudu.SelectedIndex == 3)   //30r/min
                {
                    Form1.vel[2] = 15;
                    Form1.spe[2] = 400;
                }
                if (combysudu.SelectedIndex == 4)   //40r/min
                {
                    Form1.vel[2] = 20;
                    Form1.spe[2] = 300;
                }
                if (combysudu.SelectedIndex == 5)   //50r/min
                {
                    Form1.vel[2] = 25;
                    Form1.spe[2] = 240;
                }
                if (combysudu.SelectedIndex == 6)   //60r/min
                {
                    Form1.vel[2] = 30;
                    Form1.spe[2] = 200;
                }
                if (combysudu.SelectedIndex == 7)   //70r/min
                {
                    Form1.vel[2] = 35;
                    Form1.spe[2] = 171;
                }
                if (combysudu.SelectedIndex == 8)   //80r/min
                {
                    Form1.vel[2] = 40;
                    Form1.spe[2] = 150;
                }
            }
        }

        private void combzsudu_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (combzsudu.SelectedIndex == 0)
            {
                Form1.vel[3] = 0;
                Form1.spe[3] = 0;
            }
            else if (combzsudu.SelectedIndex >= 1)
            {
                if (combmoshi.SelectedIndex == 0)
                {
                    butz1dian.Enabled = true;
                    butz2dian.Enabled = true;
                    butz1chang.Enabled = false;
                    butz2chang.Enabled = false;
                }

                if (combmoshi.SelectedIndex == 1)
                {
                    butz1dian.Enabled = false;
                    butz2dian.Enabled = false;
                    butz1chang.Enabled = true;
                    butz2chang.Enabled = true;
                }
                if (combzsudu.SelectedIndex == 1)   //10r/min
                {
                    Form1.vel[3] = 5;
                    Form1.spe[3] = 1200;
                }
                if (combzsudu.SelectedIndex == 2)   //20r/min
                {
                    Form1.vel[3] = 10;
                    Form1.spe[3] = 600;
                }
                if (combzsudu.SelectedIndex == 3)   //30r/min
                {
                    Form1.vel[3] = 15;
                    Form1.spe[3] = 400;
                }
                if (combzsudu.SelectedIndex == 4)   //40r/min
                {
                    Form1.vel[3] = 20;
                    Form1.spe[3] = 300;
                }
                if (combzsudu.SelectedIndex == 5)   //50r/min
                {
                    Form1.vel[3] = 25;
                    Form1.spe[3] = 240;
                }
                if (combzsudu.SelectedIndex == 6)   //60r/min
                {
                    Form1.vel[3] = 30;
                    Form1.spe[3] = 200;
                }
                if (combzsudu.SelectedIndex == 7)   //70r/min
                {
                    Form1.vel[3] = 35;
                    Form1.spe[3] = 171;
                }
                if (combzsudu.SelectedIndex == 8)   //80r/min
                {
                    Form1.vel[3] = 40;
                    Form1.spe[3] = 150;
                }
            }
        }


        private void combxzhuandong_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (combxzhuandong.SelectedIndex == 0)
            {
                Form1.vel[4] = 0;
                Form1.spe[4] = 0;
            }
            else if (combxzhuandong.SelectedIndex >= 1)
            {
                if (combmoshi.SelectedIndex == 0)
                {
                    pictx1dian.Enabled = true;
                    pictx2dian.Enabled = true;
                    pictx1chang.Enabled = false;
                    pictx2chang.Enabled = false;
                }

                if (combmoshi.SelectedIndex == 1)
                {
                    pictx1dian.Enabled = false;
                    pictx2dian.Enabled = false;
                    pictx1chang.Enabled = true;
                    pictx2chang.Enabled = true;
                }
                if (combxzhuandong.SelectedIndex == 1)
                {
                    Form1.vel[4] = 20;
                    Form1.spe[4] = 1200;
                }
                if (combxzhuandong.SelectedIndex == 2)
                {
                    Form1.vel[4] = 40;
                    Form1.spe[4] = 600;
                }
                if (combxzhuandong.SelectedIndex == 3)
                {
                    Form1.vel[4] = 45;
                    Form1.spe[4] = 400;
                }
                if (combxzhuandong.SelectedIndex == 4)
                {
                    Form1.vel[4] = 50;
                    Form1.spe[4] = 300;
                }
                if (combxzhuandong.SelectedIndex == 5)
                {
                    Form1.vel[4] = 55;
                    Form1.spe[4] = 240;
                }
                if (combxzhuandong.SelectedIndex == 6)
                {
                    Form1.vel[4] = 60;
                    Form1.spe[4] = 200;
                }

            }
        }


        private void combzzhuandong_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (combzzhuandong.SelectedIndex == 0)
            {
                Form1.vel[5] = 0;
                Form1.spe[5] = 0;
            }
            else if (combzzhuandong.SelectedIndex >= 1)
            {
                if (combmoshi.SelectedIndex == 0)
                {
                    pictz1dian.Enabled = true;
                    pictz2dian.Enabled = true;
                    pictz1chang.Enabled = false;
                    pictz2chang.Enabled = false;
                }

                if (combmoshi.SelectedIndex == 1)
                {
                    pictz1dian.Enabled = false;
                    pictz2dian.Enabled = false;
                    pictz1chang.Enabled = true;
                    pictz2chang.Enabled = true;
                }
                if (combzzhuandong.SelectedIndex == 1)   //10r/min
                {
                    Form1.vel[5] = 5;
                    Form1.spe[5] = 1200;
                }
                if (combzzhuandong.SelectedIndex == 2)   //20r/min
                {
                    Form1.vel[5] = 10;
                    Form1.spe[5] = 600;
                }
                if (combzzhuandong.SelectedIndex == 3)   //30r/min
                {
                    Form1.vel[5] = 15;
                    Form1.spe[5] = 400;
                }
                if (combzzhuandong.SelectedIndex == 4)   //40r/min
                {
                    Form1.vel[5] = 20;
                    Form1.spe[5] = 300;
                }
                if (combzzhuandong.SelectedIndex == 5)   //50r/min
                {
                    Form1.vel[5] = 25;
                    Form1.spe[5] = 240;
                }
                if (combzzhuandong.SelectedIndex == 6)   //60r/min
                {
                    Form1.vel[5] = 30;
                    Form1.spe[5] = 200;
                }
                if (combzzhuandong.SelectedIndex == 7)   //70r/min
                {
                    Form1.vel[5] = 35;
                    Form1.spe[5] = 171;
                }
                if (combzzhuandong.SelectedIndex == 8)   //80r/min
                {
                    Form1.vel[5] = 40;
                    Form1.spe[5] = 150;
                }
            }
        }


        private void combpaosu_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (combpaosu.SelectedIndex == 0)
            {
                Form1.vel[6] = 0;
                Form1.spe[6] = 0;
            }
            else if (combpaosu.SelectedIndex >= 1)
            {
                if (combmoshi.SelectedIndex == 0)
                {
                    butpao1dian.Enabled = true;
                    butpao2dian.Enabled = true;
                    butpao1chang.Enabled = false;
                    butpao2chang.Enabled = false;
                }

                if (combmoshi.SelectedIndex == 1)
                {
                    butpao1dian.Enabled = false;
                    butpao2dian.Enabled = false;
                    butpao1chang.Enabled = true;
                    butpao2chang.Enabled = true;
                }
                if (combpaosu.SelectedIndex == 1)   //10r/min
                {
                    Form1.vel[6] = 5;
                    Form1.spe[6] = 1200;
                }
                if (combpaosu.SelectedIndex == 2)   //20r/min
                {
                    Form1.vel[6] = 10;
                    Form1.spe[6] = 600;
                }
                if (combpaosu.SelectedIndex == 3)   //30r/min
                {
                    Form1.vel[6] = 15;
                    Form1.spe[6] = 400;
                }
                if (combpaosu.SelectedIndex == 4)   //40r/min
                {
                    Form1.vel[6] = 20;
                    Form1.spe[6] = 300;
                }
                if (combpaosu.SelectedIndex == 5)   //50r/min
                {
                    Form1.vel[6] = 25;
                    Form1.spe[6] = 240;
                }
                if (combpaosu.SelectedIndex == 6)   //60r/min
                {
                    Form1.vel[6] = 30;
                    Form1.spe[6] = 200;
                }
                if (combpaosu.SelectedIndex == 7)   //70r/min
                {
                    Form1.vel[6] = 35;
                    Form1.spe[6] = 171;
                }
                if (combpaosu.SelectedIndex == 8)   //80r/min
                {
                    Form1.vel[6] = 40;
                    Form1.spe[6] = 150;
                }
            }
        }



        private void combmoshi_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (combmoshi.SelectedIndex == 0)
            {
                butx1dian.Enabled = true;
                butx2dian.Enabled = true;
                buty1dian.Enabled = true;
                buty2dian.Enabled = true;
                butz1dian.Enabled = true;
                butz2dian.Enabled = true;
                pictx1dian.Enabled = true;
                pictx2dian.Enabled = true;
                pictz1dian.Enabled = true;
                pictz2dian.Enabled = true;
                butpao1dian.Enabled = true;
                butpao2dian.Enabled = true;   
                butx1chang.Enabled = false;
                butx2chang.Enabled = false;
                buty1chang.Enabled = false;
                buty2chang.Enabled = false;
                butz1chang.Enabled = false;
                butz2chang.Enabled = false;
                pictx1chang.Enabled = false;
                pictx2chang.Enabled = false;
                pictz1chang.Enabled = false;
                pictz2chang.Enabled = false;
                butpao1chang.Enabled = false;
                butpao2chang.Enabled = false;
                combguiji.Enabled = false;
                combzouxiang.Enabled = false;
                textBcishu.Enabled = false;
                textBxjuli.Enabled = false;
                textByjuli.Enabled = false;
                textBzhijing.Enabled = false;
            }
            if (combmoshi.SelectedIndex == 1 )
            {
                butx1dian.Enabled = false;
                butx2dian.Enabled = false;
                buty1dian.Enabled = false;
                buty2dian.Enabled = false;
                butz1dian.Enabled = false;
                butz2dian.Enabled = false;
                pictx1dian.Enabled = false;
                pictx2dian.Enabled = false;
                pictz1dian.Enabled = false;
                pictz2dian.Enabled = false;
                butpao1dian.Enabled = false;
                butpao2dian.Enabled = false;
                butx1chang.Enabled = true;
                butx2chang.Enabled = true;
                buty1chang.Enabled = true;
                buty2chang.Enabled = true;
                butz1chang.Enabled = true;
                butz2chang.Enabled = true;
                pictx1chang.Enabled = true;
                pictx2chang.Enabled = true;
                pictz1chang.Enabled = true;
                pictz2chang.Enabled = true;
                butpao1chang.Enabled = true;
                butpao2chang.Enabled = true;
                combguiji.Enabled = false;
                combzouxiang.Enabled = false;
                textBcishu.Enabled = false;
                textBxjuli.Enabled = false;
                textByjuli.Enabled = false;
                textBzhijing.Enabled = false;
            }
            if (combmoshi.SelectedIndex == 2)
            {
                butx1dian.Enabled = false;
                butx2dian.Enabled = false;
                buty1dian.Enabled = false;
                buty2dian.Enabled = false;
                butz1dian.Enabled = false;
                butz2dian.Enabled = false;
                pictx1dian.Enabled = false;
                pictx2dian.Enabled = false;
                pictz1dian.Enabled = false;
                pictz2dian.Enabled = false;
                butpao1dian.Enabled = false;
                butpao2dian.Enabled = false;
                butx1chang.Enabled = false;
                butx2chang.Enabled = false;
                buty1chang.Enabled = false;
                buty2chang.Enabled = false;
                butz1chang.Enabled = false;
                butz2chang.Enabled = false;
                pictx1chang.Enabled = false;
                pictx2chang.Enabled = false;
                pictz1chang.Enabled = false;
                pictz2chang.Enabled = false;
                butpao1chang.Enabled = false;
                butpao2chang.Enabled = false;
                combguiji.Enabled = true;
                combzouxiang.Enabled = true;
                textBcishu.Enabled = true;
                textBxjuli.Enabled = true;
                textByjuli.Enabled = true;
                textBzhijing.Enabled = true;
            }
        }

        private void butx1dian_MouseDown(object sender, MouseEventArgs e)
        {
            WJ_API.WJ_Move_Axis_Vel(1, -Form1.vel[1]);
            if (combxsudu.SelectedIndex >= 1)
            {
                label1zhou.BackColor = Color.Green;
            }
                
        }

        private void butx1dian_MouseUp(object sender, MouseEventArgs e)
        {
            WJ_API.WJ_Move_Axis_Emergency_Stop(1);
            label1zhou.BackColor = Color.Red;
        }


        private void butx2dian_MouseDown(object sender, MouseEventArgs e)
        {
            WJ_API.WJ_Move_Axis_Vel(1, Form1.vel[1]);
            if (combxsudu.SelectedIndex >= 1)
            {
                label1zhou.BackColor = Color.Green;
            }
        }

        private void butx2dian_MouseUp(object sender, MouseEventArgs e)
        {
            WJ_API.WJ_Move_Axis_Emergency_Stop(1);
            label1zhou.BackColor = Color.Red;
        }


        private void buty1dian_MouseDown(object sender, MouseEventArgs e)
        {
            WJ_API.WJ_Move_Axis_Vel(2, -Form1.vel[2]);
            if (combysudu.SelectedIndex >= 1)
            {
                label2zhou.BackColor = Color.Green;
            }
                
        }

        private void buty1dian_MouseUp(object sender, MouseEventArgs e)
        {
            WJ_API.WJ_Move_Axis_Emergency_Stop(2);
            label2zhou.BackColor = Color.Red;
        }


        private void buty2dian_MouseDown(object sender, MouseEventArgs e)
        {
            WJ_API.WJ_Move_Axis_Vel(2, Form1.vel[2]);
            if (combysudu.SelectedIndex >= 1)
            {
                label2zhou.BackColor = Color.Green;
            }
        }

        private void buty2dian_MouseUp(object sender, MouseEventArgs e)
        {
            WJ_API.WJ_Move_Axis_Emergency_Stop(2);
            label2zhou.BackColor = Color.Red;
        }



        private void butz1dian_MouseDown(object sender, MouseEventArgs e)
        {
            WJ_API.WJ_Move_Axis_Vel(4, Form1.vel[3]);
            if (combzsudu.SelectedIndex >= 1)
            {
                label3zhou.BackColor = Color.Green;
            }
                
        }

        private void butz1dian_MouseUp(object sender, MouseEventArgs e)
        {
            WJ_API.WJ_Move_Axis_Emergency_Stop(4);
            label3zhou.BackColor = Color.Red;
        }



        private void butz2dian_MouseDown(object sender, MouseEventArgs e)
        {
            WJ_API.WJ_Move_Axis_Vel(4, -Form1.vel[3]);
            if (combzsudu.SelectedIndex >= 1)
            {
                label3zhou.BackColor = Color.Green;
            }
        }

        private void butz2dian_MouseUp(object sender, MouseEventArgs e)
        {
            WJ_API.WJ_Move_Axis_Emergency_Stop(4);
            label3zhou.BackColor = Color.Red;
        }


        private void pictx1dian_MouseDown(object sender, MouseEventArgs e)
        {
            WJ_API.WJ_Move_Axis_Vel(5, -Form1.vel[4]);
            if (combxzhuandong.SelectedIndex >= 1)
            {
                label4zhou.BackColor = Color.Green;
            }
                
        }

        private void pictx1dian_MouseUp(object sender, MouseEventArgs e)
        {
            WJ_API.WJ_Move_Axis_Emergency_Stop(5);
            label4zhou.BackColor = Color.Red;
        }


        private void pictx2dian_MouseDown(object sender, MouseEventArgs e)
        {
            WJ_API.WJ_Move_Axis_Vel(5, Form1.vel[4]);
            if (combxzhuandong.SelectedIndex >= 1)
            {
                label4zhou.BackColor = Color.Green;
            }
        }

        private void pictx2dian_MouseUp(object sender, MouseEventArgs e)
        {
            WJ_API.WJ_Move_Axis_Emergency_Stop(5);
            label4zhou.BackColor = Color.Red;
        }

        private void pictz1dian_MouseDown(object sender, MouseEventArgs e)
        {
            WJ_API.WJ_Move_Axis_Vel(6, -Form1.vel[5]);
            if (combxzhuandong.SelectedIndex >= 1)
            {
                label5zhou.BackColor = Color.Green;
            }
                
        }

        private void pictz1dian_MouseUp(object sender, MouseEventArgs e)
        {
            WJ_API.WJ_Move_Axis_Emergency_Stop(6);
            label5zhou.BackColor = Color.Red;
        }


        private void pictz2dian_MouseDown(object sender, MouseEventArgs e)
        {
            WJ_API.WJ_Move_Axis_Vel(6, Form1.vel[5]);
            if (combxzhuandong.SelectedIndex >= 1)
            {
                label5zhou.BackColor = Color.Green;
            }
        }

        private void pictz2dian_MouseUp(object sender, MouseEventArgs e)
        {
            WJ_API.WJ_Move_Axis_Emergency_Stop(6);
            label5zhou.BackColor = Color.Red;
        }


        private void butpao1dian_Click(object sender, EventArgs e)
        {
            WJ_API.WJ_Move_Axis_Vel(7, -Form1.vel[6]);
            if (combpaosu.SelectedIndex >= 1)
            {
                label6zhou.BackColor = Color.Green;
            }
                
        }


        private void butpao2dian_Click(object sender, EventArgs e)
        {
            WJ_API.WJ_Move_Axis_Vel(7, Form1.vel[6]);
            if (combpaosu.SelectedIndex >= 1)
            {
                label6zhou.BackColor = Color.Green;
            }
        }

        private void butx1dian_Click(object sender, EventArgs e)
        {
            WJ_API.WJ_Move_Axis_Vel(1, -Form1.vel[1]);
            if (combxsudu.SelectedIndex >= 1)
            {
                label1zhou.BackColor = Color.Green;
            }
        }

        private void butx2dian_Click(object sender, EventArgs e)
        {
            WJ_API.WJ_Move_Axis_Vel(1, Form1.vel[1]);
            if (combxsudu.SelectedIndex >= 1)
            {
                label1zhou.BackColor = Color.Green;
            }
        }

        private void buty1dian_Click(object sender, EventArgs e)
        {
            WJ_API.WJ_Move_Axis_Vel(2, -Form1.vel[2]);
            if (combysudu.SelectedIndex >= 1)
            {
                label2zhou.BackColor = Color.Green;
            }
        }

        private void buty2dian_Click(object sender, EventArgs e)
        {
            WJ_API.WJ_Move_Axis_Vel(2, Form1.vel[2]);
            if (combysudu.SelectedIndex >= 1)
            {
                label2zhou.BackColor = Color.Green;
            }
        }

        private void butz1dian_Click(object sender, EventArgs e)
        {
            WJ_API.WJ_Move_Axis_Vel(4, Form1.vel[3]);
            if (combzsudu.SelectedIndex >= 1)
            {
                label3zhou.BackColor = Color.Green;
            }
        }

        private void butz2dian_Click(object sender, EventArgs e)
        {
            WJ_API.WJ_Move_Axis_Vel(4, -Form1.vel[3]);
            if (combzsudu.SelectedIndex >= 1)
            {
                label3zhou.BackColor = Color.Green;
            }
        }

        private void pictx1dian_Click(object sender, EventArgs e)
        {
            WJ_API.WJ_Move_Axis_Vel(5, -Form1.vel[4]);
            if (combxzhuandong.SelectedIndex >= 1)
            {
                label4zhou.BackColor = Color.Green;
            }
        }

        private void pictx2dian_Click(object sender, EventArgs e)
        {
            WJ_API.WJ_Move_Axis_Vel(5, Form1.vel[4]);
            if (combxzhuandong.SelectedIndex >= 1)
            {
                label4zhou.BackColor = Color.Green;
            }
        }

        private void pictz1dian_Click(object sender, EventArgs e)
        {
            WJ_API.WJ_Move_Axis_Vel(6, -Form1.vel[5]);
            if (combzzhuandong.SelectedIndex >= 1)
            {
                label5zhou.BackColor = Color.Green;
            }
        }

        private void pictz2dian_Click(object sender, EventArgs e)
        {
            WJ_API.WJ_Move_Axis_Vel(6, Form1.vel[5]);
            if (combzzhuandong.SelectedIndex >= 1)
            {
                label5zhou.BackColor = Color.Green;
            }
        }

        private void butx1chang_Click(object sender, EventArgs e)
        {
            WJ_API.WJ_Move_Axis_Vel(1, -Form1.vel[1]);
            if (combxsudu.SelectedIndex >= 1)
            {
                label1zhou.BackColor = Color.Green;
            }

        }


        private void butx2chang_Click(object sender, EventArgs e)
        {
            WJ_API.WJ_Move_Axis_Vel(1, Form1.vel[1]);
            if (combxsudu.SelectedIndex >= 1)
            {
                label1zhou.BackColor = Color.Green;
            }

        }


        private void buty1chang_Click(object sender, EventArgs e)
        {
            WJ_API.WJ_Move_Axis_Vel(2, -Form1.vel[2]);
            if (combysudu.SelectedIndex >= 1)
            {
                label2zhou.BackColor = Color.Green;
            }

        }


        private void buty2chang_Click(object sender, EventArgs e)
        {
            WJ_API.WJ_Move_Axis_Vel(2, Form1.vel[2]);
            if (combysudu.SelectedIndex >= 1)
            {
                label2zhou.BackColor = Color.Green;
            }

        }


        private void butz1chang_Click(object sender, EventArgs e)
        {
            WJ_API.WJ_Move_Axis_Vel(4, Form1.vel[3]);
            if (combzsudu.SelectedIndex >= 1)
            {
                label3zhou.BackColor = Color.Green;
            }

        }


        private void butz2chang_Click(object sender, EventArgs e)
        {
            WJ_API.WJ_Move_Axis_Vel(4, -Form1.vel[3]);
            if (combzsudu.SelectedIndex >= 1)
            {
                label3zhou.BackColor = Color.Green;
            }

        }


        private void pictx1chang_Click(object sender, EventArgs e)
        {
            WJ_API.WJ_Move_Axis_Vel(5, -Form1.vel[4]);
            if (combxzhuandong.SelectedIndex >= 1)
            {
                label4zhou.BackColor = Color.Green;
            }
        }


        private void pictx2chang_Click(object sender, EventArgs e)
        {
            WJ_API.WJ_Move_Axis_Vel(5, Form1.vel[4]);
            if (combxzhuandong.SelectedIndex >= 1)
            {
                label4zhou.BackColor = Color.Green;
            }
        }


        private void pictz1chang_Click(object sender, EventArgs e)
        {
            WJ_API.WJ_Move_Axis_Vel(6, -Form1.vel[5]);
            if (combzzhuandong.SelectedIndex >= 1)
            {
                label5zhou.BackColor = Color.Green;
            }
        }


        private void pictz2chang_Click(object sender, EventArgs e)
        {
            WJ_API.WJ_Move_Axis_Vel(6, Form1.vel[5]);
            if (combzzhuandong.SelectedIndex >= 1)
            {
                label5zhou.BackColor = Color.Green;
            }
        }

        private void butpao1chang_Click(object sender, EventArgs e)
        {
            WJ_API.WJ_Move_Axis_Vel(7, -Form1.vel[6]);
            if (combpaosu.SelectedIndex >= 1)
            {
                label6zhou.BackColor = Color.Green;
            }
                
        }


        private void butpao2chang_Click(object sender, EventArgs e)
        {
            WJ_API.WJ_Move_Axis_Vel(7, Form1.vel[6]);
            if (combpaosu.SelectedIndex >= 1)
            {
                label6zhou.BackColor = Color.Green;
            }
        }

        private void butxchangstop_Click(object sender, EventArgs e)
        {
            WJ_API.WJ_Move_Axis_Emergency_Stop(1);
            label1zhou.BackColor = Color.Red;

        }
        private void butxchanghuanting_Click(object sender, EventArgs e)
        {
            WJ_API.WJ_Move_Axis_Slow_Stop(1);
            label1zhou.BackColor = Color.Red;
        }


        private void butychangstop_Click(object sender, EventArgs e)
        {
            WJ_API.WJ_Move_Axis_Emergency_Stop(2);
            label2zhou.BackColor = Color.Red;

        }
        private void butychanghuanting_Click(object sender, EventArgs e)
        {
            WJ_API.WJ_Move_Axis_Slow_Stop(2);
            label2zhou.BackColor = Color.Red;
        }


        private void butzchangstop_Click(object sender, EventArgs e)
        {
            WJ_API.WJ_Move_Axis_Emergency_Stop(4);
            label3zhou.BackColor = Color.Red;

        }
        private void butzchanghuanting_Click(object sender, EventArgs e)
        {
            WJ_API.WJ_Move_Axis_Slow_Stop(4);
            label3zhou.BackColor = Color.Red;
        }


        private void butxzhuanstop_Click(object sender, EventArgs e)
        {
            WJ_API.WJ_Move_Axis_Emergency_Stop(5);
            label4zhou.BackColor = Color.Red;
        }
        private void butxzhuanhuanting_Click(object sender, EventArgs e)
        {
            WJ_API.WJ_Move_Axis_Slow_Stop(5);
            label4zhou.BackColor = Color.Red;
        }


        private void butzzhuanstop_Click(object sender, EventArgs e)
        {
            WJ_API.WJ_Move_Axis_Emergency_Stop(6);
            label5zhou.BackColor = Color.Red;
        }
        private void butzzhuanhuanting_Click(object sender, EventArgs e)
        {
            WJ_API.WJ_Move_Axis_Slow_Stop(6);
            label5zhou.BackColor = Color.Red;
        }


        private void butpaostop2_Click(object sender, EventArgs e)
        {
            WJ_API.WJ_Move_Axis_Emergency_Stop(7);
            label6zhou.BackColor = Color.Red;
        }
        private void butbutpaohuanting(object sender, EventArgs e)
        {
            WJ_API.WJ_Move_Axis_Slow_Stop(7);
            label6zhou.BackColor = Color.Red;
        }


        private void butjiting_Click(object sender, EventArgs e)
        {
            label1zhou.BackColor = Color.Red;
            label2zhou.BackColor = Color.Red;
            label3zhou.BackColor = Color.Red;
            label4zhou.BackColor = Color.Red;
            label5zhou.BackColor = Color.Red;
            label6zhou.BackColor = Color.Red;
            WJ_API.WJ_Move_Axis_Emergency_Stop(1);
            WJ_API.WJ_Move_Axis_Emergency_Stop(2);
            WJ_API.WJ_Move_Axis_Emergency_Stop(4);
            WJ_API.WJ_Move_Axis_Emergency_Stop(5);
            WJ_API.WJ_Move_Axis_Emergency_Stop(6);
            WJ_API.WJ_Move_Axis_Emergency_Stop(7);
        }


        private void butkaishi_Click(object sender, EventArgs e)
        {
            if (combguiji.SelectedIndex == 1)
            {
                Form1.zhijing[1] = Convert.ToInt16(textBzhijing.Text);
                WJ_API.WJ_Move_Axis_Vel(4, Form1.vel[3]);
                System.Threading.Thread.Sleep(1000);
                WJ_API.WJ_Move_Axis_Emergency_Stop(4);
                WJ_API.WJ_Move_Axis_Vel(1, Form1.vel[1]);
                System.Threading.Thread.Sleep(Convert.ToInt16(Form1.spe[1] * (Form1.zhijing[1] / 2)));
                WJ_API.WJ_Move_Axis_Emergency_Stop(1);
                WJ_API.WJ_Move_Axis_Vel(4, -Form1.vel[3]);
                System.Threading.Thread.Sleep(1000);
                WJ_API.WJ_Move_Axis_Emergency_Stop(4);
                for (double i = (Form1.zhijing[1] / 2); i > 0.0; i = i - 0.5)
                {
                    WJ_API.WJ_Move_Axis_Vel(6, Form1.vel[5]);
                    System.Threading.Thread.Sleep(Convert.ToInt16(Form1.spe[5] * 5));
                    WJ_API.WJ_Move_Axis_Emergency_Stop(6);
                    WJ_API.WJ_Move_Axis_Vel(1, -Form1.vel[1]);
                    System.Threading.Thread.Sleep(Convert.ToInt16(Form1.spe[1]*0.25));
                    WJ_API.WJ_Move_Axis_Emergency_Stop(1);
                }
                WJ_API.WJ_Move_Axis_Vel(4, Form1.vel[3]);
                System.Threading.Thread.Sleep(1000);
                WJ_API.WJ_Move_Axis_Emergency_Stop(4);
            }
            if (textBxjuli.Text == "" || textByjuli.Text == "" || textBcishu.Text == "")
            {

            }
            else
            {
                if (combguiji.SelectedIndex == 0)
                {
                    if (combzouxiang.SelectedIndex == 0)
                    {
                        Form1.xdis[1] = Convert.ToInt16(textBxjuli.Text);
                        Form1.ydis[1] = Convert.ToInt16(textByjuli.Text);
                        Form1.cishu[1] = Convert.ToInt16(textBcishu.Text);
                        Form1.xtim[1] = Form1.spe[1] * Form1.xdis[1];
                        Form1.ytim[1] = Form1.spe[2] * (Form1.ydis[1] / Form1.cishu[1]);
                        int a = 1;
                        for (i[1] = 1; i[1] <= Form1.cishu[1]; i[1]++)
                        {
                            if (a == 1)
                            {
                                WJ_API.WJ_Move_Axis_Vel(4, -Form1.vel[3]);
                                System.Threading.Thread.Sleep(1000);
                                WJ_API.WJ_Move_Axis_Emergency_Stop(4);
                            }
                            if (i[1] % 2 == 1)
                            {
                                WJ_API.WJ_Move_Axis_Vel(1, -Form1.vel[1]);
                                System.Threading.Thread.Sleep(Convert.ToInt16(Form1.xtim[1]));
                                WJ_API.WJ_Move_Axis_Emergency_Stop(1);

                                if (i[1] < Form1.cishu[1])
                                {
                                    WJ_API.WJ_Move_Axis_Vel(2, Form1.vel[2]);
                                    System.Threading.Thread.Sleep(Convert.ToInt16(Form1.ytim[1]));
                                    WJ_API.WJ_Move_Axis_Emergency_Stop(2);
                                }
                            }
                            if (i[1] % 2 == 0)
                            {

                                WJ_API.WJ_Move_Axis_Vel(1, Form1.vel[1]);
                                System.Threading.Thread.Sleep(Convert.ToInt16(Form1.xtim[1]));
                                WJ_API.WJ_Move_Axis_Emergency_Stop(1);
                                if (i[1] < Form1.cishu[1])
                                {
                                    WJ_API.WJ_Move_Axis_Vel(2, Form1.vel[2]);
                                    System.Threading.Thread.Sleep(Convert.ToInt16(Form1.ytim[1]));
                                    WJ_API.WJ_Move_Axis_Emergency_Stop(2);
                                }

                            }
                            if (a == Form1.cishu[1])
                            {
                                WJ_API.WJ_Move_Axis_Vel(4, Form1.vel[3]);
                                System.Threading.Thread.Sleep(1000);
                                WJ_API.WJ_Move_Axis_Emergency_Stop(4);
                            }
                            a++;
                        }


                    }

                    if (combzouxiang.SelectedIndex == 1)
                    {
                        Form1.xdis[1] = Convert.ToInt16(textBxjuli.Text);
                        Form1.ydis[1] = Convert.ToInt16(textByjuli.Text);
                        Form1.cishu[1] = Convert.ToInt16(textBcishu.Text);
                        Form1.ytim[1] = Form1.spe[2] * Form1.ydis[1];
                        Form1.xtim[1] = Form1.spe[1] * (Form1.xdis[1] / Form1.cishu[1]);
                        int a = 1;
                        for (i[1] = 1; i[1] <= Form1.cishu[1]; i[1]++)
                        {
                            if (a == 1)
                            {
                                WJ_API.WJ_Move_Axis_Vel(4, -Form1.vel[3]);
                                System.Threading.Thread.Sleep(1000);
                                WJ_API.WJ_Move_Axis_Emergency_Stop(4);
                            }
                            if (i[1] % 2 == 1)
                            {
                                WJ_API.WJ_Move_Axis_Vel(2, Form1.vel[2]);
                                System.Threading.Thread.Sleep(Convert.ToInt16(Form1.ytim[1]));
                                WJ_API.WJ_Move_Axis_Emergency_Stop(2);

                                if (i[1] < Form1.cishu[1])
                                {
                                    WJ_API.WJ_Move_Axis_Vel(1, -Form1.vel[1]);
                                    System.Threading.Thread.Sleep(Convert.ToInt16(Form1.xtim[1]));
                                    WJ_API.WJ_Move_Axis_Emergency_Stop(1);
                                }
                            }
                            if (i[1] % 2 == 0)
                            {
                                WJ_API.WJ_Move_Axis_Vel(2, -Form1.vel[2]);
                                System.Threading.Thread.Sleep(Convert.ToInt16(Form1.ytim[1]));
                                WJ_API.WJ_Move_Axis_Emergency_Stop(2);
                                if (i[1] < Form1.cishu[1])
                                {
                                    WJ_API.WJ_Move_Axis_Vel(1, -Form1.vel[1]);
                                    System.Threading.Thread.Sleep(Convert.ToInt16(Form1.xtim[1]));
                                    WJ_API.WJ_Move_Axis_Emergency_Stop(1);
                                }
                            }
                            if (a == Form1.cishu[1])
                            {
                                WJ_API.WJ_Move_Axis_Vel(4, Form1.vel[3]);
                                System.Threading.Thread.Sleep(1000);
                                WJ_API.WJ_Move_Axis_Emergency_Stop(4);
                            }
                            a++;
                        }
                    }
                }
                

            }
        }

    }
}
