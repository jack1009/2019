using System;
using System.IO;
using System.Windows.Forms;
using ClosedXML.Excel;
using System.Collections.Generic;

namespace NITTO_SD550_DataCollection
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //定義變數****************************************************************************
        private int MachineID,ScrewCount;
        //private Int32 totalLength = 0;
        private string fileName, InitFileName= @"F:\Nitto\NittoInit.xlsx";
        delegate void Display(Byte[] buffer);
        const Int32 Startbyte = 0x20;
        const Int32 Endbyte = 0x0D;
        const Int32 Errorbyte = 0x45;
        bool dataReceiveFlag = false;
        bool dataReceiveTimeoutFlag = false;
        bool dataReceiveErrorFlag = false;
        string tempScrewString = "";
        //string[] sdScrewString = new string[5];
        //**************************************************************************************

        private void Form1_Load(object sender, EventArgs e)
        {
            //取得初始化檔案資料************************************************************
            try
            {
                XLWorkbook initWB = new XLWorkbook(InitFileName);
                var initWS = initWB.Worksheet(1);
                tbFilePath.Text = initWS.Cell("A2").Value.ToString();
                tbNumAxis.Text = initWS.Cell("B2").Value.ToString();
                tbStartAdd.Text = initWS.Cell("C2").Value.ToString();
                tbNumScrew.Text = initWS.Cell("D2").Value.ToString();
                tbComPort.Text = initWS.Cell("E2").Value.ToString();
                initWS.Dispose();
                initWB.Dispose();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }
        }

        private void CheckFileExist()
        {
            //檔案處理*****************************************************************
            bool result_checkfileexist;
            string fileDate = DateTime.Now.ToString("yyyyMMdd");
            fileName = tbFilePath.Text + fileDate + @".xlsx";
            //檢查檔案是否存在
            result_checkfileexist = File.Exists(fileName);
            //檔案不存在,建立新的檔案
            if (!result_checkfileexist)
            {
                XLWorkbook wb = new XLWorkbook();
                wb.AddWorksheet("sheet1");
                var ws = wb.Worksheet("sheet1");

                ws.Cells("A1").Value = "ID";
                ws.Cells("B1").Value = "鎖付次數";
                ws.Cells("C1").Value = "軸號";
                ws.Cells("D1").Value = "產品名稱";
                ws.Cells("E1").Value = "日期";
                ws.Cells("F1").Value = "時間";
                ws.Cells("G1").Value = "鎖緊扭力";
                ws.Cells("H1").Value = "扭力判定";
                //ws.Cells("I1").Value = "鎖緊角度";
                //ws.Cells("J1").Value = "角度判定";
                //ws.Cells("K1").Value = "鎖緊時間";
                //ws.Cells("L1").Value = "停止原因";
                ws.Columns().AdjustToContents();
                MachineID = 1;
                ScrewCount = 1;
                tbMachineID.Text = MachineID.ToString();
                tbCurrentCount.Text = ScrewCount.ToString();
                wb.SaveAs(fileName);
                ws.Dispose();
                wb.Dispose();
            }
            //檔案存在，讀出後取得ID
            else
            {
                XLWorkbook wb = new XLWorkbook(fileName);
                var ws = wb.Worksheet(1);
                if (ws.Cell("A2").Value.IsNumber())
                {
                    MachineID = Convert.ToInt16(ws.Cell("A2").Value);
                    ScrewCount = Convert.ToInt16(ws.Cell("B2").Value);
                    ScrewCount++;
                    if (ScrewCount > Convert.ToInt16(tbNumScrew.Text))
                    {
                        MachineID++;
                        ScrewCount = 1;
                    }
                }
                else
                {
                    MachineID = 1;
                    ScrewCount = 1;
                }
                tbMachineID.Text = MachineID.ToString();
                tbCurrentCount.Text = ScrewCount.ToString();
                ws.Dispose();
                wb.Dispose();
            }
        }

        private void pbSavePara_Click(object sender, EventArgs e)
        {
            //儲存初始化檔案資料
            try
            {
                XLWorkbook initWB = new XLWorkbook(InitFileName);
                var initWS = initWB.Worksheet(1);
                initWS.Cell("A2").Value = tbFilePath.Text;
                initWS.Cell("B2").Value = tbNumAxis.Text;
                initWS.Cell("C2").Value = tbStartAdd.Text;
                initWS.Cell("D2").Value = tbNumScrew.Text;
                initWS.Cell("E2").Value = tbComPort.Text;
                initWS.Columns().AdjustToContents();
                initWB.Save();
                initWS.Dispose();
                initWB.Dispose();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }
        }

        private void pb_test1_Click(object sender, EventArgs e)
        {
            CheckFileExist();
            ConnectToNitto();
        }

        //鎖付完成(鎖付完成最好是用IO)

        //***************************************************************************
        private void ConnectToNitto()
        {
            //建立資料物件***********************************************************
            int AxisNum = Convert.ToInt16(tbNumAxis.Text);
            int StartAddress = Convert.ToInt16(tbStartAdd.Text);
            string[] sdScrewString = new string[AxisNum*5];
            ScrewData[] sd = new ScrewData[AxisNum];
            for (int i = 0; i < AxisNum; i++)
            {
                sd[i] = new ScrewData();
                sd[i].ID = MachineID;
                sd[i].ScrewCount = ScrewCount;
                sd[i].ProductName = tbBarcode.Text;
                sd[i].ScrewDate = DateTime.Now.ToShortDateString();
                sd[i].ScrewTime = DateTime.Now.Hour.ToString() +":"+ DateTime.Now.Minute.ToString() +":"+ DateTime.Now.Second.ToString();
            }
            //***************************************************************************
            //建立傳送字串***********************************************************
            //Protocal[] sdP = new Protocal[Convert.ToInt16(AxisNum * 5)];
            //for (int i = 0; i < AxisNum; i++)
            //{
            //    string _startaddress = Convert.ToString(StartAddress + i);
            //    //完成torque D0+4byte
            //    sdP[i * 6 + 0] = new Protocal();
            //    sdP[i * 6 + 0].ID = _startaddress;
            //    sdP[i * 6 + 0].CommandCode = "R";
            //    sdP[i * 6 + 0].Vno = "R0010";
            //    //角度 D1+8byte
            //    sdP[i * 6 + 1] = new Protocal();
            //    sdP[i * 6 + 1].ID = _startaddress;
            //    sdP[i * 6 + 1].CommandCode = "R";
            //    sdP[i * 6 + 1].Vno = "R0020";
            //    //torque判定
            //    sdP[i * 6 + 2] = new Protocal();
            //    sdP[i * 6 + 2].ID = _startaddress;
            //    sdP[i * 6 + 2].CommandCode = "R";
            //    sdP[i * 6 + 2].Vno = "R0030";
            //    //鎖付時間
            //    sdP[i * 6 + 3] = new Protocal();
            //    sdP[i * 6 + 3].ID = _startaddress;
            //    sdP[i * 6 + 3].CommandCode = "R";
            //    sdP[i * 6 + 3].Vno = "R0053";
            //    //停止原因
            //    sdP[i * 6 + 4] = new Protocal();
            //    sdP[i * 6 + 4].ID = _startaddress;
            //    sdP[i * 6 + 4].CommandCode = "R";
            //    sdP[i * 6 + 4].Vno = "R0060";
            //    //角度判定
            //    //sdP[i * 6 + 5] = new Protocal();
            //    //sdP[i * 6 + 6].ID = _startaddress;
            //    //sdP[i * 6 + 6].CommandCode = "R";
            //    //sdP[i * 6 + 6].Vno = "R0031";
            //}
            //****************************************************************************
            //開始傳送***********************************************************
            //for (int j = 0; j < AxisNum * 5; j++)
            //{
                //取得傳送byte陣列
                //char[] c = new char[11];
                //c = System.Text.Encoding.ASCII.GetChars(sdP[j].SendByte);
                ////顯示
                //string s = "";
                //foreach (var vv in c)
                //{
                //    s = s + vv;
                //}
                //label8.Text = s.ToString();
                ////開啟com port
                //serialPort1.PortName = tbComPort.Text;
                //serialPort1.Open();
                //serialPort1.Write(sdP[j].SendByte, 0, sdP[j].SendByte.Length);
                dataReceiveFlag = true;
                label7.Text = dataReceiveFlag.ToString();
                DoReceive();
            if (dataReceiveTimeoutFlag || dataReceiveErrorFlag)
            {
                MessageBox.Show("通訊異常，請檢查RS485線路，或網路設定");
                dataReceiveTimeoutFlag = false;
            }
            else
            {
                sdScrewString[j] = tempScrewString;
            }
            //關閉com port
            serialPort1.Close();
            for (int k = 0; k < AxisNum; k++)
            {
                sd[k].AxisNo = sdP[k].stringID;
                sd[k].FinalTorque = Convert.ToInt32(sdScrewString[k * 5 + 0]);
                sd[k].FinalAngle = Convert.ToInt32(sdScrewString[k * 5 + 1]);
                sd[k].TorqueJudgement = Convert.ToInt32(sdScrewString[k * 5 + 2]);
                sd[k].TotalScrewTime = Convert.ToInt32(sdScrewString[k * 5 + 3]);
                sd[k].StopReason = Convert.ToInt32(sdScrewString[k * 5 + 4]);
                //sd[k].AngleJudgement = Convert.ToInt32(sdScrewString[k+5]);
            }
          //}

            //****************************************************************************
            //存檔案
            SaveScrewData(sd, AxisNum);
            //鎖付次數計數
            ScrewCount++;
            if (ScrewCount>Convert.ToInt16(tbNumScrew.Text))
            {
                MachineID++;
                ScrewCount = 1;
            }
            tbMachineID.Text = MachineID.ToString();
            tbCurrentCount.Text = ScrewCount.ToString();
        }

        private void DoReceive()
        {
            List<Byte> tempList = new List<Byte>();
            int i = 0;
            while (dataReceiveFlag)
            {
                Int32 receivedValue = serialPort1.ReadByte();
                switch (receivedValue)
                {
                    //case Startbyte:
                    //    tempList.Clear();
                    //    tempList.Add((Byte)receivedValue);
                    //    break;
                    case Endbyte:
                        tempList.Add((Byte)receivedValue);
                        parse(tempList);
                        break;
                    case Errorbyte:
                        tempList.Clear();
                        tempList.Add((Byte)receivedValue);
                        break;
                    case -1:
                        break;
                    default:
                        tempList.Add((Byte)receivedValue);
                        break;
                }
                i++;
                if (i>1000000)
                {
                    dataReceiveFlag = false;
                    dataReceiveTimeoutFlag = true;
                }
            }
        }

        private void parse(List<Byte> tempList)
        {
            if (tempList[tempList.Count - 1] == (Byte)Endbyte)
            {
                tempList.RemoveRange(tempList.Count - 3, 3);
                //List轉成陣列
                char[] c = new char[tempList.Count];
                c = System.Text.Encoding.ASCII.GetChars(tempList.ToArray());
                string s = "";
                foreach (var vv in c)
                {
                    s = s + vv;
                }
                label9.Text = s.ToString();
                //判斷是否正常回應
                if (c[0] == 'D')
                {
                    char[] tempchar = new char[c.Length-2];
                    for (int i = 0; i < tempchar.Length; i++)
                    {
                        tempchar[i] = c[i + 2];
                    }
                    foreach (var uu in tempchar)
                    {
                        tempScrewString = tempScrewString + Convert.ToString(uu);
                    }
                }
                else
                {
                    dataReceiveErrorFlag = true;
                }
            }
            dataReceiveFlag = false;
            label7.Text = dataReceiveFlag.ToString();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            serialPort1.Close();
        }

        private void pbResetCount_Click(object sender, EventArgs e)
        {
            MachineID++;
            ScrewCount = 1;
            tbMachineID.Text = MachineID.ToString();
            tbCurrentCount.Text = ScrewCount.ToString();
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            ConnectToNitto();
        }

        //資料取得後，存成xlsx，EXCEL檔
        private void SaveScrewData(ScrewData[] _sd,int _AxisNum)
        {
            XLWorkbook wb = new XLWorkbook(fileName);
            var ws = wb.Worksheet(1);
            for (int i = 0; i < _AxisNum; i++)
            {
                ws.Row(2).InsertRowsAbove(1);
                ws.Cell(2, 1).Value = _sd[i].ID;
                ws.Cell(2, 2).Value = _sd[i].ScrewCount;
                ws.Cell(2, 3).Value = _sd[i].AxisNo;
                ws.Cell(2, 4).Value = _sd[i].ProductName;
                ws.Cell(2, 5).Value = _sd[i].ScrewDate;
                ws.Cell(2, 6).Value = _sd[i].ScrewTime;
                ws.Cell(2, 7).Value = _sd[i].FinalTorque;
                ws.Cell(2, 8).Value = _sd[i].TorqueJudgement;
                ws.Cell(2, 9).Value = _sd[i].FinalAngle;
                ws.Cell(2, 10).Value = _sd[i].AngleJudgement;
                ws.Cell(2, 11).Value = _sd[i].TotalScrewTime;
                ws.Cell(2, 12).Value = _sd[i].StopReason;
            }
            wb.Save();
            ws.Dispose();
            wb.Dispose();
        }


        private void pbTestSumCheck_Click(object sender, EventArgs e)
        {
            Protocal p = new Protocal();
            p.ID = "10";
            p.CommandCode = "R";
            p.Vno = "R0010";
            byte[] b = new byte[11];
            b = p.SendByte;
            label9.Text = b[0].ToString() + "+" + b[1].ToString() + "+" + b[2].ToString() + "+" + b[3].ToString()
                + "+" + b[4].ToString() + "+" + b[5].ToString() + "+" + b[6].ToString() + "+" + b[7].ToString()
                + "+" + b[8].ToString() + "+" + b[9].ToString() + "+" + b[10].ToString();
        }
    }
}