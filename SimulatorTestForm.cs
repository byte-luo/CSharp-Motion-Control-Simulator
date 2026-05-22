using System;
using System.Windows.Forms;
using System.Drawing;

namespace LQF_PolishingSystem
{
    public class SimulatorTestForm : Form
    {
        private Timer _updateTimer;
        private Label[] _positionLabels;
        private Label[] _velocityLabels;
        private Label[] _statusLabels;
        private int[] _axisNumbers = { 1, 2, 4, 5, 6, 7 };
        private string[] _axisNames = { "X轴", "Y轴", "Z轴", "A轴", "B轴", "C轴" };

        public SimulatorTestForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "运动控制模拟器测试 - lqf";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // 创建更新定时器
            _updateTimer = new Timer();
            _updateTimer.Interval = 100;
            _updateTimer.Tick += UpdateTimer_Tick;
            _updateTimer.Start();

            // 创建控制面板
            CreateControlPanel();

            // 创建状态显示面板
            CreateStatusPanel();
        }

        private void CreateControlPanel()
        {
            Panel controlPanel = new Panel();
            controlPanel.Location = new Point(10, 10);
            controlPanel.Size = new Size(760, 100);
            controlPanel.BorderStyle = BorderStyle.FixedSingle;

            // 打开设备按钮
            Button btnOpen = new Button();
            btnOpen.Text = "打开设备";
            btnOpen.Location = new Point(10, 10);
            btnOpen.Size = new Size(100, 40);
            btnOpen.Click += (s, e) =>
            {
                int result = WJ_API.WJ_Open(0);
                MessageBox.Show(result == 0 ? "设备打开成功！" : "设备打开失败！");
            };
            controlPanel.Controls.Add(btnOpen);

            // 关闭设备按钮
            Button btnClose = new Button();
            btnClose.Text = "关闭设备";
            btnClose.Location = new Point(120, 10);
            btnClose.Size = new Size(100, 40);
            btnClose.Click += (s, e) =>
            {
                WJ_API.WJ_Close();
                MessageBox.Show("设备已关闭");
            };
            controlPanel.Controls.Add(btnClose);

            // X轴运动按钮
            Button btnXMove = new Button();
            btnXMove.Text = "X轴正向运动";
            btnXMove.Location = new Point(10, 60);
            btnXMove.Size = new Size(120, 30);
            btnXMove.Click += (s, e) =>
            {
                WJ_API.WJ_Move_Axis_Vel(1, 50);
            };
            controlPanel.Controls.Add(btnXMove);

            Button btnXStop = new Button();
            btnXStop.Text = "X轴停止";
            btnXStop.Location = new Point(140, 60);
            btnXStop.Size = new Size(100, 30);
            btnXStop.Click += (s, e) =>
            {
                WJ_API.WJ_Move_Axis_Emergency_Stop(1);
            };
            controlPanel.Controls.Add(btnXStop);

            // Y轴运动按钮
            Button btnYMove = new Button();
            btnYMove.Text = "Y轴正向运动";
            btnYMove.Location = new Point(260, 60);
            btnYMove.Size = new Size(120, 30);
            btnYMove.Click += (s, e) =>
            {
                WJ_API.WJ_Move_Axis_Vel(2, 50);
            };
            controlPanel.Controls.Add(btnYMove);

            Button btnYStop = new Button();
            btnYStop.Text = "Y轴停止";
            btnYStop.Location = new Point(390, 60);
            btnYStop.Size = new Size(100, 30);
            btnYStop.Click += (s, e) =>
            {
                WJ_API.WJ_Move_Axis_Emergency_Stop(2);
            };
            controlPanel.Controls.Add(btnYStop);

            // 全部急停按钮
            Button btnEmergency = new Button();
            btnEmergency.Text = "全部急停";
            btnEmergency.Location = new Point(520, 60);
            btnEmergency.Size = new Size(100, 30);
            btnEmergency.BackColor = Color.Red;
            btnEmergency.ForeColor = Color.White;
            btnEmergency.Click += (s, e) =>
            {
                foreach (int axis in _axisNumbers)
                {
                    WJ_API.WJ_Move_Axis_Emergency_Stop(axis);
                }
            };
            controlPanel.Controls.Add(btnEmergency);

            this.Controls.Add(controlPanel);
        }

