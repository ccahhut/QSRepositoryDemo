using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data;
namespace QManage
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            FrmFactory.frmMain = new FrmMain();
            Application.Run(FrmFactory.frmMain);
        }
    }
}
