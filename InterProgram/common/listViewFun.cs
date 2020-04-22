using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data;
//using Excel = Microsoft.Office.Interop.Excel;
namespace QManage
{
    class listViewFun
    {
        //将表格转换为listview最新版(含小数点2位的字段以及汇总的字段),首位不支持 //列首位为0
        public static void TableToListView(System.Data.DataTable dt, ListView lv,string strDoubleCol,string strSumCol)
        {
            lv.BeginUpdate();
            lv.Clear();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                lv.Columns.Add(dt.Columns[i].Caption.ToString());
                if (dt.Columns[i].DataType == System.Type.GetType("System.Decimal"))
                    lv.Columns[i].Tag = 0; //数字
                else
                    lv.Columns[i].Tag = 1; //字符
            }
            //添加最后1列 以便控制宽度
            lv.Columns.Add("LastCol");
            bool bDoubleCol = false, bSumRow = false;
            string[] arrDCol = null;
            string[] arrSRow = null;
            if (strDoubleCol.Trim().Length > 0)
            {
                bDoubleCol = true;
                arrDCol = strDoubleCol.Trim().Split('|');
            }
            if (strSumCol.Trim().Length > 0)
            {
                bSumRow = true;
                arrSRow = strSumCol.Trim().Split(new char[]{'|'},StringSplitOptions.RemoveEmptyEntries);
            }
            string strValue = "";
            Decimal[] dRow = new Decimal[dt.Columns.Count];
            for(int k = 0;k<dRow.Length;k++)
                dRow[k] = 0;
            ListViewItem[] items = new ListViewItem[dt.Rows.Count];
            int itemIndex = 0;
            foreach (DataRow DRkeys in dt.Rows)
            {
                ListViewItem lItem = new ListViewItem();
                lItem.Tag = 0;//非汇总字段
                lItem.Text = DRkeys[0].ToString();
                for (int e = 1; e < dt.Columns.Count; e++)
                {
                    if (Convert.ToInt32(lv.Columns[e].Tag) == 0) //数字
                    {
                        if (bDoubleCol) //有小数字段
                        {
                            if (FindValue(arrDCol, e))
                                strValue = Convert.ToDecimal(DRkeys[e]).ToString("F2");
                            else
                                strValue = DRkeys[e].ToString();
                        }
                        else
                        {
                            strValue = DRkeys[e].ToString();
                        }
                    }
                    else
                    {
                        strValue = DRkeys[e].ToString();
                    }
                    lItem.SubItems.Add(strValue);
                    //汇总
                    if (bSumRow)
                    {
                        if (FindValue(arrSRow, e))
                            dRow[e] += Convert.ToDecimal(DRkeys[e]);
                    }
                }
                items[itemIndex] = lItem;
                itemIndex++;
            }
            lv.Items.AddRange(items);
            //加汇总行表格有数据才加
            if (dt.Rows.Count > 0)
            {
                if (bSumRow)
                {
                    lv.Tag = 1;//有汇总字段
                    ListViewItem lItem = new ListViewItem();
                    lItem.Tag = 1;//汇总字段
                    lItem.Text = "合  计";
                    for (int e = 1; e < dt.Columns.Count; e++)
                    {
                        if (FindValue(arrSRow, e)) //汇总字段
                        {

                            if (FindValue(arrDCol, e))//小数
                                strValue = dRow[e].ToString("F2");
                            else
                                strValue = dRow[e].ToString();
                        }
                        else
                        {
                            strValue = "";
                        }
                        lItem.SubItems.Add(strValue);
                    }
                    lv.Items.Add(lItem);
                }
                else
                {
                    lv.Tag = 0;//无汇总字段
                }
            }
            //自动控制宽度
            lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            //删除最后1列
            lv.Columns.RemoveAt(lv.Columns.Count - 1);
            lv.EndUpdate();
        }
        private static bool FindValue(string[] str, int nValue)
        {
            bool b = false;
            if (str == null)
                return b;
            for (int i = 0; i < str.Length; i++)
            {
                if (Convert.ToInt32(str[i]) == nValue)
                {
                    b = true;
                    break;
                }
            }
            return b;
        }
        //将表格转换为listview 普通显示
        public static void TableToListView(System.Data.DataTable dt, ListView lv)
        {
            lv.BeginUpdate();
            lv.Clear();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                lv.Columns.Add(dt.Columns[i].Caption.ToString());
                if (dt.Columns[i].DataType == System.Type.GetType("System.Decimal"))
                    lv.Columns[i].Tag = 0; //数字
                else
                    lv.Columns[i].Tag = 1; //字符
            }
            //添加最后1列 以便控制宽度
            lv.Columns.Add("LastCol");
            //  
            ListViewItem[] items = new ListViewItem[dt.Rows.Count];
            int itemIndex = 0;
            foreach (DataRow DRkeys in dt.Rows)
            {
                ListViewItem lItem = new ListViewItem();
                lItem.Tag = 0;
                lItem.Text = DRkeys[0].ToString();
                for (int e = 1; e < dt.Columns.Count; e++)
                {
                    lItem.SubItems.Add(DRkeys[e].ToString());
                }
                items[itemIndex] = lItem;
                itemIndex++;
            }
            lv.Items.AddRange(items);
            //
            lv.Tag = 0;//无汇总字段 
            //自动控制宽度
            lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            //删除最后1列
            lv.Columns.RemoveAt(lv.Columns.Count - 1);
            lv.EndUpdate();
        }
         
        //listView查找(重新生成lv列表用)
        public static void TableToListView(System.Data.DataTable dt, ListView lv, int lvColumn, string strFind)
        {
            lv.BeginUpdate();
            lv.Clear();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                lv.Columns.Add(dt.Columns[i].Caption.ToString());
                if (dt.Columns[i].DataType == System.Type.GetType("System.Decimal"))
                    lv.Columns[i].Tag = 0;
                else
                    lv.Columns[i].Tag = 1;
            }
            string strValue = "";
            foreach (DataRow DRkeys in dt.Rows)
            {
                strValue = DRkeys[lvColumn].ToString();
                if (strValue.IndexOf(strFind) < 0)
                    continue;
                ListViewItem lItem = new ListViewItem();
                lItem.Tag = 0;
                lItem.Text = DRkeys[0].ToString();
                for (int e = 1; e < dt.Columns.Count; e++)
                {
                    lItem.SubItems.Add(DRkeys[e].ToString());
                }
                lv.Items.Add(lItem);
            }
            lv.Tag = 0;
            lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            lv.EndUpdate();
        }
        //listView排序
        public static void OrderListView(ListView lv, ColumnClickEventArgs e, ListViewSorter lvwColumnSorter)
        {
            lv.BeginUpdate();
            lvwColumnSorter.nSortTag = Convert.ToInt32(lv.Columns[e.Column].Tag);
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // 重新设置此列的排序方法.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // 设置排序列，默认为正向排序
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }
            lv.Sort();
            lv.EndUpdate();
        }
        //导出到EXCEL
        //public static void ExpToExcel(object sender, ListView lv, string strDoubleCol,string strTitle,string strParam1)
        //{
        //    if (lv.Items.Count == 0) 
        //        return;
        //    FrmFactory.ShowFrmWaitFor("正在处理中，请稍后...");
        //    try
        //    {
        //        bool bDoubleCol = false; 
        //        string[] arrDCol = null;
        //        if (strDoubleCol.Trim().Length > 0)
        //        {
        //            bDoubleCol = true;
        //            arrDCol = strDoubleCol.Trim().Split('|');
        //        }
        //        (sender as Form).Cursor = Cursors.WaitCursor;
        //        Excel.Application excel = new Excel.Application();
        //        Excel.Workbooks workbooks = excel.Workbooks;
        //        Excel.Workbook workbook = workbooks.Add(Excel.XlWBATemplate.xlWBATWorksheet);
        //        Excel.Sheets worksheets = workbook.Worksheets;
        //        Excel.Worksheet sheet = (Excel.Worksheet)worksheets.get_Item(1);


        //        Excel.Range range;
        //        excel.Cells.Select();
        //        excel.Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;

        //        //标题
        //        int rowPos = 2;
        //        range = excel.get_Range(sheet.Cells[rowPos, 1], sheet.Cells[rowPos, lv.Columns.Count]);
        //        range.Select();
        //        range.Merge(false);
        //        sheet.Cells[rowPos, 1] = strTitle;
        //        range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
        //        range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
        //        range.Font.Size = 16;
        //        range.Font.Bold = true;
        //        rowPos++;
        //        rowPos++;
        //        //参数字段
        //        range = excel.get_Range(sheet.Cells[rowPos, 1], sheet.Cells[rowPos, lv.Columns.Count]);
        //        range.Select();
        //        range.Merge(false);
        //        sheet.Cells[rowPos, 1] = strParam1;
        //        range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
        //        range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;             
        //        rowPos++;
        //        rowPos++;
        //        //表头
        //        for (int i = 1; i <= lv.Columns.Count; i++)
        //        {
        //            range = excel.get_Range(sheet.Cells[rowPos, i], sheet.Cells[rowPos, i]);
        //            range.Borders.LineStyle = 1;
        //            //range.Font.Name = "华文仿宋";
        //            //range.Font.Size = 16;
        //            //range.Font.Bold = true;
        //            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
        //            range.ColumnWidth = (lv.Columns[i - 1].Width * 15.0 / 567) * 4.374+5;
        //            range.NumberFormatLocal = "@";
        //            sheet.Cells[rowPos, i] = lv.Columns[i - 1].Text;
                    
        //        }
        //        rowPos++;
        //        //内容
        //        foreach (ListViewItem item in lv.Items)
        //        {
        //            for (int i = 1; i <= lv.Columns.Count; ++i)
        //            {
        //                range = excel.get_Range(sheet.Cells[rowPos, i], sheet.Cells[rowPos, i]);
        //                range.Borders.LineStyle = 1;
        //                //range.Font.Name = "华文仿宋";
        //                //range.Font.Size = 12;
        //                range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
        //                if (Convert.ToInt32(lv.Columns[i - 1].Tag) == 0)
        //                {
        //                    if (bDoubleCol) //有小数字段
        //                    {
        //                        if (FindValue(arrDCol, i-1))
        //                             range.NumberFormatLocal = "0.00_ ";
        //                        else
        //                            range.NumberFormatLocal = "0_ ";
        //                    }
        //                    else
        //                    {
        //                        range.NumberFormatLocal = "0_ ";
        //                    } 
        //                }
        //                else
        //                    range.NumberFormatLocal = "@";

        //                sheet.Cells[rowPos, i] = item.SubItems[i - 1].Text;
                         
        //            }
        //            rowPos++;
        //        }
        //        excel.Cells.WrapText = true;//自动换行
        //        excel.Cells.Rows.AutoFit();//自动调整行高
        //        excel.Cells.Columns.AutoFit();//自动调整；列宽
        //        excel.Visible = true; 
        //    }
        //    catch (Exception se)
        //    {
        //        (sender as Form).Cursor = Cursors.Default;
        //        MessageBox.Show("系统故障:" + se.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //    finally
        //    {
        //        (sender as Form).Cursor = Cursors.Default;
        //        FrmFactory.HideFrmWaitFor();
        //    }
            
            
        //}
    }
}
