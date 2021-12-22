using MachineLog;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace NittoDriver
{
    public class csNittoDriver
    {
        #region --define--
        SerialPort sp1;
        csLog myLog;
        string myComPort = "COM1";
        int myBaudrate = 9600;
        Parity myParity = Parity.None;
        int myDataBits = 8;
        StopBits myStopBits = StopBits.One;
        bool dataReceiveFlag = false;
        const Int32 Startbyte = 0x44;
        const Int32 Endbyte = 0x0D;
        const Int32 Errorbyte = 0x45;
        public string Data { get; set; }
        public bool dataReceiveFinishFlag { get; set; }
        public bool dataReceiveTimeoutFlag { get; set; }
        #endregion

        //************************************************************************************************
        /// <summary>
        /// 建構Nitto Driver
        /// </summary>
        /// <param name="comport">com port名稱</param>
        /// <param name="baudRate">BaudBate</param>
        /// <param name="parity"  >Parity</param>
        /// /// <param name="dataBits" >DataBits</param>
        public csNittoDriver(string _comport,int _baudRate,Parity _parity,int _dataBits,StopBits _stopBits)
        {
            myComPort = _comport;
            myBaudrate = _baudRate;
            myParity = _parity;
            myDataBits = _dataBits;
            myStopBits = _stopBits;
            sp1 = new SerialPort(myComPort, myBaudrate, myParity, myDataBits, myStopBits);
            sp1.DataReceived += Sp1_DataReceived;
            sp1.Close();
            Thread.Sleep(1000);
            sp1.Open();
        }

        private void Sp1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Thread.Sleep(1000);
            byte[] buffer = new byte[1000];
            sp1.Read(buffer, 0, sp1.BytesToRead);
            Array.Resize(ref buffer, 11);
            parse(buffer);
            myLog = new csLog();
            myLog.LogText= System.Text.ASCIIEncoding.ASCII.GetString(buffer);
            dataReceiveFlag = false;
            dataReceiveFinishFlag = true;
            myLog = null;
        }

        ~csNittoDriver()
        {
            if (sp1!=null)
            {
                sp1.Dispose();
            }
            
        }
        //************************************************************************************************
        /// <summary>
        /// Funtion:傳送命令
        /// </summary>
        /// <param name="cp">Protocal</param>
        public void sendToDriver(csProtocal cp)
        {
            dataReceiveFinishFlag = false;
            byte[] bs = cp.SendByte;
            //sp1.DiscardOutBuffer();
            //sp1.DiscardInBuffer();
            sp1.Write(bs, 0, cp.SendByte.Length);
            dataReceiveFlag = true;
        }
        /// <summary>
        /// Funtion:接收com port,已過期
        /// </summary>
        private void DoReceive()
        {
            List<Byte> tempList = new List<Byte>();
            int i = 0;
            while (dataReceiveFlag && i<10000)
            {
                try
                {
                    int receivedValue = sp1.ReadByte();
                    switch (receivedValue)
                    {
                        //case Startbyte:
                        //    tempList.Clear();
                        //    break;
                        case Endbyte:
                            tempList.Add((Byte)receivedValue);
                            parse(tempList);
                            break;
                        case -1:
                            tempList.Clear();
                            break;
                        default:
                            tempList.Add((Byte)receivedValue);
                            break;
                    }
                }
                catch (Exception err)
                {
                    myLog = new csLog();
                    myLog.LogText = err.ToString();
                    
                }
                finally
                {
                    myLog = null;
                }
                i++;
            }
            if (i >= 10000)
            {
                dataReceiveFlag = false;
                dataReceiveTimeoutFlag = true;
            }
        }
        /// <summary>
        /// 解析從com port接來的字元
        /// </summary>
        /// <param name="tempByteArray">輸入的byte陣列</param>
        private void parse(byte[] tempByteArray)
        {
            if (tempByteArray[tempByteArray.Length - 1] == (Byte)Endbyte)
            {
                char[] c = System.Text.Encoding.ASCII.GetChars(tempByteArray);
                string s = "";
                for (int i = 0; i < tempByteArray.Length-3; i++)
                {
                    s += c[i];
                }
                Data = s;
            }
        }
        /// <summary>
        /// 解析從com port接來的字元
        /// </summary>
        /// <param name="tempList">輸入的byte List</param>
        private void parse(List<Byte> tempList)
        {
            if (tempList[tempList.Count - 1] == (Byte)Endbyte)
            {
                tempList.RemoveRange(tempList.Count - 3, 3);
                //List轉成陣列
                char[] c = new char[tempList.Count];
                c = System.Text.Encoding.ASCII.GetChars(tempList.ToArray());
                string s = "";
                foreach (var x in c)
                {
                    s = s + x;
                }
                Data = s;
            }
            dataReceiveFlag = false;
            dataReceiveFinishFlag = true;
        }
    }
    //************************************************************************************************
    /// <summary>
    /// Protocal
    /// </summary>
    public class csProtocal
    {
        #region --define--
        private byte[] _id = new byte[2];
        private byte[] _commandcode = new byte[1];
        private byte[] _vno = new byte[5];
        private byte[] _sumcheck = new byte[2];
        private byte[] _sendbyte = new byte[11];

        /// <summary>
        /// 站號
        /// </summary>
        public string ID
        {
            get
            {
                string s = System.Text.Encoding.ASCII.GetString(_id);
                return s;
            }
            set
            {
                string s = value;
                s = s.PadLeft(2, '0');
                _id = System.Text.Encoding.ASCII.GetBytes(s);
            }
        }
        /// <summary>
        /// get 通訊命令:讀取:R
        /// </summary>
        public string CommandCode { set { _commandcode = System.Text.Encoding.ASCII.GetBytes(value); } }
        /// <summary>
        /// set 讀取Driver位址
        /// </summary>
        public string Vno { set { _vno = System.Text.Encoding.ASCII.GetBytes(value); } }
        /// <summary>
        /// get 取得傳送至driver的byte組
        /// </summary>
        public byte[] SendByte
        {
            get
            {
                CalculateChecksum();
                _sendbyte[0] = _id[0];
                _sendbyte[1] = _id[1];
                _sendbyte[2] = _commandcode[0];
                _sendbyte[3] = _vno[0];
                _sendbyte[4] = _vno[1];
                _sendbyte[5] = _vno[2];
                _sendbyte[6] = _vno[3];
                _sendbyte[7] = _vno[4];
                _sendbyte[8] = _sumcheck[0];
                _sendbyte[9] = _sumcheck[1];
                _sendbyte[10] = 0x0D;
                return _sendbyte;
            }
        }
        #endregion

        /// <summary>
        /// Check sum
        /// </summary>
        private void CalculateChecksum()
        {
            int sum = 0;
            sum = _id[0] + _id[1] + _commandcode[0] + _vno[0] + _vno[1] + _vno[2] + _vno[3] + _vno[4];
            sum = sum & 0xFF;
            string s = "";
            s = String.Format("{0:X}", sum);
            byte[] b = new byte[2];
            b = Encoding.ASCII.GetBytes(s);
            _sumcheck = b;
        }
    }
}
