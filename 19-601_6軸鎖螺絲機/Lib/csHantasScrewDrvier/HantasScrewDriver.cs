using System;


namespace HantasScrewDrvier
{
    #region --RS232取得資料分析--
    //總長33bytes內容皆為HEX
    //b[0]=ID1
    //b[1][2]=ID2
    //b[3][4]=Number
    //b[5][6]=鎖付時間
    //b[7][8]=preset
    //b[9][10]=目標扭力
    //b[11][12]=控制扭力
    //b[13][14]=速度
    //b[15][16]=角度1
    //b[17][18]=角度2
    //b[19][20]=角度
    //b[21][22]=count
    //b[23][24]=Error
    //b[25][26]=F/L
    //b[27][28]=Statsu=01:OK,03:切換F/L
    //b[29][30]=Sung角度
    //b[31][32]=Check
    #endregion
    public class csHantasScrewDrvier
    {
        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="_screwData">傳入RS232取得的資料</param>
        public csHantasScrewDrvier(byte[] _screwData)
        {
            setScrewData(_screwData);
        }
        public csHantasScrewDrvier() { }
        #region --prop--
        private ushort mID1;
        private ushort mID2;
        private ushort mNumber;
        private ushort mFasteningTime;
        private ushort mPreset;
        private ushort mTargetTorque;
        private ushort mControlTorque;
        private ushort mSpeed;
        private ushort mAngle1;
        private ushort mAngle2;
        private ushort mAngle;
        private ushort mCount;
        private ushort mError;
        private ushort mForL;
        private ushort mStatus;
        private ushort mSungAngle;
        private ushort mCheckByte;

        public ushort ID1 { get { return mID1; } }
        public ushort ID2 { get { return mID2; } }
        public ushort Number { get { return mNumber; } }
        public ushort FasteningTime { get { return mFasteningTime; } }
        public ushort Preset { get { return mPreset; } }
        public ushort TargetTorque { get { return mTargetTorque; } }
        public ushort ControlTorque { get { return mControlTorque; } }
        public ushort Speed { get { return mSpeed; } }
        public ushort Angle1 { get { return mAngle1; } }
        public ushort Angle2 { get { return mAngle2; } }
        public ushort Angle { get { return mAngle; } }
        public ushort Count { get { return mCount; } }
        public ushort Error { get { return mError; } }
        public DriverForL ForL
        {
            get
            {
                switch (mForL)
                {
                    case 0:
                        return DriverForL.Fsaten;
                    case 1:
                        return DriverForL.Losten;
                    default:
                        return DriverForL.Fsaten;
                }; } }
        public ScrewStaus Status
        {
            get
            {
                switch (mStatus)
                {
                    case 0:
                        return ScrewStaus.FreeRun;
                    case 1:
                        return ScrewStaus.FastenOK;
                    case 2:
                        return ScrewStaus.FastenNG;
                    case 3:
                        return ScrewStaus.FRChange;
                    case 4:
                        return ScrewStaus.PresetChang;
                    case 5:
                        return ScrewStaus.AlarmReset;
                    case 6:
                        return ScrewStaus.SystemError;
                    default:
                        return ScrewStaus.FreeRun;
                }; } }
        public ushort SungAngle { get { return mSungAngle; } }
        public byte[] ScrewData { set {setScrewData(value); } }
        #endregion

        #region --funtion--
        /// <summary>
        /// set由RS232取得的byte[33]資料
        /// </summary>
        /// <param name="_data">byte[]資料內容</param>
        private void setScrewData(byte[] _data)
        {
            //檢查陣列數
            if (_data.Length==33)
            {
                byte[] bs = _data;
                ushort[] i16 = new ushort[17];
                i16[0] = bs[0];
                int j = 1;
                for (int i = 1; i < bs.Length; i += 2)
                {
                    int st = bs[i] << 8;
                    st += bs[i + 1];
                    string s = st.ToString();
                    i16[j] = Convert.ToUInt16(s, 10);
                    j++;
                }
                mID1 = i16[0];
                mID2 = i16[1];
                mNumber = i16[2];
                mFasteningTime = i16[3];
                mPreset = i16[4];
                mTargetTorque = i16[5];
                mControlTorque = i16[6];
                mSpeed = i16[7];
                mAngle1 = i16[8];
                mAngle2 = i16[9];
                mAngle = i16[10];
                mCount = i16[11];
                mError = i16[12];
                mForL = i16[13];
                mStatus = i16[14];
                mSungAngle = i16[15];
                mCheckByte = i16[16];
            }
        }
        #endregion
    }

    public enum ScrewStaus { FreeRun, FastenOK, FastenNG, FRChange, PresetChang, AlarmReset, SystemError }
    public enum DriverForL { Fsaten , Losten}
}
