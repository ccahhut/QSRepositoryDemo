using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
//using QManage.rpt;
namespace QManage
{
    public class FrmFactory
    {    
        public static FrmMain frmMain = null;
        public static FrmWaiter frmWaiter = null;
        public static void ShowFrmWaitFor(string msg)
        {
            //if (frmWaiter == null)
            //{
            //    frmWaiter = new FrmWaiter();
            //}
            //frmWaiter.SetMsg(msg);
            //frmWaiter.Show();
            //frmWaiter.Update();
            return;
        }
        public static void HideFrmWaitFor()
        {
            //if (frmWaiter != null)
            //{
            //    frmWaiter.Hide();
            //}
            return;
        }
        public static FrmQryBar frmQryBar = null;
        public static string ShowFrmQryBar(Point p, int flag, string strParam)
        {
            string str = "";
            if (frmQryBar == null)
            {
                frmQryBar = new FrmQryBar();
            }
            frmQryBar.Init(p,flag,strParam);
            frmQryBar.ShowDialog(); 
            str = frmQryBar.strRetValue;
            frmQryBar.Dispose();
            frmQryBar = null;
            return str;
        }
        
    }
}
