using HslCommunication;
using HslCommunication.Profinet.Melsec;
using MachineLog;
using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace _19_601
{
    public partial class FormStart : Form
    {
        #region --define--
        private System.Windows.Forms.Timer _t1;
        private MelsecMcNet PlcRead;
        private csLog _ErrLog;
        private MachineStatus myStatus;
        #endregion
        #region --Form--
        public FormStart()  
        {
            InitializeComponent();
            myStatus = new MachineStatus();
            _ErrLog = new csLog();
            Initialize();
        }
        private void FormStart_Load(object sender, EventArgs e)
        {
            
        }
        /// <summary>
        /// 取得初始檔案,初始化
        /// </summary>
        private void Initialize()
        {
            string initFilename = @"D:\Resource\MachineInitialFile_19601.txt";
            try
            {
                string[] sif = File.ReadAllLines(initFilename);
                foreach (var x in sif)
                {
                    string[] ss = x.Split(':');
                    if (ss[0].Equals("IP"))
                    {
                        myStatus.PlcIP = ss[1];
                    }
                    if (ss[0].Equals("Port"))
                    {
                        myStatus.PlcPort = Convert.ToInt32(ss[1]);
                    }
                }
                //取得檔案OK,建立PLC連線
                CreatPLCConnect();
                //建立timer
                OperateResult opr = PlcRead.ConnectServer();
                if (opr.IsSuccess)
                {
                    _t1 = new System.Windows.Forms.Timer();
                    _t1.Interval = 1000;
                    _t1.Tick += _t1_Tick;
                    _t1.Start();
                }
                else
                {
                    _ErrLog.LogText = "連線建立失敗!!" + Environment.NewLine;
                    MessageBox.Show("PLC未連線!!請關閉軟件,檢查連線後重開!!");
                }
            }
            catch (Exception err)
            {
                _ErrLog.LogText ="開始初始檔案時發生異常:"+ err.ToString()+Environment.NewLine;
                MessageBox.Show(@"未取得初始化資料!!請確認D:\Resource\MachineInitialFile_19601.txt檔案是否正確?");
            }
        }
        /// <summary>
        /// 建立PLC連線
        /// </summary>
        private void CreatPLCConnect()
        {
            if (myStatus.PlcPort != 0)
            {
                PlcRead = new MelsecMcNet(myStatus.PlcIP, myStatus.PlcPort+2);
            }
            else
            {
                MessageBox.Show(@"未取得PLC資料!!請確認D:\Resource\MachineInitialFile_19601.txt檔案是否正確?"); 
            }
        }
        #endregion
        #region --Timer 1--
        private void _t1_Tick(object sender, EventArgs e)
        {
            _t1.Stop();
            //檢查是否按下運轉準備,如果按下開始FORM1
            bool[] b = PlcRead.ReadBool("Y0",48).Content;
            if (b!=null)
            {
                if (b[32])
                {
                    this.Hide();
                }
                else
                {
                    this.Show();
                    this.Activate();
                }
            }
            _t1.Start();
        }
        #endregion
        #region --Test--
        private void button1_Click(object sender, EventArgs e)
        {
            FormOp1 f1 = new FormOp1();
            f1.Show();
        }
        #endregion
    }
}
