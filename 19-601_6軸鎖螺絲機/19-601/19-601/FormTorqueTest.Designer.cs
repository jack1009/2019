namespace _19_601
{
    partial class FormTorqueTest
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pbTorqueCheck = new System.Windows.Forms.Button();
            this.pbTestStart = new System.Windows.Forms.Button();
            this.pbDoubleScrew = new System.Windows.Forms.Button();
            this.lbChannelNo_PTest = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // pbTorqueCheck
            // 
            this.pbTorqueCheck.BackColor = System.Drawing.Color.White;
            this.pbTorqueCheck.Font = new System.Drawing.Font("新細明體", 14F);
            this.pbTorqueCheck.Location = new System.Drawing.Point(38, 37);
            this.pbTorqueCheck.Name = "pbTorqueCheck";
            this.pbTorqueCheck.Size = new System.Drawing.Size(146, 87);
            this.pbTorqueCheck.TabIndex = 1;
            this.pbTorqueCheck.Text = "扭力檢查\r\n無效中";
            this.pbTorqueCheck.UseVisualStyleBackColor = false;
            this.pbTorqueCheck.Click += new System.EventHandler(this.pbTorqueCheck_Click);
            // 
            // pbTestStart
            // 
            this.pbTestStart.BackColor = System.Drawing.Color.White;
            this.pbTestStart.Font = new System.Drawing.Font("新細明體", 14F);
            this.pbTestStart.ForeColor = System.Drawing.Color.Black;
            this.pbTestStart.Location = new System.Drawing.Point(277, 199);
            this.pbTestStart.Name = "pbTestStart";
            this.pbTestStart.Size = new System.Drawing.Size(218, 145);
            this.pbTestStart.TabIndex = 2;
            this.pbTestStart.Text = "螺絲機連續動作";
            this.pbTestStart.UseVisualStyleBackColor = false;
            this.pbTestStart.Click += new System.EventHandler(this.pbTestStart_Click);
            // 
            // pbDoubleScrew
            // 
            this.pbDoubleScrew.BackColor = System.Drawing.Color.White;
            this.pbDoubleScrew.Font = new System.Drawing.Font("新細明體", 14F);
            this.pbDoubleScrew.Location = new System.Drawing.Point(207, 37);
            this.pbDoubleScrew.Name = "pbDoubleScrew";
            this.pbDoubleScrew.Size = new System.Drawing.Size(146, 87);
            this.pbDoubleScrew.TabIndex = 3;
            this.pbDoubleScrew.Text = "再次鎖付";
            this.pbDoubleScrew.UseVisualStyleBackColor = false;
            this.pbDoubleScrew.Click += new System.EventHandler(this.pbDoubleScrew_Click);
            // 
            // lbChannelNo_PTest
            // 
            this.lbChannelNo_PTest.AutoSize = true;
            this.lbChannelNo_PTest.Font = new System.Drawing.Font("新細明體", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbChannelNo_PTest.Location = new System.Drawing.Point(631, 69);
            this.lbChannelNo_PTest.Margin = new System.Windows.Forms.Padding(3);
            this.lbChannelNo_PTest.Name = "lbChannelNo_PTest";
            this.lbChannelNo_PTest.Size = new System.Drawing.Size(110, 21);
            this.lbChannelNo_PTest.TabIndex = 14;
            this.lbChannelNo_PTest.Text = "1234567890";
            this.lbChannelNo_PTest.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("新細明體", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label18.Location = new System.Drawing.Point(573, 69);
            this.label18.Margin = new System.Windows.Forms.Padding(3);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(52, 21);
            this.label18.TabIndex = 15;
            this.label18.Text = "頻道";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FormTorqueTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.ControlBox = false;
            this.Controls.Add(this.label18);
            this.Controls.Add(this.lbChannelNo_PTest);
            this.Controls.Add(this.pbDoubleScrew);
            this.Controls.Add(this.pbTestStart);
            this.Controls.Add(this.pbTorqueCheck);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormTorqueTest";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormTorqueTest";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FormTorqueTest_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button pbTorqueCheck;
        private System.Windows.Forms.Button pbTestStart;
        private System.Windows.Forms.Button pbDoubleScrew;
        private System.Windows.Forms.Label lbChannelNo_PTest;
        private System.Windows.Forms.Label label18;
    }
}