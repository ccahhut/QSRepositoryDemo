using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Management;
using System.Data;

using System.Windows.Forms;
using System.Security.Cryptography;
using System.Collections;
using System.IO;

namespace QManage
{
    public static class SessionInfo
    {  
        public static string strIpAddr = "";
        public static string strHostName = "";
        public static string strMac = "";
        public static bool bLogin = false;

        public static string CurrVersion = "2.0";

        public static int nSystemType = 0;//系统类别 0:普通 1:税务
        public static int nStatus = 1;//服务状态 0:暂停 1:正常
        public static int nDelay = 3000;//延迟启动秒数
        public static int nTimeOut = 30000;//超时时间,接口不返回
        public static int BUFFER = 2048;
        public static int CALL_ERR_CODE = 999;
        public static string CALL_ERR_MSG = "调用远程接口失败,请检查网络";

        public static int DEBUG = 0;//0 不记录日志 1:记录日志

        //参数文件参数
        public static string strServer = "";
        public static string strDB = "";
        public static string strUser = "";
        public static string strPwd = "";
        public static string strConn = "";

        //接口地址
        public static string strUrl = "";
        public static string strUUID = "";
        /// <summary>
        /// 中心数据管理服务器的接口地址
        /// </summary>
        public static string strDMServerUrl = "";

        /// <summary>
        /// 航信人脸识别channel_id
        /// </summary>
        public static string channel_id_hx = "";
        public static string strUrl_hx = "";
    } 
    public class FConst
    {
        //记录日志
        public static void WriteLog(string strLog)
        {
            //if (SessionInfo.DEBUG == 0)
            //    return;
            string sPath = ".\\log\\";
            string FileName;
            FileStream fs = null;
            StreamWriter srd = null;
            try
            {
                if (!Directory.Exists(sPath))
                    Directory.CreateDirectory(sPath);
                FileName = sPath + System.DateTime.Today.ToString("yyyy-MM-dd") + ".log";

                fs = File.Open(FileName, FileMode.Append, FileAccess.Write);
                srd = new StreamWriter(fs);
                srd.WriteLine(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " => " + strLog);
            }
            catch
            {
                return;
            }
            finally
            {
                try
                {
                    if (srd != null)
                        srd.Close();
                }
                catch { ;}

                try
                {
                    if (fs != null)
                        fs.Close();
                }
                catch { ;}
            }
        }
        //记录错误日志
        public static void WriteErrLog(string strLog)
        {
            string sPath = ".\\log\\";
            string FileName;
            FileStream fs = null;
            StreamWriter srd = null;
            try
            {
                if (!Directory.Exists(sPath))
                    Directory.CreateDirectory(sPath);
                FileName = sPath + System.DateTime.Today.ToString("yyyy-MM-dd_Err") + ".log";

                fs = File.Open(FileName, FileMode.Append, FileAccess.Write);
                srd = new StreamWriter(fs);
                srd.WriteLine(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " => " + strLog);
            }
            catch
            {
                return;
            }
            finally
            {
                try
                {
                    if (srd != null)
                        srd.Close();
                }
                catch { ;}

                try
                {
                    if (fs != null)
                        fs.Close();
                }
                catch { ;}
            }
        }
        //get MD5
        public static string GetMD5(string input)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = Encoding.Default.GetBytes(input);
            byte[] outStr = md5.ComputeHash(result);
            return BitConverter.ToString(outStr).Replace("-", "").ToLower();
        }
        //检查权限
        public static bool CheckRight(string strTrCode)
        {

            bool bFind = false;
            //if (SessionInfo.nOpLevel == 1)
            //    return true;
            //if (SessionInfo.dtRight == null)
            //    return bFind;
            //foreach (DataRow dr in SessionInfo.dtRight.Rows)
            //{
            //    if (strTrCode == dr[0].ToString())
            //    {
            //        bFind = true;
            //        break;
            //    }
            //}
            return bFind;
        }
        //获取本机的IP
        public static string GetIpAddr()
        {
            string strHostName = Dns.GetHostName(); //得到本机的主机名
            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
            //Dns.GetHostByName(strHostName); //取得本机IP v4
            string strAddr = "";
            for (int i = 0; i < ipEntry.AddressList.Length;i++ )
            {
                if (ipEntry.AddressList[i].ToString().IndexOf('.') < 0)
                    continue;
                strAddr = ipEntry.AddressList[i].ToString();
                break;
            } 
            return strAddr;
        }
        //获取机器名
        public static string GetHostName()
        {
            return Dns.GetHostName();
        }
        //获取本机的MAC
        public static string GetMac()
        {
            string mac = "";
            ManagementObjectSearcher query = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection queryCollection = query.Get();
            foreach (ManagementObject mo in queryCollection)
            {
                if ((bool)mo["IPEnabled"] == true)
                {
                    mac = mo["MacAddress"].ToString();
                    break ;
                }
            }
            return mac;
        }
        //生成参数串
        public static string SetParams(string strParamName,int nParamType,object ParamValue)
        { 
            char ch = Convert.ToChar(0);
            string str = "";
            str = string.Format("{0}{1}{2}{3}{4}{5}", strParamName, ch, nParamType.ToString(), ch, ParamValue.ToString(), ch);
            return str;
        }
        //将得到的bytestream转为DataTable
        public static DataTable byteToTable(byte[] byteIn)
        {
            if (byteIn.Length == 0)
                return null;
            DataTable dt = new DataTable();
            DataRow myDataRow;
            DataColumn myDataColumn;
            char[] charIn;
            charIn = Encoding.Default.GetChars(byteIn);
            string strDataInfo = new string(charIn);
            string[] strData = strDataInfo.Split(Convert.ToChar(0));
            //string a = "1" + Convert.ToChar(0) + "2";
            int nRowCount = Convert.ToInt32(strData[0]);
            int nColCount = Convert.ToInt32(strData[1]);
            string strFieldName, strFieldType;
            int nLoopRow = 0, nLoopCol = 0;
            for (nLoopCol = 0; nLoopCol < nColCount; nLoopCol++)
            {
                string[] strTmp = strData[nLoopCol + 2].Split(Convert.ToChar(1));
                strFieldName = strTmp[0];
                strFieldType = strTmp[1];
                myDataColumn = new DataColumn();
                if (strFieldType == "NUMBER")
                    myDataColumn.DataType = System.Type.GetType("System.Decimal");
                else
                    myDataColumn.DataType = System.Type.GetType("System.String");
                myDataColumn.ColumnName = strFieldName;
                myDataColumn.ReadOnly = false;
                dt.Columns.Add(myDataColumn);
            }
            int intPos = 0;
            for (nLoopRow = 0; nLoopRow < nRowCount; nLoopRow++)
            {
                myDataRow = dt.NewRow();
                for (nLoopCol = 0; nLoopCol < nColCount; nLoopCol++)
                {
                    intPos = (nLoopRow + 1) * nColCount + nLoopCol + 2;
                    if (strData[intPos] != "")
                    {
                        myDataRow[nLoopCol] = strData[intPos];
                    }
                }
                dt.Rows.Add(myDataRow);
            }
            return dt;
        }

