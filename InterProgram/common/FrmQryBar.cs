using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace QManage
{
    public partial class FrmQryBar : Form
    {
        public string strRetValue = "";
        private DataTable dt = null;
        private Point _p;
        private ListViewSorter lvwColumnSorter = null;
        public FrmQryBar()
        {
            InitializeComponent();
            lvwColumnSorter = new ListViewSorter();
            this.listData.ListViewItemSorter = lvwColumnSorter;
            FConst.SetColor(this);

        }
        public void Init(Point p, int flag, string strParam)
        { 
            _p.X = p.X -160;
            _p.Y = p.Y -130;
            if (_p.X < 0)
                _p.X = 0;
            if (_p.Y < 0)
                _p.Y = 0;
            Rectangle ScreenArea = System.Windows.Forms.Screen.GetWorkingArea(this);
            if (_p.X > ScreenArea.Width - this.Width)
                _p.X = ScreenArea.Width - this.Width;
            if (_p.Y > ScreenArea.Height - this.Height)
                _p.Y = ScreenArea.Height - this.Height;   
            int nSQLID = flag;
            byte[] bParamInfo;
            int nRetCode;
            string strRetText;
            //if (strParam != "")
            bParamInfo = Encoding.Default.GetBytes(strParam);
            FrmFactory.ShowFrmWaitFor("正在处理中，请稍后...");
            bool bRet = false;
            try
            {
                this.Cursor = Cursors.WaitCursor;
                bRet = DataInfo.getTable(nSQLID, bParamInfo, out dt, out nRetCode, out strRetText);

            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
            FrmFactory.HideFrmWaitFor();
            if(bRet)
            {
                if (nRetCode == 0)
                {
                    this.Cursor = Cursors.WaitCursor; 
                    listViewFun.TableToListView(dt,listData);
                    if (dt.Columns.Count == 1)
                    {
                        //label2.Visible = false;
                        txtName.Enabled = false;
                    }
                    this.Text = "【当前记录 " + listData.Items.Count.ToString() + " 条】";
                    this.Cursor = Cursors.Default;
                }
            }
            
        }

        private void FrmQryBar_FormClosed(object sender, FormClosedEventArgs e)
        {
            //FrmFactory.frmQryBar.Dispose();
            //FrmFactory.frmQryBar = null;
              
        }

        private void FrmQryBar_Load(object sender, EventArgs e)
        {
            this.Location = _p;
            //this.Location =  PointToClient(_p);
             
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            listViewFun.OrderListView((ListView)sender, e, lvwColumnSorter);    
        }

        private void btnQry_Click(object sender, EventArgs e)
        {
            if (txtID.Text.Trim().Length>0 )
                listViewFun.TableToListView(dt, listData,0,txtID.Text.Trim());
            else if (txtName.Text.Trim().Length > 0)
                listViewFun.TableToListView(dt, listData, 1, txtName.Text.Trim());
            else
                listViewFun.TableToListView(dt, listData);
            this.Text = "【当前记录 " + listData.Items.Count.ToString() + " 条】";
        }

        private void listData_DoubleClick(object sender, EventArgs e)
        {
            if (listData.SelectedItems.Count == 1)
            {
                if (dt.Columns.Count == 1)
                {
                    strRetValue = listData.SelectedItems[0].SubItems[0].Text;
                }
                else
                    strRetValue = listData.SelectedItems[0].SubItems[0].Text + "|" + listData.SelectedItems[0].SubItems[1].Text;
                this.Close();
            }
        }
    }
}
