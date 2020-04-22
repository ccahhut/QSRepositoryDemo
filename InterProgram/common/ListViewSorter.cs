using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Windows.Forms;
namespace QManage
{
    class ListViewSorter: IComparer
    {           
        private int ColumnToSort;// 指定按照哪个列排序      
        private SortOrder OrderOfSort;// 指定排序的方式               
        private CaseInsensitiveComparer ObjectCompare;// 声明CaseInsensitiveComparer类对象，
        private int _nSortTag;//此列的排序类型，0为数字，1为字符
        public ListViewSorter()// 构造函数
        {
            _nSortTag = 0;
            ColumnToSort = 0;// 默认按第一列排序            
            OrderOfSort = SortOrder.None;// 排序方式为不排序            
            ObjectCompare = new CaseInsensitiveComparer();// 初始化CaseInsensitiveComparer类对象
        }    
        // 重写IComparer接口.        
        // <returns>比较的结果.如果相等返回0，如果x大于y返回1，如果x小于y返回-1</returns>
        public int Compare(object x, object y)
        {   int compareResult;
            ListViewItem listviewX, listviewY;
            // 将比较对象转换为ListViewItem对象
            listviewX = (ListViewItem)x;
            listviewY = (ListViewItem)y;
            if (Convert.ToInt32(listviewX.Tag) == 1) //合计行不排序
                return 0;
            if (Convert.ToInt32(listviewY.Tag) == 1) //合计行不排序
                return 0;
            //if (listviewY.Index == listviewY.
            // 比较
            if (nSortTag == 0) //数字排序 
            {
                Decimal dValueX, dValueY;
                if (!Decimal.TryParse(listviewX.SubItems[ColumnToSort].Text, out dValueX))
                {
                    return 1;
                }
                if (!Decimal.TryParse(listviewY.SubItems[ColumnToSort].Text, out dValueY))
                {
                    return 1;
                }
                if (dValueX > dValueY)
                    compareResult = 1;
                else
                    compareResult = -1;
            }
            else //字符排序
                compareResult = ObjectCompare.Compare(listviewX.SubItems[ColumnToSort].Text, listviewY.SubItems[ColumnToSort].Text);
            // 根据上面的比较结果返回正确的比较结果
            if (OrderOfSort == SortOrder.Ascending)
            {   // 因为是正序排序，所以直接返回结果
                return compareResult;
            }
            else if (OrderOfSort == SortOrder.Descending)
            {  // 如果是反序排序，所以要取负值再返回
                return (-compareResult);
            }
            else
            {
                // 如果相等返回0
                return 0;
            }
        }       
        /// 获取或设置按照哪一列排序.        
        public int SortColumn
        {            set
            {      ColumnToSort = value;
            }
            get
            {
                return ColumnToSort;
            }
        }
        /// 获取或设置按照哪一列排序.        
        public int nSortTag
        {
            set
            {
                _nSortTag = value;
            }
            get
            {
                return _nSortTag;
            }
        }       
        /// 获取或设置排序方式.    
        public SortOrder Order
        {    set
            {
                OrderOfSort = value;
            }
            get
            {
                return OrderOfSort;
            }
        }    
    }
}