        //刷新菜单权限
        public static void RefreshOpRight(ToolStrip ts)
        {
            string strTrCode;
            foreach (ToolStripItem item in ts.Items)
            {
                if (item.Tag == null)
                    item.Enabled = false;
                else if (item.Tag.ToString() == "1")
                    item.Enabled = true;
                else
                {
                    strTrCode = item.Tag.ToString();
                    if (FConst.CheckRight(strTrCode))
                        item.Enabled = true;
                    else
                        item.Enabled = false;
                }
            }
        }
        //修改窗口中各个控件的颜色
        public static void SetColor(object sender)
        {
            foreach (object c in (sender as Control).Controls)
            {
                if (c is TextBox)
                {
                    ((TextBox)c).ForeColor = System.Drawing.Color.Chocolate;
                }
                else if (c is ComboBox)
                {

                    ((ComboBox)c).ForeColor = System.Drawing.Color.DarkOrange;
                }
                else if (c is Button)
                {

                    ((Button)c).ForeColor = System.Drawing.Color.Blue;
                }
                else if (c is CheckBox)
                {

                    ((CheckBox)c).ForeColor = System.Drawing.Color.DarkSlateBlue;
                }
                else if (c is DateTimePicker)
                {

                    ((DateTimePicker)c).CalendarForeColor = System.Drawing.Color.DarkSlateBlue;
                }
                //else if (c is Label)
                //{
                //    //((Label)c).ForeColor = System.Drawing.Color.DarkSlateBlue;
                //}
                else if (c is TreeView)
                {
                    ((TreeView)c).ForeColor = System.Drawing.Color.Orange;
                }
                else if (c is ListView)
                {
                    ((ListView)c).ForeColor = System.Drawing.Color.Orange;
                } 
                else if (c is ToolStrip)
                {
                    foreach (ToolStripItem item in ((ToolStrip)c).Items)
                    {

                        item.ForeColor = System.Drawing.Color.DarkSlateBlue;
                    }   
                }
                 
                if ((c as Control).HasChildren)
                {
                    SetColor(c);
                }
            }
        }
        //检查文本控件是否包括竖线
        public static bool CheckVLine(object sender)
        {
            foreach (object c in (sender as Control).Controls)
            {
                if (c is TextBox)
                {
                    if (((TextBox)c).Text.IndexOf('|') >= 0)
                    {
                        return false;
                    }
                }
                if (c is RichTextBox)
                {
                    if (((RichTextBox)c).Text.IndexOf('|') >= 0)
                    {
                        return false;
                    }
                }
                if ((c as Control).HasChildren)
                {
                    CheckVLine(c);
                }
            }
            return true;
        }
        //禁用各个输入框控件
        public static void SetUnable(object sender)
        {
            foreach (object c in (sender as Control).Controls)
            {
                if (c is TextBox)
                {
                    ((TextBox)c).ReadOnly = true;
                }
                else if (c is ComboBox)
                {

                    ((ComboBox)c).Enabled = false;
                }
                else if (c is Button)
                { 
                    ((Button)c).Enabled = false;
                }
                else if (c is CheckBox)
                {

                    ((CheckBox)c).Enabled = false;
                }

                if ((c as Control).HasChildren)
                {
                    SetUnable(c);
                }
            }
        }
        //将商户添加到树形图
        public static void TableToTree(DataTable dt, TreeView tv)
        {
            tv.BeginUpdate();
            foreach (DataRow dr in dt.Rows)
            {
                TreeNode node = new TreeNode();
                node.Text = dr[2].ToString();
                node.Tag = dr[0].ToString();
                if (dr[1].ToString() == "")
                {
                    tv.Nodes.Add(node);
                }
                else
                {
                    TreeNode pnode = null;
                    foreach (TreeNode tn in tv.Nodes)
                    {
                        pnode = FindNode(tn, dr[1].ToString());
                        if (pnode != null) break;
                    }
                    if (pnode == null)
                        tv.Nodes.Add(node);
                    else
                        pnode.Nodes.Add(node);
                }
            }
            tv.EndUpdate();
        }
       
