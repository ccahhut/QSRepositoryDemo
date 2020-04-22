using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
//功能：1.双击编辑ListView中被双击的subitem项
//          2.如果是第一项就出现ComboBox
//扩展listview控件
namespace QManage
{
    class listViewEx : ListView
    {
        private TextBox m_tb;
        private ComboBox m_cb;

        public listViewEx()
        {
            m_tb = new TextBox();
            m_cb = new ComboBox();
            m_tb.Multiline = true;
            m_tb.Visible = false;
            m_cb.Visible = false;
            this.GridLines = true;
            this.CheckBoxes = false;
            this.FullRowSelect = true;
            this.Controls.Add(m_tb);
            this.Controls.Add(m_cb);
        }
        private void EditItem(ListViewItem.ListViewSubItem subItem)
        {
            if (this.SelectedItems.Count <= 0)
            {
                return;
            }
            
            Rectangle _rect = subItem.Bounds;
            m_tb.Bounds = _rect;
            m_tb.BringToFront();
            m_tb.Text = subItem.Text;
            m_tb.Leave += new EventHandler(tb_Leave);
            m_tb.TextChanged += new EventHandler(m_tb_TextChanged);
            m_tb.Visible = true;
            m_tb.Tag = subItem;
            m_tb.Select();
        }
        private void EditItem(ListViewItem.ListViewSubItem subItem, Rectangle rt)
        {
            if (this.SelectedItems.Count <= 0)
            {
                return;
            }

            //Rectangle _rect = rt;
            //m_cb.Bounds = _rect;
            //m_cb.BringToFront();
            //m_cb.Items.Add(subItem.Text);
            //m_cb.Text = subItem.Text;
            //m_cb.Leave += new EventHandler(lstb_Leave);
            //m_cb.TextChanged += new EventHandler(m_lstb_TextChanged);
            //m_cb.Visible = true;
            //m_cb.Tag = subItem;
            //m_cb.Select();

            Rectangle _rect = rt;
            m_tb.Bounds = _rect;
            m_tb.BringToFront();
            m_tb.Text = subItem.Text;
            m_tb.Leave += new EventHandler(tb_Leave);
            m_tb.TextChanged += new EventHandler(m_tb_TextChanged);
            m_tb.Visible = true;
            m_tb.Tag = subItem;
            m_tb.Select();
        }


        //protected override void OnKeyDown(KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.F2)
        //    {

        //        if (this.SelectedItems.Count > 0)
        //        {
        //            //this.SelectedItems[0].BeginEdit();
        //            ListViewItem lvi = this.SelectedItems[0];
        //            EditItem(lvi.SubItems[0],new Rectangle(lvi.Bounds.Left,lvi.Bounds.Top,this.Columns[0].Width,lvi.Bounds.Height-2));
        //        }
        //    }
        //    base.OnKeyDown(e);
        //}

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            this.m_tb.Visible = false;
            this.m_cb.Visible = false;
            base.OnSelectedIndexChanged(e);
        }

        //protected override void OnDoubleClick(EventArgs e)
        //{
        //    Point tmpPoint = this.PointToClient(Cursor.Position);
        //    ListViewItem item = this.GetItemAt(tmpPoint.X, tmpPoint.Y);
        //    if (item != null)
        //    {
        //        if (tmpPoint.X > this.Columns[0].Width && tmpPoint.X < this.Width)
        //        {
        //            EditItem(1);
        //        }
        //    }

        //    base.OnDoubleClick(e);
        //}
        protected override void OnDoubleClick(EventArgs e)
        {
            Point tmpPoint = this.PointToClient(Cursor.Position);
            ListViewItem.ListViewSubItem subitem = this.HitTest(tmpPoint).SubItem;
            ListViewItem item = this.HitTest(tmpPoint).Item;
            if (subitem != null)
            {
                if (item.SubItems[0].Equals(subitem))
                {
                    //EditItem(subitem, new Rectangle(item.Bounds.Left, item.Bounds.Top, this.Columns[0].Width, item.Bounds.Height - 2));
                    EditItem(subitem, new Rectangle(item.Bounds.Left, item.Bounds.Top, this.Columns[0].Width, item.Bounds.Height));
                }
                else
                {
                    EditItem(subitem);
                }
            }

            base.OnDoubleClick(e);
        }

        protected override void WndProc(ref   Message m)
        {
            if (m.Msg == 0x115 || m.Msg == 0x114)
            {
                this.m_tb.Visible = false;
            }
            base.WndProc(ref   m);
        }

        private void tb_Leave(object sender, EventArgs e)
        {
            m_tb.TextChanged -= new EventHandler(m_tb_TextChanged);
            (sender as TextBox).Visible = false;
        }

        private void m_tb_TextChanged(object sender, EventArgs e)
        {
            if ((sender as TextBox).Tag is ListViewItem.ListViewSubItem)
            {
                (this.m_tb.Tag as ListViewItem.ListViewSubItem).Text = this.m_tb.Text;
            }
            
        }
        private void lstb_Leave(object sender, EventArgs e)
        {
            m_cb.TextChanged -= new EventHandler(m_lstb_TextChanged);
        }
        private void m_lstb_TextChanged(object sender, EventArgs e)
        {
            if ((sender as ListBox).Tag is ListViewItem.ListViewSubItem)
            {
                (this.m_cb.Tag as ListViewItem.ListViewSubItem).Text = this.m_cb.Text;
            }
        }


    }
}
