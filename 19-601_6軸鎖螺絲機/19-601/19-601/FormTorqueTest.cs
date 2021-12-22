using HslCommunication.Profinet.Melsec;
using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace _19_601
{
    public partial class FormTorqueTest : Form
    {
        private MelsecMcNet plcRead, plcWrite;
        private string PlcIPAddress;
        private int PlcPort;
        private System.Windows.Forms.Timer t1;
        public string Enable = "";
        public string Disable = "";

        public FormTorqueTest()
        {
            InitializeComponent();
        }

        private void FormTorqueTest_Load(object sender, EventArgs e)
        {
            this.FormClosing += FormTorqueTest_FormClosing;
            string initFilename = @"D:\Resource\MachineInitialFile_19601.txt";
            //初始檔
            try
            {
                string[] sif = File.ReadAllLines(initFilename);
                foreach (var x in sif)
                {
                    string[] ss = x.Split(':');
                    if (ss[0].Equals("IP"))
                    {
                        PlcIPAddress = ss[1];
                    }
                    if (ss[0].Equals("Port"))
                    {
                        PlcPort = Convert.ToInt32(ss[1]);
                    }
                }
                //取得檔案OK,建立PLC連線
                if (PlcPort != 0)
                {
                    plcRead = new MelsecMcNet(PlcIPAddress, PlcPort+3);
                    plcRead.ConnectTimeOut = 3000;
                    plcWrite = new MelsecMcNet(PlcIPAddress, PlcPort + 4);
                    plcWrite.ConnectTimeOut = 3000;

                    t1 = new System.Windows.Forms.Timer();
                    t1.Interval = 500;
                    t1.Tick += T1_Tick;
                    t1.Start();
                }
                else
                {
                    MessageBox.Show(@"未取得PLC資料!!請確認D:\Resource\MachineInitialFile.txt檔案是否正確?");
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }

        private void T1_Tick(object sender, EventArgs e)
        {
            t1.Stop();
            bool b= plcRead.ReadBool("M1126").Content;
            if (b)
            {
                pbTorqueCheck.BackColor = Color.Yellow;
                pbTorqueCheck.Text = Enable;
            }
            else
            {
                pbTorqueCheck.BackColor = Color.White;
                pbTorqueCheck.Text = Disable;
            }
            b = plcWrite.ReadBool("M1141").Content;
            if (b)
            {
                pbDoubleScrew.BackColor = Color.Yellow;
                pbDoubleScrew.Text = "再次鎖付";
            }
            else
            {
                pbDoubleScrew.BackColor = Color.White;
                pbDoubleScrew.Text = "扭力測試";
            }
            short channel = plcRead.ReadInt16("R1100").Content;
            lbChannelNo_PTest.Text = channel.ToString();
            t1.Start();
        }

        private void pbTorqueCheck_Click(object sender, EventArgs e)
        {
            plcWrite.Write("M1126", false);
            Thread.Sleep(50);
            plcWrite.Write("M1000", false);
            this.Hide();
        }

        private void pbTestStart_Click(object sender, EventArgs e)
        {
            plcWrite.Write("M1137", true);
            Thread.Sleep(300);
            plcWrite.Write("M1137", false);
        }

        private void pbDoubleScrew_Click(object sender, EventArgs e)
        {
            bool state = plcRead.ReadBool("M1141").Content;
            state = !state;
            plcWrite.Write("M1141", state);
        }

        private void FormTorqueTest_FormClosing(object sender, FormClosingEventArgs e)
        {
        }
    }
}
