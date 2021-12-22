using HslCommunication;
using HslCommunication.Profinet.Melsec;
using MachineLog;
using NittoDriver;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ClosedXML.Excel;
using MitsubishiPLC;

namespace _19_601
{
    public partial class FormOp1 : Form
    {
        #region --define--
        FormTorqueTest ftt;
        csTorqueValue myTV = new csTorqueValue();
        private NittoDriver.csNittoDriver Driver = null;
        private MelsecMcNet PlcRead;
        private csFX5 PlcWrite;
        private System.Windows.Forms.Timer tPbTorqueTest;
        private System.Windows.Forms.Timer _t1,_t2,_t3;
        private MachineStatus myStatus;
        private csLog myLog;
        private bool[] M1000 = new bool[4];
        private bool[] M1400=new bool[14];//M1400 自動,原點,異常,螺絲不足,吹螺絲SOL
        private bool[] M1126;//M1126設備設定
        private bool[] M1200;//鎖付狀態
        private bool[] M3000, _m3000;//異常
        private Int16[] R1000;//R1001 頻道,R1002 Cycle Time
        private Int16[] R1100;
        private Int32[] R2000;//R2000時間
        private Int32 R5004;//R5004 All count
        private bool[] X0;//X0-X77
        private bool[] Y0;//Y0-Y57
        private Label[] lampX0;
        private Label[] lampY0;
        private Label[] lbScrewState;
        private Label[] lbTorqueValue;
        private Button[] pbY0;
        private bool readTorqueFlag = false;
        bool firstRunFlag = false;
        bool clearTorqueFlag = false;
        //Error
        private string[] ErrorMessage;
        bool ErrorStatus;
        //語言
        private int LanguageSelection = 0;
        private string[] LanguageString;
        #region --虛假的物件,有新增就要拿掉--
        //假的物件如果有新增就拿掉
        private Label lbX0000, lbX0001, lbX0002, lbX0003,
            lbX0050, lbX0051, lbX0052, lbX0053, lbX0054, lbX0055, lbX0056, lbX0057,
            lbX0060, lbX0061, lbX0062, lbX0063, lbX0064, lbX0065, lbX0066, lbX0067,
            lbX0070, lbX0071, lbX0072, lbX0073, lbX0074, lbX0075, lbX0076, lbX0077,
            lbY0003, lbY0005, lbY0031, lbY0032, lbY0034, lbY0035, lbY0036, lbY0037,
            lbY0040, lbY0041, lbY0042, lbY0043, lbY0044, lbY0045, lbY0046, lbY0047,
            lbY0050, lbY0051, lbY0052, lbY0053, lbY0054, lbY0055, lbY0056, lbY0057;
        private Label lbScrewState7, lbScrewState8, lbScrewState9, lbScrewState10;
        private Label lbTorqueAx7, lbTorqueAx8, lbTorqueAx9, lbTorqueAx10;
        private Button pbY0003, pbY0005, pbY0031, pbY0032, pbY0034, pbY0035, pbY0036, pbY0037,
            pbY0040, pbY0041, pbY0042, pbY0043, pbY0044, pbY0045, pbY0046, pbY0047,
            pbY0050, pbY0051, pbY0052, pbY0053, pbY0054, pbY0055, pbY0056, pbY0057;

        private void FormOp1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Driver = null;
        }

        /// <summary>
        /// 建構各虛假的物件
        /// </summary>
        private void NewLabel()
        {
            lbX0000 = new Label(); lbX0001 = new Label(); lbX0002 = new Label(); lbX0003 = new Label();
            lbX0050 = new Label(); lbX0051 = new Label(); lbX0052 = new Label(); lbX0053 = new Label();
            lbX0054 = new Label(); lbX0055 = new Label(); lbX0056 = new Label(); lbX0057 = new Label();
            lbX0060 = new Label(); lbX0061 = new Label(); lbX0062 = new Label(); lbX0063 = new Label();
            lbX0064 = new Label(); lbX0065 = new Label(); lbX0066 = new Label(); lbX0067 = new Label();
            lbX0070 = new Label(); lbX0071 = new Label(); lbX0072 = new Label(); lbX0073 = new Label();
            lbX0074 = new Label(); lbX0075 = new Label(); lbX0076 = new Label(); lbX0077 = new Label();
            lbY0003 = new Label(); lbY0005 = new Label();
            lbY0031 = new Label(); lbY0032 = new Label();
            lbY0034 = new Label(); lbY0035 = new Label(); lbY0036 = new Label(); lbY0037 = new Label();
            lbY0040 = new Label(); lbY0041 = new Label(); lbY0042 = new Label(); lbY0043 = new Label();
            lbY0044 = new Label(); lbY0045 = new Label(); lbY0046 = new Label(); lbY0047 = new Label();
            lbY0050 = new Label(); lbY0051 = new Label(); lbY0052 = new Label(); lbY0053 = new Label();
            lbY0054 = new Label(); lbY0055 = new Label(); lbY0056 = new Label(); lbY0057 = new Label();
            lbScrewState7 = new Label(); lbScrewState8 = new Label(); lbScrewState9 = new Label(); lbScrewState10 = new Label();
            lbTorqueAx7 = new Label();lbTorqueAx8 = new Label();lbTorqueAx9 = new Label();lbTorqueAx10 = new Label();
            pbY0003 = new Button(); pbY0005 = new Button();
            pbY0031 = new Button(); pbY0032 = new Button();
            pbY0034 = new Button(); pbY0035 = new Button(); pbY0036 = new Button(); pbY0037 = new Button();
            pbY0040 = new Button(); pbY0041 = new Button(); pbY0042 = new Button(); pbY0043 = new Button();
            pbY0044 = new Button(); pbY0045 = new Button(); pbY0046 = new Button(); pbY0047 = new Button();
            pbY0050 = new Button(); pbY0051 = new Button(); pbY0052 = new Button(); pbY0053 = new Button();
            pbY0054 = new Button(); pbY0055 = new Button(); pbY0056 = new Button(); pbY0057 = new Button();
        }

        #endregion

        #endregion
        #region --Form--
        public FormOp1()
        {
            InitializeComponent();
        }

        private void FormOp1_Load(object sender, EventArgs e)
        {
            ftt = new FormTorqueTest();
            //tPbTorqueTest = new System.Windows.Forms.Timer();
            M1126 = new bool[15];
            M1200 = new bool[50];
            R1000 = new short[3];
            R1100 = new short[2];
            X0 = new bool[64] ;
            Y0 = new bool[48];
            pbY0 = new Button[48];
            NewLabel();
            lampX0 = new Label[64]
            {lbX0000,lbX0001,lbX0002,lbX0003,lbX0004,lbX0005,lbX0006,lbX0007,
            lbX0010,lbX0011,lbX0012,lbX0013,lbX0014,lbX0015,lbX0016,lbX0017,
            lbX0020,lbX0021,lbX0022,lbX0023,lbX0024,lbX0025,lbX0026,lbX0027,
            lbX0030,lbX0031,lbX0032,lbX0033,lbX0034,lbX0035,lbX0036,lbX0037,
            lbX0040,lbX0041,lbX0042,lbX0043,lbX0044,lbX0045,lbX0046,lbX0047,
            lbX0050,lbX0051,lbX0052,lbX0053,lbX0054,lbX0055,lbX0056,lbX0057,
            lbX0060,lbX0061,lbX0062,lbX0063,lbX0064,lbX0065,lbX0066,lbX0067,
            lbX0070,lbX0071,lbX0072,lbX0073,lbX0074,lbX0075,lbX0076,lbX0077,};
            lampY0 = new Label[48]
            { lbY0000,lbY0001,lbY0002,lbY0003,lbY0004,lbY0005,lbY0006,lbY0007,
            lbY0010,lbY0011,lbY0012,lbY0013,lbY0014,lbY0015,lbY0016,lbY0017,
            lbY0020,lbY0021,lbY0022,lbY0023,lbY0024,lbY0025,lbY0026,lbY0027,
            lbY0030,lbY0031,lbY0032,lbY0033,lbY0034,lbY0035,lbY0036,lbY0037,
            lbY0040,lbY0041,lbY0042,lbY0043,lbY0044,lbY0045,lbY0046,lbY0047,
            lbY0050,lbY0051,lbY0052,lbY0053,lbY0054,lbY0055,lbY0056,lbY0057,};
            lbScrewState = new Label[10]
            {lbScrewStateAx1,lbScrewStateAx2,lbScrewStateAx3,lbScrewStateAx4,lbScrewStateAx5,lbScrewStateAx6,lbScrewState7,lbScrewState8,lbScrewState9,lbScrewState10 };
            lbTorqueValue = new Label[10]
            {lbTorqueAx1,lbTorqueAx2,lbTorqueAx3,lbTorqueAx4,lbTorqueAx5,lbTorqueAx6,lbTorqueAx7,lbTorqueAx8,lbTorqueAx9,lbTorqueAx10 };
            pbY0 = new Button[48]
            { pbY0000,pbY0001,pbY0002,pbY0003,pbY0004,pbY0005,pbY0006,pbY0007,
            pbY0010,pbY0011,pbY0012,pbY0013,pbY0014,pbY0015,pbY0016,pbY0017,
            pbY0020,pbY0021,pbY0022,pbY0023,pbY0024,pbY0025,pbY0026,pbY0027,
            pbY0030,pbY0031,pbY0032,pbY0033,pbY0034,pbY0035,pbY0036,pbY0037,
            pbY0040,pbY0041,pbY0042,pbY0043,pbY0044,pbY0045,pbY0046,pbY0047,
            pbY0050,pbY0051,pbY0052,pbY0053,pbY0054,pbY0055,pbY0056,pbY0057,};
            ErrorMessage = new string[100];
            myLog = new csLog();
            myStatus = new MachineStatus();
            Initialize();
            M3000 = new bool[ErrorMessage.Length];
            _m3000 = new bool[ErrorMessage.Length];
            tabControlMain.SelectedTab = tabPageManual;
            tabControlManual.SelectedTab = tabPageM1;
            tabControlMain.Selecting += TabControlMain_Selecting;
            FormStart fs = new FormStart();
            fs.ShowDialog();
            fs.Activate();
        }

