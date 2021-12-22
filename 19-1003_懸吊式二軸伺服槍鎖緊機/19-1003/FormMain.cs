using AdvanTechDIOCard;
using MachineLog;
using SBDDriver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using ClassLibraryCommonUse;
using System.Drawing;

namespace _19_1003
{
    public partial class FormMain : Form
    {
        FormOP fop;
        FormLogIn fLn;
        public FormMain()
        {
            InitializeComponent();
            getConfig();
            getWorkTypeList();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            mBarcodeMatch = false;
            initialAdvanIoCard();
            initialScrewDriver();
            initialDateTime();
            InitialUser();
            fop = new FormOP();
            fop.Show();
            fLn = new FormLogIn();
            fLn.mUsers = myUsers;
            lbDateTime.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
            fop.myDateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
            getWorkTypeList();
            //顯示Count
            fop.OKCount = mOKCount;
            //run tcp server(barcode reader)
            server = null;
            IPAddress serverIIP = IPAddress.Parse("10.5.30.200");
            server = new TcpListener(serverIIP, 23);
            server.Start();
            serverThread = new Thread(serverListion);
            serverThread.IsBackground = true;
            serverThread.Start();
        }
        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            saveConfig();
            t1Min.Stop();
            t1Min.Dispose();
            ScrewDriver = null;
            mDriverL = null;
            mDriverR = null;
            ioCard = null;
            serverThread.Abort();
            server.Stop();
            server = null;
            fop.Dispose();
        }
        //*******************主要工作**************************
        #region --Date Time--
        //**************define******************
        System.Windows.Forms.Timer t1Min = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer t1Sec;
        System.Windows.Forms.Timer t1Sec2;
        //工作計時器
        int mWorkTimer;
        int mSvWorkTimer;
        int m1ScrewFinish;
        //**************funtion******************
        private void initialDateTime()
        {
            t1Min.Interval = 60000;
            t1Min.Tick += T1Min_Tick;
            t1Min.Start();
            t1Sec = new System.Windows.Forms.Timer();
            t1Sec.Interval = 1000;
            t1Sec.Tick += T1Sec_Tick;
            t1Sec.Start();
            t1Sec2 = new System.Windows.Forms.Timer();
            t1Sec2.Interval = 1000;
            t1Sec2.Tick += T2Sec_Tick;
            t1Sec2.Start();
            mSvWorkTimer = Convert.ToInt32(textBoxSvWorkTimer.Text);
            mWorkTimer = 0;
        }
        
        private void T2Sec_Tick(object sender, EventArgs e)
        {
            t1Sec2.Stop();
            //取回操作箱work type
            int i = fop.workTypeIndex;
            if (i!=mCurrentWorkTypeIndex)
            {
                cbWorkType.SelectedIndex = i;
            }
            if (m1ScrewFinish >= 10 && (mMachineState == 1 || mMachineState==2 || mMachineState==3))
            {
                mMachineState = 0;
                m1ScrewFinish = 0;
            }
            if (m1ScrewFinish >= 5 && mMachineState == 4)
            {
                mMachineState = 0;
                m1ScrewFinish = 0;
            }
            if (m1ScrewFinish>=3 && mMachineState==5)
            {
                mMachineState = 0;
                m1ScrewFinish = 0;
            }
            if (mMachineState!=0)
            {
                m1ScrewFinish++;
            }
            //取回操作箱軸數
            int axis = fop.StateAxisEnable;
            switch (axis)
            {
                case 1:
                    ScrewDriver.AxisCount = 1;
                    break;
                case 2:
                    ScrewDriver.AxisCount = 1;
                    break;
                case 3:
                    ScrewDriver.AxisCount = 2;
                    break;
            }
            t1Sec2.Start();
        }

        private void T1Sec_Tick(object sender, EventArgs e)
        {
            t1Sec.Stop();
            if (!outputPauseLamp)
            {
                mWorkTimer += 1;
                fop.WorkTimerValue = mWorkTimer;
                if (mWorkTimer >= mSvWorkTimer && mMachineState==0)
                {
                    mMachineState = 3;
                    fop.WorkTimerState = true;
                    LogOperate.LogText = "工作計時器警報";
                }
                else
                {
                    fop.WorkTimerState = false;
                }
                t1Sec.Start();
            }
        }

        private void T1Min_Tick(object sender, EventArgs e)
        {
            t1Min.Stop();
            lbDateTime.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
            fop.myDateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
            t1Min.Start();
        }
        #endregion
        #region --Log--
        //**************define******************
        csLog LogOperate = new csLog("MachineOperate");
        #endregion
        #region --研華--
        #region --define--
        //**************define******************
        DioCard ioCard;
        System.Windows.Forms.Timer t100msec = new System.Windows.Forms.Timer();
        bool screwDriverInputC = false; //X00 In Cycle
        bool InputDriverReady = false; //X01 Ready
        bool inputDriverCycleOK1 = false; //X02 cycle ok ax1
        bool inputDriverCycleOK2 = false; //X03 cycle ok ax2
        bool InputDriverCycleNOK = false; //X04 cycle nok any
        bool InputDriverJobBit2 = false; //X05 job bit2
        bool InputDriverJobBit3 = false; //X06 job bit3
        bool InputDriverJobBit4 = false; //X07 job bit4
        bool pbStart1 = false;//X10 Start按鍵
        bool pbStart2 = false;//X11 Start按鍵
        bool pbEStop = false;//X12 E-STOP
        bool pbResetSaftyModule = false;//X13 safty module reset按鍵
        bool pbDriverReverse = false;//X20 伺服槍反轉
        bool pbAddCount = false;//X21 計數+
        bool pbSubCount = false;//X22 計數-
        bool pbResetToStart = false;//X23 reset後重鎖
        bool pbResetToThisOne = false;//X24 reset後此顆螺絲鎖到好
        bool pbPause = false;//X25 停止計時器
        bool pbPass = false;//X26 放棄此顆,繼續下一個
        bool contantSaftyModule = false;//X27 safty module state

