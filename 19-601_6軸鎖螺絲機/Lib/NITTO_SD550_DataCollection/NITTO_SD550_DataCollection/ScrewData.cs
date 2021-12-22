using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NITTO_SD550_DataCollection
{
    class ScrewData
    {
        private int _ID, _ScrewCount, _FinalTorque, _FinalAngle, _TotalScrewTime;
        private string _AxisNo, _ProductName, _ScrewDate, _ScrewTime;
        private int _TorqueJudgement, _AngleJudgement;
        private int _stop;

        public int ID { get { return _ID; } set { _ID = value; } }//鎖付計數ID
        public int ScrewCount { get { return _ScrewCount; } set { _ScrewCount = value; } }//產品的螺絲計數
        public string AxisNo { get { return _AxisNo; } set { _AxisNo = value; } }//軸號
        public string ProductName { get { return _ProductName; } set { _ProductName = value; } }//產品名稱
        public string ScrewDate { get { return _ScrewDate; } set { _ScrewDate = value; } }//日期
        public string ScrewTime { get { return _ScrewTime; } set { _ScrewTime = value; } }//時間
        public int FinalTorque { get { return _FinalTorque; } set { _FinalTorque = value; } }//最終扭力
        public int TorqueJudgement { get { return _TorqueJudgement; } set { _TorqueJudgement = value; } }//扭力判定
        public int FinalAngle { get { return _FinalAngle; } set { _FinalAngle = value; } }//最終角度
        public int AngleJudgement { get { return _AngleJudgement; } set { _AngleJudgement = value; } }//角度判定
        public int TotalScrewTime { get { return _TotalScrewTime; } set { _TotalScrewTime = value; } }//鎖付時間
        public int StopReason { get { return _stop; } set { _stop = value; } }//停止原因
    }
}
