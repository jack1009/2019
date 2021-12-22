using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NITTO_SD550_DataCollection
{
    class Protocal
    {
        private byte[] _id=new byte[2];
        private byte[] _commandcode = new byte[1];
        private byte[] _vno = new byte[5];
        private byte[] _sumcheck = new byte[2];
        private byte[] _sendbyte=new byte[11];
        private string _stringID;

        public string stringID
        {
            get
            {
                return _stringID;
            }
        }
        public string ID
        {
            get
            {
                string s = "";
                foreach (var item in _id)
                {
                    s = s + item;
                }
                return s;
            }
            set
            {
                _id = System.Text.Encoding.ASCII.GetBytes(value);
                _stringID = value;
            }
        }
        public string CommandCode
        {
            set
            {
                _commandcode =System.Text.Encoding.ASCII.GetBytes(value);
            }
        }
        public string Vno
        {
            set
            {
                _vno = System.Text.Encoding.ASCII.GetBytes(value);
            }
        }

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

        private void CalculateChecksum()
        {
            int sum = 0;
            sum = _id[0] + _id[1] + _commandcode[0] + _vno[0] + _vno[1] + _vno[2] + _vno[3] + _vno[4];
            sum =sum & 0xFF;
            string s = "";
            s = String.Format("{0:X}", sum);
            byte[] b = new byte[2];
            b = Encoding.ASCII.GetBytes(s);
            _sumcheck = b;
            //_sumcheck[0] = 0x45;
            //_sumcheck[1] = 0x45;
        }
    }
}
