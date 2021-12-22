using HslCommunication.Profinet.Melsec;
using System;
using System.Windows.Forms;

namespace _19_601
{
    public class csWritePLC
    {
        string m_Ip;
        int m_port;
        MelsecMcNet plc;
        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="plcIP">PLC IP</param>
        /// <param name="port">PLC Port</param>
        public csWritePLC(string plcIP, int port)
        {
            try
            {
                m_Ip = plcIP;
                m_port = port;
                plc = new MelsecMcNet(m_Ip, m_port);
                plc.ConnectTimeOut = 3000;
                plc.ConnectServer();
            }
            catch (Exception ex)
            {
                MessageBox.Show("PLC連結異常!!"+ex.ToString());
            }
            
        }
        /// <summary>
        /// 寫入bool
        /// </summary>
        public void writeToPLC(string _device,bool _data)
        {
            plc.Write(_device, _data);
        }
        /// <summary>
        /// 寫入32位數值
        /// </summary>
        /// <param name="dDevice">寫入位置</param>
        /// <param name="d32Data">寫入32位數值</param>
        public void writeD32ToPLC(string dDevice, Int32 d32Data)
        {
            plc.Write(dDevice, d32Data);
        }
        public void writeD16ToPLC(string dDevice, short d16Data)
        {
            plc.Write(dDevice, d16Data);
        }
    }
}