        private void CreateStatusPanel()
        {
            Panel statusPanel = new Panel();
            statusPanel.Location = new Point(10, 120);
            statusPanel.Size = new Size(760, 400);
            statusPanel.BorderStyle = BorderStyle.FixedSingle;

            // 标题
            Label titleLabel = new Label();
            titleLabel.Text = "轴状态监控";
            titleLabel.Font = new Font("Microsoft YaHei", 12, FontStyle.Bold);
            titleLabel.Location = new Point(10, 10);
            titleLabel.Size = new Size(200, 30);
            statusPanel.Controls.Add(titleLabel);

            // 创建表头
            Label lblAxis = new Label();
            lblAxis.Text = "轴号";
            lblAxis.Location = new Point(20, 50);
            lblAxis.Size = new Size(60, 20);
            statusPanel.Controls.Add(lblAxis);

            Label lblPosition = new Label();
            lblPosition.Text = "当前位置";
            lblPosition.Location = new Point(100, 50);
            lblPosition.Size = new Size(100, 20);
            statusPanel.Controls.Add(lblPosition);

            Label lblVelocity = new Label();
            lblVelocity.Text = "当前速度";
            lblVelocity.Location = new Point(250, 50);
            lblVelocity.Size = new Size(100, 20);
            statusPanel.Controls.Add(lblVelocity);

            Label lblStatus = new Label();
            lblStatus.Text = "状态";
            lblStatus.Location = new Point(400, 50);
            lblStatus.Size = new Size(100, 20);
            statusPanel.Controls.Add(lblStatus);

            // 为每个轴创建显示标签
            _positionLabels = new Label[_axisNumbers.Length];
            _velocityLabels = new Label[_axisNumbers.Length];
            _statusLabels = new Label[_axisNumbers.Length];

            for (int i = 0; i < _axisNumbers.Length; i++)
            {
                int yPos = 80 + i * 50;

                // 轴名称
                Label lblAxisName = new Label();
                lblAxisName.Text = $"{_axisNames[i]} (轴{_axisNumbers[i]})";
                lblAxisName.Location = new Point(20, yPos);
                lblAxisName.Size = new Size(100, 20);
                statusPanel.Controls.Add(lblAxisName);

                // 位置标签
                _positionLabels[i] = new Label();
                _positionLabels[i].Location = new Point(100, yPos);
                _positionLabels[i].Size = new Size(120, 20);
                _positionLabels[i].Text = "0";
                statusPanel.Controls.Add(_positionLabels[i]);

                // 速度标签
                _velocityLabels[i] = new Label();
                _velocityLabels[i].Location = new Point(250, yPos);
                _velocityLabels[i].Size = new Size(120, 20);
                _velocityLabels[i].Text = "0";
                statusPanel.Controls.Add(_velocityLabels[i]);

                // 状态标签
                _statusLabels[i] = new Label();
                _statusLabels[i].Location = new Point(400, yPos);
                _statusLabels[i].Size = new Size(100, 20);
                _statusLabels[i].Text = "空闲";
                _statusLabels[i].BackColor = Color.LightGreen;
                statusPanel.Controls.Add(_statusLabels[i]);
            }

            this.Controls.Add(statusPanel);
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            // 更新所有轴的显示
            for (int i = 0; i < _axisNumbers.Length; i++)
            {
                int axisNum = _axisNumbers[i];

                // 获取位置
                int position = 0;
                WJ_API.WJ_Get_Axis_Pulses(axisNum, ref position);
                _positionLabels[i].Text = position.ToString();

                // 获取速度
                int velocity = 0;
                WJ_API.WJ_Get_Axis_Vel(axisNum, ref velocity);
                _velocityLabels[i].Text = velocity.ToString();

                // 获取状态
                int status = 0;
                WJ_API.WJ_Get_Axis_Status(axisNum, ref status);
                
                switch (status)
                {
                    case 0: // Idle
                        _statusLabels[i].Text = "空闲";
                        _statusLabels[i].BackColor = Color.LightGreen;
                        break;
                    case 1: // Running
                        _statusLabels[i].Text = "运动中";
                        _statusLabels[i].BackColor = Color.LightBlue;
                        break;
                    case 2: // Stopping
                        _statusLabels[i].Text = "停止中";
                        _statusLabels[i].BackColor = Color.Yellow;
                        break;
                    case 3: // Error
                        _statusLabels[i].Text = "错误";
                        _statusLabels[i].BackColor = Color.Red;
                        break;
                }
            }
        }
    }
}