        /// <summary>
        /// 自動切手動時檢查
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControlMain_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (!M1000[3] && tabControlMain.SelectedTab==tabPageManual)
            {
                PlcWrite.writeToPLC("M1000",false);
                Thread.Sleep(100);
                PlcWrite.writeToPLC("M1001", true);
            }
            if (M1000[3])
            {
                tabControlMain.SelectedTab = tabPageAuto;
            }
        }

        private void FormOp1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Driver = null;
        }

        /// <summary>
        /// 取得設定參數
        /// </summary>
        private void getSetting()
        {
            if (R2000 != null)
            {
                //Feed screw time
                int iv = R2000[15];
                double dv = iv;
                dv = dv / 10;
                tbFeedScrewTime.Text = dv.ToString();
                //bowl feeder time
                int[] iv2 = new int[15];
                double[] dv2 = new double[15];
                for (int i = 0; i < 15; i++)
                {
                    iv2[i] = R2000[i];
                    dv2[i] = iv2[i];
                    dv2[i] = dv2[i] / 10;
                }
                tbFeedStartTime_1.Text = dv2[0].ToString();
                tbFeedStartTime_2.Text = dv2[1].ToString();
                tbFeedStopTime_1.Text = dv2[5].ToString();
                tbFeedStopTime_2.Text = dv2[6].ToString();
                tbFeedErrorTime_1.Text = dv2[10].ToString();
                tbFeedErrorTime_2.Text = dv2[11].ToString();
            }
        }

        /// <summary>
        /// 取得初始檔案,初始化
        /// </summary>
        private void Initialize()
        {
            string initFilename = @"D:\Resource\MachineInitialFile_19601.txt";
            string errFilename = @"D:\Resource\ErrorMessage_19601.txt";
            //初始檔
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
                    if (ss[0].Equals("ComPort"))
                    {
                        myStatus.ComPort = ss[1];
                    }
                    if (ss[0].Equals("Language"))
                    {
                        myStatus.Language = int.Parse(ss[1]);
                        LanguageSelection = myStatus.Language;
                    }
                }
                //取得文字檔案
                ChangeLanglue();
                //取得檔案OK,建立PLC連線
                CreatPLCConnect();
                //建立com port
                try
                {
                    Driver = new csNittoDriver(myStatus.ComPort, 9600, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
                }
                catch (Exception err)
                {
                    MessageBox.Show("SerialPort:" + err.ToString());
                }
                //建立timer1
                OperateResult opr = PlcRead.ConnectServer();
                if (opr.IsSuccess)
                {
                    _t1 = new System.Windows.Forms.Timer();
                    _t1.Interval = 200;
                    _t1.Tick += _t1_Tick;
                    _t1.Start();
                }
                else
                {
                    myLog.LogText = "連線建立失敗!!" + Environment.NewLine;
                    MessageBox.Show("PLC未連線!!請關閉軟件,檢查連線後重開!!");
                }
                //建立timer2
                _t2 = new System.Windows.Forms.Timer();
                _t2.Interval = 100;
                _t2.Tick += _t2_Tick;
                _t2.Start();
                //建立timer3
                _t3 = new System.Windows.Forms.Timer();
                _t3.Interval = 500;
                _t3.Tick += _t3_Tick;
                //
                tPbTorqueTest = new System.Windows.Forms.Timer();
                tPbTorqueTest.Interval = 500;
                tPbTorqueTest.Tick += TPbTorqueTest_Tick;
            }
            catch (Exception err)
            {
                myLog.LogText = "開始初始檔案時發生異常:" + err.ToString() + Environment.NewLine;
                MessageBox.Show(@"未取得初始化資料!!請確認D:\Resource\MachineInitialFile_19601.txt檔案是否正確?");
            }
            //異常檔
            try
            {
                ErrorMessage= File.ReadAllLines(errFilename,Encoding.Default);
            }
            catch (Exception err)
            {
                myLog.LogText = "開始異常訊息檔案時發生異常:" + err.ToString() + Environment.NewLine;
                MessageBox.Show(@"未取得異常訊息檔案!!請確認D:\Resource\ErrorMessage_19601.txt檔案是否正確?");
            }
        }

        /// <summary>
        /// 建立PLC連線
        /// </summary>
        private void CreatPLCConnect()
        {
            if (myStatus.PlcPort != 0)
            {
                PlcRead = new MelsecMcNet();
                PlcRead.IpAddress = myStatus.PlcIP;
                PlcRead.Port = myStatus.PlcPort;
                PlcRead.ConnectTimeOut = 3000;

                PlcWrite = new csFX5(myStatus.PlcIP,myStatus.PlcPort);
            }
            else
            {
                MessageBox.Show(@"未取得PLC資料!!請確認D:\Resource\MachineInitialFile.txt檔案是否正確?");
            }
        }

        #endregion
        #region --Timer--
        //T1 PLC
        private void _t1_Tick(object sender, EventArgs e)
        {
            _t1.Stop();
            //時間更新
            lbTimeAuto.Text = DateTime.Now.ToString("HH:mm");
            lbTimeManual.Text = DateTime.Now.ToString("HH:mm");
            lbTimeSetting.Text = DateTime.Now.ToString("HH:mm");
            lbTimeError.Text = DateTime.Now.ToString("HH:mm");
            //讀取PLC狀態
            M1000 = PlcRead.ReadBool("M1000", 4).Content;
            Thread.Sleep(100);
            M1400 = PlcRead.ReadBool("M1400",14).Content;//M1400 自動,原點,異常,螺絲不足,吹螺絲SOL
            Thread.Sleep(100);
            M1126 = PlcRead.ReadBool("M1126", 15).Content;//M1126設備設定
            Thread.Sleep(100);
            M1200 = PlcRead.ReadBool("M1200", 50).Content;//M1200鎖付狀態
            Thread.Sleep(100);
            M3000 = PlcRead.ReadBool("M3000", 120).Content;//異常
            Thread.Sleep(100);
            X0 = PlcRead.ReadBool("X0", 64).Content;//X0-X77
            Thread.Sleep(100);
            Y0 = PlcRead.ReadBool("Y0", 48).Content;//Y0-Y57
            Thread.Sleep(100);
            R1000 = PlcRead.ReadInt16("R1000",3).Content;//R1002 Cycle Time
            Thread.Sleep(100);
            R1100 = PlcRead.ReadInt16("R1100", 2).Content;
            Thread.Sleep(100);
            R2000 = PlcRead.ReadInt32("R2000", 16).Content;//R2000時間
            Thread.Sleep(100);
            R5004 = PlcRead.ReadInt32("R5004").Content;//R5004 All count
            //****************************************************************************
            //讀取後處理
            //更新
            ReflashLampStatus();
            //手自動變化
            CheckAutoManualPage();
            //first
            if (!firstRunFlag)
            {
                getSetting();
                firstRunFlag = true;
            }
            //****************************************************************************
            _t1.Start();
        }

        /// <summary>
        /// 檢查設備異常狀態
        /// </summary>
        private void CheckMachineErrorStatus()
        {
            if (M3000!=null)
            {
                ErrorStatus = false;
                for (int i = 0; i < ErrorMessage.Length; i++)
                {
                    if (M3000[i]==true && _m3000[i]==false)
                    {
                        _m3000[i] = true;
                        myLog.ErrorLogText = ErrorMessage[i];
                    }
                    if (M3000[i]==false && _m3000[i]==true)
                    {
                        _m3000[i] = false;
                        myLog.ClearErrorLogText();
                    }
                    ErrorStatus = ErrorStatus || M3000[i];
                }
                tbCurrentError.Text = myLog.ErrorLogText;
                tbAlarmHistory.Text = myLog.AlarmHistory;
            }
        }

        /// <summary>
        /// 變更Lamp狀態
        /// </summary>
        private void ReflashLampStatus()
        {
            #region --自動頁--
            if (M1126!=null && M1200!=null && R1000!=null && R1100!=null && M1400!=null && tabControlMain.SelectedTab == tabPageAuto)
            {
                setData(myTV);
                #region --鎖付扭力--
                if (R1000[0] > 0 && !readTorqueFlag)
                {
                    readTorqueFlag = true;
                    Thread t1 = new Thread(getDriverData);
                    t1.IsBackground = true;
                    t1.Start();
                    short st = 0;
                    PlcWrite.writeToPLC("R1000", st);
                }
                if (R1000[0] == 0 && readTorqueFlag)
                {
                    readTorqueFlag = false;
                }
                #endregion
                //自動狀態
                if (M1400[0])
                {
                    lbAuto_PAuto.BackColor = Color.Lime;
                }
                else
                {
                    lbAuto_PAuto.BackColor = Color.White;
                }
                //原點狀態
                if (M1400[1])
                {
                    lbOrg_PAuto.BackColor = Color.Lime;
                }
                else
                {
                    lbOrg_PAuto.BackColor = Color.White;
                }
                //異常狀態
                if (M1400[2])
                {
                    lbError_PAuto.BackColor = Color.Red;
                }
                else
                {
                    lbError_PAuto.BackColor = Color.White;
                }
                //螺絲狀態
                if (M1400[3])
                {
                    lbFeedScrew_PAuto.BackColor = Color.Yellow;
                }
                else
                {
                    lbFeedScrew_PAuto.BackColor = Color.White;
                }
                //頻道檢查
                lbChannelNo_PAuto.Text = R1100[0].ToString();
                //頻道
                if (M1126[1])
                {
                    lbChannel_PAuto.Text = LanguageString[98];
                }
                if (M1126[2])
                {
                    lbChannel_PAuto.Text = LanguageString[99];
                }
                //螺絲種類
                if (M1126[3])
                {
                    lbScrewType_PAuto.Text = LanguageString[101];
                }
                if (M1126[4])
                {
                    lbScrewType_PAuto.Text = LanguageString[102];
                }
                //NG後處理方式顯示
                if (M1126[5])
                {
                    lbNGProc_PAuto.Text = LanguageString[104];
                }
                if (M1126[6])
                {
                    lbNGProc_PAuto.Text = LanguageString[105];
                }
                //光柵
                if (M1126[10])
                {
                    lbAreaSensor_PAuto.Text = LanguageString[108];
                }
                else
                {
                    lbAreaSensor_PAuto.Text = LanguageString[107];
                }
                //膠水完成
                if (M1000[2])
                {
                    lbM1002.BackColor = Color.Lime;
                    lbM1002.ForeColor = Color.Black;
                }
                else
                {
                    lbM1002.BackColor = Color.Gray;
                    lbM1002.ForeColor = Color.White;
                }
                //Cycle time
                double db = Convert.ToDouble(R1000[2]) / 1000;
                lbCycleTime_PAuto.Text = db.ToString();
                //count
                lbCounter_PAuto.Text = R5004.ToString();
                //SOL
                if (M1400[4])
                {
                    lbY1000.BackColor = Color.Orange;
                    lbY1000.ForeColor = Color.Black;
                }
                else
                {
                    lbY1000.BackColor = Color.Gray;
                    lbY1000.ForeColor = Color.White;
                }
                for (int i = 0; i < 48; i++)
                {
                    if (Y0[i])
                    {
                        lampY0[i].BackColor = Color.Orange;
                        lampY0[i].ForeColor = Color.Black;
                    }
                    else
                    {
                        lampY0[i].BackColor = Color.Gray;
                        lampY0[i].ForeColor = Color.White;
                    }
                }
                #region --自動畫面的鎖付狀態--
                for (int i = 0; i < 10; i++)
                {
                    //OK
                    if (M1200[i + 0])
                    {
                        lbScrewState[i].BackColor = Color.Lime;
                    }
                    else
                    {
                        //角度NG
                        if (M1200[i + 10])
                        {
                            lbScrewState[i].BackColor = Color.Blue;
                        }
                        else
                        {
                            //鎖付NG
                            if (M1200[i + 20])
                            {
                                lbScrewState[i].BackColor = Color.Red;
                            }
                            else
                            {
                                //浮鎖
                                if (M1200[i + 30])
                                {
                                    lbScrewState[i].BackColor = Color.Yellow;
                                }
                                else
                                {
                                    //假鎖付NG
                                    if (M1200[i + 40])
                                    {
                                        lbScrewState[i].BackColor = Color.Fuchsia;
                                    }
                                    else
                                    {
                                        //無狀態
                                        lbScrewState[i].BackColor = Color.White;
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion
                #region --扭力清除--
                if (M1400[13] && !clearTorqueFlag)
                {
                    clearTorqueFlag = true;
                    if (myTV.TorqueValue != null)
                    {
                        myTV.TorqueValue.Clear();
                    }
                }
                if (!M1400[13] && R1000[0]==0 && clearTorqueFlag)
                {
                    clearTorqueFlag = false;
                }
                #endregion
            }
            #endregion
            #region --手動頁--
            if (M1126!=null && M1200!=null && M1400!=null && tabControlMain.SelectedTab == tabPageManual)
            {
                //自動狀態
                if (M1400[0])
                {
                    lbAuto_PManual.BackColor = Color.Lime;
                }
                else
                {
                    lbAuto_PManual.BackColor = Color.White;
                }
                //原點狀態
                if (M1400[1])
                {
                    lbOrg_PManual.BackColor = Color.Lime;
                }
                else
                {
                    lbOrg_PManual.BackColor = Color.White;
                }
                //異常狀態
                if (M1400[2])
                {
                    lbError_PManual.BackColor = Color.Red;
                }
                else
                {
                    lbError_PManual.BackColor = Color.White;
                }
                //螺絲狀態
                if (M1400[3])
                {
                    lbFeedScrew_PManual.BackColor = Color.Yellow;
                }
                else
                {
                    lbFeedScrew_PManual.BackColor = Color.White;
                }
                //扭力測試
                if (M1126[0])
                {
                    pbTorqueCheck.BackColor = Color.Yellow;
                    pbTorqueCheck.Text = LanguageString[32];
                }
                else
                {
                    pbTorqueCheck.BackColor = Color.White;
                    pbTorqueCheck.Text = LanguageString[31];
                }
                //頻道
                if (M1126[1])
                {
                    lbChannel_PManual.Text = LanguageString[98];
                }
                if (M1126[2])
                {
                    lbChannel_PManual.Text = LanguageString[99];
                }
                //光柵
                if (M1126[10])
                {
                    lbAreaSensor_PManual.Text = LanguageString[108];
                }
                else
                {
                    lbAreaSensor_PManual.Text = LanguageString[107];
                }
                #region --P1--
                if (tabControlManual.SelectedTab == tabPageM1)
                {
                    //LS回
                    for (int i = 28; i < 32; i += 2)
                    {
                        if (X0[i])
                        {
                            lampX0[i].BackColor = Color.Lime;
                        }
                        else
                        {
                            lampX0[i].BackColor = Color.White;
                        }
                    }
                    //LS出
                    for (int i = 29; i < 32; i += 2)
                    {
                        if (X0[i])
                        {
                            lampX0[i].BackColor = Color.Orange;
                        }
                        else
                        {
                            lampX0[i].BackColor = Color.White;
                        }
                    }
                    //SOL & PB
                    for (int i = 14; i < 19; i++)
                    {
                        if (Y0[i])
                        {
                            lampY0[i].BackColor = Color.Orange;
                            lampY0[i].ForeColor = Color.Black;
                            pbY0[i].BackColor = Color.Lime;
                            pbY0[i].ForeColor = Color.Black;
                        }
                        else
                        {
                            lampY0[i].BackColor = Color.Gray;
                            lampY0[i].ForeColor = Color.White;
                            pbY0[i].BackColor = Color.Gray;
                            pbY0[i].ForeColor = Color.White;
                        }
                    }
                    //INIT
                    //if (M1200[9])
                    //{
                    //    pbInitialStart.BackColor = Color.Lime;
                    //}
                    //else
                    //{
                    //    pbInitialStart.BackColor = Color.Gray;
                    //}
                }
                #endregion
                #region --P2--
                if (tabControlManual.SelectedTab == tabPageM2)
                {
                    //LS回
                    for (int i = 4; i < 14; i += 2)
                    {
                        if (X0[i])
                        {
                            lampX0[i].BackColor = Color.Lime;
                        }
                        else
                        {
                            lampX0[i].BackColor = Color.White;
                        }
                    }
                    //LS出
                    for (int i = 5; i < 14; i += 2)
                    {
                        if (X0[i])
                        {
                            lampX0[i].BackColor = Color.Orange;
                        }
                        else
                        {
                            lampX0[i].BackColor = Color.White;
                        }
                    }
                    //SOL & PB
                    for (int i = 0; i < 5; i++)
                    {
                        if (Y0[i])
                        {
                            lampY0[i].BackColor = Color.Orange;
                            lampY0[i].ForeColor = Color.Black;
                            pbY0[i].BackColor = Color.Lime;
                            pbY0[i].ForeColor = Color.Black;
                        }
                        else
                        {
                            lampY0[i].BackColor = Color.Gray;
                            lampY0[i].ForeColor = Color.White;
                            pbY0[i].BackColor = Color.Gray;
                            pbY0[i].ForeColor = Color.White;
                        }
                    }
                }
                #endregion
                #region --P3--
                if (tabControlManual.SelectedTab == tabPageM3)
                {
                    //LS回
                    for (int i = 14; i < 20; i += 2)
                    {
                        if (X0[i])
                        {
                            lampX0[i].BackColor = Color.Lime;
                            lampX0[i].ForeColor = Color.Black;
                        }
                        else
                        {
                            lampX0[i].BackColor = Color.White;
                            lampX0[i].ForeColor = Color.White;
                        }
                    }
                    //LS出
                    for (int i = 15; i < 20; i += 2)
                    {
                        if (X0[i])
                        {
                            lampX0[i].BackColor = Color.Orange;
                        }
                        else
                        {
                            lampX0[i].BackColor = Color.White;
                        }
                    }
                    //SOL & PB
                    for (int i = 6; i < 10; i++)
                    {
                        if (Y0[i])
                        {
                            lampY0[i].BackColor = Color.Orange;
                            lampY0[i].ForeColor = Color.Black;
                            pbY0[i].BackColor = Color.Lime;
                            pbY0[i].ForeColor = Color.Black;
                        }
                        else
                        {
                            lampY0[i].BackColor = Color.Gray;
                            lampY0[i].ForeColor = Color.White;
                            pbY0[i].BackColor = Color.Gray;
                            pbY0[i].ForeColor = Color.White;
                        }
                    }
                }
                #endregion
                #region --P4--
                if (tabControlManual.SelectedTab == tabPageM4)
                {
                    //LS回
                    for (int i = 20; i < 26; i += 2)
                    {
                        if (X0[i])
                        {
                            lampX0[i].BackColor = Color.Lime;
                        }
                        else
                        {
                            lampX0[i].BackColor = Color.White;
                        }
                    }
                    //LS出
                    for (int i = 21; i < 26; i += 2)
                    {
                        if (X0[i])
                        {
                            lampX0[i].BackColor = Color.Orange;
                        }
                        else
                        {
                            lampX0[i].BackColor = Color.White;
                        }
                    }
                    //SOL & PB
                    for (int i = 10; i < 14; i++)
                    {
                        if (Y0[i])
                        {
                            lampY0[i].BackColor = Color.Orange;
                            lampY0[i].ForeColor = Color.Black;
                            pbY0[i].BackColor = Color.Lime;
                            pbY0[i].ForeColor = Color.Black;
                        }
                        else
                        {
                            lampY0[i].BackColor = Color.Gray;
                            lampY0[i].ForeColor = Color.White;
                            pbY0[i].BackColor = Color.Gray;
                            pbY0[i].ForeColor = Color.White;
                        }
                    }
                }
                #endregion
                #region --P5--
                if (tabControlManual.SelectedTab == tabPageM5)
                {
                    //LS回
                    for (int i = 32; i < 40; i += 2)
                    {
                        if (X0[i])
                        {
                            lampX0[i].BackColor = Color.Lime;
                        }
                        else
                        {
                            lampX0[i].BackColor = Color.White;
                        }
                    }
                    //LS出
                    for (int i = 33; i < 40; i += 2)
                    {
                        if (X0[i])
                        {
                            lampX0[i].BackColor = Color.Orange;
                        }
                        else
                        {
                            lampX0[i].BackColor = Color.White;
                        }
                    }
                    //SOL & PB
                    for (int i = 19; i < 25; i++)
                    {
                        if (Y0[i])
                        {
                            lampY0[i].BackColor = Color.Orange;
                            lampY0[i].ForeColor = Color.Black;
                            pbY0[i].BackColor = Color.Lime;
                            pbY0[i].ForeColor = Color.Black;
                        }
                        else
                        {
                            lampY0[i].BackColor = Color.Gray;
                            lampY0[i].ForeColor = Color.White;
                            pbY0[i].BackColor = Color.Gray;
                            pbY0[i].ForeColor = Color.White;
                        }
                    }
                }
                #endregion
                #region --P6--
                if (tabControlManual.SelectedTab == tabPageM6)
                {
                    //LS回
                    for (int i = 26; i < 28; i += 2)
                    {
                        if (X0[i])
                        {
                            lampX0[i].BackColor = Color.Lime;
                        }
                        else
                        {
                            lampX0[i].BackColor = Color.White;
                        }
                    }
                    //LS出
                    for (int i = 27; i < 28; i += 2)
                    {
                        if (X0[i])
                        {
                            lampX0[i].BackColor = Color.Orange;
                        }
                        else
                        {
                            lampX0[i].BackColor = Color.White;
                        }
                    }
                    //SOL & PB
                    for (int i = 16; i < 28; i++)
                    {
                        if (Y0[i])
                        {
                            lampY0[i].BackColor = Color.Orange;
                            lampY0[i].ForeColor = Color.Black;
                            pbY0[i].BackColor = Color.Lime;
                            pbY0[i].ForeColor = Color.Black;
                        }
                        else
                        {
                            lampY0[i].BackColor = Color.Gray;
                            lampY0[i].ForeColor = Color.White;
                            pbY0[i].BackColor = Color.Gray;
                            pbY0[i].ForeColor = Color.White;
                        }
                    }
                }
                #endregion
                #region --定位--
                if (tabControlManual.SelectedTab == tabPageTest)
                {
                    //LS回
                    if (X0[4])
                    {
                        lbX0004_M7.BackColor = Color.Lime;
                    }
                    else
                    {
                        lbX0004_M7.BackColor = Color.White;
                    }
                    if (X0[6])
                    {
                        lbX0006_M7.BackColor = Color.Lime;
                    }
                    else
                    {
                        lbX0006_M7.BackColor = Color.White;
                    }
                    if (X0[8])
                    {
                        lbX0010_M7.BackColor = Color.Lime;
                    }
                    else
                    {
                        lbX0010_M7.BackColor = Color.White;
                    }
                    if (X0[10])
                    {
                        lbX0012_M7.BackColor = Color.Lime;
                    }
                    else
                    {
                        lbX0012_M7.BackColor = Color.White;
                    }
                    if (X0[12])
                    {
                        lbX0014_M7.BackColor = Color.Lime;
                    }
                    else
                    {
                        lbX0014_M7.BackColor = Color.White;
                    }
                    if (X0[26])
                    {
                        lbX0032_M7.BackColor = Color.Lime;
                    }
                    else
                    {
                        lbX0032_M7.BackColor = Color.White;
                    }
                    if (X0[28])
                    {
                        lbX0034_M7.BackColor = Color.Lime;
                    }
                    else
                    {
                        lbX0034_M7.BackColor = Color.White;
                    }
                    if (X0[30])
                    {
                        lbX0036_M7.BackColor = Color.Lime;
                    }
                    else
                    {
                        lbX0036_M7.BackColor = Color.White;
                    }
                    //LS出
                    if (X0[5])
                    {
                        lbX0005_M7.BackColor = Color.Orange;
                    }
                    else
                    {
                        lbX0005_M7.BackColor = Color.White;
                    }
                    if (X0[7])
                    {
                        lbX0007_M7.BackColor = Color.Orange;
                    }
                    else
                    {
                        lbX0007_M7.BackColor = Color.White;
                    }
                    if (X0[9])
                    {
                        lbX0011_M7.BackColor = Color.Orange;
                    }
                    else
                    {
                        lbX0011_M7.BackColor = Color.White;
                    }
                    if (X0[11])
                    {
                        lbX0013_M7.BackColor = Color.Orange;
                    }
                    else
                    {
                        lbX0013_M7.BackColor = Color.White;
                    }
                    if (X0[13])
                    {
                        lbX0015_M7.BackColor = Color.Orange;
                    }
                    else
                    {
                        lbX0015_M7.BackColor = Color.White;
                    }
                    if (X0[27])
                    {
                        lbX0033_M7.BackColor = Color.Orange;
                    }
                    else
                    {
                        lbX0033_M7.BackColor = Color.White;
                    }
                    if (X0[29])
                    {
                        lbX0035_M7.BackColor = Color.Orange;
                    }
                    else
                    {
                        lbX0035_M7.BackColor = Color.White;
                    }
                    if (X0[31])
                    {
                        lbX0037_M7.BackColor = Color.Orange;
                    }
                    else
                    {
                        lbX0037_M7.BackColor = Color.White;
                    }
                    //SOL & PB
                    if (Y0[0])
                    {
                        pbY0000_M7.BackColor = Color.Lime;
                        pbY0000_M7.ForeColor = Color.Black;
                    }
                    else
                    {
                        pbY0000_M7.BackColor = Color.Gray;
                        pbY0000_M7.ForeColor = Color.White;
                    }
                    if (Y0[1])
                    {
                        pbY0001_M7.BackColor = Color.Lime;
                        pbY0001_M7.ForeColor = Color.Black;
                    }
                    else
                    {
                        pbY0001_M7.BackColor = Color.Gray;
                        pbY0001_M7.ForeColor = Color.White;
                    }
                    if (Y0[2])
                    {
                        pbY0002_M7.BackColor = Color.Lime;
                        pbY0002_M7.ForeColor = Color.Black;
                    }
                    else
                    {
                        pbY0002_M7.BackColor = Color.Gray;
                        pbY0002_M7.ForeColor = Color.White;
                    }
                    if (Y0[4])
                    {
                        pbY0004_M7.BackColor = Color.Lime;
                        pbY0004_M7.ForeColor = Color.Black;
                    }
                    else
                    {
                        pbY0004_M7.BackColor = Color.Gray;
                        pbY0004_M7.ForeColor = Color.White;
                    }
                    if (Y0[14])
                    {
                        pbY0016_M7.BackColor = Color.Lime;
                        pbY0016_M7.ForeColor = Color.Black;
                    }
                    else
                    {
                        pbY0016_M7.BackColor = Color.Gray;
                        pbY0016_M7.ForeColor = Color.White;
                    }
                    if (Y0[15])
                    {
                        pbY0017_M7.BackColor = Color.Lime;
                        pbY0017_M7.ForeColor = Color.Black;
                    }
                    else
                    {
                        pbY0017_M7.BackColor = Color.Gray;
                        pbY0017_M7.ForeColor = Color.White;
                    }
                    if (Y0[16])
                    {
                        pbY0020_M7.BackColor = Color.Lime;
                        pbY0020_M7.ForeColor = Color.Black;
                    }
                    else
                    {
                        pbY0020_M7.BackColor = Color.Gray;
                        pbY0020_M7.ForeColor = Color.White;
                    }
                    if (Y0[17])
                    {
                        pbY0021_M7.BackColor = Color.Lime;
                        pbY0021_M7.ForeColor = Color.Black;
                    }
                    else
                    {
                        pbY0021_M7.BackColor = Color.Gray;
                        pbY0021_M7.ForeColor = Color.White;
                    }
                    if (Y0[18])
                    {
                        pbY0022_M7.BackColor = Color.Lime;
                        pbY0022_M7.ForeColor = Color.Black;
                    }
                    else
                    {
                        pbY0022_M7.BackColor = Color.Gray;
                        pbY0022_M7.ForeColor = Color.White;
                    }
                    if (Y0[27])
                    {
                        pbY0033_M7.BackColor = Color.Lime;
                        pbY0033_M7.ForeColor = Color.Black;
                    }
                    else
                    {
                        pbY0033_M7.BackColor = Color.Gray;
                        pbY0033_M7.ForeColor = Color.White;
                    }
                }
                #endregion
            }
            #endregion
            #region --設定頁--
            if (R2000!=null && M1126!=null && tabControlMain.SelectedTab == tabPageSetting)
            {
                //Feed screw time
                int iv = R2000[15];
                double dv = iv;
                dv = dv / 10;
                lbFeedScrewTimeValue.Text = dv.ToString();
                //bowl feeder time
                int[] iv2 = new int[15];
                double[] dv2 = new double[15];
                for (int i = 0; i < 15; i++)
                {
                    iv2[i] = R2000[i];
                    dv2[i] = iv2[i];
                    dv2[i] = dv2[i] / 10;
                }
                lbFeedStartTime_1.Text = dv2[0].ToString();
                lbFeedStartTime_2.Text = dv2[1].ToString();
                lbFeedStopTime_1.Text = dv2[5].ToString();
                lbFeedStopTime_2.Text = dv2[6].ToString();
                lbFeedErrorTime_1.Text = dv2[10].ToString();
                lbFeedErrorTime_2.Text = dv2[11].ToString();
                //頻道
                if (M1126[1])
                {
                    pbNew_PSetting.BackColor = Color.Lime;
                    pbNew_PSetting.ForeColor = Color.Black;
                    pbRepair_PSetting.BackColor = Color.Gray;
                    pbRepair_PSetting.ForeColor = Color.White;
                }
                if (M1126[2])
                {
                    pbNew_PSetting.BackColor = Color.Gray;
                    pbNew_PSetting.ForeColor = Color.White;
                    pbRepair_PSetting.BackColor = Color.Lime;
                    pbRepair_PSetting.ForeColor = Color.Black;
                }
                //螺絲種類
                if (M1126[3])
                {
                    pbSus_PSetting.BackColor = Color.Lime;
                    pbSus_PSetting.ForeColor = Color.Black;
                    pbSteel_PSetting.BackColor = Color.Gray;
                    pbSteel_PSetting.ForeColor = Color.White;
                }
                if (M1126[4])
                {
                    pbSus_PSetting.BackColor = Color.Gray;
                    pbSus_PSetting.ForeColor = Color.White;
                    pbSteel_PSetting.BackColor = Color.Lime;
                    pbSteel_PSetting.ForeColor = Color.Black;
                }
                //NG後處理方式顯示
                if (M1126[5])
                {
                    pbBackOut_PSetting.BackColor = Color.Lime;
                    pbBackOut_PSetting.ForeColor = Color.Black;
                    pbContinue_PSetting.BackColor = Color.Gray;
                    pbContinue_PSetting.ForeColor = Color.White;
                }
                if (M1126[6])
                {
                    pbBackOut_PSetting.BackColor = Color.Gray;
                    pbBackOut_PSetting.ForeColor = Color.White;
                    pbContinue_PSetting.BackColor = Color.Lime;
                    pbContinue_PSetting.ForeColor = Color.Black;
                }
                //光柵
                if (M1126[10])
                {
                    pbGateDisable.BackColor = Color.Lime;
                    pbGateDisable.ForeColor = Color.Black;
                    pbGateEnable.BackColor = Color.Gray;
                    pbGateEnable.ForeColor = Color.White;
                }
                else
                {
                    pbGateDisable.BackColor = Color.Gray;
                    pbGateDisable.ForeColor = Color.White;
                    pbGateEnable.BackColor = Color.Lime;
                    pbGateEnable.ForeColor = Color.Black ;
                }
                //count
                lbScrewCount_PSetting.Text = R5004.ToString();
            }
            #endregion
            #region --Error頁--
            CheckMachineErrorStatus();
            if (M1126!=null && tabControlMain.SelectedTab == tabPageError)
            {
                if (M1126[12])
                {
                    pbErrorReset.BackColor = Color.Lime;
                    pbErrorReset.ForeColor = Color.Black;
                }
                else
                {
                    pbErrorReset.BackColor = Color.Gray;
                    pbErrorReset.ForeColor = Color.White;
                }
            }
            #endregion
            #region --BZ頁--
            if (M1126!=null && tabControlMain.SelectedTab == tabPageStopBZ)
            {
                if (M1126[8])
                {
                    pbStopBZ.BackColor = Color.Lime;
                    pbStopBZ.ForeColor = Color.Black;
                }
                else
                {
                    pbStopBZ.BackColor = Color.Gray;
                    pbStopBZ.ForeColor = Color.White;
                }
            }
            #endregion
        }
        /// <summary>
        /// 檢查是自動或手動頁,切換PLC狀態
        /// </summary>
        private void CheckAutoManualPage()
        {
            if (tabControlMain.SelectedTab == tabPageAuto && !M1000[0])
            {
                    PlcWrite.writeToPLC("M1001", false);
                    Thread.Sleep(100);
                    PlcWrite.writeToPLC("M1000", true);
            }
            if (tabControlMain.SelectedTab == tabPageManual && M1000[0])
            {
                PlcWrite.writeToPLC("M1000", false);
                Thread.Sleep(100);
                PlcWrite.writeToPLC("M1001", true);
            }
        }
        #endregion
        #region --按鍵動作Timer2--
        //T2 Not PLC
        private void _t2_Tick(object sender, EventArgs e)
        {
            _t2.Stop();
            CheckManualPage();
            _t2.Start();
        }
        /// <summary>
        /// 檢查是否更換手動頁,變更頁面抬頭文字
        /// </summary>
        private void CheckManualPage()
        {
            if (tabControlMain.SelectedTab==tabPageManual)
            {
                if (tabControlManual.SelectedTab == tabPageM1)
                {
                    lbTitile.Text = "手動 1/6";
                }
                if (tabControlManual.SelectedTab == tabPageM2)
                {
                    lbTitile.Text = "手動 2/6";
                }
                if (tabControlManual.SelectedTab == tabPageM3)
                {
                    lbTitile.Text = "手動 3/6";
                }
                if (tabControlManual.SelectedTab == tabPageM4)
                {
                    lbTitile.Text = "手動 4/6";
                }
                if (tabControlManual.SelectedTab == tabPageM5)
                {
                    lbTitile.Text = "手動 5/6";
                }
                if (tabControlManual.SelectedTab == tabPageM6)
                {
                    lbTitile.Text = "手動 6/6";
                }
            }
        }
        #endregion
        #region --手動按鍵-
        #region --扭力測試
        private void pbTorqueCheck_MouseDown(object sender, MouseEventArgs e)
        {
            tPbTorqueTest.Interval = 500;
            tPbTorqueTest.Start();
        }
        private void pbTorqueCheck_MouseUp(object sender, MouseEventArgs e)
        {
            tPbTorqueTest.Stop();
        }
        private void TPbTorqueTest_Tick(object sender, EventArgs e)
        {
            tPbTorqueTest.Stop();
            PlcWrite.writeToPLC("M1126", true);
            Thread.Sleep(100);
            PlcWrite.writeToPLC("M1000", true);
            pbTorqueCheck.Text = LanguageString[32];
            pbTorqueCheck.BackColor = Color.Yellow;
            ftt.Show();
        }
        #endregion
        #region --復歸--
        private void pbInitialStart_Click(object sender, EventArgs e)
        {
            PlcWrite.writeToPLC("M1135", true);
            Thread.Sleep(100);
            PlcWrite.writeToPLC("M1135", false);
        }
        #endregion
        #region --鎖螺絲--
        //上
        private void pbY0020_Click(object sender, EventArgs e)
        {
            PlcWrite.writeToPLC("M1114", true);
            Thread.Sleep(100);
            PlcWrite.writeToPLC("M1114", false);
        }
        //下
        private void pbY0033_Click(object sender, EventArgs e)
        {
            PlcWrite.writeToPLC("M1123", true);
            Thread.Sleep(100);
            PlcWrite.writeToPLC("M1123", false);
        }
        //頂起
        private void pbY0021_Click(object sender, EventArgs e)
        {
            PlcWrite.writeToPLC("M1115", true);
            Thread.Sleep(100);
            PlcWrite.writeToPLC("M1115", false);
        }
        //送螺絲
        private void pbFeedScrew_Click(object sender, EventArgs e)
        {
            PlcWrite.writeToPLC("M1124", true);
            Thread.Sleep(100);
            PlcWrite.writeToPLC("M1124", false);
        }
        //起子起動
        private void pbScrewDriverStart_Click(object sender, EventArgs e)
        {
            PlcWrite.writeToPLC("M1125", true);
            Thread.Sleep(100);
            PlcWrite.writeToPLC("M1125", false);
        }
        #endregion
        #region --單軸--
        //單軸移位回
        private void pbY0016_Click(object sender, EventArgs e)
        {
            PlcWrite.writeToPLC("M1112", true);
            Thread.Sleep(100);
            PlcWrite.writeToPLC("M1112", false);
        }
        //單軸移位出
        private void pbY0017_Click(object sender, EventArgs e)
        {
            PlcWrite.writeToPLC("M1113", true);
            Thread.Sleep(100);
            PlcWrite.writeToPLC("M1113", false);
        }
        //上下
        private void pbY0022_Click(object sender, EventArgs e)
        {
            PlcWrite.writeToPLC("M1116", true);
            Thread.Sleep(100);
            PlcWrite.writeToPLC("M1116", false);
            Thread.Sleep(100);
        }
        #endregion
        #region --底模--
        //底模置中前後
        private void pbY0002_Click(object sender, EventArgs e)
        {
            PlcWrite.writeToPLC("M1102", true);
            Thread.Sleep(100);
            PlcWrite.writeToPLC("M1102", false);
        }
        //底模置中左右
        private void pbY0004_Click(object sender, EventArgs e)
        {
            PlcWrite.writeToPLC("M1103", true);
            Thread.Sleep(100);
            PlcWrite.writeToPLC("M1103", false);
        }
        //底模移位回
        private void pbY0000_Click(object sender, EventArgs e)
        {
            PlcWrite.writeToPLC("M1100", true);
            Thread.Sleep(100);
            PlcWrite.writeToPLC("M1100", false);
        }
        //底模移位出
        private void pbY0001_Click(object sender, EventArgs e)
        {
            PlcWrite.writeToPLC("M1101", true);
            Thread.Sleep(100);
            PlcWrite.writeToPLC("M1101", false);
        }
        #endregion
        #region --前取PIN--
        //移位回
        private void pbY0006_Click(object sender, EventArgs e)
        {
            PlcWrite.writeToPLC("M1104", true);
            Thread.Sleep(100);
            PlcWrite.writeToPLC("M1104", false);
        }
        //移位出
        private void pbY0007_Click(object sender, EventArgs e)
        {
            PlcWrite.writeToPLC("M1105", true);
            Thread.Sleep(100);
            PlcWrite.writeToPLC("M1105", false);
        }
        //上下
        private void pbY0010_Click(object sender, EventArgs e)
        {
            PlcWrite.writeToPLC("M1106", true);
            Thread.Sleep(100);
            PlcWrite.writeToPLC("M1106", false);
        }
        //夾持
        private void pbY0011_Click(object sender, EventArgs e)
        {
            PlcWrite.writeToPLC("M1107", true);
            Thread.Sleep(100);
            PlcWrite.writeToPLC("M1107", false);
        }
        #endregion
        #region --後取PIN--
        //移位回
        private void pbY0012_Click(object sender, EventArgs e)
        {
            PlcWrite.writeToPLC("M1108", true);
            Thread.Sleep(100);
            PlcWrite.writeToPLC("M1108", false);
        }
        //移位出
        private void pbY0013_Click(object sender, EventArgs e)
        {
            PlcWrite.writeToPLC("M1109", true);
            Thread.Sleep(100);
            PlcWrite.writeToPLC("M1109", false);
        }
        //上下
        private void pbY0014_Click(object sender, EventArgs e)
        {
            PlcWrite.writeToPLC("M1110", true);
            Thread.Sleep(100);
            PlcWrite.writeToPLC("M1110", false);
        }
        //夾持
        private void pbY0015_Click(object sender, EventArgs e)
        {
            PlcWrite.writeToPLC("M1111", true);
            Thread.Sleep(100);
            PlcWrite.writeToPLC("M1111", false);
        }
        #endregion
        #region --前定位--
        //移位回
        private void pbY0023_Click(object sender, EventArgs e)
        {
            PlcWrite.writeToPLC("M1117", true);
            Thread.Sleep(100);
            PlcWrite.writeToPLC("M1117", false);
        }
        //移位出
        private void pbY0024_Click(object sender, EventArgs e)
        {
            PlcWrite.writeToPLC("M1118", true);
            Thread.Sleep(100);
            PlcWrite.writeToPLC("M1118", false);
        }
        //上下
        private void pbY0025_Click(object sender, EventArgs e)
        {
            PlcWrite.writeToPLC("M1119", true);
            Thread.Sleep(100);
            PlcWrite.writeToPLC("M1119", false);
        }
        #endregion
        #region --後定位--
        //移位回
        private void pbY0026_Click(object sender, EventArgs e)
        {
            PlcWrite.writeToPLC("M1120", true);
            Thread.Sleep(100);
            PlcWrite.writeToPLC("M1120", false);
        }
        //移位出
        private void pbY0027_Click(object sender, EventArgs e)
        {
            PlcWrite.writeToPLC("M1121", true);
            Thread.Sleep(100);
            PlcWrite.writeToPLC("M1121", false);
        }
        //上下
        private void pbY0030_Click(object sender, EventArgs e)
        {
            PlcWrite.writeToPLC("M1122", true);
            Thread.Sleep(100);
            PlcWrite.writeToPLC("M1122", false);
        }
        #endregion
        #region --BZ--
        private void pbStopBZ_Click(object sender, EventArgs e)
        {
            if (M1126[8])
            {
                PlcWrite.writeToPLC("M1134", false);
            }
            else
            {
                PlcWrite.writeToPLC("M1134", true);
            }
        }
        #endregion
        #region --Error reset--
        private void pbErrorReset_Click(object sender, EventArgs e)
        {
            if (ErrorStatus)
            {
                PlcWrite.writeToPLC("M1138", true);
            }
        }
        #endregion
        #region --設定頁--
        #region --計數清除--
        private void pbResetCount_MouseUp(object sender, MouseEventArgs e)
        {
            _t3.Stop();
        }
        private void pbResetCount_MouseDown(object sender, MouseEventArgs e)
        {
            _t3.Start();
        }
        private void _t3_Tick(object sender, EventArgs e)
        {
            _t3.Stop();
            Int32 iv = 0;
            PlcWrite.writeToPLC("R5004", iv);
        }
        #endregion
        #region --吹螺絲時間設定--
        private void pbSetFeedScrewTime_Click(object sender, EventArgs e)
        {
            _t1.Stop();
            double dv = Convert.ToDouble(tbFeedScrewTime.Text);
            int iv = Convert.ToInt32(dv * 10);
            PlcWrite.writeToPLC("R2030", iv);
            _t1.Start();
        }
        #endregion
        #region --振動筒時間設定--
        private void pbSetBowFeederTime_Click(object sender, EventArgs e)
        {
            _t1.Stop();
            Int32[] iv = new Int32[15];
            for (int i = 0; i < 15; i++)
            {
                iv[i] = 0;
            }
            double dv = Convert.ToDouble(tbFeedStartTime_1.Text);
            int k = Convert.ToInt32(dv * 10);
            iv[0] = k;
            dv = Convert.ToDouble(tbFeedStartTime_2.Text);
            k = Convert.ToInt32(dv * 10);
            iv[1] = k;
            dv = Convert.ToDouble(tbFeedStopTime_1.Text);
            k = Convert.ToInt32(dv * 10);
            iv[5] = k;
            dv = Convert.ToDouble(tbFeedStopTime_2.Text);
            k = Convert.ToInt32(dv * 10);
            iv[6] = k;
            dv = Convert.ToDouble(tbFeedErrorTime_1.Text);
            k = Convert.ToInt32(dv * 10);
            iv[10] = k;
            dv = Convert.ToDouble(tbFeedErrorTime_2.Text);
            k = Convert.ToInt32(dv * 10);
            iv[11] = k;
            for (int i = 0; i < 15; i++)
            {
                PlcWrite.writeToPLC("R" + (2000 + 2 * i).ToString(), iv[i]);
                Thread.Sleep(100);
            }
            _t1.Start();
        }
        #endregion
        #region --新品/修理品--
        private void pbNew_PSetting_Click(object sender, EventArgs e)
        {
            PlcWrite.writeToPLC("M1127", true);
            Thread.Sleep(100);
            PlcWrite.writeToPLC("M1128", false);
        }
        private void pbRepair_PSetting_Click(object sender, EventArgs e)
        {
            PlcWrite.writeToPLC("M1127", false);
            Thread.Sleep(100);
            PlcWrite.writeToPLC("M1128", true);
        }
        #endregion
        #region --螺絲種類--
        private void pbSus_PSetting_Click(object sender, EventArgs e)
        {
            PlcWrite.writeToPLC("M1129", true);
            Thread.Sleep(100);
            PlcWrite.writeToPLC("M1130", false);
        }
        private void pbSteel_PSetting_Click(object sender, EventArgs e)
        {
            PlcWrite.writeToPLC("M1129", false);
            Thread.Sleep(100);
            PlcWrite.writeToPLC("M1130", true);
        }
        #endregion
        #region --NG逆轉處理--
        private void pbContinue_PSetting_Click(object sender, EventArgs e)
        {
            PlcWrite.writeToPLC("M1131", false);
            Thread.Sleep(100);
            PlcWrite.writeToPLC("M1132", true);
        }
        private void pbBackOut_PSetting_Click(object sender, EventArgs e)
        {
            PlcWrite.writeToPLC("M1131", true);
            Thread.Sleep(100);
            PlcWrite.writeToPLC("M1132", false);
        }
        #endregion
        #region --光柵--
        private void pbGateEnable_Click(object sender, EventArgs e)
        {
            PlcWrite.writeToPLC("M1136", false);
        }
        private void pbGateDisable_Click(object sender, EventArgs e)
        {
            PlcWrite.writeToPLC("M1136", true);
        }
        #endregion
        #region --Language--
        private void pbChangeToJapan_Click(object sender, EventArgs e)
        {
            //設定
            LanguageSelection = 1;//中文=0,日文=1
            //取得文字檔
            ChangeLanglue();
            //改變初始檔
        }

        private void pbChangeToChinese_Click(object sender, EventArgs e)
        {
            //設定
            LanguageSelection = 0;//中文=0,日文=1
            //取得文字檔
            ChangeLanglue();
            //改變初始檔
        }

        #endregion
        #endregion
        #endregion
        //取得伺服槍數值
        private void getDriverData()
        {
            csProtocal p = new csProtocal();
            if (myTV.TorqueValue!=null)
            {
                myTV.TorqueValue.Clear();
            }
            p.CommandCode = "R";
            p.Vno = "R0010";
            for (int i = 0; i < 6; i++)
            {
                p.ID = (i + 1).ToString();
                myLog.LogText = System.Text.ASCIIEncoding.ASCII.GetString(p.SendByte);
                Driver.dataReceiveFinishFlag = false;
                int goflag = 0;
            s1:
                Driver.sendToDriver(p);
                goflag++;
                int j = 0;
                while (!(Driver.dataReceiveFinishFlag || j >= 20))
                {
                    j++;
                    Thread.Sleep(100);
                }
                if (Driver.dataReceiveFinishFlag)
                {
                    myLog.LogText = (i + 1).ToString() + "完成" + Driver.Data;
                    myTV.TorqueValue.Add(Driver.Data);
                }
                else
                {
                    if (goflag<5)
                    {
                        goto s1;
                    }
                    myLog.LogText = (i + 1).ToString() + "超時";
                   myTV.TorqueValue.Add($"{i+1}D00000");
                }
                Thread.Sleep(100);
            }
            myTV.saveTorqueData();
        }
        private void setData(csTorqueValue _data)
        {
            foreach (var x in _data.TorqueValue)
            {
                if (x.Contains("01D"))
                {
                    int index = x.LastIndexOf('D');
                    string s=x.Substring(x.Length - 2 - index);
                    try
                    {
                        double d = double.Parse(s);
                        d = d / 1000;
                        lbTorqueValue[0].Text = string.Format("{0:#.###NM}",d);
                    }
                    catch (Exception ex)
                    {
                        myLog.LogText = ex.ToString();
                    }
                }

                if (x.Contains("02D"))
                {
                    int index = x.LastIndexOf('D');
                    string s = x.Substring(x.Length - 2 - index);
                    try
                    {
                        double d = double.Parse(s);
                        d = d / 1000;
                        lbTorqueValue[1].Text = string.Format("{0:#.###NM}", d);
                    }
                    catch (Exception ex)
                    {
                        myLog.LogText = ex.ToString();
                    }
                }

                if (x.Contains("03D"))
                {
                    int index = x.LastIndexOf('D');
                    string s = x.Substring(x.Length - 2 - index);
                    try
                    {
                        double d = double.Parse(s);
                        d = d / 1000;
                        lbTorqueValue[2].Text = string.Format("{0:#.###NM}", d);
                    }
                    catch (Exception ex)
                    {
                        myLog.LogText = ex.ToString();
                    }
                }

                if (x.Contains("04D"))
                {
                    int index = x.LastIndexOf('D');
                    string s = x.Substring(x.Length - 2 - index);
                    try
                    {
                        double d = double.Parse(s);
                        d = d / 1000;
                        lbTorqueValue[3].Text = string.Format("{0:#.###NM}", d);
                    }
                    catch (Exception ex)
                    {
                        myLog.LogText = ex.ToString();
                    }
                }

                if (x.Contains("05D"))
                {
                    int index = x.LastIndexOf('D');
                    string s = x.Substring(x.Length - 2 - index);
                    try
                    {
                        double d = double.Parse(s);
                        d = d / 1000;
                        lbTorqueValue[4].Text = string.Format("{0:#.###NM}", d);
                    }
                    catch (Exception ex)
                    {
                        myLog.LogText = ex.ToString();
                    }
                }

                if (x.Contains("06D"))
                {
                    int index = x.LastIndexOf('D');
                    string s = x.Substring(x.Length - 2 - index);
                    try
                    {
                        double d = double.Parse(s);
                        d = d / 1000;
                        lbTorqueValue[5].Text = string.Format("{0:#.###NM}", d);
                    }
                    catch (Exception ex)
                    {
                        myLog.LogText = ex.ToString();
                    }
                }
            }
        }
        //變更顯示文字
        private void ChangeLanglue()
        {
            //讀出檔案
            XLWorkbook wb = new XLWorkbook(@"D:\Resource\Language_19601.xlsx");
            var ws = wb.Worksheet(1);
            int lastCount = ws.LastRowUsed().RowNumber();
            //放入字串陣列
            LanguageString = new string[lastCount];
            for (int i = 0; i < lastCount; i++)
            {
                switch (LanguageSelection)
                {
                    case 0:
                        LanguageString[i] = (string)ws.Cell(i + 1, 1).Value;
                        break;
                    case 1:
                        LanguageString[i] = (string)ws.Cell(i + 1, 2).Value;
                        break;
                    default:
                        LanguageString[i] = (string)ws.Cell(i + 1, 1).Value;
                        break;
                }
            }
            //改變顯示
            label13.Text = LanguageString[111];
            label24.Text = LanguageString[111];
            label26.Text = LanguageString[112];
            label30.Text = LanguageString[112];
            label28.Text = LanguageString[113];
            label32.Text = LanguageString[113];
            pbNew_PSetting.Text = LanguageString[98];
            pbRepair_PSetting.Text = LanguageString[99];
            pbSteel_PSetting.Text = LanguageString[102];
            pbBackOut_PSetting.Text = LanguageString[104];
            pbContinue_PSetting.Text = LanguageString[105];
            pbGateEnable.Text = LanguageString[107];
            pbGateDisable.Text = LanguageString[108];
            label14.Text = LanguageString[109];
            label7.Text = LanguageString[115];
            label8.Text = LanguageString[116];
            pbErrorReset.Text = LanguageString[117];
            pbStopBZ.Text = LanguageString[118];
            ftt.Enable = LanguageString[33];
            ftt.Disable = LanguageString[32];
            label9.Text = LanguageString[0];
            lbAuto_PAuto.Text = LanguageString[0];
            lbAuto_PManual.Text = LanguageString[0];
            lbOrg_PAuto.Text = LanguageString[1];
            lbOrg_PManual.Text = LanguageString[1];
            lbError_PAuto.Text = LanguageString[2];
            lbError_PManual.Text = LanguageString[2];
            lbFeedScrew_PAuto.Text = LanguageString[3];
            lbFeedScrew_PManual.Text = LanguageString[3];
            label16.Text = LanguageString[4];
            label17.Text = LanguageString[5];
            label2.Text = LanguageString[5];
            lbY0000.Text = LanguageString[6];
            pbY0000.Text = LanguageString[6];
            pbY0000_M7.Text = LanguageString[6];
            lbY0001.Text = LanguageString[7];
            pbY0001.Text = LanguageString[7];
            pbY0001_M7.Text = LanguageString[7];
            lbY0002.Text = LanguageString[8];
            pbY0002.Text = LanguageString[8];
            pbY0002_M7.Text = LanguageString[8];
            lbY0004.Text = LanguageString[9];
            pbY0004.Text = LanguageString[9];
            pbY0004_M7.Text = LanguageString[9];
            lbY0006.Text = LanguageString[10];
            pbY0006.Text = LanguageString[10];
            lbY0007.Text = LanguageString[11];
            pbY0007.Text = LanguageString[11];
            lbY0010.Text = LanguageString[12];
            pbY0010.Text = LanguageString[12];
            lbY0011.Text = LanguageString[13];
            pbY0011.Text = LanguageString[13];
            lbY0012.Text = LanguageString[14];
            pbY0012.Text = LanguageString[14];
            lbY0013.Text = LanguageString[15];
            pbY0013.Text = LanguageString[15];
            lbY0014.Text = LanguageString[16];
            pbY0014.Text = LanguageString[16];
            lbY0015.Text = LanguageString[17];
            pbY0015.Text = LanguageString[17];
            lbY0016.Text = LanguageString[18];
            pbY0016.Text = LanguageString[18];
            lbY0017.Text = LanguageString[19];
            pbY0017.Text = LanguageString[19];
            lbY0020.Text = LanguageString[20];
            pbY0020.Text = LanguageString[20];
            pbY0020_M7.Text = LanguageString[20];
            lbY0021.Text = LanguageString[21];
            pbY0021.Text = LanguageString[21];
            pbY0021_M7.Text = LanguageString[21];
            lbY0022.Text = LanguageString[22];
            pbY0022.Text = LanguageString[22];
            lbY0023.Text = LanguageString[23];
            pbY0023.Text = LanguageString[23];
            lbY0024.Text = LanguageString[24];
            pbY0024.Text = LanguageString[24];
            lbY0025.Text = LanguageString[25];
            pbY0025.Text = LanguageString[25];
            lbY0026.Text = LanguageString[26];
            pbY0026.Text = LanguageString[26];
            lbY0027.Text = LanguageString[27];
            pbY0027.Text = LanguageString[27];
            lbY0030.Text = LanguageString[28];
            pbY0030.Text = LanguageString[28];
            lbY0033.Text = LanguageString[29];
            pbY0033.Text = LanguageString[29];
            pbY0033_M7.Text = LanguageString[29];
            lbY1000.Text = LanguageString[30];
            pbFeedScrew.Text = LanguageString[30];
            label3.Text = LanguageString[7];
            label5.Text = LanguageString[97];
            label18.Text = LanguageString[97];
            label1.Text = LanguageString[100];
            label4.Text = LanguageString[100];
            label19.Text = LanguageString[103];
            label12.Text = LanguageString[103];
            label20.Text = LanguageString[106];
        }
    }
}
