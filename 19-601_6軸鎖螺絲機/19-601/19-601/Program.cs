using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _19_601
{
    static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (Process.GetProcessesByName(Application.ProductName).Length<=1)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FormOp1());
            }
            else
            {
                MessageBox.Show("應用程式已執行!!", "錯誤警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
