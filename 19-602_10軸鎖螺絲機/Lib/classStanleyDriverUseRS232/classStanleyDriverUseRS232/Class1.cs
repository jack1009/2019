using System;
using System.IO;
using ClosedXML.Excel;
using System.Collections.Generic;

namespace classStanleyDriverUseRS232
{
    public class classStanleyDriver
    {
        public string fileEnd = @".xlsx";
        public string fileYear = DateTime.Now.ToString("yyyy");
        public string fileMonth = DateTime.Now.ToString("MM");
        public string fileDate = DateTime.Now.ToString("MMdd");
        private string _bodybarcode = "", _topscrewbarcode = "", _rsbarcode = "", _topcapbarcode = "",
            _bottomscrewbarcode = "", _bottomcapbarcode = "";
        private int _ProductState = 0;
        private const int _NumOfBarcode = 10;
        /// <summary>
        /// 本體條碼
        /// </summary>
        public string BodyBarcode
        {
            get { return _bodybarcode; }
            set { _bodybarcode = value; }
        }
        /// <summary>
        /// 良品=1不良品=0狀態
        /// </summary>
        public int ProductState { get { return _ProductState; } set { _ProductState = value; } }
        /// <summary>
        /// 壓板螺絲條碼
        /// </summary>
        public string TopScrewBarcode
        {
            get { return _topscrewbarcode; }
            set { _topscrewbarcode = value; }
        }
        /// <summary>
        /// RS條碼
        /// </summary>
        public string RsBarcode
        {
            get { return _rsbarcode; }
            set { _rsbarcode = value; }
        }
        /// <summary>
        /// 壓板條碼
        /// </summary>
        public string TopCapBarcode
        {
            get { return _topcapbarcode; }
            set { _topcapbarcode = value; }
        }
        /// <summary>
        /// 防塵蓋螺絲條碼
        /// </summary>
        public string BottomScrewBarcode
        {
            get { return _bottomscrewbarcode; }
            set { _bottomscrewbarcode = value; }
        }
        /// <summary>
        /// 防塵蓋條碼
        /// </summary>
        public string BottomCapBarcode
        {
            get { return _bottomcapbarcode; }
            set { _bottomcapbarcode = value; }
        }
        /// <summary>
        /// 人員ID資料
        /// </summary>
        public string[] StaffID = new string[3] { "", "", "" };
        /// <summary>
        /// List鎖付資料
        /// </summary>
        public List<StanleyDriverScrewData> listScrewDatas = new List<StanleyDriverScrewData>();
        //建構子
        public classStanleyDriver()
        {
        }
        /// <summary>
        /// 檢查檔案是否存在,不存在就建立新檔
        /// </summary>
        /// <param name="filePath">檔案根目錄</param>
        /// <param name="filename">檔案名稱</param>
        public void checkFileExist(string filePath, string filename)
        {
            string fullfileName = filePath + fileYear + @"\" + fileMonth + @"\" + fileDate + @"\" + filename.TrimEnd() + fileEnd;

            bool fileExist = File.Exists(fullfileName);
            if (!fileExist)
            {
                XLWorkbook wb = new XLWorkbook();
                wb.AddWorksheet("sheet1");
                var ws = wb.Worksheet(1);
                ws.Cell("B1").Value = "條碼";
                ws.Cell("C1").Value = "設定號碼";
                ws.Cell("D1").Value = "JOB號碼";
                ws.Cell("E1").Value = "扭力輸出";
                ws.Cell("F1").Value = "扭力判定,A=OK,L=Low,H=High";
                ws.Cell("G1").Value = "角度輸出";
                ws.Cell("H1").Value = "角度判定,A=OK,L=Low,H=High";
                ws.Cell("I1").Value = "總合判定,A=OK,R=NOK";
                ws.Cell("J1").Value = "鎖付日期時間";

                ws.Cell("A2").Value = "壓板螺絲條碼";
                ws.Cell("A3").Value = "RS條碼";
                ws.Cell("A4").Value = "壓板條碼";
                ws.Cell("A5").Value = "防塵蓋螺絲條碼";
                ws.Cell("A6").Value = "防塵蓋條碼";
                ws.Cell("A7").Value = "7110人員ID";
                ws.Cell("A8").Value = "7120人員ID";
                ws.Cell("A9").Value = "7130人員ID";

                ws.Columns().AdjustToContents();
                wb.SaveAs(fullfileName);
                ws.Dispose();
                wb.Dispose();
            }
        }
        /// <summary>
        /// 待所有軸都載入後,儲存鎖付檔案
        /// </summary>
        /// <param name="filePath">檔案根目錄</param>
        /// <param name="filename">檔案名稱,即本體條碼</param>
        /// <param name="stationNumber">站別,e.g.7110=0,7120=1,7130=2</param>
        public virtual void SaveScrewData(string filePath, string filename, int stationNumber)
        {
            string fullfileName = filePath + fileYear + @"\" + fileMonth + @"\" + fileDate + @"\" + filename.TrimEnd() + fileEnd;
            XLWorkbook wb = new XLWorkbook(fullfileName);
            var ws = wb.Worksheet(1);
            ws.Cell("B2").Value = TopScrewBarcode;
            ws.Cell("B3").DataType = XLDataType.Text;
            ws.Cell("B3").Value = RsBarcode;
            ws.Cell("B3").Style.NumberFormat.SetNumberFormatId((int)XLPredefinedFormat.Number.Integer);
            ws.Cell("B4").Value = TopCapBarcode;
            ws.Cell("B5").Value = BottomScrewBarcode;
            ws.Cell("B6").Value = BottomCapBarcode;
            ws.Cell("B7").Value = StaffID[0];
            ws.Cell("B8").Value = StaffID[1];
            ws.Cell("B9").Value = StaffID[2];
            int lastRowNumber = ws.LastRowUsed().RowNumber();
            if (stationNumber > 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    ws.Cell(i + _NumOfBarcode + (3 * stationNumber), 1).Value = listScrewDatas[i].StationNo;
                    ws.Cell(i + _NumOfBarcode + (3 * stationNumber), 3).Value = listScrewDatas[i].SpindleNumber;
                    ws.Cell(i + _NumOfBarcode + (3 * stationNumber), 4).Value = listScrewDatas[i].JobNumber;
                    ws.Cell(i + _NumOfBarcode + (3 * stationNumber), 5).Value = listScrewDatas[i].TorqueResult;
                    ws.Cell(i + _NumOfBarcode + (3 * stationNumber), 6).Value = listScrewDatas[i].TorqueStatus;
                    ws.Cell(i + _NumOfBarcode + (3 * stationNumber), 7).Value = listScrewDatas[i].AngleResult;
                    ws.Cell(i + _NumOfBarcode + (3 * stationNumber), 8).Value = listScrewDatas[i].AngleStatus;
                    ws.Cell(i + _NumOfBarcode + (3 * stationNumber), 9).Value = listScrewDatas[i].OverrallStatus;
                    ws.Cell(i + _NumOfBarcode + (3 * stationNumber), 10).Value = listScrewDatas[i].ScrewDateTime;
                }
            }
            if (ProductState == 1)
            {
                ws.Cell("A1").Value = "良品";
                ws.Cell("A1").Style.Font.FontColor = XLColor.Black;
                ws.Cell("A1").Style.Fill.BackgroundColor = XLColor.Green;
            }
            else
            {
                ws.Cell("A1").Value = "不良品";
                ws.Cell("A1").Style.Font.FontColor = XLColor.White;
                ws.Cell("A1").Style.Fill.BackgroundColor = XLColor.Red;
            }
            ws.Columns().AdjustToContents();
            wb.SaveAs(fullfileName);
            ws.Dispose();
            wb.Dispose();
        }
        /// <summary>
        /// 取得鎖付檔案資料
        /// </summary>
        /// <param name="filePath">檔案根目錄</param>
        /// <param name="filename">檔案名稱,即本體Barcode</param>
        public virtual void GetFileData(string filePath, string filename)
        {
            string fullfileName = filePath + fileYear + @"\" + fileMonth + @"\" + fileDate + @"\" + filename.TrimEnd() + fileEnd;
            XLWorkbook wb = new XLWorkbook(fullfileName);
            var ws = wb.Worksheet(1);
            TopScrewBarcode = ws.Cell("B2").Value.ToString();
            RsBarcode = ws.Cell("B3").Value.ToString()+"\t";
            TopCapBarcode = ws.Cell("B4").Value.ToString();
            BottomScrewBarcode = ws.Cell("B5").Value.ToString();
            BottomCapBarcode = ws.Cell("B6").Value.ToString();
            StaffID[0] = ws.Cell("B7").Value.ToString();
            StaffID[1] = ws.Cell("B8").Value.ToString();
            StaffID[2] = ws.Cell("B9").Value.ToString();
            ws.Dispose();
            wb.Dispose();
        }
        /// <summary>
        /// 解析Stanley伺服槍RS232接收的資料,並加入List鎖付資料
        /// </summary>
        /// <param name="rs232data">byte[]解析Stanley伺服槍RS232接收的byte陣列</param>
        public virtual void GetRs232ScrewData(byte[] rs232data)
        {
            string str = System.Text.Encoding.Default.GetString(rs232data);
            char c = ',';
            string[] substrings = str.Split(c);
            StanleyDriverScrewData sda = new StanleyDriverScrewData();
            sda.SpindleNumber = substrings[0];
            sda.JobNumber = substrings[1];
            sda.TorqueResult = substrings[2];
            sda.TorqueStatus = substrings[3];
            sda.AngleResult = substrings[4];
            sda.AngleStatus = substrings[5];
            sda.OverrallStatus = substrings[6];
            sda.ScrewDateTime = substrings[7];
            listScrewDatas.Add(sda);
        }
    }
    public class StanleyDriverScrewData
    {
        private string _StationNo, _spindlenumber, _jobnumber, _torqueresult, _torquestatus, _angleresult, _anglestatus, _overrallstatus, _screwdatetime;

        public string StationNo { get { return _StationNo; } set { _StationNo = value; } }
        public string JobNumber { get { return _spindlenumber; } set { _spindlenumber = value; } }          //軸號碼
        public string SpindleNumber { get { return _jobnumber; } set { _jobnumber = value; } }               //JOB號碼
        public string TorqueResult { get { return _torqueresult; } set { _torqueresult = value; } }         //扭力輸出
        public string TorqueStatus { get { return _torquestatus; } set { _torquestatus = value; } }         //扭力判定A=OK,L=Low,H=High
        public string AngleResult { get { return _angleresult; } set { _angleresult = value; } }            //角度輸出
        public string AngleStatus { get { return _anglestatus; } set { _anglestatus = value; } }            //角度判定A=OK,L=Low,H=High
        public string OverrallStatus { get { return _overrallstatus; } set { _overrallstatus = value; } }   //總合判定A=OK,R=NOK
        public string ScrewDateTime { get { return _screwdatetime; } set { _screwdatetime = value; } }          //鎖付的時間
    }
}