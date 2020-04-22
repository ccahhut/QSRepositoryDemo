using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Threading;
namespace QManage
{
    public  class Fun
    {
        public static DataTable getAuthRemoteRecord()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("id", typeof(int));
            dt.Columns.Add("calltype", typeof(int));
            dt.Columns.Add("sendinfo", typeof(string));

            SqlConnection con = null;
            SqlDataReader sdr = null;
            try
            {
                con = new SqlConnection(SessionInfo.strConn);
                con.Open();
                SqlCommand cmd = new SqlCommand(string.Format("SELECT id,calltype,sendinfo from tblAuthRemote where status=0 and rq='{0}'", DateTime.Now.ToString("yyyy-MM-dd")), con);
                sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    int id = Convert.ToInt32(sdr[0].ToString()+"");
                    int calltype = Convert.ToInt32(sdr[1].ToString() + "");
                    string sendinfo = sdr[2].ToString()+"";
                    dt.Rows.Add(id, calltype, sendinfo);
                }
            }
            catch (Exception se)
            {
                FConst.WriteErrLog("getAuthRemoteRecord:" + se.Message);
            }
            finally
            {
                try
                {
                    if (sdr != null)
                        sdr.Close();
                }
                catch { ;}
                try
                {
                    if (con != null)
                        con.Close();
                }
                catch { ;}
            }
            return dt;
        }

        //private static SqlConnection con = null; 
        //public static void Connect()
        //{
        //    con = new SqlConnection(SessionInfo.strConn);
        //    con.Open();
        //}
        public static void GetSendInfo(out int nTaxFlowID,out string strTrCode,out string strParams) 
        {
            nTaxFlowID = 0;
            strTrCode = "";
            strParams = "";
            SqlConnection con = null; 
            try
            {
                con = new SqlConnection(SessionInfo.strConn);
                con.Open();
                SqlCommand cmd = new SqlCommand("spTaxGetInfo", con);
                cmd.CommandType = CommandType.StoredProcedure;
                //@TaxFlowID int output,
                //@TrCode varchar(4) output,
                //@ParamsIn varchar(128) output 
                cmd.Parameters.Add("@TaxFlowID", SqlDbType.Int);
                cmd.Parameters["@TaxFlowID"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@TrCode", SqlDbType.VarChar,4);
                cmd.Parameters["@TrCode"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@ParamsIn", SqlDbType.VarChar, 512);
                cmd.Parameters["@ParamsIn"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                nTaxFlowID = Convert.ToInt32(cmd.Parameters["@TaxFlowID"].Value);
                strTrCode = cmd.Parameters["@TrCode"].Value.ToString();
                strParams = cmd.Parameters["@ParamsIn"].Value.ToString();
            }
            catch (Exception se)
            {
                FConst.WriteErrLog("GetSendInfo:" + se.Message);
                //try
                //{
                //    if (con != null)
                //        con.Close();
                //}
                //catch { ;}
                //con = null;
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
        public static void WriteRcvInfo(int nTaxFlowID, string strParamsOut, int nBusiStatus,string strBusiParams)
        {
            SqlConnection con = null;
            try
            {
                con = new SqlConnection(SessionInfo.strConn);
                con.Open();
                SqlCommand cmd = new SqlCommand("spTaxSetInfo", con);
                cmd.CommandType = CommandType.StoredProcedure;
                //@TaxFlowID int ,
                //@ParamsOut varchar(2048),
                //@BusiStatus int,
                //@BusiParams varchar(2048)
                cmd.Parameters.Add("@TaxFlowID", SqlDbType.Int);
                cmd.Parameters["@TaxFlowID"].Value = nTaxFlowID;

                cmd.Parameters.Add("@ParamsOut", SqlDbType.VarChar, 2048);
                cmd.Parameters["@ParamsOut"].Value = strParamsOut;

                cmd.Parameters.Add("@BusiStatus", SqlDbType.Int);
                cmd.Parameters["@BusiStatus"].Value = nBusiStatus;

                cmd.Parameters.Add("@BusiParams", SqlDbType.VarChar, 2048);
                cmd.Parameters["@BusiParams"].Value = strBusiParams;

                cmd.ExecuteNonQuery();
            }
            catch (Exception se)
            {
                FConst.WriteErrLog("WriteRcvInfo:" + se.Message);
                //try
                //{
                //    if (con != null)
                //        con.Close();
                //}
                //catch { ;} 
                //con = null;
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
        //发送国税信息
        public static void GetSendInfoGTax(out int nTaxFlowID,out string strTrCode, out string strParams)
        {
            nTaxFlowID = 0;
            strTrCode = "";
            strParams = "";
            SqlConnection con = null;
            try
            {
                con = new SqlConnection(SessionInfo.strConn);
                con.Open();
                SqlCommand cmd = new SqlCommand("spTaxGetInfoGTax", con);
                cmd.CommandType = CommandType.StoredProcedure;
                //@TaxFlowID int output, 
                //@Params varchar(2048) output 
                cmd.Parameters.Add("@TaxFlowID", SqlDbType.Int);
                cmd.Parameters["@TaxFlowID"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@TrCode", SqlDbType.VarChar, 4);
                cmd.Parameters["@TrCode"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@Params", SqlDbType.VarChar, 2048);
                cmd.Parameters["@Params"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                nTaxFlowID = Convert.ToInt32(cmd.Parameters["@TaxFlowID"].Value);
                strTrCode = cmd.Parameters["@TrCode"].Value.ToString();
                strParams = cmd.Parameters["@ParamsIn"].Value.ToString();
            }
            catch (Exception se)
            {
                FConst.WriteErrLog("GetSendInfoGTax:" + se.Message);
                //try
                //{
                //    if (con != null)
                //        con.Close();
                //}
                //catch { ;}
                //con = null;
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
        //修改网络状态
        public static void ModifySystemType()
        {
            SqlConnection con = null;
            try
            {
                con = new SqlConnection(SessionInfo.strConn);
                con.Open();
                SqlCommand cmd = new SqlCommand("spModifySystemType", con);
                cmd.CommandType = CommandType.StoredProcedure;
                //@Status int  
                cmd.Parameters.Add("@Status", SqlDbType.Int);
                cmd.Parameters["@Status"].Value = SessionInfo.nSystemType; 

                cmd.ExecuteNonQuery();
            }
            catch (Exception se)
            {
                FConst.WriteErrLog("ModifySystemType:" + se.Message);
                //try
                //{
                //    if (con != null)
                //        con.Close();
                //}
                //catch { ;}
                //con = null;
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
        //处理未评价信息
        public static void ModifyAppraise()
        {
            SqlConnection con = null;
            try
            {
                con = new SqlConnection(SessionInfo.strConn);
                con.Open();
                SqlCommand cmd = new SqlCommand("spAppriseNull", con);
                cmd.CommandType = CommandType.StoredProcedure;  
                cmd.ExecuteNonQuery();
            }
            catch (Exception se)
            {
                FConst.WriteErrLog("ModifyAppraise:" + se.Message);
                //try
                //{
                //    if (con != null)
                //        con.Close();
                //}
                //catch { ;}
                //con = null;
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
        /// <summary>
        /// 获取系统时间，每日启动1次即可
        /// </summary>
        /// <returns></returns>
        public static int getSystemTime()
        {
            int nTaxFlowID = 0;
            SqlConnection con = null;
            try
            {
                con = new SqlConnection(SessionInfo.strConn);
                con.Open();
                SqlCommand cmd = new SqlCommand("spTaxGetSystemTime", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@TaxFlowID", SqlDbType.Int);
                cmd.Parameters["@TaxFlowID"].Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                nTaxFlowID = Convert.ToInt32(cmd.Parameters["@TaxFlowID"].Value); 
                return nTaxFlowID;
            }
            catch (Exception se)
            {
                FConst.WriteErrLog("getSystemTime:"+se.Message);
                //try
                //{
                //    if (con != null)
                //        con.Close();
                //}
                //catch { ;}
                //con = null;
                return nTaxFlowID;
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

        /// <summary>
        /// 获取远程接口返回
        /// </summary>
        /// <param name="nTaxFlowID"></param>
        /// <param name="strResult"></param>
        /// <returns></returns>
        public static bool getTaxResult(int nTaxFlowID, out int nStatus, out string strResult)
        {
            strResult = "";
            nStatus = 0;
            bool bRet = false;
            int nRetCode = 0;
            DateTime stime = System.DateTime.Now;
            DateTime etime;
            SqlConnection con = null;
            try
            {
                con = new SqlConnection(SessionInfo.strConn);
                con.Open();
                SqlCommand cmd = new SqlCommand("spGetTaxResult", con);
                cmd.CommandType = CommandType.StoredProcedure;
                //@TaxFlowID     int ,--接口ID
                //@BusiStatus   int output
                //@Result        varchar(2048) output,
                //@RetCode		int output 	 
                cmd.Parameters.Add("@TaxFlowID", SqlDbType.Int);
                cmd.Parameters["@TaxFlowID"].Value = nTaxFlowID;

                cmd.Parameters.Add("@BusiStatus", SqlDbType.Int);
                cmd.Parameters["@BusiStatus"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@Result", SqlDbType.VarChar, 2048);
                cmd.Parameters["@Result"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@RetCode", SqlDbType.Int);
                cmd.Parameters["@RetCode"].Direction = ParameterDirection.Output;
                while (true)
                {
                    etime = System.DateTime.Now;
                    TimeSpan ts = etime - stime;
                    if (ts.Seconds * 1000 > SessionInfo.nTimeOut)
                    {
                        return bRet;
                    }
                    cmd.ExecuteNonQuery();
                    nRetCode = Convert.ToInt32(cmd.Parameters["@RetCode"].Value);
                    if (nRetCode == 0)
                    {
                        bRet = true;
                        strResult = cmd.Parameters["@Result"].Value.ToString();
                        nStatus = Convert.ToInt32(cmd.Parameters["@BusiStatus"].Value);
                        return bRet;
                    }
                    Thread.Sleep(200);
                }
            }
            catch (Exception se)
            {
                //try
                //{
                //    if (con != null)
                //        con.Close();
                //}
                //catch { ;}
                //con = null;
                return bRet;
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
    }
     
}
