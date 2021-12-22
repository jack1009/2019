using System;
using System.IO;

namespace MachineLog
{
    public class csLog
    {
        public csLog()
        {
            _fileName = "Log_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            _errFileName = "ErrorLog" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
        }
        public csLog(string _logName)
        {
            LogName = _logName;
            _fileName = "Log_" + LogName + "_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            _errFileName = "ErrorLog" + LogName + "_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
        }
        #region --define--
        string LogName = "";
        private string _filePath = @"D:\Log\", _errFilePath = @"D:\ErrorLog\";
        private string _fileName = "", _errFileName = "";
        private string _logText, _errorLogText, _alarmhistory;
        #endregion
        /// <summary>
        /// 操作記錄
        /// </summary>
        public string LogText
        {
            get { return _logText; }
            set
            {
                string ss =DateTime.Now.ToString("HH:mm:ss=>")+ value;
                CheckDir(_filePath);
                string name= _filePath + _fileName;
                SaveLog(name, ss);
                _logText =_logText+ss+ Environment.NewLine;
            }
        }
        
        /// <summary>
        /// 當前警報
        /// </summary>
        public string ErrorLogText
        {
            get {return _errorLogText; }
            set
            {
                string ss = DateTime.Now.ToString("HH:mm:ss=>") + value;
                CheckDir(_errFilePath);
                string name = _errFilePath + _errFileName;
                SaveLog(name, ss);
                _errorLogText = ss + Environment.NewLine + _errorLogText;
                _alarmhistory = ss + Environment.NewLine + _alarmhistory;
            }
        }

        /// <summary>
        /// 歷史警報
        /// </summary>
        public string AlarmHistory
        {
            get { return _alarmhistory; }
            set { _alarmhistory = value; }
        }

        /// <summary>
        /// 清除異常訊息
        /// </summary>
        public void ClearErrorLogText()
        {
            _errorLogText = "";
        }

        //檢查目錄是否存在,不存在就建立
        private void CheckDir(string ipath)
        {
            if (!Directory.Exists(ipath))
            {
                Directory.CreateDirectory(ipath);
            }
        }
        //存檔
        private void SaveLog(string ifullname,string text)
        {
            string ss = ifullname;
            StreamWriter sw = new StreamWriter(ss,true,System.Text.Encoding.UTF8);
            sw.WriteLine(text);
            sw.Dispose();
        }
    }
}
