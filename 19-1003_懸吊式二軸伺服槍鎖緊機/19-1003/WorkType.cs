using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _19_1003
{
    public class WorkType
    {
        public WorkType(string _txt)
        {
            WorkSteps = new List<workStep>();
            _typeName = "";
            _typeSN = "";
            MyType = _txt;
        }
        private string _typeName;
        private string _typeSN;
        /// <summary>
        /// 工作字串
        /// </summary>
        public string MyType
        {
            get
            {
                return _typeName + "," + _typeSN;
            }
            set
            {
                string s = value;
                string[] ss = s.Split(',');
                _typeName = ss[0];
                _typeSN = ss[1];
            }
        }

        /// <summary>
        /// 工作名稱
        /// </summary>
        public string TypeName { get { return _typeName; } set { _typeName = value; } }

        /// <summary>
        /// 工作檢查碼
        /// </summary>
        public string TypeSN
        {
            get
            {
                return _typeSN;
            }
            set
            {
                _typeSN = value;
            }
        }

        /// <summary>
        /// 工作步驟
        /// </summary>
        public List<workStep> WorkSteps { get; set; }

        /// <summary>
        /// 新增步驟
        /// </summary>
        /// <param name="stepText">鎖付步驟字串:螺絲孔位碼,鎖付方法,數值</param>
        public void addWorkStep(string stepText)
        {
            string step = stepText.Trim();
            string[] ss = step.Split(',');
            workStep ws = new workStep();
            if (ss.Length==3)
            {
                ws.ScrewNo = Convert.ToInt32(ss[0]);
                ws.ScrewMethod = ss[1];
                ws.TargetValue = ss[2];
                WorkSteps.Add(ws);
                ws = null;
            }
        }

        /// <summary>
        /// 類別,工作步驟
        /// </summary>
        public class workStep 
        {
            /// <summary>
            /// 鎖付孔位號碼
            /// </summary>
            public int ScrewNo { get; set; }
            /// <summary>
            /// 鎖付方法,T:扭力,A:角度,B:鬆脫
            /// </summary>
            public string ScrewMethod { get; set; }
            /// <summary>
            /// 目標值,扭力或角度值
            /// </summary>
            public string TargetValue { get; set; }
            /// <summary>
            /// 設定setp
            /// </summary>
            public string SetStep 
            { 
                set
                {
                    string s = value;
                    string[] ss = s.Split(',');
                    if (ss.Length==3)
                    {
                        ScrewNo = Convert.ToInt32(ss[0]);
                        ScrewMethod = ss[1];
                        TargetValue = ss[2];
                    }
                }
            }
            /// <summary>
            /// 傳給SBD伺服槍改變JOB的條碼字串
            /// </summary>
            public string getSBDDriverBarcodeString()
            {
                string s = ScrewMethod + TargetValue;
                return s;
            }
        }
    }
}
