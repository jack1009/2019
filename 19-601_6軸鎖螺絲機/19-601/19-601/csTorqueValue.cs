using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace _19_601
{
    class csTorqueValue
    {
        public csTorqueValue()
        {
            TorqueValue = new List<string>();
        }
        //****************************************************************
        string mFilePath = @"D:\ScrewData\";
        public List<string> TorqueValue { get; set; }
        //****************************************************************
        public void saveTorqueData()
        {
            bool b = Directory.Exists(mFilePath);
            string filename = DateTime.Now.ToString("yyyyMMdd") + ".csv";
            string datas = DateTime.Now.ToString("HHmmss") + ",";
            foreach (string x in TorqueValue)
            {
                int index = x.LastIndexOf('D');
                string s = x.Substring(x.Length - index - 2) + "NM,";
                s.Insert(1, ".");
                datas += s;
            }
            datas += Environment.NewLine;
            if (b)
            {
                if (File.Exists(mFilePath+filename))
                {
                    File.AppendAllText(mFilePath + filename, datas);
                }
                else
                {
                    File.AppendAllText(mFilePath + filename, "時間,軸1,軸2,軸3,軸4,軸5,軸6" + Environment.NewLine);
                    Thread.Sleep(100);
                    File.AppendAllText(mFilePath + filename, datas);
                }
            }
            else
            {
                Directory.CreateDirectory(mFilePath);
                Thread.Sleep(100);
                File.AppendAllText(mFilePath + filename, "時間,軸1,軸2,軸3,軸4,軸5,軸6" + Environment.NewLine);
                Thread.Sleep(100);
                File.AppendAllText(mFilePath + filename, datas);
            }
        }
    }
}
