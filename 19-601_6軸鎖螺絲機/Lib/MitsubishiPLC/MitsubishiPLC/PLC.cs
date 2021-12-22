using HslCommunication.Profinet.Melsec;
using HslCommunication;
using System;
using System.Windows.Forms;

namespace MitsubishiPLC
{
    public class csFX5
    {
        //MelsecMcNet plcRead;
        MelsecMcNet plcWrite;
        /// <summary>
        /// 建構子,內部自動開通二個ports,一個讀取一個寫入。
        /// </summary>
        /// <param name="_plcIP">PLC IP</param>
        /// <param name="_plcPort">PLC Port</param>
        public csFX5(string _plcIP, int _plcPort)
        {
            string m_Ip = _plcIP;
            int m_port = _plcPort;
            //plcRead = new MelsecMcNet(m_Ip, m_port);
            //plcRead.ConnectTimeOut = 3000;
            plcWrite = new MelsecMcNet(m_Ip, m_port+1);
            plcWrite.ConnectTimeOut = 3000;
        }
        /// <summary>
        /// 寫入bool
        /// </summary>
        public void writeToPLC(string _device, bool _data)
        {
                plcWrite.Write(_device, _data);
        }
        /// <summary>
        /// 寫入32位數值
        /// </summary>
        /// <param name="dDevice">寫入位置</param>
        /// <param name="d32Data">寫入32位數值</param>
        public void writeToPLC(string _device, Int32 _data)
        {
                plcWrite.Write(_device, _data);
        }
        /// <summary>
        /// 寫入16位數值
        /// </summary>
        /// <param name="dDevice"></param>
        /// <param name="d16Data"></param>
        public void writeToPLC(string _device, short _data)
        {
                plcWrite.Write(_device, _data);
        }
    }
}
