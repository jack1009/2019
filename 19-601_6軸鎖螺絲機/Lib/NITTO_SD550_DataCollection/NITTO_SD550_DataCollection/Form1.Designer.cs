namespace NITTO_SD550_DataCollection
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pb_test1 = new System.Windows.Forms.Button();
            this.tbTestResult = new System.Windows.Forms.TextBox();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label11 = new System.Windows.Forms.Label();
            this.tbMachineID = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbCurrentCount = new System.Windows.Forms.TextBox();
            this.pbResetCount = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.tbBarcode = new System.Windows.Forms.TextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbComPort = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbNumScrew = new System.Windows.Forms.TextBox();
            this.pbSavePara = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tbStartAdd = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbNumAxis = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbFilePath = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.pbTestSumCheck = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // pb_test1
            // 
            this.pb_test1.Font = new System.Drawing.Font("新細明體", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.pb_test1.Location = new System.Drawing.Point(6, 6);
            this.pb_test1.Name = "pb_test1";
            this.pb_test1.Size = new System.Drawing.Size(118, 26);
            this.pb_test1.TabIndex = 36;
            this.pb_test1.Text = "通訊取得資料並存檔";
            this.pb_test1.UseVisualStyleBackColor = true;
            this.pb_test1.Click += new System.EventHandler(this.pb_test1_Click);
            // 
            // tbTestResult
            // 
            this.tbTestResult.Location = new System.Drawing.Point(130, 6);
            this.tbTestResult.Multiline = true;
            this.tbTestResult.Name = "tbTestResult";
            this.tbTestResult.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbTestResult.Size = new System.Drawing.Size(741, 26);
            this.tbTestResult.TabIndex = 38;
            // 
            // serialPort1
            // 
            this.serialPort1.PortName = "COM2";
            this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(885, 418);
            this.tabControl1.TabIndex = 40;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label11);
            this.tabPage1.Controls.Add(this.tbMachineID);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.tbCurrentCount);
            this.tabPage1.Controls.Add(this.pbResetCount);
            this.tabPage1.Controls.Add(this.label10);
            this.tabPage1.Controls.Add(this.tbBarcode);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(877, 392);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "主頁";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 47);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(80, 12);
            this.label11.TabIndex = 41;
            this.label11.Text = "當前鎖付計數:";
            // 
            // tbMachineID
            // 
            this.tbMachineID.Location = new System.Drawing.Point(92, 44);
            this.tbMachineID.Name = "tbMachineID";
            this.tbMachineID.Size = new System.Drawing.Size(245, 22);
            this.tbMachineID.TabIndex = 42;
            this.tbMachineID.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 75);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 12);
            this.label5.TabIndex = 39;
            this.label5.Text = "當前螺絲計數:";
            // 
            // tbCurrentCount
            // 
            this.tbCurrentCount.Location = new System.Drawing.Point(92, 72);
            this.tbCurrentCount.Name = "tbCurrentCount";
            this.tbCurrentCount.Size = new System.Drawing.Size(245, 22);
            this.tbCurrentCount.TabIndex = 40;
            this.tbCurrentCount.Text = "0";
            // 
            // pbResetCount
            // 
            this.pbResetCount.Location = new System.Drawing.Point(244, 100);
            this.pbResetCount.Name = "pbResetCount";
            this.pbResetCount.Size = new System.Drawing.Size(93, 26);
            this.pbResetCount.TabIndex = 38;
            this.pbResetCount.Text = "重設螺絲計數";
            this.pbResetCount.UseVisualStyleBackColor = true;
            this.pbResetCount.Click += new System.EventHandler(this.pbResetCount_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 19);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(56, 12);
            this.label10.TabIndex = 38;
            this.label10.Text = "工件號碼:";
            // 
            // tbBarcode
            // 
            this.tbBarcode.Location = new System.Drawing.Point(68, 16);
            this.tbBarcode.Name = "tbBarcode";
            this.tbBarcode.Size = new System.Drawing.Size(269, 22);
            this.tbBarcode.TabIndex = 38;
            this.tbBarcode.Text = "ABCD";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.groupBox2);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(877, 392);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "參數";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.tbComPort);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.tbNumScrew);
            this.groupBox2.Controls.Add(this.pbSavePara);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.tbStartAdd);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.tbNumAxis);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.tbFilePath);
            this.groupBox2.Location = new System.Drawing.Point(6, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(230, 223);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "參數欄";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 46);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 37;
            this.label6.Text = "COM Port";
            // 
            // tbComPort
            // 
            this.tbComPort.Location = new System.Drawing.Point(64, 43);
            this.tbComPort.Name = "tbComPort";
            this.tbComPort.Size = new System.Drawing.Size(160, 22);
            this.tbComPort.TabIndex = 36;
            this.tbComPort.Text = "COM1";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 130);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 12);
            this.label3.TabIndex = 32;
            this.label3.Text = "產品螺絲隻數";
            // 
            // tbNumScrew
            // 
            this.tbNumScrew.Location = new System.Drawing.Point(89, 127);
            this.tbNumScrew.Name = "tbNumScrew";
            this.tbNumScrew.Size = new System.Drawing.Size(135, 22);
            this.tbNumScrew.TabIndex = 31;
            // 
            // pbSavePara
            // 
            this.pbSavePara.Location = new System.Drawing.Point(146, 183);
            this.pbSavePara.Name = "pbSavePara";
            this.pbSavePara.Size = new System.Drawing.Size(78, 26);
            this.pbSavePara.TabIndex = 30;
            this.pbSavePara.Text = "參數儲存";
            this.pbSavePara.UseVisualStyleBackColor = true;
            this.pbSavePara.Click += new System.EventHandler(this.pbSavePara_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 102);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 12);
            this.label1.TabIndex = 12;
            this.label1.Text = "軸啟始號(設成2位數)";
            // 
            // tbStartAdd
            // 
            this.tbStartAdd.Location = new System.Drawing.Point(127, 99);
            this.tbStartAdd.Name = "tbStartAdd";
            this.tbStartAdd.Size = new System.Drawing.Size(97, 22);
            this.tbStartAdd.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 74);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 10;
            this.label4.Text = "軸數量";
            // 
            // tbNumAxis
            // 
            this.tbNumAxis.Location = new System.Drawing.Point(64, 71);
            this.tbNumAxis.Name = "tbNumAxis";
            this.tbNumAxis.Size = new System.Drawing.Size(160, 22);
            this.tbNumAxis.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "檔案位置";
            // 
            // tbFilePath
            // 
            this.tbFilePath.Location = new System.Drawing.Point(64, 15);
            this.tbFilePath.Name = "tbFilePath";
            this.tbFilePath.Size = new System.Drawing.Size(160, 22);
            this.tbFilePath.TabIndex = 3;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label9);
            this.tabPage2.Controls.Add(this.label8);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Controls.Add(this.pbTestSumCheck);
            this.tabPage2.Controls.Add(this.pb_test1);
            this.tabPage2.Controls.Add(this.tbTestResult);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(877, 392);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "工程測試";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 123);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(33, 12);
            this.label9.TabIndex = 42;
            this.label9.Text = "label9";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 102);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(33, 12);
            this.label8.TabIndex = 41;
            this.label8.Text = "label8";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 81);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(33, 12);
            this.label7.TabIndex = 40;
            this.label7.Text = "label7";
            // 
            // pbTestSumCheck
            // 
            this.pbTestSumCheck.Location = new System.Drawing.Point(6, 38);
            this.pbTestSumCheck.Name = "pbTestSumCheck";
            this.pbTestSumCheck.Size = new System.Drawing.Size(118, 26);
            this.pbTestSumCheck.TabIndex = 39;
            this.pbTestSumCheck.Text = "SumCheck測試";
            this.pbTestSumCheck.UseVisualStyleBackColor = true;
            this.pbTestSumCheck.Click += new System.EventHandler(this.pbTestSumCheck_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(909, 442);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button pb_test1;
        private System.Windows.Forms.TextBox tbTestResult;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button pbTestSumCheck;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tbBarcode;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button pbResetCount;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbCurrentCount;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox tbMachineID;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbComPort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbNumScrew;
        private System.Windows.Forms.Button pbSavePara;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbStartAdd;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbNumAxis;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbFilePath;
    }
}