        private static TreeNode FindNode(TreeNode node, string strValue)
        {
            if (node == null) 
                return null;
            if (node.Tag.ToString() == strValue)
                return node;
            TreeNode tnRet = null;
            foreach (TreeNode tn in node.Nodes)
            {
                tnRet = FindNode(tn, strValue);
                if (tnRet != null) break;
            }
            return tnRet;
        }

    }

    /// <summary>
    /// Base64工具类
    /// </summary>
    public class Base64Util
    {

        /// <summary>
        /// Base64加密，采用utf8编码方式加密
        /// </summary>
        /// <param name="source">待加密的明文</param>
        /// <returns>加密后的字符串</returns>
        public static string Base64Encode(string source)
        {
            return Base64Encode(Encoding.UTF8, source);
        }

        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="encodeType">加密采用的编码方式</param>
        /// <param name="source">待加密的明文</param>
        /// <returns></returns>
        public static string Base64Encode(Encoding encodeType, string source)
        {
            string encode = string.Empty;
            try
            {
                byte[] bytes = encodeType.GetBytes(source);
                //encode = Convert.ToBase64String(bytes);
                encode = Base64Encode(bytes);
            }
            catch
            {
                encode = source;
            }
            return encode;
        }
        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="sourceBuffer">待加密的字节数据</param>
        /// <returns></returns>
        public static string Base64Encode(byte[] sourceBuffer)
        {
            string encode = string.Empty;
            try
            {
                encode = Convert.ToBase64String(sourceBuffer);
            }
            catch (Exception ex)
            {
                ex.ToString();
                encode = "";
            }
            return encode;
        }

        /// <summary>
        /// Base64解密，采用utf8编码方式解密
        /// </summary>
        /// <param name="result">待解密的密文</param>
        /// <returns>解密后的字符串</returns>
        public static string Base64Decode(string result)
        {
            return Base64Decode(Encoding.UTF8, result);
        }

        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="encodeType">解密采用的编码方式，注意和加密时采用的方式一致</param>
        /// <param name="result">待解密的密文</param>
        /// <returns>解密后的字符串</returns>
        public static string Base64Decode(Encoding encodeType, string result)
        {
            string decode = string.Empty;
            try
            {
                byte[] bytes = Convert.FromBase64String(result);
                decode = encodeType.GetString(bytes);
            }
            catch
            {
                decode = result;
            }

            return decode;
        }
    }
}