        bool outputDriverStartAll; //Y00 DriverL 雙軸起動
        bool outputDriverStartL; //Y01 DriverM 左軸起動
        bool outputDriverStartR;//Y02 DriverN 右軸起動
        bool outputDriverStop;//Y03 DriverP 軸停止
        bool outputDriverReverseL;//Y04 DriverR 軸1反轉
        bool outputDriverReverseR;//Y05 DriverS 軸2反轉
        bool outputDriverT;//Y06 DriverT
        bool outputDriverU;//Y07 DriverU
        bool outputPauseLamp;//Y10 Pause Lamp
        bool outputPassLamp;//Y11 Pass Lamp
        bool outputK2RelaySaftyModuleReset;//Y12 K2 relay Safty Module Reset
        bool outputBZ0;//Y20 BZ0
        bool outputBZ1;//Y21 BZ1
        bool outputBZ2;//Y22 BZ2
        bool outputBZ3;//Y23 BZ3
        bool outputGL;//Y24 GL
        bool outputRL;//Y25 RL
        bool statePBResetOnce = false;
        bool statePBCountAdd = false;
        bool statePBCountSub = false;
        bool stateDriverReady = false;
        bool stateDriverStart = false;
        bool stateDriverOK1 = false;
        bool stateDriverOK2 = false;
        bool stateDriverNOK = false;
        int mSwitchAxis;
        /// <summary>
        /// 機器狀態,0:無,1:barcdoe error,2:screw error,3:工作計時,ESTOP,4:全部鎖付完成,5:單次鎖付完成
        /// </summary>
        int mMachineState;
        int mOKCount;
        #endregion
        //**************funtion******************
        private void initialAdvanIoCard()
        {
            ioCard = new DioCard("PCIE-1756,BID#0", 4, 4);
            t100msec.Interval = 100;
            t100msec.Tick += t100mSec_Tick;
            t100msec.Start();
        }
        private void t100mSec_Tick(object sender, EventArgs e)
        {
            t100msec.Stop();
            mappingInputCard();
            MainFlow();
            procssOutput();
            showCallBackText(labelScrewStep, mScrewStep.ToString());
            showCallBackText(labelScrewCount, mScrewCount.ToString());
            if (fLn!= null && fLn.CurrentUser.UserLevel!=0)
            {
                labelLogin_PageConfig.Text = fLn.CurrentUser.UserId;
                labelLogin_PageType.Text = fLn.CurrentUser.UserId;
                labelLogin_PageUser.Text = fLn.CurrentUser.UserId;
                textBoxSvWorkTimer.Enabled = true;
                bnReadWorkTypeList.Enabled = true;
                bnNewType.Enabled = true;
                bnDeleteType.Enabled = true;
                pbInsertStep.Enabled = true;
                pbDeleteStep.Enabled = true;
                if (fLn.CurrentUser.UserLevel==8)
                {
                    pbReadUser.Enabled = true;
                    pbModifyUser.Enabled = true;
                    pbDeleteUser.Enabled = true;
                }
                else
                {
                    pbReadUser.Enabled = false;
                    pbModifyUser.Enabled = false;
                    pbDeleteUser.Enabled = false;
                }
            }
            else
            {
                labelLogin_PageConfig.Text = fLn.CurrentUser.UserId;
                labelLogin_PageType.Text = fLn.CurrentUser.UserId;
                labelLogin_PageUser.Text = fLn.CurrentUser.UserId;
                textBoxSvWorkTimer.Enabled = false;
                bnReadWorkTypeList.Enabled = false;
                bnNewType.Enabled = false;
                bnDeleteType.Enabled = false;
                pbInsertStep.Enabled = false;
                pbDeleteStep.Enabled = false;
                pbReadUser.Enabled = false;
                pbModifyUser.Enabled = false;
                pbDeleteUser.Enabled = false;
            }
            t100msec.Start();
        }
        private void mappingInputCard()
        {
            screwDriverInputC = ioCard.inputX[0, 0];
            InputDriverReady = ioCard.inputX[0, 1];
            inputDriverCycleOK1 = ioCard.inputX[0, 2];
            inputDriverCycleOK2 = ioCard.inputX[0, 3];
            InputDriverCycleNOK = ioCard.inputX[0, 4];
            InputDriverJobBit2 = ioCard.inputX[0, 5];
            InputDriverJobBit3 = ioCard.inputX[0, 6];
            InputDriverJobBit4 = ioCard.inputX[0, 7];
            pbStart1 = ioCard.inputX[1, 0];
            pbStart2 = ioCard.inputX[1, 1];
            pbEStop = ioCard.inputX[1, 2];
            pbResetSaftyModule = ioCard.inputX[1, 3];
            pbDriverReverse = ioCard.inputX[2, 0];
            pbAddCount = ioCard.inputX[2, 1];
            pbSubCount = ioCard.inputX[2, 2];
            pbResetToStart = ioCard.inputX[2, 3];
            pbResetToThisOne = ioCard.inputX[2, 4];
            pbPause = ioCard.inputX[2, 5];
            pbPass = ioCard.inputX[2, 6];
            contantSaftyModule = ioCard.inputX[2, 7];

            outputDriverStartAll = ioCard.outputY[0, 0];
            outputDriverStartL = ioCard.outputY[0, 1];
            outputDriverStartR = ioCard.outputY[0, 2];
            outputDriverStop = ioCard.outputY[0, 3];
            outputDriverReverseL = ioCard.outputY[0, 4];
            outputDriverReverseR = ioCard.outputY[0, 5];
            outputDriverT = ioCard.outputY[0, 6];
            outputDriverU = ioCard.outputY[0, 7];
            outputPauseLamp = ioCard.outputY[1, 0];
            outputPassLamp = ioCard.outputY[1, 1];
            outputK2RelaySaftyModuleReset = ioCard.outputY[1, 2];
            //outputDriverStop = ioCard.outputY[1, 3];
            //outputDriverReverseL = ioCard.outputY[1, 4];
            //outputDriverReverseR = ioCard.outputY[1, 5];
            //outputDriverT = ioCard.outputY[1, 6];
            //outputDriverU = ioCard.outputY[1, 7];
            outputBZ0 = ioCard.outputY[2, 0];
            outputBZ1 = ioCard.outputY[2, 1];
            outputBZ2 = ioCard.outputY[2, 2];
            outputBZ3 = ioCard.outputY[2, 3];
            outputGL = ioCard.outputY[2, 4];
            outputRL = ioCard.outputY[2, 5];
            //outputDriverT = ioCard.outputY[2, 6];
            //outputDriverU = ioCard.outputY[2, 7];
        }
        private void procssOutput()
        {
            if (fop!=null)
            {
                mSwitchAxis = fop.StateAxisEnable;
            }
            #region --起動按鈕--
            //起動按鈕
            if (pbStart1 && pbStart2 && ((InputDriverReady && mBarcodeMatch && mMachineState == 0) || pbDriverReverse) && !stateDriverStart)
            {
                stateDriverStart = true;
                mWorkTimer = 0;
                mSwitchAxis = fop.StateAxisEnable;
                switch (mSwitchAxis)
                {
                    case 1:
                        ioCard.setOutputDeviceState(0, 1, 1);
                        break;
                    case 2:
                        ioCard.setOutputDeviceState(0, 2, 1);
                        break;
                    case 3:
                        ioCard.setOutputDeviceState(0, 0, 1);
                        break;
                    default:
                        break;
                }
            }
            if (!pbStart1 && !pbStart2 && stateDriverStart)
            {
                stateDriverStart = false;
            }
            if ((!pbStart1 || !pbStart2) && pbDriverReverse)
            {
                ioCard.setOutputDeviceState(0, 0, 0);
                ioCard.setOutputDeviceState(0, 1, 0);
                ioCard.setOutputDeviceState(0, 2, 0);
            }
            #endregion
            //Driver反轉
            if (pbDriverReverse)
            {
                //mWorkTimer = 0;
                mSwitchAxis = fop.StateAxisEnable;
                switch (mSwitchAxis)
                {
                    case 1:
                        ioCard.setOutputDeviceState(0, 4, 1);
                        break;
                    case 2:
                        ioCard.setOutputDeviceState(0, 5, 1);
                        break;
                    case 3:
                        ioCard.setOutputDeviceState(0, 4, 1);
                        ioCard.setOutputDeviceState(0, 5, 1);
                        break;
                    default:
                        ioCard.setOutputDeviceState(0, 4, 1);
                        ioCard.setOutputDeviceState(0, 5, 1);
                        break;
                }
            }
            else
            {
                ioCard.setOutputDeviceState(0, 4, 0);
                ioCard.setOutputDeviceState(0, 5, 0);
            }
            //緊急停止
            if (pbEStop && mMachineState==0)
            {
                mMachineState = 3;
                LogOperate.LogText = "緊急停止扣押";
                fop.stateEStop = true;
            }
            else
            {
                fop.stateEStop = false;
            }
            //重置安全繼電器X13,Y12
            if (pbResetSaftyModule)
            {
                if (!contantSaftyModule)
                {
                    ioCard.setOutputDeviceState(1, 2, 1);
                }
                if (mMachineState!=0)
                {
                    mMachineState = 0;
                }
                mWorkTimer = 0;
                mMachineState = 0;
                m1ScrewFinish = 0;
            }
            else
            {
                ioCard.setOutputDeviceState(1, 2, 0);
            }
            //安全繼電器狀態
            if (contantSaftyModule)
            {
                fop.rySaftyModule = true;
            }
            else
            {
                fop.rySaftyModule = false;
                mBarcodeMatch = false;
            }
            //SBD Driver ready狀態
            if (InputDriverReady)
            {
                fop.DriverReady = true;
                if (!stateDriverReady)
                {
                    stateDriverReady = true;
                    //設定work type
                    if (mCurrentWorkTypeIndex >= 0 && cbWorkType.Items.Count > mCurrentWorkTypeIndex)
                    {
                        cbWorkType.SelectedIndex = mCurrentWorkTypeIndex;
                    }
                    else
                    {
                        mCurrentWorkTypeIndex = 0;
                        cbWorkType.SelectedIndex = mCurrentWorkTypeIndex;
                    }
                }
            }
            else
            {
                stateDriverReady = false;
                fop.DriverReady = false;
            }
            #region --cycle ok,nok--
            //AX1 OK
            if (mSwitchAxis == 1 && inputDriverCycleOK1 && !stateDriverOK1)
            {
                stateDriverOK1 = true;
                procssGetData();
            }
            if (!inputDriverCycleOK1 && stateDriverOK1)
            {
                stateDriverOK1 = false;
            }
            //AX2 OK
            if (mSwitchAxis == 2 && inputDriverCycleOK2 && !stateDriverOK2)
            {
                stateDriverOK2 = true;
                procssGetData();
            }
            if (!inputDriverCycleOK2 && stateDriverOK2)
            {
                stateDriverOK2 = false;
            }
            //AX all OK
            if (mSwitchAxis==3 && inputDriverCycleOK1 && inputDriverCycleOK2 && !stateDriverOK1 && !stateDriverOK2)
            {
                stateDriverOK1 = true;
                stateDriverOK2 = true;
                procssGetData();
            }

            //SBD Driver CycleNOK
            if (InputDriverCycleNOK && !stateDriverNOK)
            {
                stateDriverNOK = true;
                procssGetData();
            }
            if (!InputDriverCycleNOK && stateDriverNOK)
            {
                stateDriverNOK = false;
            }
            #endregion
            //count +
            if (pbAddCount && !statePBCountAdd)
            {
                mWorkTimer = 0;
                statePBCountAdd = true;
                mOKCount++;
                lbOKCount.Text = mOKCount.ToString();
                fop.OKCount = mOKCount;
            }
            if (!pbAddCount && statePBCountAdd)
            {
                statePBCountAdd = false;
            }
            //count -
            if (pbSubCount && !statePBCountSub)
            {
                mWorkTimer = 0;
                statePBCountSub = true;
                mOKCount--;
                lbOKCount.Text = mOKCount.ToString();
                fop.OKCount = mOKCount;
            }
            if (!pbSubCount && statePBCountSub)
            {
                statePBCountSub = false;
            }
            //全部RESET
            if (pbResetToStart && mScrewFlowStartFlag)
            {
                mWorkTimer = 0;
                if (mDriverL.ScrewDatas.Count!=0 && mDriverR.ScrewDatas.Count!=0)
                {
                    saveScrewData();
                }
                mBarcodeMatch = false;
                fop.BarcodeMatch = 0;
                fop.workBarcode = " ";
                tbBarcode.Text = "";
                //重置fop
                mDriverL.ScrewDatas.Clear();
                mDriverR.ScrewDatas.Clear();
                mScrewStep = 0;
                mScrewCount = mCurrentWorkType.WorkSteps[mScrewStep].ScrewNo;
                string s = mCurrentWorkType.WorkSteps[0].getSBDDriverBarcodeString();
                ScrewDriver.sendJobBarcode(s);
                fop.screwStateL1 = 0;
                fop.screwStateR1 = 0;
                fop.screwStateL2 = 0;
                fop.screwStateR2 = 0;
                fop.screwStateL3 = 0;
                fop.screwStateR3 = 0;
                fop.screwStateL4 = 0;
                fop.screwStateR4 = 0;
                fop.screwStateL5 = 0;
                fop.screwStateR5 = 0;
                fop.screwTorqueL1 = "0";
                fop.screwTorqueR1 = "0";
                fop.screwTorqueL2 = "0";
                fop.screwTorqueR2 = "0";
                fop.screwTorqueL3 = "0";
                fop.screwTorqueR3 = "0";
                fop.screwTorqueL4 = "0";
                fop.screwTorqueR4 = "0";
                fop.screwTorqueL5 = "0";
                fop.screwTorqueR5 = "0";
                fop.screwAngleL1 = "0";
                fop.screwAngleL2 = "0";
                fop.screwAngleL3 = "0";
                fop.screwAngleL4 = "0";
                fop.screwAngleL5 = "0";
                fop.screwAngleR1 = "0";
                fop.screwAngleR2 = "0";
                fop.screwAngleR3 = "0";
                fop.screwAngleR4 = "0";
                fop.screwAngleR5 = "0";
                LogOperate.LogText = "按下全部RESET:" + mScrewCount.ToString() + "," + mScrewStep.ToString();
            }
            //單動RESET,清除顯示
            if (pbResetToThisOne && mScrewFlowStartFlag && !statePBResetOnce && mScrewStep!=0)
            {
                mWorkTimer = 0;
                statePBResetOnce = true;
                //取得重鎖step
                mSwitchAxis = fop.StateAxisEnable;
                int lastStep;
                int lastCount;
                int lastlastCount;
                if (mScrewStep <2)
                {
                    lastStep = 0;
                    mDriverL.ScrewDatas.Clear();
                    mDriverR.ScrewDatas.Clear();
                }
                else
                {
                    //取得前二個孔位
                    lastStep = mScrewStep - 1;
                    lastCount = mCurrentWorkType.WorkSteps[lastStep].ScrewNo;
                    lastlastCount = mCurrentWorkType.WorkSteps[lastStep - 1].ScrewNo;
                    //刪除資料
                    switch (mSwitchAxis)
                    {
                        case 1:
                            if (mDriverL.ScrewDatas.Count != 0)
                            {
                                mDriverL.ScrewDatas.RemoveAt(mDriverL.ScrewDatas.Count - 1);
                            }
                            break;
                        case 2:
                            if (mDriverR.ScrewDatas.Count != 0)
                            {
                                mDriverR.ScrewDatas.RemoveAt(mDriverR.ScrewDatas.Count - 1);
                            }
                            break;
                        case 3:
                            if (mDriverL.ScrewDatas.Count!=0)
                            {
                                mDriverL.ScrewDatas.RemoveAt(mDriverL.ScrewDatas.Count - 1);
                            }
                            if (mDriverR.ScrewDatas.Count != 0)
                            {
                                mDriverR.ScrewDatas.RemoveAt(mDriverR.ScrewDatas.Count - 1);
                            }
                            break;
                        default:
                            break;
                    }
                    //前二孔位相同
                    while (lastCount == lastlastCount)
                    {
                        //刪除資料
                        switch (mSwitchAxis)
                        {
                            case 1:
                                if (mDriverL.ScrewDatas.Count != 0)
                                {
                                    mDriverL.ScrewDatas.RemoveAt(mDriverL.ScrewDatas.Count - 1);
                                }
                                break;
                            case 2:
                                if (mDriverR.ScrewDatas.Count != 0)
                                {
                                    mDriverR.ScrewDatas.RemoveAt(mDriverR.ScrewDatas.Count - 1);
                                }
                                break;
                            case 3:
                                if (mDriverL.ScrewDatas.Count != 0)
                                {
                                    mDriverL.ScrewDatas.RemoveAt(mDriverL.ScrewDatas.Count - 1);
                                }
                                if (mDriverR.ScrewDatas.Count != 0)
                                {
                                    mDriverR.ScrewDatas.RemoveAt(mDriverR.ScrewDatas.Count - 1);
                                }
                                break;
                            default:
                                break;
                        }
                        lastStep--;
                        if (lastStep == 0)
                        {
                            mDriverL.ScrewDatas.Clear();
                            mDriverR.ScrewDatas.Clear();
                            break;
                        }
                        else
                        {
                            lastCount = mCurrentWorkType.WorkSteps[lastStep].ScrewNo;
                            lastlastCount = mCurrentWorkType.WorkSteps[lastStep - 1].ScrewNo;
                        }
                    }
                }
                
                mScrewStep = lastStep;
                mScrewCount = mCurrentWorkType.WorkSteps[mScrewStep].ScrewNo;
                string s = mCurrentWorkType.WorkSteps[mScrewStep].getSBDDriverBarcodeString();
                ScrewDriver.sendJobBarcode(s);
                //改變指示至下一孔位
                changePoint();
                mSwitchAxis = fop.StateAxisEnable;
                switch (mSwitchAxis)
                {
                    case 1:
                        switch (mScrewCount)
                        {
                            case 1:
                                fop.screwStateL2 = 0;
                                break;
                            case 2:
                                fop.screwStateL3 = 0;
                                break;
                            case 3:
                                fop.screwStateL4 = 0;
                                break;
                            case 4:
                                fop.screwStateL5 = 0;
                                break;
                            case 5:
                                //fop.screwStateL4 = 0;
                                break;
                            default:
                                break;
                        }
                        break;
                    case 2:
                        switch (mScrewCount)
                        {
                            case 1:
                                fop.screwStateR2 = 0;
                                break;
                            case 2:
                                fop.screwStateR3 = 0;
                                break;
                            case 3:
                                fop.screwStateR4 = 0;
                                break;
                            case 4:
                                fop.screwStateR5 = 0;
                                break;
                            case 5:
                                //fop.screwStateR4 = 0;
                                break;
                            default:
                                break;
                        }
                        break;
                    case 3:
                        switch (mScrewCount)
                        {
                            case 1:
                                fop.screwStateL2 = 0;
                                fop.screwStateR2 = 0;
                                break;
                            case 2:
                                fop.screwStateL3 = 0;
                                fop.screwStateR3 = 0;
                                break;
                            case 3:
                                fop.screwStateL4 = 0;
                                fop.screwStateR4 = 0;
                                break;
                            case 4:
                                fop.screwStateL5 = 0;
                                fop.screwStateR5 = 0;
                                break;
                            case 5:
                                //fop.screwStateL4 = 0;
                                //fop.screwStateR4 = 0;
                                break;
                            default:
                                break;
                        }
                        break;
                }
                mBarcodeMatch = true;
                LogOperate.LogText = "按下單動RESET:孔位:" + mScrewCount.ToString() + ",步驟:" + mScrewStep.ToString();
                Thread.Sleep(100);
            }
            if (!pbResetToThisOne && statePBResetOnce)
            {
                statePBResetOnce = false;
                Thread.Sleep(100);
            }
            //Pass
            if (pbPass && mScrewFlowStartFlag)
            {
                mWorkTimer = 0;
                //取得重鎖step
                mSwitchAxis = fop.StateAxisEnable;
                int currentStep=mScrewStep;
                int currentCount;
                int nextCount;
                if (mScrewStep + 1 < mCurrentCountOfScrewStep)
                {
                    //取得前1個孔位
                    currentCount = mCurrentWorkType.WorkSteps[currentStep].ScrewNo;
                    nextCount = mCurrentWorkType.WorkSteps[currentStep+1].ScrewNo;
                    while (nextCount == currentCount)
                    {
                        currentStep++;
                        if (currentStep + 1 < mCurrentCountOfScrewStep)
                        {
                            currentCount = mCurrentWorkType.WorkSteps[currentStep].ScrewNo;
                            nextCount = mCurrentWorkType.WorkSteps[currentStep + 1].ScrewNo;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                if (currentStep + 1 < mCurrentCountOfScrewStep)
                {
                    //改變JOB
                    mScrewStep = currentStep + 1;
                    mScrewCount = mCurrentWorkType.WorkSteps[mScrewStep].ScrewNo;
                    string s = mCurrentWorkType.WorkSteps[mScrewStep].getSBDDriverBarcodeString();
                    ScrewDriver.sendJobBarcode(s);
                    //改變指示至下一孔位
                    changePoint();
                    mSwitchAxis = fop.StateAxisEnable;
                    switch (mSwitchAxis)
                    {
                        case 1:
                            switch (mScrewCount)
                            {
                                case 1:
                                    //fop.screwStateL1 = 0;
                                    break;
                                case 2:
                                    fop.screwStateL1 = 0;
                                    break;
                                case 3:
                                    fop.screwStateL2 = 0;
                                    break;
                                case 4:
                                    fop.screwStateL3 = 0;
                                    break;
                                case 5:
                                    fop.screwStateL4 = 0;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case 2:
                            switch (mScrewCount)
                            {
                                case 1:
                                    //fop.screwStateR1 = 0;
                                    break;
                                case 2:
                                    fop.screwStateR1 = 0;
                                    break;
                                case 3:
                                    fop.screwStateR2 = 0;
                                    break;
                                case 4:
                                    fop.screwStateR3 = 0;
                                    break;
                                case 5:
                                    fop.screwStateR4 = 0;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case 3:
                            switch (mScrewCount)
                            {
                                case 1:
                                    //fop.screwStateL1 = 0;
                                    //fop.screwStateR1 = 0;
                                    break;
                                case 2:
                                    fop.screwStateL1 = 0;
                                    fop.screwStateR1 = 0;
                                    break;
                                case 3:
                                    fop.screwStateL2 = 0;
                                    fop.screwStateR2 = 0;
                                    break;
                                case 4:
                                    fop.screwStateL3 = 0;
                                    fop.screwStateR3 = 0;
                                    break;
                                case 5:
                                    fop.screwStateL4 = 0;
                                    fop.screwStateR4 = 0;
                                    break;
                                default:
                                    break;
                            }
                            break;  
                    }
                    mBarcodeMatch = true;
                    fop.BarcodeMatch = 1;
                }
                Thread.Sleep(500);
            }
            //Pause
            if (pbPause)
            {
                if (outputPauseLamp)
                {
                    ioCard.setOutputDeviceState(1, 0, 0);
                    t1Sec.Start();
                }
                else
                {
                    ioCard.setOutputDeviceState(1, 0, 1);
                    t1Sec.Stop();
                    mWorkTimer = 0;
                    fop.WorkTimerValue = mWorkTimer;
                    fop.WorkTimerState = false;
                }
                Thread.Sleep(500);
            }
            //蜂鳴器 0:OK,1:Barcode不正確,2:鎖付異常
            switch (mMachineState)
            {
                case 0:
                    ioCard.setOutputDeviceState(2, 0, 0);
                    ioCard.setOutputDeviceState(2, 1, 0);
                    ioCard.setOutputDeviceState(2, 2, 0);
                    ioCard.setOutputDeviceState(2, 3, 0);
                    break;
                case 1:
                    ioCard.setOutputDeviceState(2, 0, 1);
                    ioCard.setOutputDeviceState(2, 1, 0);
                    ioCard.setOutputDeviceState(2, 2, 0);
                    ioCard.setOutputDeviceState(2, 3, 0);
                    break;
                case 2:
                    ioCard.setOutputDeviceState(2, 0, 0);
                    ioCard.setOutputDeviceState(2, 1, 1);
                    ioCard.setOutputDeviceState(2, 2, 0);
                    ioCard.setOutputDeviceState(2, 3, 0);
                    break;
                case 3:
                    ioCard.setOutputDeviceState(2, 0, 0);
                    ioCard.setOutputDeviceState(2, 1, 0);
                    ioCard.setOutputDeviceState(2, 2, 1);
                    ioCard.setOutputDeviceState(2, 3, 0);
                    break;
                case 4:
                    ioCard.setOutputDeviceState(2, 0, 0);
                    ioCard.setOutputDeviceState(2, 1, 0);
                    ioCard.setOutputDeviceState(2, 2, 0);
                    ioCard.setOutputDeviceState(2, 3, 1);
                    break;
                case 5:
                    ioCard.setOutputDeviceState(2, 0, 0);
                    ioCard.setOutputDeviceState(2, 1, 0);
                    ioCard.setOutputDeviceState(2, 2, 0);
                    ioCard.setOutputDeviceState(2, 3, 1);
                    break;
            }
            //綠燈
            switch (mMachineState)
            {
                case 0:
                    ioCard.setOutputDeviceState(2, 4, 0);
                    break;
                case 4:
                    ioCard.setOutputDeviceState(2, 4, 1);
                    break;
                case 5:
                    ioCard.setOutputDeviceState(2, 4, 1);
                    break;
            }
            //紅燈
            switch (mMachineState)
            {
                case 0:
                    ioCard.setOutputDeviceState(2, 5, 0);
                    break;
                case 1:
                    ioCard.setOutputDeviceState(2, 5, 1);
                    break;
                case 2:
                    ioCard.setOutputDeviceState(2, 5, 1);
                    break;
                case 3:
                    ioCard.setOutputDeviceState(2, 5, 1);
                    break;
            }
        }
        private void procssGetData()
        {
            mWorkTimer = 0;
            Thread.Sleep(1000);
            //接本次鎖付孔位
            mScrewCount = mCurrentWorkType.WorkSteps[mScrewStep].ScrewNo;
            //資料傳入mDriver
            foreach (var x in ScrewDriver.ScrewDatas)
            {
                //軸1
                if (x.SpindleNumber.Equals("S01"))
                {
                    csSBDScrewData sd = x;
                    sd.ScrewCount = mScrewCount;
                    //顯示鎖付結果
                    switch (mScrewCount)
                    {
                        //孔1
                        case 1:
                            //show value
                            fop.screwTorqueL1 = sd.TorqueValue;
                            fop.screwAngleL1 = sd.AngleValue;

                            //show image
                            if (sd.OverrallStatus == "A")
                            {
                                fop.screwStateL1 = 2;
                            }
                            else
                            {
                                fop.screwStateL1 = 3;
                                mMachineState = 2;
                                mBarcodeMatch = false;
                            }
                            break;
                        //孔2
                        case 2:
                            //show value
                            fop.screwTorqueL2 = sd.TorqueValue;
                            fop.screwAngleL2 = sd.AngleValue;

                            //show image
                            if (sd.OverrallStatus == "A")
                            {
                                fop.screwStateL2 = 2;
                            }
                            else
                            {
                                fop.screwStateL2 = 3;
                                mMachineState = 2;
                                mBarcodeMatch = false;
                            }
                            break;
                        //孔3
                        case 3:
                            //show value
                            fop.screwTorqueL3 = sd.TorqueValue;
                            fop.screwAngleL3 = sd.AngleValue;

                            //show image
                            if (sd.OverrallStatus == "A")
                            {
                                fop.screwStateL3 = 2;
                            }
                            else
                            {
                                fop.screwStateL3 = 3;
                                mMachineState = 2;
                                mBarcodeMatch = false;
                            }
                            break;
                        //孔4
                        case 4:
                            //show value
                            fop.screwTorqueL4 = sd.TorqueValue;
                            fop.screwAngleL4 = sd.AngleValue;

                            //show image
                            if (sd.OverrallStatus == "A")
                            {
                                fop.screwStateL4 = 2;
                            }
                            else
                            {
                                fop.screwStateL4 = 3;
                                mMachineState = 2;
                                mBarcodeMatch = false;
                            }
                            break;
                        //孔5
                        case 5:
                            //show value
                            fop.screwTorqueL5 = sd.TorqueValue;
                            fop.screwAngleL5 = sd.AngleValue;

                            //show image
                            if (sd.OverrallStatus == "A")
                            {
                                fop.screwStateL5 = 2;
                            }
                            else
                            {
                                fop.screwStateL5 = 3;
                                mMachineState = 2;
                                mBarcodeMatch = false;
                            }
                            break;
                        default:
                            break;
                    }
                    //填入data
                    mDriverL.ScrewDatas.Add(sd);
                }
                //軸2
                if (x.SpindleNumber.Equals("S02"))
                {
                    csSBDScrewData sd = x;
                    sd.ScrewCount = mScrewCount;
                    //顯示鎖付結果
                    switch (mScrewCount)
                    {
                        //孔1
                        case 1:
                            //show value
                            fop.screwTorqueR1 = sd.TorqueValue;
                            fop.screwAngleR1 = sd.AngleValue;

                            //show image
                            if (x.OverrallStatus == "A")
                            {
                                fop.screwStateR1 = 2;
                            }
                            else
                            {
                                fop.screwStateR1 = 3;
                                mMachineState = 2;
                                mBarcodeMatch = false;
                            }
                            break;
                        //孔2
                        case 2:
                            //show value
                            fop.screwTorqueR2 = x.TorqueValue;
                            fop.screwAngleR2 = x.AngleValue;

                            //show image
                            if (x.OverrallStatus == "A")
                            {
                                fop.screwStateR2 = 2;
                            }
                            else
                            {
                                fop.screwStateR2 = 3;
                                mMachineState = 2;
                                mBarcodeMatch = false;
                            }
                            break;
                        //孔3
                        case 3:
                            //show value
                            fop.screwTorqueR3 = x.TorqueValue;
                            fop.screwAngleR3 = x.AngleValue;

                            //show image
                            if (x.OverrallStatus == "A")
                            {
                                fop.screwStateR3 = 2;
                            }
                            else
                            {
                                fop.screwStateR3 = 3;
                                mMachineState = 2;
                                mBarcodeMatch = false;
                            }
                            break;
                        //孔4
                        case 4:
                            //show value
                            fop.screwTorqueR4 = x.TorqueValue;
                            fop.screwAngleR4 = x.AngleValue;

                            //show image
                            if (x.OverrallStatus == "A")
                            {
                                fop.screwStateR4 = 2;
                            }
                            else
                            {
                                fop.screwStateR4 = 3;
                                mMachineState = 2;
                                mBarcodeMatch = false;
                            }
                            break;
                        //孔5
                        case 5:
                            //show value
                            fop.screwTorqueR5 = x.TorqueValue;
                            fop.screwAngleR5 = x.AngleValue;

                            //show image
                            if (x.OverrallStatus == "A")
                            {
                                fop.screwStateR5 = 2;
                            }
                            else
                            {
                                fop.screwStateR5 = 3;
                                mMachineState = 2;
                                mBarcodeMatch = false;
                            }
                            break;
                        default:
                            break;
                    }
                    //孔位填入data
                    mDriverR.ScrewDatas.Add(sd);
                }
            }

            //driver stop
            ioCard.setOutputDeviceState(0, 0, 0);
            ioCard.setOutputDeviceState(0, 1, 0);
            ioCard.setOutputDeviceState(0, 2, 0);
            mScrewStep++;
            mOldScrewCount = mScrewCount;
            if (mCurrentCountOfScrewStep > mScrewStep)
            {
                mScrewCount = mCurrentWorkType.WorkSteps[mScrewStep].ScrewNo;
            }
            if (mBarcodeMatch)
            {
                //next step
                if (mCurrentCountOfScrewStep > mScrewStep)
                {
                    //切換伺服槍JOB
                    string s = mCurrentWorkType.WorkSteps[mScrewStep].getSBDDriverBarcodeString();
                    ScrewDriver.sendJobBarcode(s);
                    Thread.Sleep(800);
                    //判下一孔位是否相同
                    if (mScrewCount == mOldScrewCount)
                    {
                        //繼續鎖付動作
                        if (!mStepNotContinum)
                        {
                            funtionScrewStart();
                        }
                        switch (mScrewCount)
                        {
                            case 1:
                                fop.screwStateL1 = 0;
                                fop.screwStateR1 = 0;
                                break;
                            case 2:
                                fop.screwStateL2 = 0;
                                fop.screwStateR2 = 0;
                                break;
                            case 3:
                                fop.screwStateL3 = 0;
                                fop.screwStateR3 = 0;
                                break;
                            case 4:
                                fop.screwStateL4 = 0;
                                fop.screwStateR4 = 0;
                                break;
                            case 5:
                                fop.screwStateL5 = 0;
                                fop.screwStateR5 = 0;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        //改變指示至下一孔位
                        if (mMachineState == 0)
                        {
                            switch (mScrewCount)
                            {
                                case 1:
                                    fop.screwStateL1 = 1;
                                    fop.screwStateR1 = 1;
                                    break;
                                case 2:
                                    fop.screwStateL2 = 1;
                                    fop.screwStateR2 = 1;
                                    break;
                                case 3:
                                    fop.screwStateL3 = 1;
                                    fop.screwStateR3 = 1;
                                    break;
                                case 4:
                                    fop.screwStateL4 = 1;
                                    fop.screwStateR4 = 1;
                                    break;
                                case 5:
                                    fop.screwStateL5 = 1;
                                    fop.screwStateR5 = 1;
                                    break;
                                default:
                                    break;
                            }
                        }
                        //切換回2軸全動,防止人忘了切回
                        fop.StateAxisEnable = 3;
                        mMachineState = 5;
                    }
                }
                else
                {
                    //已無下步驟,完成整台鎖付
                    saveScrewData();
                    mBarcodeMatch = false;
                    fop.BarcodeMatch = 0;
                    mOKCount++;
                    fop.OKCount = mOKCount;
                    showCallBackText(lbOKCount, mOKCount.ToString());
                    mMachineState = 4;
                }
            }
            //清除
            ScrewDriver.ScrewDatas.Clear();
        }
        #endregion
        #region --機種 & 參數--
        #region --機種--
        //***************************define****************************
        csFileCommonLibrary commLib = new csFileCommonLibrary();
        string resourcePath = @"D:\Resource\WorkType\";
        string sourcePath = @"C:\Users\MyUser\Documents\WorkTypeBackup\";
        List<WorkType> mWorkTypes = new List<WorkType>();
        int[] mScrewStepCount = new int[]{ 0, 0, 0, 0, 0 };
        //***************************funtion****************************
        //取得列表
        private void getWorkTypeList()
        {
            //clear
            mWorkTypes.Clear();
            cbWorkType.Items.Clear();
            listBoxWorkType.Items.Clear();
            
            //check dir,if not create
            commLib.checkDirExist(resourcePath);
            //取得資料夾內所有機種,沒有資料就copy
            string[] files = Directory.GetFiles(resourcePath);
            if (files.Length==0)
            {
                files = Directory.GetFiles(sourcePath);
                foreach (var x in files)
                {
                    string filename = Path.GetFileName(x);
                    string targetPath = resourcePath;
                    File.Copy(sourcePath+filename, targetPath+filename);
                }
            }
            //取得TYPE資料
            files = Directory.GetFiles(resourcePath);
            foreach (var x in files)
            {
                string[] data = File.ReadAllLines(x);
                WorkType wt = new WorkType(data[0]);
                for (int i = 1; i < data.Length; i++)
                {
                    wt.addWorkStep(data[i]);
                }
                mWorkTypes.Add(wt);
            }
            //load file to combobox
            foreach (var x in mWorkTypes)
            {
                cbWorkType.Items.Add(x.TypeName);
            }
            if (fop!=null)
            {
                fop.clearWorkTypeList();
                foreach (var x in mWorkTypes)
                {
                    fop.workTypeName = x.TypeName;
                }
            }
        }
        
        //當combox改變時,事件
        private void cbWorkType_SelectedIndexChanged(object sender, EventArgs e)
        {
            mWorkTimer = 0;
            if (cbWorkType.SelectedIndex >= 0)
            {
                mCurrentWorkTypeIndex = cbWorkType.SelectedIndex;
                mCurrentWorkType = mWorkTypes[mCurrentWorkTypeIndex];
                mCurrentCountOfScrewStep = mCurrentWorkType.WorkSteps.Count;
                string s = mWorkTypes[mCurrentWorkTypeIndex].TypeName;
                lbCurrentWorkTypeIndex.Text = mCurrentWorkTypeIndex.ToString();
                //fop change
                fop.workTypeIndex = mCurrentWorkTypeIndex;
                LogOperate.LogText = "切換TYPE:" + mWorkTypes[mCurrentWorkTypeIndex].TypeName;
            }
        }
        //當listbox機種改變時,事件
        private void listBoxWorkType_SelectedIndexChanged(object sender, EventArgs e)
        {
            mWorkTimer = 0;
            pbGetTypeData_Click(null, null);
        }
        //當listbox鎖付步驟改變時,事件
        private void lbScrewSteps_SelectedIndexChanged(object sender, EventArgs e)
        {
            mWorkTimer = 0;
            int stepindex = lbScrewSteps.SelectedIndex;
            if (stepindex>=0)
            {
                //取出列表
                string s = lbScrewSteps.Items[stepindex].ToString();
                string[] ss = s.Split(',');
                int i = 0;
                foreach (var x in cbScrewNo.Items)
                {
                    if (ss[0].Equals(x))
                    {
                        cbScrewNo.SelectedIndex = i;
                        break;
                    }
                    i++;
                }
                i = 0;
                foreach (var x in cbScrewMethod.Items)
                {
                    if (ss[1].Equals(x))
                    {
                        cbScrewMethod.SelectedIndex = i;
                        break;
                    }
                    i++;
                }
                string[] ss2 = ss[2].Split('-');
                if (ss2.Length>1)
                {
                    tbTorqueValue.Text = ss2[0];
                    tbAngleValue.Text = ss2[1];
                }
                else
                {
                    tbTorqueValue.Text = ss2[0];
                    tbAngleValue.Text = "";
                }
            }
        }
        //讀取機種列表
        private void bnReadWorkTypeList_Click(object sender, EventArgs e)
        {
            mWorkTimer = 0;
            listBoxWorkType.Items.Clear();
            getWorkTypeList();
            foreach (var x in mWorkTypes)
            {
                listBoxWorkType.Items.Add(x.MyType);
            }
        }
        //取得機種資料
        private void pbGetTypeData_Click(object sender, EventArgs e)
        {
            mWorkTimer = 0;
            lbScrewSteps.Items.Clear();
            int index = listBoxWorkType.SelectedIndex;
            WorkType wt = mWorkTypes[index];
            tbTypeName.Text = wt.TypeName;
            tbTypeSN.Text = wt.TypeSN;
            foreach (var y in wt.WorkSteps)
            {
                string s = y.ScrewNo + "," + y.ScrewMethod + "," + y.TargetValue;
                lbScrewSteps.Items.Add(s);
            }
        }
        //新增/修改機種
        private void bnNewType_Click(object sender, EventArgs e)
        {
            mWorkTimer = 0;
            string name = tbTypeName.Text;
            string sn = tbTypeSN.Text;
            if (!name.Equals(" ") && !name.Equals(""))
            {
                //新增
                string s = name + "," + sn + Environment.NewLine;
                foreach (var y in lbScrewSteps.Items)
                {
                    s = s + y.ToString() + Environment.NewLine;
                }
                bool fileok = commLib.checkFileName(name);
                if (!fileok)
                {
                    File.WriteAllText(resourcePath + name + ".csv", s);
                    getWorkTypeList();
                    tbTypeName.Text = " ";
                    tbTypeSN.Text = " ";
                    lbScrewSteps.Items.Clear();
                    LogOperate.LogText = fLn.CurrentUser.UserId + "新增機種:" + s;
                }
                else
                {
                    MessageBox.Show(@"檔案名稱不合法,不可包含:,,,>,<,/,\,|,?,*,""等字元");
                }
            }
        }
        //刪除機種
        private void bnDeleteType_Click(object sender, EventArgs e)
        {
            mWorkTimer = 0;
            string name = tbTypeName.Text;
            //檢查是否存在
            foreach (var x in mWorkTypes)
            {
                if (name.Equals(x.TypeName))
                {
                    File.Delete(resourcePath + name + ".csv");
                    tbTypeName.Text = " ";
                    tbTypeSN.Text = " ";
                    lbScrewSteps.Items.Clear();
                    getWorkTypeList();
                    LogOperate.LogText = fLn.CurrentUser.UserId + "刪除機種" + name;
                    break;
                }
            }
        }
        //插入步驟
        private void pbInsertStep_Click(object sender, EventArgs e)
        {
            mWorkTimer = 0;
            int index = lbScrewSteps.SelectedIndex;
            int methodIndex = cbScrewMethod.SelectedIndex;
            string text = "";
            switch (methodIndex)
            {
                case 0:
                    if (index >= 0)
                    {
                        text = cbScrewNo.Items[cbScrewNo.SelectedIndex].ToString() + "," + cbScrewMethod.Items[cbScrewMethod.SelectedIndex].ToString() + "," +
                        tbTorqueValue.Text.Trim();
                        lbScrewSteps.Items.Insert(index, text);
                    }
                    else
                    {
                        text = cbScrewNo.Items[cbScrewNo.SelectedIndex].ToString() + "," + cbScrewMethod.Items[cbScrewMethod.SelectedIndex].ToString() + "," +
                        tbTorqueValue.Text.Trim();
                        lbScrewSteps.Items.Insert(index + 1, text);
                    }
                    break;
                case 1:
                    if (index >= 0)
                    {
                        text = cbScrewNo.Items[cbScrewNo.SelectedIndex].ToString() + "," + cbScrewMethod.Items[cbScrewMethod.SelectedIndex].ToString() + "," +
                        tbTorqueValue.Text.Trim() + "-" + tbAngleValue.Text.Trim();
                        lbScrewSteps.Items.Insert(index, text);
                    }
                    else
                    {
                        text = cbScrewNo.Items[cbScrewNo.SelectedIndex].ToString() + "," + cbScrewMethod.Items[cbScrewMethod.SelectedIndex].ToString() + "," +
                        tbTorqueValue.Text.Trim() + "-" + tbAngleValue.Text.Trim();
                        lbScrewSteps.Items.Insert(index + 1, text);
                    }
                    break;
            }
        }
        //刪除步驟
        private void pbDeleteStep_Click(object sender, EventArgs e)
        {
            mWorkTimer = 0;
            int index = lbScrewSteps.SelectedIndex;
            if (index>=0)
            {
                lbScrewSteps.ClearSelected();
                lbScrewSteps.Items.RemoveAt(index);
            }
        }
        #endregion
        #region --參數--
        //***************************define****************************
        string resourceMachineParameterDir= @"D:\Resource\";
        string resourceMachineParameterFile = @"D:\Resource\config_191003.txt";
        int mCurrentWorkTypeIndex;
        WorkType mCurrentWorkType;
        //***************************funtion****************************
        //取得參數
        private void getConfig()
        {
            //建立目錄
            commLib.checkDirExist(resourceMachineParameterDir);
            //建立檔案
            string initString = "WorkType=0" + Environment.NewLine +
            "OKCount=0" + Environment.NewLine +
            "Timer=60" + Environment.NewLine;
            commLib.checkFileExist(resourceMachineParameterFile, initString);
            //show parameter
            string[] ss = File.ReadAllLines(resourceMachineParameterFile);
            foreach (var x in ss)
            {
                string s2 = x;
                s2.Trim();
                string[] txt = s2.Split('=');
                if (txt[0].Equals("WorkType"))
                {
                    mCurrentWorkTypeIndex = Convert.ToInt32(txt[1]);
                    lbCurrentWorkTypeIndex.Text = mCurrentWorkTypeIndex.ToString();
                }
                if (txt[0].Equals("OKCount"))
                {
                    mOKCount = Convert.ToInt32(txt[1]);
                    lbOKCount.Text = mOKCount.ToString();
                }
                if (txt[0].Equals("Timer"))
                {
                    mSvWorkTimer = Convert.ToInt32(txt[1]);
                    textBoxSvWorkTimer.Text = mSvWorkTimer.ToString();
                }
            }
        }
        //save 參數
        private void saveConfig()
        {
            string s = "WorkType=" + mCurrentWorkTypeIndex.ToString() + Environment.NewLine +
                "OKCount=" + mOKCount.ToString() + Environment.NewLine +
                "Timer=" + mSvWorkTimer.ToString() + Environment.NewLine;
            File.WriteAllText(resourceMachineParameterFile, s);
            LogOperate.LogText = "儲存config檔案:當前工作TYPE"+mCurrentWorkTypeIndex.ToString();
        }
        #endregion
        #endregion
        #region --SBD Driver--
        //************************define********************************
        csSBDDriver ScrewDriver;
        csSBDDriver mDriverL, mDriverR;
        /// <summary>
        /// 當前鎖付步驟
        /// </summary>
        int mScrewStep;
        /// <summary>
        /// 鎖付步驟不連續旗標
        /// </summary>
        bool mStepNotContinum = false;
        /// <summary>
        /// 前次鎖付孔位
        /// </summary>
        int mOldScrewCount;
        //************************fution********************************
        private void initialScrewDriver()
        {
            ScrewDriver = new csSBDDriver("COM2", 9600, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One,2);
            //ScrewDriver.dataReceivedEvent += SacrewDriver_dataReceivedEvent;
            mDriverL = new csSBDDriver();
            mDriverR = new csSBDDriver();
        }
        //單動選項
        private void checkBoxStepMotion_CheckedChanged(object sender, EventArgs e)
        {
            mStepNotContinum = checkBoxStepMotion.Checked;
        }
        
        #endregion
        #region --tcp server--
        //************************define*****************************
        TcpListener server;
        Thread serverThread;
        string mBarcode;
        int flagBarcodeOK;
        delegate void TextBoxAppendCallback(TextBox Ctl, string Str);
        //************************funtion*****************************
        private void serverListion()
        {
            // Buffer for reading data
            Byte[] bytes = new Byte[256];
            String data;
            //Console.Write("Waiting for a connection... ");

            // Perform a blocking call to accept requests.
            // You could also user server.AcceptSocket() here.
            TcpClient client = server.AcceptTcpClient();
            //Console.WriteLine("Connected!");
            data = null;

            // Get a stream object for reading and writing
            NetworkStream stream = client.GetStream();

            int i;

            // Loop to receive all the data sent by the client.
            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                // Translate data bytes to a ASCII string.
                data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                //Console.WriteLine("Received: {0}", data);
                mBarcode = data;
                flagBarcodeOK = 1;
            }
            // Shutdown and end connection
            client.Close();
        }
        private void showTextBoxMessage(TextBox ctl,string str)
        {
            flagBarcodeOK = 0;
            if (ctl.InvokeRequired)
            {
                TextBoxAppendCallback d = new TextBoxAppendCallback(showTextBoxMessage);
                this.Invoke(d, new object[] { ctl, str });
            }
            else
            {
                ctl.Text = str;
            }
        }
        #endregion
        #region --機器流程--
        //*****************************************define*********************************
        /// <summary>
        /// 當前鎖付的孔位
        /// </summary>
        int mScrewCount;
        /// <summary>
        /// 當前鎖付的總步驟數
        /// </summary>
        int mCurrentCountOfScrewStep;
        /// <summary>
        /// 條碼相符旗標
        /// </summary>
        bool mBarcodeMatch;
        /// <summary>
        /// 鎖付流程開始旗標
        /// </summary>
        bool mScrewFlowStartFlag;
        //*****************************************funtion*********************************
        //掃Barcode
        private void MainFlow()
        {
            //取得barcode
            if (flagBarcodeOK==1)
            {
                flagBarcodeOK = 0;
                //檢查條碼是否和機種相符
                bool bCheckType = checkWorkSN(mBarcode);
                if (bCheckType)
                {
                    mBarcodeMatch = true;
                    fop.BarcodeMatch = 1;
                    showTextBoxMessage(tbBarcode, mBarcode);
                    fop.workBarcode = mBarcode;
                    startScrewFlow();
                }
                else
                {
                    mBarcodeMatch = false;
                    mMachineState = 1;
                    showTextBoxMessage(tbBarcode, mBarcode);
                    fop.workBarcode = mBarcode;
                    fop.BarcodeMatch = 2;
                }
            }
        }
        /// <summary>
        /// 檢查機種是否符合
        /// </summary>
        /// <param name="txt">傳入工作barcode</param>
        /// <returns></returns>
        private bool checkWorkSN(string txt)
        {
            mWorkTimer = 0;
            string s = txt;
            if (s.Length>5)
            {
                s = s.Remove(5);
                string modle = mCurrentWorkType.TypeSN;
                if (modle.Equals(s))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        //開始鎖付流程
        private void startScrewFlow()
        {
            //重置fop
            mDriverL.ScrewDatas.Clear();
            mDriverR.ScrewDatas.Clear();
            mScrewStep = 0;
            mScrewCount = mCurrentWorkType.WorkSteps[mScrewStep].ScrewNo;
            string s = mCurrentWorkType.WorkSteps[0].getSBDDriverBarcodeString();
            ScrewDriver.sendJobBarcode(s);
            fop.screwStateL1 = 1;
            fop.screwStateR1 = 1;
            fop.screwStateL2 = 0;
            fop.screwStateR2 = 0;
            fop.screwStateL3 = 0;
            fop.screwStateR3 = 0;
            fop.screwStateL4 = 0;
            fop.screwStateR4 = 0;
            fop.screwStateL5 = 0;
            fop.screwStateR5 = 0;
            fop.screwTorqueL1 = "0";
            fop.screwTorqueR1 = "0";
            fop.screwTorqueL2 = "0";
            fop.screwTorqueR2 = "0";
            fop.screwTorqueL3 = "0";
            fop.screwTorqueR3 = "0";
            fop.screwTorqueL4 = "0";
            fop.screwTorqueR4 = "0";
            fop.screwTorqueL5 = "0";
            fop.screwTorqueR5 = "0";
            fop.screwAngleL1 = "0";
            fop.screwAngleL2 = "0";
            fop.screwAngleL3 = "0";
            fop.screwAngleL4 = "0";
            fop.screwAngleL5 = "0";
            fop.screwAngleR1 = "0";
            fop.screwAngleR2 = "0";
            fop.screwAngleR3 = "0";
            fop.screwAngleR4 = "0";
            fop.screwAngleR5 = "0";
            mScrewFlowStartFlag = true;
        }
        //重置計數器
        private void pbResetCount_Click(object sender, EventArgs e)
        {
            mWorkTimer = 0;
            mOKCount = 0;
            lbOKCount.Text = mOKCount.ToString();
            fop.OKCount = mOKCount;
        }
        //鎖付完畢
        private void finishScrewFlow()
        {
            mWorkTimer = 0;
            mBarcodeMatch = false;
            fop.BarcodeMatch = 0;
            mScrewFlowStartFlag = false;
        }
        //鎖付NG,判斷處理方式,單動RESET=>,PASS=>此,全部RESET=>重頭開始
        #endregion
        #region --save data--
        //********************************************define*****************************************
        string resourceScrewDataDir = @"D:\ScrewData\";
        string resourceScrewDataFileEnd = @".csv";
        string[] fileNameCantIncloudString=new string[] { @">",@"<",@":",@"/",@"\",@"|",@"?",@"*",@""""};
        string textTitle = "螺絲孔位,軸號,鎖付JOB,扭力值,扭力結果,角度值,角度結果,總鎖付結果,鎖付時間,鎖付步驟"+Environment.NewLine;
        //********************************************fution*****************************************
        /// <summary>
        /// 儲存鎖付檔案
        /// </summary>
        private void saveScrewData()
        {
            //建立目錄
            commLib.checkDirExist(resourceScrewDataDir);
            //建立檔案
            bool nameCheckOK = true;
            foreach (var x in fileNameCantIncloudString)
            {
                if (mBarcode == null || mBarcode.Contains(x) || mBarcode == " ")
                {
                    nameCheckOK = false;
                    MessageBox.Show(@"檔案名稱不合法,名稱內含有>,<,:,/,\,|,?,*,""");
                    break;
                }
            }
            //建立文字
            if (nameCheckOK)
            {
                string text = "";
                foreach (var x in mDriverL.ScrewDatas)
                {
                    text = text + x.ScrewCount.ToString() + "," + x.SpindleNumber + "," + x.JobNumber + "," + x.TorqueValue + "," + x.TorqueStatus + "," +
                        x.AngleValue + "," + x.AngleStatus + "," + x.OverrallStatus + "," + x.ScrewDateTime + "," + x.Barcode + Environment.NewLine;
                }
                foreach (var x in mDriverR.ScrewDatas)
                {
                    text = text + x.ScrewCount.ToString() + "," + x.SpindleNumber + "," + x.JobNumber + "," + x.TorqueValue + "," + x.TorqueStatus + "," +
                        x.AngleValue + "," + x.AngleStatus + "," + x.OverrallStatus + "," + x.ScrewDateTime + "," + x.Barcode + Environment.NewLine;
                }
                string filepath = resourceScrewDataDir + mBarcode.Trim() + resourceScrewDataFileEnd;
                //存檔
                File.WriteAllText(filepath, textTitle + text);
            }
        }
        #endregion
        #region --User--
        List<csUser> myUsers;
        string UserFilePath = @"D:\Resource\user.txt";
        private void InitialUser() 
        {
            myUsers = new List<csUser>();
            myUsers = GetUserData(UserFilePath);
        }
        //取得USER資訊
        private List<csUser> GetUserData(string filePath)
        {
            List<csUser> us = new List<csUser>();
            bool b = File.Exists(filePath);
            if (b)
            {
                string[] data = File.ReadAllLines(filePath);
                if (data.Length != 0 || data != null)
                {
                    foreach (var x in data)
                    {
                        string[] ss = x.Split(',');
                        if (ss.Length == 3)
                        {
                            csUser cu = new csUser();
                            cu.UserId = ss[0];
                            cu.UserPassWord = ss[1];
                            cu.UserLevel = Convert.ToInt32(ss[2]);
                            us.Add(cu);
                        }
                    }
                }
                else
                {
                    string st = "risepower,28652299,8" + Environment.NewLine +
                    "HONDA,12345678,8";
                    File.AppendText(st);
                }
            }
            else
            {
                string st = "risepower,28652299,8" + Environment.NewLine +
                    "HONDA,12345678,8";
                File.AppendText(st);
                string[] data = File.ReadAllLines(filePath);
                if (data.Length != 0 || data != null)
                {
                    foreach (var x in data)
                    {
                        string[] ss = x.Split(',');
                        if (ss.Length == 3)
                        {
                            csUser cu = new csUser();
                            cu.UserId = ss[0];
                            cu.UserPassWord = ss[1];
                            cu.UserLevel = Convert.ToInt32(ss[2]);
                            us.Add(cu);
                        }
                    }
                }
            }
            return us;
        }
        //讀取
        private void pbReadUser_Click(object sender, EventArgs e)
        {
            listBoxUser.Items.Clear();
            myUsers = GetUserData(UserFilePath);
            foreach (var x in myUsers)
            {
                listBoxUser.Items.Add(x.UserId);
            }
            fLn.mUsers = myUsers;
        }
        //新增/修改
        private void pbModifyUser_Click(object sender, EventArgs e)
        {
            string id = textBoxUserID.Text.ToUpper();
            id.Trim();
            bool b = id.Equals("");
            if (!b)
            {
                string pw = textBoxUserPW.Text;
                int level = 0;
                try
                {
                    level = Convert.ToInt32(textBoxUserLevel.Text);
                }
                catch (Exception)
                {
                    MessageBox.Show("請輸入介於1~8之間的數值");
                }
                if (level>0 && level<=8)
                {
                    bool datachanged = false;
                    int index = 0;
                    foreach (var x in myUsers)
                    {
                        string s = x.UserId.ToUpper();
                        if (id.Equals(s))
                        {
                            myUsers[index].UserPassWord = pw;
                            myUsers[index].UserLevel = level;
                            datachanged = true;
                            break;
                        }
                        index += 1;
                    }
                    if (!datachanged)
                    {
                        csUser cu = new csUser();
                        cu.UserId = id;
                        cu.UserPassWord = pw;
                        cu.UserLevel = level;
                        myUsers.Add(cu);
                    }
                    string txt = "";
                    foreach (var y in myUsers)
                    {
                        txt = txt + y.UserId + "," + y.UserPassWord + "," + y.UserLevel.ToString() + Environment.NewLine;
                    }
                    File.WriteAllText(UserFilePath, txt);
                    listBoxUser.Items.Clear();
                    pbReadUser_Click(null, null);
                }
                else
                {
                    MessageBox.Show("請輸入介於1~8之間的數值");
                }
            }
        }
        //刪除
        private void pbDeleteUser_Click(object sender, EventArgs e)
        {
            if (listBoxUser.SelectedIndex>=0)
            {
                string id = listBoxUser.Items[listBoxUser.SelectedIndex].ToString();
                int index = 0;
                foreach (var x in myUsers)
                {
                    if (id.Equals(x.UserId))
                    {
                        myUsers.RemoveAt(index);
                        break;
                    }
                    index += 1;
                }
                string txt = "";
                foreach (var x in myUsers)
                {
                    txt = txt + x.UserId + "," + x.UserPassWord + "," + x.UserLevel.ToString() + Environment.NewLine;
                }
                File.WriteAllText(UserFilePath, txt);
                listBoxUser.Items.Clear();
                pbReadUser_Click(null, null);
            }
        }

        #endregion
        //*******************通用funtion**************************
        /// <summary>
        /// 流程中使用的伺服槍起動
        /// </summary>
        private void funtionScrewStart()
        {
            mWorkTimer = 0;
            stateDriverStart = true;
            switch (mSwitchAxis)
            {
                case 1:
                    ioCard.setOutputDeviceState(0, 1, 1);
                    Thread.Sleep(500);
                    break;
                case 2:
                    ioCard.setOutputDeviceState(0, 2, 1);
                    Thread.Sleep(500);
                    break;
                case 3:
                    ioCard.setOutputDeviceState(0, 0, 1);
                    Thread.Sleep(500);
                    break;
                default:
                    break;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
        }
        delegate void showTextCallBack(object ctl, string txt);
        /// <summary>
        /// 不同執行緒控制項處理
        /// </summary>
        /// <param name="ctl">控制項</param>
        /// <param name="txt">傳送的文字</param>
        private void showCallBackText(object ctl, string txt)
        {
            string sType = ctl.GetType().ToString();
            if (sType.Contains("Button"))
            {
                Button button = (Button)ctl;
                if (button.InvokeRequired)
                {
                    showTextCallBack d = new showTextCallBack(showCallBackText);
                    this.Invoke(d, new object[] { button, txt });
                }
                else
                {
                    button.Text = txt;
                }
            }
            if (sType.Contains("Label"))
            {
                Label lebel = (Label)ctl;
                if (lebel.InvokeRequired)
                {
                    showTextCallBack d = new showTextCallBack(showCallBackText);
                    this.Invoke(d, new object[] { lebel, txt });
                }
                else
                {
                    lebel.Text = txt;
                }
            }
            if (sType.Contains("ListBox"))
            {
                ListBox listBox = (ListBox)ctl;
                if (listBox.InvokeRequired)
                {
                    showTextCallBack d = new showTextCallBack(showCallBackText);
                    this.Invoke(d, new object[] { listBox, txt });
                }
                else
                {
                    listBox.Items.Clear();
                    listBox.Items.Add(txt);
                }
            }
        }
        //計時器數值變動
        private void textBoxSvWorkTimer_TextChanged(object sender, EventArgs e)
        {
            mWorkTimer = 0;
            mSvWorkTimer = Convert.ToInt32(textBoxSvWorkTimer.Text);
        }
        //改變孔位狀態
        private void changePoint()
        {
            mWorkTimer = 0;
            mSwitchAxis = fop.StateAxisEnable;
            switch (mScrewCount)
            {
                case 1:
                    switch (mSwitchAxis)
                    {
                        case 1:
                            fop.screwStateL1 = 1;
                            break;
                        case 2:
                            fop.screwStateR1 = 1;
                            break;
                        case 3:
                            fop.screwStateL1 = 1;
                            fop.screwStateR1 = 1;
                            break;
                        default:
                            break;
                    }
                    break;
                case 2:
                    switch (mSwitchAxis)
                    {
                        case 1:
                            fop.screwStateL2 = 1;
                            break;
                        case 2:
                            fop.screwStateR2 = 1;
                            break;
                        case 3:
                            fop.screwStateL2 = 1;
                            fop.screwStateR2 = 1;
                            break;
                        default:
                            break;
                    }
                    break;
                case 3:
                    switch (mSwitchAxis)
                    {
                        case 1:
                            fop.screwStateL3 = 1;
                            break;
                        case 2:
                            fop.screwStateR3 = 1;
                            break;
                        case 3:
                            fop.screwStateL3 = 1;
                            fop.screwStateR3 = 1;
                            break;
                        default:
                            break;
                    }
                    break;
                case 4:
                    switch (mSwitchAxis)
                    {
                        case 1:
                            fop.screwStateL4 = 1;
                            break;
                        case 2:
                            fop.screwStateR4 = 1;
                            break;
                        case 3:
                            fop.screwStateL4 = 1;
                            fop.screwStateR4 = 1;
                            break;
                        default:
                            break;
                    }
                    break;
                case 5:
                    switch (mSwitchAxis)
                    {
                        case 1:
                            fop.screwStateL5 = 1;
                            break;
                        case 2:
                            fop.screwStateR5 = 1;
                            break;
                        case 3:
                            fop.screwStateL5 = 1;
                            fop.screwStateR5 = 1;
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }
        //切換頁面時,show登入畫面
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabControl tb = (TabControl)sender;
            if (tb.SelectedIndex == 0)
            {
                fLn.CurrentUser.UserLevel = 0;
                fLn.CurrentUser.UserId = "";
                fLn.CurrentUser.UserPassWord = "";
            }
            if (tb.SelectedIndex >0 && fLn.CurrentUser.UserLevel==0)
            {
                if (fLn!=null)
                {
                    myUsers = GetUserData(UserFilePath);
                    fLn.mUsers = myUsers;
                    fLn.ShowDialog();
                }
            }
        }

        //條碼輸入
        private void pbNewProdoct_Click(object sender, EventArgs e)
        {
            mWorkTimer = 0;
            string s = tbBarcode.Text;
            s = s.ToUpper();
            mBarcode = s;
            flagBarcodeOK = 1;
            MainFlow();
        }
    }
}
