using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using ClassLibraryCommonUse;

namespace _19_1003
{
    public partial class FormLogIn : Form
    {
        public FormLogIn()
        {
            InitializeComponent();
            CurrentUser = new csUser();
            mUsers = new List<csUser>();
            CurrentUser.UserId = "";
            CurrentUser.UserPassWord = "";
            CurrentUser.UserLevel = 0;
        }
        public List<csUser> mUsers { get; set; }
        public csUser CurrentUser { get; set; }
        private void pbLogin_Click(object sender, EventArgs e)
        {
            string id = textBoxID.Text.ToUpper();
            id.Trim();
            string password = textBoxPW.Text;
            bool finded = false;
            foreach (var x in mUsers)
            {
                if (id.Equals(x.UserId.ToUpper()))
                {
                    if (password.Equals(x.UserPassWord))
                    {
                        CurrentUser = x;
                        textBoxID.Text = "";
                        textBoxPW.Text = "";
                        finded = true;
                        this.Close();
                    }
                    else
                    {
                        CurrentUser.UserId = "";
                        CurrentUser.UserPassWord = "";
                        CurrentUser.UserLevel = 0;
                        textBoxID.Text = "";
                        textBoxPW.Text = "";
                        finded = true;
                        MessageBox.Show("帳號或密碼不符!!");
                    }
                    break;
                }
            }
            if (!finded)
            {
                MessageBox.Show("未找到符合的ID");
            }
        }

        private void pbCancel_Click(object sender, EventArgs e)
        {
            CurrentUser.UserId = "";
            CurrentUser.UserPassWord = "";
            CurrentUser.UserLevel = 0;
            textBoxID.Text = "";
            textBoxPW.Text = "";
            this.Close();
        }
    }
}
