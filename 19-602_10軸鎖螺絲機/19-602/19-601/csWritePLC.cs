using HslCommunication.Profinet.Melsec;
using System;

namespace _19_602
{
    public class csWritePLC
    {
        string m_Ip;
        int m_port;
        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="plcIP">PLC IP</param>
        /// <param name="port">PLC Port</param>
        /// <param name="deviceAdd">寫入的PLC位置</param>
        /// <param name="deviceState">寫入的值</param>
        public csWritePLC(string plcIP, int port)
        {
            m_Ip = plcIP;
            m_port = port;
        }
        /// <summary>
        /// 寫入bool
        /// </summary>
        public void writeToPLC(string _device, bool _data)
        {
            using (MelsecMcNet plc = new MelsecMcNet(m_Ip, m_port))
            {
                plc.Write(_device, _data);
            }
        }
        /// <summary>
        /// 寫入32位數值
        /// </summary>
        /// <param name="dDevice">寫入位置</param>
        /// <param name="d32Data">寫入32位數值</param>
        public void writeD32ToPLC(string dDevice, Int32 d32Data)
        {
            using (MelsecMcNet plc = new MelsecMcNet(m_Ip, m_port))
            {
                plc.Write(dDevice, d32Data);
            }
        }
        public void writeD16ToPLC(string dDevice, short d16Data)
        {
            using (MelsecMcNet plc = new MelsecMcNet(m_Ip, m_port))
            {
                plc.Write(dDevice, d16Data);
            }
        }
    }
}
