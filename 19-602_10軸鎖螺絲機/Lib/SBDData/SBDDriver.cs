using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;

namespace SBDDriver
{
    public class csSBDDriver
    {
        #region --Create--
        public csSBDDriver(string _comport, int _baudRate, Parity _parity, int _dataBits, StopBits _stopBits)
        {
            ScrewData = new csSBDScrewData();
            DataReceived = 0;
            sp = new SerialPort(_comport, _baudRate, _parity, _dataBits, _stopBits);
            sp.DataReceived += Sp_DataReceived;
            sp.Close();
            Thread.Sleep(100);
            sp.Open();
        }
        ~csSBDDriver()
        {
            sp.Close();
            sp.Dispose();
            sd = null;
        }
        #endregion
        #region --define--
        //Load
        SerialPort sp;

        public csSBDScrewData ScrewData;
        public delegate void dataReceivedHandler();
        public event dataReceivedHandler dataReceivedEvent;
        #endregion
        #region --Event--
        private void Sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Thread.Sleep(100);
            Byte[] buffer = new Byte[1024];
            Int32 length = (sender as SerialPort).Read(buffer, 0, buffer.Length);
            Array.Resize(ref buffer, length);
            string ss = System.Text.Encoding.Default.GetString(buffer);
            ss = ss.Trim();
            char c = ',';
            string[] substrings = ss.Split(c);
            ScrewData.SpindleNumber = substrings[0];
            ScrewData.JobNumber = substrings[1];
            ScrewData.TorqueValue = substrings[2];
            ScrewData.TorqueStatus = substrings[3];
            ScrewData.AngleValue = substrings[4];
            ScrewData.AngleStatus = substrings[5];
            ScrewData.OverrallStatus = substrings[6];
            ScrewData.ScrewDateTime = substrings[7];
            dataReceivedEvent();
        }
        #endregion
    }
    public class csSBDScrewData
    {
        #region --prop--
        public string SpindleNumber { get; set; }               //軸號碼
        public string JobNumber { get; set; }          //JOB號碼
        public string TorqueValue { get; set; }         //扭力輸出
        public string TorqueStatus { get; set; }         //扭力判定A=OK,L=Low,H=High
        public string AngleValue { get; set; }            //角度輸出
        public string AngleStatus { get; set; }            //角度判定A=OK,L=Low,H=High
        public string OverrallStatus { get; set; }   //總合判定A=OK,R=NOK
        public string ScrewDateTime { get; set; }          //鎖付的時間
        #endregion
    }
}
