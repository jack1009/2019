using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace _19_1003
{
    public partial class FormOP : Form
    {
        public FormOP()
        {
            InitializeComponent();
        }
        private void FormOP_Load(object sender, EventArgs e)
        {
            screwStateL1 = 0;
            screwStateL2 = 0;
            screwStateL3 = 0;
            screwStateL4 = 0;
            screwStateL5 = 0;
            screwStateR1 = 0;
            screwStateR2 = 0;
            screwStateR3 = 0;
            screwStateR4 = 0;
            screwStateR5 = 0;
            BarcodeMatch = 0;
            _enableAxis1 = 1;
            _enableAxis2 = 1;
        }
        #region --Date Time--
        //*****************define*******************
        public string myDateTime 
        {
            set
            {
                string s = value;
                lbDateTime.Text = s;
            }
        }
        //work timer
        public int WorkTimerValue 
        {
            set 
            {
                int i = value;
                showCallBackText(labelWorkTimerValue, i.ToString());
            }
        }
        public bool WorkTimerState
        {
            set
            {
                bool b = value;
                if (b)
                {
                    labelWorkTimer.BackColor = Color.Red;
                }
                else
                {
                    labelWorkTimer.BackColor = Color.Lime;
                }
            }
        }
        #endregion
        #region --IO--
        //*******************define******************
        private bool _rySaftyModule;
        public bool stateEStop { 
            set 
            {
                bool b = value;
                if (b)
                {
                    labelEStop.BackColor = Color.Red;
                }
                else
                {
                    labelEStop.BackColor = Color.Lime;
                }
            }
        }
        public bool rySaftyModule
        {
            get { return _rySaftyModule; }
            set
            {
                _rySaftyModule = value;
                if (_rySaftyModule)
                {
                    lbSaftyModuleState.Text = "安全繼電器OK";
                    lbSaftyModuleState.BackColor = Color.Lime;
                }
                else
                {
                    lbSaftyModuleState.Text = "安全繼電器NOK";
                    lbSaftyModuleState.BackColor = Color.Red;
                }
            }
        }
        #endregion
        #region --鎖付狀態--
        //******************************define**************************
        delegate void showTextCallBack(object ctl, string txt);
        int _enableAxis1, _enableAxis2;
        /// <summary>
        /// 軸使用狀態 1:Axis1,2:Axis2,3:ALL
        /// </summary>
        public int StateAxisEnable 
        {
            get
            {
                int i=0;
                i = _enableAxis2 << 1;
                i = i | _enableAxis1;
                return i;
            }
            set
            {
                int i = value;
                switch (i)
                {
                    case 1:
                        _enableAxis1 = 1;
                        _enableAxis2 = 0;
                        this.pbAxis1Enable.Text = "軸1使用";
                        this.pbAxis1Enable.BackColor = Color.Lime;
                        this.pbAxis2Enable.Text = "軸2停用";
                        this.pbAxis2Enable.BackColor = Color.Red;
                        break;
                    case 2:
                        _enableAxis1 = 0;
                        _enableAxis2 = 1;
                        this.pbAxis1Enable.Text = "軸1停用";
                        this.pbAxis1Enable.BackColor = Color.Red;
                        this.pbAxis2Enable.Text = "軸2使用";
                        this.pbAxis2Enable.BackColor = Color.Lime;
                        break;
                    case 3:
                        _enableAxis1 = 1;
                        _enableAxis2 = 1;
                        showCallBackText(pbAxis1Enable, "軸1使用");
                        this.pbAxis1Enable.BackColor = Color.Lime;
                        showCallBackText(pbAxis2Enable, "軸2使用");
                        this.pbAxis2Enable.BackColor = Color.Lime;
                        break;
                    default:
                        break;
                }
            }
        }
        /// <summary>
        /// 0:無狀態,1:Ready,2:OK,3:NG
        /// </summary>
        public int screwStateL1
        {
            get
            {
                return screwStateL1;
            }
            set
            {
                int i = value;
                switch (i)
                {
                    case 0://無狀態
                        pictureBoxAxis1L.Image = _19_1003.Properties.Resources.Gray_r;
                        break;
                    case 1://Ready
                        pictureBoxAxis1L.Image = _19_1003.Properties.Resources.Blue_r;
                        break;
                    case 2://OK
                        pictureBoxAxis1L.Image = _19_1003.Properties.Resources.Green_r;
                        break;
                    case 3://NG
                        pictureBoxAxis1L.Image = _19_1003.Properties.Resources.Red_r;
                        break;
                }
            }
        }
        /// <summary>
        /// 0:無狀態,1:Ready,2:OK,3:NG
        /// </summary>
        public int screwStateL2
        {
            get
            {
                return screwStateL2;
            }
            set
            {
                int i = value;
                switch (i)
                {
                    case 0://無狀態
                        pictureBoxAxis2L.Image = _19_1003.Properties.Resources.Gray_r;
                        break;
                    case 1://Ready
                        pictureBoxAxis2L.Image = _19_1003.Properties.Resources.Blue_r;
                        break;
                    case 2://OK
                        pictureBoxAxis2L.Image = _19_1003.Properties.Resources.Green_r;
                        break;
                    case 3://NG
                        pictureBoxAxis2L.Image = _19_1003.Properties.Resources.Red_r;
                        break;
                }
            }
        }
        /// <summary>
        /// 0:無狀態,1:Ready,2:OK,3:NG
        /// </summary>
        public int screwStateL3
        {
            get
            {
                return screwStateL3;
            }
            set
            {
                int i = value;
                switch (i)
                {
                    case 0://無狀態
                        pictureBoxAxis3L.Image = _19_1003.Properties.Resources.Gray_r;
                        break;
                    case 1://Ready
                        pictureBoxAxis3L.Image = _19_1003.Properties.Resources.Blue_r;
                        break;
                    case 2://OK
                        pictureBoxAxis3L.Image = _19_1003.Properties.Resources.Green_r;
                        break;
                    case 3://NG
                        pictureBoxAxis3L.Image = _19_1003.Properties.Resources.Red_r;
                        break;
                }
            }
        }
        /// <summary>
        /// 0:無狀態,1:Ready,2:OK,3:NG
        /// </summary>
        public int screwStateL4
        {
            get
            {
                return screwStateL4;
            }
            set
            {
                int i = value;
                switch (i)
                {
                    case 0://無狀態
                        pictureBoxAxis4L.Image = _19_1003.Properties.Resources.Gray_r;
                        break;
                    case 1://Ready
                        pictureBoxAxis4L.Image = _19_1003.Properties.Resources.Blue_r;
                        break;
                    case 2://OK
                        pictureBoxAxis4L.Image = _19_1003.Properties.Resources.Green_r;
                        break;
                    case 3://NG
                        pictureBoxAxis4L.Image = _19_1003.Properties.Resources.Red_r;
                        break;
                }
            }
        }
        /// <summary>
        /// 0:無狀態,1:Ready,2:OK,3:NG
        /// </summary>
        public int screwStateL5
        {
            get
            {
                return screwStateL5;
            }
            set
            {
                int i = value;
                switch (i)
                {
                    case 0://無狀態
                        pictureBoxAxis5L.Image = _19_1003.Properties.Resources.Gray_r;
                        break;
                    case 1://Ready
                        pictureBoxAxis5L.Image = _19_1003.Properties.Resources.Blue_r;
                        break;
                    case 2://OK
                        pictureBoxAxis5L.Image = _19_1003.Properties.Resources.Green_r;
                        break;
                    case 3://NG
                        pictureBoxAxis5L.Image = _19_1003.Properties.Resources.Red_r;
                        break;
                }
            }
        }
        /// <summary>
        /// 0:無狀態,1:Ready,2:OK,3:NG
        /// </summary>
        public int screwStateR1
        {
            get
            {
                return screwStateR1;
            }
            set
            {
                int i = value;
                switch (i)
                {
                    case 0://無狀態
                        pictureBoxAxis1R.Image = _19_1003.Properties.Resources.Gray_r;
                        break;
                    case 1://Ready
                        pictureBoxAxis1R.Image = _19_1003.Properties.Resources.Blue_r;
                        break;
                    case 2://OK
                        pictureBoxAxis1R.Image = _19_1003.Properties.Resources.Green_r;
                        break;
                    case 3://NG
                        pictureBoxAxis1R.Image = _19_1003.Properties.Resources.Red_r;
                        break;
                }
            }
        }
        /// <summary>
        /// 0:無狀態,1:Ready,2:OK,3:NG
        /// </summary>
        public int screwStateR2
        {
            get
            {
                return screwStateR2;
            }
            set
            {
                int i = value;
                switch (i)
                {
                    case 0://無狀態
                        pictureBoxAxis2R.Image = _19_1003.Properties.Resources.Gray_r;
                        break;
                    case 1://Ready
                        pictureBoxAxis2R.Image = _19_1003.Properties.Resources.Blue_r;
                        break;
                    case 2://OK
                        pictureBoxAxis2R.Image = _19_1003.Properties.Resources.Green_r;
                        break;
                    case 3://NG
                        pictureBoxAxis2R.Image = _19_1003.Properties.Resources.Red_r;
                        break;
                }
            }
        }
        /// <summary>
        /// 0:無狀態,1:Ready,2:OK,3:NG
        /// </summary>
        public int screwStateR3
        {
            get
            {
                return screwStateR3;
            }
            set
            {
                int i = value;
                switch (i)
                {
                    case 0://無狀態
                        pictureBoxAxis3R.Image = _19_1003.Properties.Resources.Gray_r;
                        break;
                    case 1://Ready
                        pictureBoxAxis3R.Image = _19_1003.Properties.Resources.Blue_r;
                        break;
                    case 2://OK
                        pictureBoxAxis3R.Image = _19_1003.Properties.Resources.Green_r;
                        break;
                    case 3://NG
                        pictureBoxAxis3R.Image = _19_1003.Properties.Resources.Red_r;
                        break;
                }
            }
        }
        /// <summary>
        /// 0:無狀態,1:Ready,2:OK,3:NG
        /// </summary>
        public int screwStateR4
        {
            get
            {
                return screwStateR4;
            }
            set
            {
                int i = value;
                switch (i)
                {
                    case 0://無狀態
                        pictureBoxAxis4R.Image = _19_1003.Properties.Resources.Gray_r;
                        break;
                    case 1://Ready
                        pictureBoxAxis4R.Image = _19_1003.Properties.Resources.Blue_r;
                        break;
                    case 2://OK
                        pictureBoxAxis4R.Image = _19_1003.Properties.Resources.Green_r;
                        break;
                    case 3://NG
                        pictureBoxAxis4R.Image = _19_1003.Properties.Resources.Red_r;
                        break;
                }
            }
        }
        /// <summary>
        /// 0:無狀態,1:Ready,2:OK,3:NG
        /// </summary>
        public int screwStateR5
        {
            get
            {
                return screwStateR5;
            }
            set
            {
                int i = value;
                switch (i)
                {
                    case 0://無狀態
                        pictureBoxAxis5R.Image = _19_1003.Properties.Resources.Gray_r;
                        break;
                    case 1://Ready
                        pictureBoxAxis5R.Image = _19_1003.Properties.Resources.Blue_r;
                        break;
                    case 2://OK
                        pictureBoxAxis5R.Image = _19_1003.Properties.Resources.Green_r;
                        break;
                    case 3://NG
                        pictureBoxAxis5R.Image = _19_1003.Properties.Resources.Red_r;
                        break;
                }
            }
        }
        #region --torque value--
        public string screwTorqueL1
        {
            set
            {
                string torque = value;
                showCallBackText(lbTorque1L, torque);
            }
        }
        public string screwTorqueR1
        {
            set
            {
                string torque = value;
                showCallBackText(lbTorque1R, torque);
            }
        }
        public string screwTorqueL2
        {
            set
            {
                string torque = value;
                showCallBackText(lbTorque2L, torque);
            }
        }
        public string screwTorqueR2
        {
            set
            {
                string torque = value;
                showCallBackText(lbTorque2R, torque);
            }
        }
        public string screwTorqueL3
        {
            set
            {
                string torque = value;
                showCallBackText(lbTorque3L, torque);
            }
        }
        public string screwTorqueR3
        {
            set
            {
                string torque = value;
                showCallBackText(lbTorque3R, torque);
            }
        }
        public string screwTorqueL4
        {
            set
            {
                string torque = value;
                showCallBackText(lbTorque4L, torque);
            }
        }
        public string screwTorqueR4
        {
            set
            {
                string torque = value;
                showCallBackText(lbTorque4R, torque);
            }
        }
        public string screwTorqueL5
        {
            set
            {
                string torque = value;
                showCallBackText(lbTorque5L, torque);
            }
        }
        public string screwTorqueR5
        {
            set
            {
                string torque = value;
                showCallBackText(lbTorque5R, torque);
            }
        }
        #endregion
        #region --anble value--
        public string screwAngleL1
        {
            set
            {
                string Angle = value;
                showCallBackText(lbAngle1L, Angle);
            }
        }
        public string screwAngleR1
        {
            set
            {
                string Angle = value;
                showCallBackText(lbAngle1R, Angle);
            }
        }
        public string screwAngleL2
        {
            set
            {
                string Angle = value;
                showCallBackText(lbAngle2L, Angle);
            }
        }
        public string screwAngleR2
        {
            set
            {
                string Angle = value;
                showCallBackText(lbAngle2R, Angle);
            }
        }
        public string screwAngleL3
        {
            set
            {
                string Angle = value;
                showCallBackText(lbAngle3L, Angle);
            }
        }
        public string screwAngleR3
        {
            set
            {
                string Angle = value;
                showCallBackText(lbAngle3R, Angle);
            }
        }
        public string screwAngleL4
        {
            set
            {
                string Angle = value;
                showCallBackText(lbAngle4L, Angle);
            }
        }
        public string screwAngleR4
        {
            set
            {
                string Angle = value;
                showCallBackText(lbAngle4R, Angle);
            }
        }
        public string screwAngleL5
        {
            set
            {
                string Angle = value;
                showCallBackText(lbAngle5L, Angle);
            }
        }
        public string screwAngleR5
        {
            set
            {
                string Angle = value;
                showCallBackText(lbAngle5R, Angle);
            }
        }
        #endregion
        /// <summary>
        /// 條碼狀態 0:無狀態,1:相符,2:不符
        /// </summary>
        public int BarcodeMatch 
        {
            set 
            {
                switch (value)
                {
                    case 0:
                        showCallBackText(lbBarcodeNotMatch, " ");
                        lbBarcodeNotMatch.BackColor = Color.Gray;
                        break;
                    case 1:
                        showCallBackText(lbBarcodeNotMatch, "條碼與機種相符");
                        lbBarcodeNotMatch.BackColor = Color.Lime;
                        break;
                    case 2:
                        showCallBackText(lbBarcodeNotMatch, "條碼與機種不符");
                        lbBarcodeNotMatch.BackColor = Color.Red;
                        break;
                    default:
                        showCallBackText(lbBarcodeNotMatch, " ");
                        lbBarcodeNotMatch.BackColor = Color.Gray;
                        break;
                }
            }
        }
        //伺服槍READY
        public bool DriverReady 
        {
            set
            {
                if (value)
                {
                    lbDriverReady.BackColor = Color.Lime;
                }
                else
                {
                    lbDriverReady.BackColor = Color.Red;
                }
            }
        }
        #endregion
        #region --Type & Barcode & count--
        string _workTypeName = "";
        int _workTypeIndex = 0;
        string _workBarcode = "";
        public delegate void delcomboboxworktypeChanged();
        public string workTypeName 
        {
            get { return _workTypeName; }
            set 
            {
                _workTypeName = value;
                comboBoxWorkType.Items.Add(_workTypeName);
            } 
        }
        public int workTypeIndex 
        { 
            get 
            {
                _workTypeIndex = comboBoxWorkType.SelectedIndex;
                return _workTypeIndex; 
            } 
            set 
            { 
                _workTypeIndex = value;
                comboBoxWorkType.SelectedIndex = _workTypeIndex;
            } 
        }
        public void clearWorkTypeList()
        {
            comboBoxWorkType.Items.Clear();
        }
        private void pbAxis1Enable_Click(object sender, EventArgs e)
        {
            if (_enableAxis1==1)
            {
                _enableAxis1 = 0;
                pbAxis1Enable.BackColor = Color.Red;
                pbAxis1Enable.Text = "軸1停用";
            }
            else
            {
                _enableAxis1 = 1;
                pbAxis1Enable.BackColor = Color.Lime;
                pbAxis1Enable.Text = "軸1使用";
            }
        }

        private void pbAxis2Enable_Click(object sender, EventArgs e)
        {
            if (_enableAxis2 == 1)
            {
                _enableAxis2 = 0;
                pbAxis2Enable.BackColor = Color.Red;
                pbAxis2Enable.Text = "軸2停用";
            }
            else
            {
                _enableAxis2 = 1;
                pbAxis2Enable.BackColor = Color.Lime;
                pbAxis2Enable.Text = "軸2使用";
            }
        }

        public string workBarcode 
        {
            get { return _workBarcode; }
            set
            {
                _workBarcode = value;
                showCallBackText(lbBarcode, _workBarcode);
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        public int OKCount { set {showCallBackText(lbOKCount,value.ToString()); } }

        
        #endregion

        //******************************funtion**************************
        /// <summary>
        /// 不同執行緒控制項處理
        /// </summary>
        /// <param name="ctl">控制項</param>
        /// <param name="txt">傳送的文字</param>
        private void showCallBackText(object ctl, string txt)
        {
            string sType = ctl.GetType().ToString();
            if (sType.Contains("Button"))
            {
                Button button = (Button)ctl;
                if (button.InvokeRequired)
                {
                    showTextCallBack d = new showTextCallBack(showCallBackText);
                    this.Invoke(d, new object[] { button, txt });
                }
                else
                {
                    button.Text = txt;
                }
            }
            if (sType.Contains("Label"))
            {
                Label lebel = (Label)ctl;
                if (lebel.InvokeRequired)
                {
                    showTextCallBack d = new showTextCallBack(showCallBackText);
                    this.Invoke(d, new object[] { lebel, txt });
                }
                else
                {
                    lebel.Text = txt;
                }
            }
        }
    }
}
