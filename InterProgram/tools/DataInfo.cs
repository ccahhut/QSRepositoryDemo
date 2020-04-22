using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace QManage
{
    public class DataInfo
    {
        public struct stParams
        {
            public string ParamName;
            public int DataType;
            public string strValue;
            public long intValue;//2011-11-23修改
            public decimal fValue;
        };
        public static bool getTable(int SQLCMD, byte[] paramInfo,  out DataTable dt, out int intRetCode, out string strRetText)
        {
            stParams[] chParam = new stParams[50];
            dt = new DataTable();
            intRetCode = 99;
            strRetText = "系统忙,请稍后再试";
            string strSQL = "";
            string strParam = "";
            SqlConnection con = new SqlConnection(SessionInfo.strConn);
            try
            {
                con.Open();
                if (!getQryCfg(con, SQLCMD, out strSQL, out strParam))
                {
                    intRetCode = 7;
                    strRetText = "错误的命令字";
                    return true;
                }
                //分解参数
                string strtmp = new string(Encoding.Default.GetChars(paramInfo));
                string[] strData = strtmp.Split(Convert.ToChar(0));
                int nPos = 0;
                string strParmName;
                int nParamType;
                object nParamValue;
                SqlCommand cmd = new SqlCommand(strSQL, con);
                int nParamCnt = 0;
                while (nPos < strData.Length)
                {
                    strParmName = strData[nPos];
                    nPos++;
                    if (nPos == strData.Length)
                        break;
                    nParamType = Convert.ToInt32(strData[nPos]);
                    nPos++;
                    if (nPos == strData.Length)
                        break;
                    nParamValue = strData[nPos];
                    nPos++;
                    chParam[nParamCnt].ParamName = strParmName;
                    chParam[nParamCnt].DataType = nParamType;
                    if (nParamType == 0) //number
                        chParam[nParamCnt].intValue = Convert.ToInt32(nParamValue);
                    else if (nParamType == 1) //varchar
                        chParam[nParamCnt].strValue = nParamValue.ToString();
                    else //decimal
                        chParam[nParamCnt].fValue = Convert.ToDecimal(nParamValue);
                    nParamCnt++;
                }
                //绑定参数
                strParam = strParam.Trim();
                if (strParam.Length > 0)
                {
                    int index = 0;
                    char chParamPos;
                    int nCnt = 0;
                    string strpName;
                    for (int i = 0; i < strParam.Length; i++)
                    {
                        strpName = "@p" + i.ToString();
                        chParamPos = strParam[i];
                        if (chParamPos >= '0' && chParamPos <= '9')
                        {
                            index = Convert.ToInt32(chParamPos) - 48;
                        }
                        else
                        {
                            index = Convert.ToInt32(chParamPos) - 87;
                        }
                        if (index >= nParamCnt)
                        {
                            intRetCode = 3;
                            strRetText = "参数错误"; 
                            return false;
                        }
                        if (chParam[index].DataType == 0)
                        {
                            cmd.Parameters.Add(new SqlParameter(strpName, chParam[index].intValue));
                        }
                        else if (chParam[index].DataType == 1)
                        { cmd.Parameters.Add(new SqlParameter(strpName, chParam[index].strValue));
                        }
                        else
                        { 
                            cmd.Parameters.Add(new SqlParameter(strpName, chParam[index].fValue));
                        }
                        nCnt++;
                    }
                }
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);
                intRetCode = 0;
                strRetText = "成功"; 
                return true;
            }
            catch (Exception se)
            {
                intRetCode = 99;
                strRetText = "系统忙,请稍后再试:"+se.Message; 
                return true;
            }
            finally
            {
                try
                {
                    if (con != null)
                        con.Close();
                }
                catch { ;}
            }
        }
        private static bool getQryCfg(SqlConnection con, int nSQLCMD, out string strSQL, out string strParam)
        {
            strSQL = "";
            strParam = "";
            SqlCommand cmd = new SqlCommand(string.Format("select ParamInfo,SqlInfo from tblQryCfg where SqlCmd={0}", nSQLCMD), con);
            SqlDataReader sdr = null;
            sdr = cmd.ExecuteReader();
            if (sdr.Read())
            {
                strParam = sdr[0].ToString();
                strSQL = sdr[1].ToString();
                sdr.Close();
                return true;
            }
            else
            {
                sdr.Close();
                return false;
            }

        }
    }
}
