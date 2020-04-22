using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace QManage
{
    public partial class FrmWaiter : Form
    {
        public FrmWaiter()
        {
            InitializeComponent();
        }
        public void SetMsg(string textMsg)
        {
            /*Int16 rowLen = 10;
            string msg = "";

            textMsg = "  " + textMsg.Trim();
            Int16 rowCount = Convert.ToInt16(Math.Ceiling(Convert.ToDouble(textMsg.Length) / Convert.ToDouble(rowLen)));
            //Int16 rowLenMod = Convert.ToInt16(Math.Floor(Convert.ToDouble(textMsg.Length) % Convert.ToDouble(rowLen)));
            textMsg = textMsg.PadRight(rowCount * rowLen, ' ');

            for (int i = 0; i < rowCount; i++)
            {
                msg = msg + textMsg.Substring(i * rowLen, rowLen) + "\r\n";
            }
            */
            lblMsg.Text = textMsg;
        }
    }
}
