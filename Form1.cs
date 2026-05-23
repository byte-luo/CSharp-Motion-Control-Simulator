using System;
using System.Drawing;
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
        public static int[] i;
        public static int[] cishu;
        public static int[] zhijing;
        public static double[] rdis;

        // 模拟动画相关
        private Timer _simulationTimer;
        private Label[] _axisPositionLabels;
        private Label[] _axisVelocityLabels;
        private Label[] _axisStatusLabels;
        private int[] _axisNumbers = { 1, 2, 4, 5, 6, 7 };
        private string[] _axisNames = { "X轴", "Y轴", "Z轴", "A轴(旋转)", "B轴(旋转)", "C轴(抛刀)" };

        // 坐标原点偏移量
        private int _offsetX = 0;
        private int _offsetY = 0;
        private int _offsetZ = 0;

        // 轨迹执行后台任务控制
        private System.Threading.CancellationTokenSource _trajectoryCts;
        private bool _isExecutingTrajectory = false;

        public Form1()
        {
            InitializeComponent();

            // 应用美化样式
            ApplyModernUIStyle();

            // 初始化模拟动画定时器
            _simulationTimer = new Timer();
            _simulationTimer.Interval = 100; // 100ms更新一次
            _simulationTimer.Tick += SimulationTimer_Tick;
            _simulationTimer.Start();

            // 初始化模拟动画面板
            CreateSimulationPanel();

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

        // ========== 配色方案 ==========
        private static readonly Color CBg = Color.FromArgb(22, 24, 36);
        private static readonly Color CPanel = Color.FromArgb(35, 38, 55);
        private static readonly Color CInput = Color.FromArgb(48, 52, 72);
        private static readonly Color CAccent = Color.FromArgb(0, 170, 255);
        private static readonly Color CCyan = Color.FromArgb(0, 230, 200);
        private static readonly Color COrange = Color.FromArgb(255, 170, 0);
        private static readonly Color CRed = Color.FromArgb(255, 60, 60);
        private static readonly Color CText = Color.FromArgb(220, 220, 235);
        private static readonly Color CTextDim = Color.FromArgb(150, 150, 170);
        private static readonly Color CBorder = Color.FromArgb(60, 65, 90);
        private static readonly Color CBtnGreen = Color.FromArgb(0, 180, 100);

        /// <summary>
        /// 应用现代化UI样式 - 专业工业控制风格
        /// </summary>
        private void ApplyModernUIStyle()
        {
            this.Text = " 多自由度磁流体抛光机控制系统";
            this.BackColor = CBg;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimumSize = new Size(1400, 1000);
            this.ClientSize = new Size(1400, 1000);
            this.Font = new Font("Microsoft YaHei", 10);

            // ===== 左侧栏 =====
            LayoutPanel(panel1, 20, 20, 270, 80);
            LayoutButton(butstart, 15, 18, 110, 45, "启动", CBtnGreen);
            LayoutButton(butstop, 145, 18, 110, 45, "停止", CRed);

            LayoutPanel(panel4, 20, 110, 270, 210);
            LayoutLabel(label7, 100, 10, "轴状态", 12, true, CAccent);
            string[] axisNames = { "1轴", "2轴", "3轴", "4轴", "5轴", "6轴" };
            Label[] axisLbls = { label8, label9, label10, label11, label12, label13 };
            Label[] axisLeds = { label1zhou, label2zhou, label3zhou, label4zhou, label5zhou, label6zhou };
            for (int i = 0; i < 6; i++)
            {
                int row = i / 2, col = i % 2;
                int x = col == 0 ? 35 : 155;
                int y = 48 + row * 50;
                LayoutLabel(axisLbls[i], x, y + 2, axisNames[i], 11, false, CText);
                LayoutLed(axisLeds[i], x + 45, y, 24, 24);
            }

            LayoutPanel(panel2, 20, 330, 270, 380);
            LayoutLabel(label1, 80, 10, "三维坐标", 13, true, CAccent);
            LayoutLabel(label2, 55, 42, "设置当前坐标为原点", 10, false, CTextDim);
            LayoutButton(butzuobiaoqingchu, 35, 72, 85, 34, "清除", Color.FromArgb(100, 100, 120));
            LayoutButton(butzuobiaoqueding, 150, 72, 85, 34, "确定", CAccent);
            LayoutLabel(label3, 30, 125, "X 轴", 11, false, CText);
            LayoutDisplay(labelxzuobiao, 90, 123, 145, 34);
            LayoutLabel(label4, 30, 172, "Y 轴", 11, false, CText);
            LayoutDisplay(labelyzuobiao, 90, 170, 145, 34);
            LayoutLabel(label5, 30, 219, "Z 轴", 11, false, CText);
            LayoutDisplay(labelzzuobiao, 90, 217, 145, 34);
            LayoutLabel(label48, 30, 270, "当前位置", 10, false, CTextDim);
            LayoutDisplay(labelxyzzuobiao, 90, 268, 145, 34, "(0,0,0)");
            LayoutLabel(label14, 30, 320, "单位: 脉冲(pulse)", 9, false, CTextDim);
            pictureBox1.Location = new Point(125, 300);
            pictureBox1.Size = new Size(125, 70);

            // ===== 右侧主区域 =====
            panel5.Location = new Point(310, 20);
            panel5.Size = new Size(1060, 690);
            panel5.BackColor = CBg;

            // 速度选择
            LayoutPanel(panel6, 0, 0, 1060, 155);
            LayoutLabel(label16, 15, 10, "速度设置", 12, true, CAccent);
            string[] spdNames = { "X轴直动速度", "Y轴直动速度", "Z轴直动速度", "X轴转动速度", "Z轴转动速度", "抛光轮转速" };
            Label[] spdLbls = { label20, label6, label22, label21, label23, label24 };
            ComboBox[] spdCombos = { combxsudu, combysudu, combzsudu, combxzhuandong, combzzhuandong, combpaosu };
            for (int i = 0; i < 6; i++)
            {
                int col = i % 3, row = i / 3;
                int x = 15 + col * 350;
                int y = 48 + row * 50;
                LayoutLabel(spdLbls[i], x, y + 4, spdNames[i], 10, false, CText);
                LayoutCombo(spdCombos[i], x + 105, y, 115, 32);
            }

            // 运动控制
            LayoutPanel(panel7, 0, 165, 1060, 415);
            LayoutLabel(label25, 860, 10, "控制模式", 10, false, CTextDim);
            LayoutCombo(combmoshi, 925, 6, 120, 32);

            // 表头
            int hx = 15;
            LayoutHeader("轴名称", hx, 48, 85); hx += 95;
            LayoutHeader("点动", hx, 48, 110); hx += 120;
            LayoutHeader("长动", hx, 48, 110); hx += 120;
            LayoutHeader("缓停", hx, 48, 72); hx += 82;
            LayoutHeader("急停", hx, 48, 72);

            // 6行数据
            string[] mNames = { "X轴直动", "Y轴直动", "Z轴直动", "X轴转动", "Z轴转动", "抛光轮" };
            Label[] mLbls = { label26, label27, label28, label29, label30, label15 };
            Control[][] mCtrls = new Control[][] {
                new Control[] { butx1dian, butx2dian, butx1chang, butx2chang, butxchanghuanting, butxchangstop },
                new Control[] { buty1dian, buty2dian, buty1chang, buty2chang, butychanghuanting, butychangstop },
                new Control[] { butz1dian, butz2dian, butz1chang, butz2chang, butzchanghuanting, butzchangstop },
                new Control[] { pictx1dian, pictx2dian, pictx1chang, pictx2chang, butxzhuanhuanting, butxzhuanstop },
                new Control[] { pictz1dian, pictz2dian, pictz1chang, pictz2chang, butzzhuanhuanting, butzzhuanstop },
                new Control[] { butpao1dian, butpao2dian, butpao1chang, butpao2chang, butpaohuanting, butpaostop2 }
            };
            for (int i = 0; i < 6; i++)
            {
                int y = 85 + i * 52;
                LayoutLabel(mLbls[i], 15, y + 10, mNames[i], 10, false, CText);
                LayoutMotionBtn(mCtrls[i][0], 115, y, 44, 42, "+", CAccent);
                LayoutMotionBtn(mCtrls[i][1], 165, y, 44, 42, "-", CAccent);
                LayoutMotionBtn(mCtrls[i][2], 265, y, 44, 42, "+", CCyan);
                LayoutMotionBtn(mCtrls[i][3], 315, y, 44, 42, "-", CCyan);
                LayoutStopBtn((Button)mCtrls[i][4], 435, y, 70, 42, "缓停", Color.FromArgb(200, 140, 0));
                LayoutStopBtn((Button)mCtrls[i][5], 520, y, 70, 42, "急停", CRed);
            }

            // 大急停
            butjiting.Location = new Point(900, 48);
            butjiting.Size = new Size(145, 345);
            butjiting.BackColor = CRed;
            butjiting.ForeColor = Color.White;
            butjiting.Font = new Font("Microsoft YaHei", 16, FontStyle.Bold);
            butjiting.FlatStyle = FlatStyle.Flat;
            butjiting.FlatAppearance.BorderSize = 0;
            butjiting.Text = "紧急\r\n停止";
            butjiting.Cursor = Cursors.Hand;
            butjiting.Paint += (s, e) =>
            {
                var btn = s as Button;
                var r = new Rectangle(2, 2, btn.Width - 5, btn.Height - 5);
                using (var p = new Pen(Color.FromArgb(100, 255, 255, 255), 2))
                    e.Graphics.DrawRectangle(p, r);
            };

            // 轨迹设置
            LayoutPanel(panel3, 0, 590, 1060, 100);
            LayoutLabel(label53, 15, 8, "轨迹参数设置", 12, true, CAccent);
            LayoutLabel(label38, 15, 50, "轨迹", 10, false, CTextDim);
            LayoutCombo(combguiji, 55, 47, 130, 30);
            LayoutLabel(label54, 200, 50, "走向", 10, false, CTextDim);
            LayoutCombo(combzouxiang, 240, 47, 90, 30);
            LayoutLabel(label39, 350, 50, "X距离", 10, false, CTextDim);
            LayoutTextBox(textBxjuli, 395, 47, 75, 30);
            LayoutLabel(label40, 490, 50, "Y距离", 10, false, CTextDim);
            LayoutTextBox(textByjuli, 535, 47, 75, 30);
            LayoutLabel(label55, 630, 50, "次数", 10, false, CTextDim);
            LayoutTextBox(textBcishu, 670, 47, 75, 30);
            LayoutLabel(label41, 765, 50, "直径", 10, false, CTextDim);
            LayoutTextBox(textBzhijing, 805, 47, 75, 30);
            LayoutButton(butkaishi, 910, 43, 120, 38, "开始执行", COrange);

            // 隐藏未使用的标签
            Label[] hideLabels = { label31, label32, label33, label34, label35, label36 };
            foreach (var lbl in hideLabels) if (lbl != null) lbl.Visible = false;

            // 设置响应式锚定，支持窗口缩放
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            panel4.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            panel2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            panel5.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel6.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel7.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            // 右上角控件锚定到右侧
            butjiting.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            combmoshi.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label25.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        }

        private void LayoutPanel(Panel p, int x, int y, int w, int h)
        {
            if (p == null) return;
            p.Location = new Point(x, y);
            p.Size = new Size(w, h);
            p.BackColor = CPanel;
            p.BorderStyle = BorderStyle.None;
            p.Paint += (s, e) =>
            {
                using (var pen = new Pen(CBorder, 1))
                    e.Graphics.DrawRectangle(pen, 0, 0, p.Width - 1, p.Height - 1);
            };
        }

        private void LayoutLabel(Label lbl, int x, int y, string text, int fontSize, bool bold, Color color)
        {
            if (lbl == null) return;
            lbl.Location = new Point(x, y);
            lbl.AutoSize = true;
            lbl.Text = text;
            lbl.Font = new Font("Microsoft YaHei", fontSize, bold ? FontStyle.Bold : FontStyle.Regular);
            lbl.ForeColor = color;
            lbl.BackColor = Color.Transparent;
        }

        private void LayoutHeader(string text, int x, int y, int w)
        {
            Label lbl = new Label();
            lbl.Location = new Point(x, y);
            lbl.Size = new Size(w, 28);
            lbl.Text = text;
            lbl.Font = new Font("Microsoft YaHei", 10, FontStyle.Bold);
            lbl.ForeColor = CAccent;
            lbl.BackColor = Color.FromArgb(50, 55, 80);
            lbl.TextAlign = ContentAlignment.MiddleCenter;
            if (panel7 != null) panel7.Controls.Add(lbl);
        }

        private void LayoutLed(Label lbl, int x, int y, int w, int h)
        {
            if (lbl == null) return;
            lbl.Location = new Point(x, y);
            lbl.AutoSize = false;
            lbl.Size = new Size(w, h);
            lbl.BackColor = CRed;
            lbl.Text = "";
            lbl.Paint += (s, e) =>
            {
                var l = s as Label;
                using (var b = new SolidBrush(l.BackColor))
                    e.Graphics.FillEllipse(b, 2, 2, l.Width - 5, l.Height - 5);
                using (var p = new Pen(Color.FromArgb(80, 80, 100), 1))
                    e.Graphics.DrawEllipse(p, 2, 2, l.Width - 5, l.Height - 5);
            };
        }

        private void LayoutDisplay(Label lbl, int x, int y, int w, int h, string text = "0")
        {
            if (lbl == null) return;
            lbl.Location = new Point(x, y);
            lbl.AutoSize = false;
            lbl.Size = new Size(w, h);
            lbl.Text = text;
            lbl.Font = new Font("Consolas", 12, FontStyle.Bold);
            lbl.ForeColor = CAccent;
            lbl.BackColor = Color.FromArgb(15, 18, 30);
            lbl.TextAlign = ContentAlignment.MiddleCenter;
            lbl.BorderStyle = BorderStyle.None;
            lbl.Paint += (s, e) =>
            {
                var l = s as Label;
                using (var p = new Pen(CAccent, 1))
                    e.Graphics.DrawRectangle(p, 0, 0, l.Width - 1, l.Height - 1);
            };
        }

        private void LayoutButton(Button btn, int x, int y, int w, int h, string text, Color bg)
        {
            if (btn == null) return;
            btn.Location = new Point(x, y);
            btn.Size = new Size(w, h);
            btn.Text = text;
            btn.BackColor = bg;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Microsoft YaHei", 10, FontStyle.Bold);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Cursor = Cursors.Hand;
            btn.TextAlign = ContentAlignment.MiddleCenter;
            Color original = bg;
            btn.MouseEnter += (s, e) => { btn.BackColor = ControlPaint.Light(original, 0.15f); };
            btn.MouseLeave += (s, e) => { btn.BackColor = original; };
        }

        private void LayoutMotionBtn(Control ctrl, int x, int y, int w, int h, string text, Color bg)
        {
            if (ctrl == null) return;
            ctrl.Location = new Point(x, y);
            ctrl.Size = new Size(w, h);
            if (ctrl is Button btn)
            {
                btn.Text = text;
                btn.BackColor = bg;
                btn.ForeColor = Color.White;
                btn.Font = new Font("Microsoft YaHei", 12, FontStyle.Bold);
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.Cursor = Cursors.Hand;
                Color original = bg;
                btn.MouseEnter += (s, e) => { btn.BackColor = ControlPaint.Light(original, 0.15f); };
                btn.MouseLeave += (s, e) => { btn.BackColor = original; };
            }
            else if (ctrl is PictureBox pb)
            {
                pb.BackColor = CInput;
                pb.BorderStyle = BorderStyle.FixedSingle;
                pb.SizeMode = PictureBoxSizeMode.Zoom;
                pb.Cursor = Cursors.Hand;
            }
        }

        private void LayoutStopBtn(Button btn, int x, int y, int w, int h, string text, Color bg)
        {
            if (btn == null) return;
            btn.Location = new Point(x, y);
            btn.Size = new Size(w, h);
            btn.Text = text;
            btn.BackColor = bg;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Microsoft YaHei", 10, FontStyle.Bold);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Cursor = Cursors.Hand;
            Color original = bg;
            btn.MouseEnter += (s, e) => { btn.BackColor = ControlPaint.Light(original, 0.15f); };
            btn.MouseLeave += (s, e) => { btn.BackColor = original; };
        }

        private void LayoutCombo(ComboBox cmb, int x, int y, int w, int h)
        {
            if (cmb == null) return;
            cmb.Location = new Point(x, y);
            cmb.Size = new Size(w, h);
            cmb.Font = new Font("Microsoft YaHei", 10);
            cmb.BackColor = CInput;
            cmb.ForeColor = CText;
            cmb.FlatStyle = FlatStyle.Flat;
        }

        private void LayoutTextBox(TextBox txt, int x, int y, int w, int h)
        {
            if (txt == null) return;
            txt.Location = new Point(x, y);
            txt.Size = new Size(w, h);
            txt.Font = new Font("Consolas", 10);
            txt.BackColor = CInput;
            txt.ForeColor = CText;
            txt.BorderStyle = BorderStyle.FixedSingle;
            txt.TextAlign = HorizontalAlignment.Center;
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
                MessageBox.Show("控制系统已启动");
            }
            else
            {
                MessageBox.Show("控制系统已关闭");
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
            else if (combxsudu.SelectedIndex >= 1)
            {
                if (combmoshi.SelectedIndex == 0)
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
            else if (combysudu.SelectedIndex >= 1)
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
            if (combmoshi.SelectedIndex == 1)
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
            // 如果正在执行轨迹，先取消后台任务
            if (_isExecutingTrajectory && _trajectoryCts != null)
            {
                _trajectoryCts.Cancel();
            }

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


        private async void butkaishi_Click(object sender, EventArgs e)
        {
            if (_isExecutingTrajectory) return;

            // 读取所有 UI 值
            int guijiIndex = combguiji.SelectedIndex;
            int zouxiangIndex = combzouxiang.SelectedIndex;
            int zhijingVal = 0, xdisVal = 0, ydisVal = 0, cishuVal = 0;

            if (guijiIndex == 1)
            {
                if (!int.TryParse(textBzhijing.Text, out zhijingVal)) return;
            }
            else if (guijiIndex == 0)
            {
                if (textBxjuli.Text == "" || textByjuli.Text == "" || textBcishu.Text == "") return;
                xdisVal = Convert.ToInt16(textBxjuli.Text);
                ydisVal = Convert.ToInt16(textByjuli.Text);
                cishuVal = Convert.ToInt16(textBcishu.Text);
            }

            _isExecutingTrajectory = true;
            butkaishi.Enabled = false;
            butkaishi.Text = "执行中...";
            _trajectoryCts = new System.Threading.CancellationTokenSource();

            try
            {
                await System.Threading.Tasks.Task.Run(() =>
                    ExecuteTrajectory(guijiIndex, zouxiangIndex, zhijingVal, xdisVal, ydisVal, cishuVal, _trajectoryCts.Token));
            }
            catch (System.OperationCanceledException)
            {
                // 任务被取消，急停已处理
            }
            finally
            {
                _isExecutingTrajectory = false;
                if (butkaishi != null)
                {
                    butkaishi.Enabled = true;
                    butkaishi.Text = "开始执行";
                }
                _trajectoryCts?.Dispose();
                _trajectoryCts = null;
            }
        }

        /// <summary>
        /// 在后台线程执行轨迹运动
        /// </summary>
        private void ExecuteTrajectory(int guijiIndex, int zouxiangIndex, int zhijingVal, int xdisVal, int ydisVal, int cishuVal, System.Threading.CancellationToken token)
        {
            if (guijiIndex == 1)
            {
                Form1.zhijing[1] = zhijingVal;
                WJ_API.WJ_Move_Axis_Vel(4, Form1.vel[3]);
                System.Threading.Tasks.Task.Delay(1000, token).Wait();
                WJ_API.WJ_Move_Axis_Emergency_Stop(4);
                WJ_API.WJ_Move_Axis_Vel(1, Form1.vel[1]);
                System.Threading.Tasks.Task.Delay(Convert.ToInt16(Form1.spe[1] * (Form1.zhijing[1] / 2)), token).Wait();
                WJ_API.WJ_Move_Axis_Emergency_Stop(1);
                WJ_API.WJ_Move_Axis_Vel(4, -Form1.vel[3]);
                System.Threading.Tasks.Task.Delay(1000, token).Wait();
                WJ_API.WJ_Move_Axis_Emergency_Stop(4);
                for (double idx = (Form1.zhijing[1] / 2); idx > 0.0; idx = idx - 0.5)
                {
                    token.ThrowIfCancellationRequested();
                    WJ_API.WJ_Move_Axis_Vel(6, Form1.vel[5]);
                    System.Threading.Tasks.Task.Delay(Convert.ToInt16(Form1.spe[5] * 5), token).Wait();
                    WJ_API.WJ_Move_Axis_Emergency_Stop(6);
                    WJ_API.WJ_Move_Axis_Vel(1, -Form1.vel[1]);
                    System.Threading.Tasks.Task.Delay(Convert.ToInt16(Form1.spe[1] * 0.25), token).Wait();
                    WJ_API.WJ_Move_Axis_Emergency_Stop(1);
                }
                WJ_API.WJ_Move_Axis_Vel(4, Form1.vel[3]);
                System.Threading.Tasks.Task.Delay(1000, token).Wait();
                WJ_API.WJ_Move_Axis_Emergency_Stop(4);
            }
            else if (guijiIndex == 0)
            {
                Form1.xdis[1] = xdisVal;
                Form1.ydis[1] = ydisVal;
                Form1.cishu[1] = cishuVal;
                Form1.xtim[1] = Form1.spe[1] * Form1.xdis[1];
                Form1.ytim[1] = Form1.spe[2] * (Form1.ydis[1] / Form1.cishu[1]);

                if (zouxiangIndex == 0)
                {
                    int a = 1;
                    for (int idx = 1; idx <= Form1.cishu[1]; idx++)
                    {
                        token.ThrowIfCancellationRequested();
                        if (a == 1)
                        {
                            WJ_API.WJ_Move_Axis_Vel(4, -Form1.vel[3]);
                            System.Threading.Tasks.Task.Delay(1000, token).Wait();
                            WJ_API.WJ_Move_Axis_Emergency_Stop(4);
                        }
                        if (idx % 2 == 1)
                        {
                            WJ_API.WJ_Move_Axis_Vel(1, -Form1.vel[1]);
                            System.Threading.Tasks.Task.Delay(Convert.ToInt16(Form1.xtim[1]), token).Wait();
                            WJ_API.WJ_Move_Axis_Emergency_Stop(1);

                            if (idx < Form1.cishu[1])
                            {
                                WJ_API.WJ_Move_Axis_Vel(2, Form1.vel[2]);
                                System.Threading.Tasks.Task.Delay(Convert.ToInt16(Form1.ytim[1]), token).Wait();
                                WJ_API.WJ_Move_Axis_Emergency_Stop(2);
                            }
                        }
                        if (idx % 2 == 0)
                        {
                            WJ_API.WJ_Move_Axis_Vel(1, Form1.vel[1]);
                            System.Threading.Tasks.Task.Delay(Convert.ToInt16(Form1.xtim[1]), token).Wait();
                            WJ_API.WJ_Move_Axis_Emergency_Stop(1);
                            if (idx < Form1.cishu[1])
                            {
                                WJ_API.WJ_Move_Axis_Vel(2, Form1.vel[2]);
                                System.Threading.Tasks.Task.Delay(Convert.ToInt16(Form1.ytim[1]), token).Wait();
                                WJ_API.WJ_Move_Axis_Emergency_Stop(2);
                            }
                        }
                        if (a == Form1.cishu[1])
                        {
                            WJ_API.WJ_Move_Axis_Vel(4, Form1.vel[3]);
                            System.Threading.Tasks.Task.Delay(1000, token).Wait();
                            WJ_API.WJ_Move_Axis_Emergency_Stop(4);
                        }
                        a++;
                    }
                }
                else if (zouxiangIndex == 1)
                {
                    Form1.ytim[1] = Form1.spe[2] * Form1.ydis[1];
                    Form1.xtim[1] = Form1.spe[1] * (Form1.xdis[1] / Form1.cishu[1]);
                    int a = 1;
                    for (int idx = 1; idx <= Form1.cishu[1]; idx++)
                    {
                        token.ThrowIfCancellationRequested();
                        if (a == 1)
                        {
                            WJ_API.WJ_Move_Axis_Vel(4, -Form1.vel[3]);
                            System.Threading.Tasks.Task.Delay(1000, token).Wait();
                            WJ_API.WJ_Move_Axis_Emergency_Stop(4);
                        }
                        if (idx % 2 == 1)
                        {
                            WJ_API.WJ_Move_Axis_Vel(2, Form1.vel[2]);
                            System.Threading.Tasks.Task.Delay(Convert.ToInt16(Form1.ytim[1]), token).Wait();
                            WJ_API.WJ_Move_Axis_Emergency_Stop(2);

                            if (idx < Form1.cishu[1])
                            {
                                WJ_API.WJ_Move_Axis_Vel(1, -Form1.vel[1]);
                                System.Threading.Tasks.Task.Delay(Convert.ToInt16(Form1.xtim[1]), token).Wait();
                                WJ_API.WJ_Move_Axis_Emergency_Stop(1);
                            }
                        }
                        if (idx % 2 == 0)
                        {
                            WJ_API.WJ_Move_Axis_Vel(2, -Form1.vel[2]);
                            System.Threading.Tasks.Task.Delay(Convert.ToInt16(Form1.ytim[1]), token).Wait();
                            WJ_API.WJ_Move_Axis_Emergency_Stop(2);
                            if (idx < Form1.cishu[1])
                            {
                                WJ_API.WJ_Move_Axis_Vel(1, -Form1.vel[1]);
                                System.Threading.Tasks.Task.Delay(Convert.ToInt16(Form1.xtim[1]), token).Wait();
                                WJ_API.WJ_Move_Axis_Emergency_Stop(1);
                            }
                        }
                        if (a == Form1.cishu[1])
                        {
                            WJ_API.WJ_Move_Axis_Vel(4, Form1.vel[3]);
                            System.Threading.Tasks.Task.Delay(1000, token).Wait();
                            WJ_API.WJ_Move_Axis_Emergency_Stop(4);
                        }
                        a++;
                    }
                }
            }
        }

        /// <summary>
        /// 创建模拟动画面板
        /// </summary>
        private void CreateSimulationPanel()
        {
            // 创建主面板 - 深色科技风
            Panel simPanel = new Panel();
            simPanel.Location = new Point(20, 740);
            simPanel.Size = new Size(1350, 240);
            simPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            simPanel.BorderStyle = BorderStyle.None;
            simPanel.BackColor = Color.FromArgb(44, 47, 68);
            simPanel.Padding = new Padding(15);

            // 添加边框
            simPanel.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(80, 80, 100), 1))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, simPanel.Width - 1, simPanel.Height - 1);
                }
            };

            // 标题 - 霓虹蓝
            Label titleLabel = new Label();
            titleLabel.Text = "🎯 实时运动状态监控";
            titleLabel.Font = new Font("Microsoft YaHei", 14, FontStyle.Bold);
            titleLabel.Location = new Point(15, 8);
            titleLabel.Size = new Size(300, 28);
            titleLabel.ForeColor = Color.FromArgb(0, 230, 255); // 青色霓虹
            simPanel.Controls.Add(titleLabel);

            // 创建表头
            string[] headers = { "轴名称", "当前位置(pulse)", "当前速度", "运动状态" };
            int[] headerWidths = { 160, 180, 160, 140 };
            int headerY = 40;
            int xPos = 20;

            for (int i = 0; i < headers.Length; i++)
            {
                Label header = new Label();
                header.Text = headers[i];
                header.Font = new Font("Microsoft YaHei", 10, FontStyle.Bold);
                header.Location = new Point(xPos, headerY);
                header.Size = new Size(headerWidths[i], 28);
                header.BackColor = Color.FromArgb(60, 63, 90);
                header.ForeColor = Color.FromArgb(0, 230, 255);
                header.TextAlign = ContentAlignment.MiddleCenter;
                header.BorderStyle = BorderStyle.None;
                simPanel.Controls.Add(header);
                xPos += headerWidths[i] + 8;
            }

            // 为每个轴创建显示标签
            _axisPositionLabels = new Label[_axisNumbers.Length];
            _axisVelocityLabels = new Label[_axisNumbers.Length];
            _axisStatusLabels = new Label[_axisNumbers.Length];

            for (int i = 0; i < _axisNumbers.Length; i++)
            {
                int yPos = 68 + i * 26;
                xPos = 20;

                // 轴名称
                Label lblAxisName = new Label();
                lblAxisName.Text = $"{_axisNames[i]} (轴{_axisNumbers[i]})";
                lblAxisName.Font = new Font("Microsoft YaHei", 10, FontStyle.Regular);
                lblAxisName.Location = new Point(xPos, yPos);
                lblAxisName.Size = new Size(130, 28);
                lblAxisName.TextAlign = ContentAlignment.MiddleLeft;
                lblAxisName.ForeColor = Color.FromArgb(224, 224, 240);
                simPanel.Controls.Add(lblAxisName);
                xPos += 138;

                // 位置标签 - 数字显示屏风格
                _axisPositionLabels[i] = new Label();
                _axisPositionLabels[i].Font = new Font("Consolas", 11, FontStyle.Bold);
                _axisPositionLabels[i].Location = new Point(xPos, yPos);
                _axisPositionLabels[i].Size = new Size(150, 28);
                _axisPositionLabels[i].Text = "0";
                _axisPositionLabels[i].BackColor = Color.FromArgb(20, 20, 35);
                _axisPositionLabels[i].ForeColor = Color.FromArgb(0, 230, 255);
                _axisPositionLabels[i].TextAlign = ContentAlignment.MiddleCenter;
                _axisPositionLabels[i].BorderStyle = BorderStyle.FixedSingle;
                _axisPositionLabels[i].Paint += (s, e) =>
                {
                    var lbl = s as Label;
                    using (var pen = new Pen(Color.FromArgb(0, 230, 255), 1))
                    {
                        e.Graphics.DrawRectangle(pen, 0, 0, lbl.Width - 1, lbl.Height - 1);
                    }
                };
                simPanel.Controls.Add(_axisPositionLabels[i]);
                xPos += 158;

                // 速度标签 - 数字显示屏风格
                _axisVelocityLabels[i] = new Label();
                _axisVelocityLabels[i].Font = new Font("Consolas", 11, FontStyle.Bold);
                _axisVelocityLabels[i].Location = new Point(xPos, yPos);
                _axisVelocityLabels[i].Size = new Size(130, 28);
                _axisVelocityLabels[i].Text = "0";
                _axisVelocityLabels[i].BackColor = Color.FromArgb(20, 20, 35);
                _axisVelocityLabels[i].ForeColor = Color.FromArgb(0, 200, 83); // 绿色霓虹
                _axisVelocityLabels[i].TextAlign = ContentAlignment.MiddleCenter;
                _axisVelocityLabels[i].BorderStyle = BorderStyle.FixedSingle;
                _axisVelocityLabels[i].Paint += (s, e) =>
                {
                    var lbl = s as Label;
                    using (var pen = new Pen(Color.FromArgb(0, 200, 83), 1))
                    {
                        e.Graphics.DrawRectangle(pen, 0, 0, lbl.Width - 1, lbl.Height - 1);
                    }
                };
                simPanel.Controls.Add(_axisVelocityLabels[i]);
                xPos += 138;

                // 状态标签 - LED指示灯风格
                _axisStatusLabels[i] = new Label();
                _axisStatusLabels[i].Font = new Font("Microsoft YaHei", 10, FontStyle.Bold);
                _axisStatusLabels[i].Location = new Point(xPos, yPos);
                _axisStatusLabels[i].Size = new Size(110, 28);
                _axisStatusLabels[i].Text = "● 空闲";
                _axisStatusLabels[i].BackColor = Color.FromArgb(0, 200, 83); // 绿色
                _axisStatusLabels[i].ForeColor = Color.White;
                _axisStatusLabels[i].TextAlign = ContentAlignment.MiddleCenter;
                _axisStatusLabels[i].BorderStyle = BorderStyle.None;
                simPanel.Controls.Add(_axisStatusLabels[i]);
            }

            // 添加说明文字
            Label noteLabel = new Label();
            noteLabel.Text = "💡 提示：所有运动控制功能均已启用软件模拟模式，无需实际硬件即可测试";
            noteLabel.Font = new Font("Microsoft YaHei", 9, FontStyle.Italic);
            noteLabel.Location = new Point(15, 235);
            noteLabel.Size = new Size(700, 22);
            noteLabel.ForeColor = Color.FromArgb(150, 150, 170);
            simPanel.Controls.Add(noteLabel);

            // 将面板添加到窗体
            this.Controls.Add(simPanel);
        }

        /// <summary>
        /// 模拟定时器更新
        /// </summary>
        private void SimulationTimer_Tick(object sender, EventArgs e)
        {
            // 更新所有轴的显示
            for (int i = 0; i < _axisNumbers.Length; i++)
            {
                int axisNum = _axisNumbers[i];

                // 获取位置
                int position = 0;
                WJ_API.WJ_Get_Axis_Pulses(axisNum, ref position);
                _axisPositionLabels[i].Text = position.ToString();

                // 获取速度
                int velocity = 0;
                WJ_API.WJ_Get_Axis_Vel(axisNum, ref velocity);
                _axisVelocityLabels[i].Text = velocity.ToString();

                // 获取状态
                int status = 0;
                WJ_API.WJ_Get_Axis_Status(axisNum, ref status);

                switch (status)
                {
                    case 0: // Idle
                        _axisStatusLabels[i].Text = "● 空闲";
                        _axisStatusLabels[i].BackColor = Color.FromArgb(0, 200, 83); // 绿色霓虹
                        _axisStatusLabels[i].ForeColor = Color.White;
                        break;
                    case 1: // Running
                        _axisStatusLabels[i].Text = "▶ 运动中";
                        _axisStatusLabels[i].BackColor = Color.FromArgb(0, 150, 255); // 蓝色霓虹
                        _axisStatusLabels[i].ForeColor = Color.White;
                        break;
                    case 2: // Stopping
                        _axisStatusLabels[i].Text = "⏹ 停止中";
                        _axisStatusLabels[i].BackColor = Color.FromArgb(255, 193, 7); // 黄色警示
                        _axisStatusLabels[i].ForeColor = Color.Black;
                        break;
                    case 3: // Error
                        _axisStatusLabels[i].Text = "✖ 错误";
                        _axisStatusLabels[i].BackColor = Color.FromArgb(255, 23, 68); // 红色危险
                        _axisStatusLabels[i].ForeColor = Color.White;
                        break;
                }
            }

            // 更新坐标显示（如果面板存在）
            try
            {
                int x = 0, y = 0, z = 0;
                WJ_API.WJ_Get_Axis_Pulses(1, ref x);
                WJ_API.WJ_Get_Axis_Pulses(2, ref y);
                WJ_API.WJ_Get_Axis_Pulses(4, ref z);

                int displayX = x - _offsetX;
                int displayY = y - _offsetY;
                int displayZ = z - _offsetZ;

                labelxzuobiao.Text = displayX.ToString();
                labelyzuobiao.Text = displayY.ToString();
                labelzzuobiao.Text = displayZ.ToString();
                labelxyzzuobiao.Text = $"({displayX},{displayY},{displayZ})";
            }
            catch
            {
                // 忽略坐标更新错误
            }
        }

        /// <summary>
        /// 清除坐标原点偏移，恢复绝对坐标显示
        /// </summary>
        private void butzuobiaoqingchu_Click(object sender, EventArgs e)
        {
            _offsetX = 0;
            _offsetY = 0;
            _offsetZ = 0;
            MessageBox.Show("坐标原点已清除，恢复为绝对坐标", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 将当前位置设为坐标原点
        /// </summary>
        private void butzuobiaoqueding_Click(object sender, EventArgs e)
        {
            int x = 0, y = 0, z = 0;
            WJ_API.WJ_Get_Axis_Pulses(1, ref x);
            WJ_API.WJ_Get_Axis_Pulses(2, ref y);
            WJ_API.WJ_Get_Axis_Pulses(4, ref z);

            _offsetX = x;
            _offsetY = y;
            _offsetZ = z;

            MessageBox.Show($"当前坐标已设为原点\n偏移量: X={x}, Y={y}, Z={z}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

    }
}
