using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
namespace QManage
{
    public  class FunEmp
    { 
        /// <summary>
        /// 登录8001 操作员|密码|呼叫器地址|
        /// </summary>
        /// <param name="pIn"></param>
        /// <param name="pOut"></param>
        public static void Logon(cParamInfo pIn,out cParamInfo pOut)
        {
            pOut = new cParamInfo();
            bool bRet = false;
            string strResult = "";
            SqlConnection con = new SqlConnection(SessionInfo.strConn);
            SqlTransaction tran = null;
            try
            {
                con.Open();
                tran = con.BeginTransaction();
                SqlCommand cmd = new SqlCommand("spLogin", con, tran);
                cmd.CommandType = CommandType.StoredProcedure;
                // @EmpNo 		varchar(20),   --操作员
                // @CallerAddr   varchar(32),   --呼叫器地址
                // @EmpPwd        varchar(32),   --密码   
                // @EmpName		varchar(60) output,  --姓名
                // @EmpLevel		varchar(20) output,  --星级
                // @LastLoginTime varchar(30) output, --上次登录时间
                // @FormNo        varchar(10) output, --登录窗口
                // @FormName      varchar(60) output, --窗口名称
                // @TaxFlowID     int output,--接口ID
                // @RetCode		int output,		  
                // @RetText		varchar(120) output
                cmd.Parameters.Add("@EmpNo", SqlDbType.VarChar, 20);
                cmd.Parameters["@EmpNo"].Value = pIn.strEmpNo;

                cmd.Parameters.Add("@CallerAddr", SqlDbType.VarChar, 32);
                cmd.Parameters["@CallerAddr"].Value = pIn.strCallerAddr;

                cmd.Parameters.Add("@EmpPwd", SqlDbType.VarChar, 32);
                cmd.Parameters["@EmpPwd"].Value = pIn.strEmpPwd;

                cmd.Parameters.Add("@EmpName", SqlDbType.VarChar,60);
                cmd.Parameters["@EmpName"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@EmpLevel", SqlDbType.VarChar, 20);
                cmd.Parameters["@EmpLevel"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@LastLoginTime", SqlDbType.VarChar, 30);
                cmd.Parameters["@LastLoginTime"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@FormNo", SqlDbType.VarChar, 10);
                cmd.Parameters["@FormNo"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@FormName", SqlDbType.VarChar, 60);
                cmd.Parameters["@FormName"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@TaxFlowID", SqlDbType.Int);
                cmd.Parameters["@TaxFlowID"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@RetCode", SqlDbType.Int);
                cmd.Parameters["@RetCode"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@RetText", SqlDbType.VarChar, 120);
                cmd.Parameters["@RetText"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                pOut.nRetCode = Convert.ToInt32(cmd.Parameters["@RetCode"].Value);
                pOut.strRetText = cmd.Parameters["@RetText"].Value.ToString();

                if (pOut.nRetCode == 0)
                {
                    pOut.strEmpName = cmd.Parameters["@EmpName"].Value.ToString();
                    pOut.strEmpLevel = cmd.Parameters["@EmpLevel"].Value.ToString();
                    pOut.strFormNo = cmd.Parameters["@FormNo"].Value.ToString();
                    pOut.strFormName = cmd.Parameters["@FormName"].Value.ToString();
                    pOut.nTaxFlowID = Convert.ToInt32(cmd.Parameters["@TaxFlowID"].Value);

                }
                if (pOut.nRetCode == 0)
                    tran.Commit();
                else
                    tran.Rollback(); 
                if (pOut.nRetCode != 0)
                    return;
                //开始处理远程接口
                if (SessionInfo.nSystemType == 1)
                {
                    int nBusiStatus = 0;
                    bRet = FunTax.getJosonResult(pOut.nTaxFlowID,out nBusiStatus,out strResult);
                    if (!bRet)
                    {
                        pOut.nRetCode = SessionInfo.CALL_ERR_CODE;
                        pOut.strRetText = SessionInfo.CALL_ERR_MSG;
                    }
                    //解析jason字符串
                }
                
            }
            catch (Exception se)
            {
                pOut.nRetCode = 99;
                pOut.strRetText = "系统忙,请稍后再试:" + se.Message;
                try
                {
                    if (tran != null)
                        tran.Rollback();
                }
                catch { ;}
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
        /// 退出登录8006 操作员|窗口号|
        /// </summary>
        /// <param name="pIn"></param>
        /// <param name="pOut"></param>
        public static void LogonOut(cParamInfo pIn, out cParamInfo pOut)
        {
            pOut = new cParamInfo();
            SqlConnection con = new SqlConnection(SessionInfo.strConn);
            SqlTransaction tran = null;
            try
            {
                con.Open();
                tran = con.BeginTransaction();
                SqlCommand cmd = new SqlCommand("spLoginOut", con, tran);
                cmd.CommandType = CommandType.StoredProcedure;
                // @EmpNo 		varchar(20),   --操作员
                //@FormNo       varchar(10),
                // @RetCode		int output,		  
                // @RetText		varchar(120) output
                cmd.Parameters.Add("@EmpNo", SqlDbType.VarChar, 20);
                cmd.Parameters["@EmpNo"].Value = pIn.strEmpNo;

                cmd.Parameters.Add("@FormNo", SqlDbType.VarChar, 10);
                cmd.Parameters["@FormNo"].Value = pIn.strFormNo;

                cmd.Parameters.Add("@RetCode", SqlDbType.Int);
                cmd.Parameters["@RetCode"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@RetText", SqlDbType.VarChar, 120);
                cmd.Parameters["@RetText"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                pOut.nRetCode = Convert.ToInt32(cmd.Parameters["@RetCode"].Value);
                pOut.strRetText = cmd.Parameters["@RetText"].Value.ToString();

               
                if (pOut.nRetCode == 0)
                    tran.Commit();
                else
                    tran.Rollback(); 
                if (pOut.nRetCode != 0)
                    return;

            }
            catch (Exception se)
            {
                pOut.nRetCode = 99;
                pOut.strRetText = "系统忙,请稍后再试:" + se.Message;
                try
                {
                    if (tran != null)
                        tran.Rollback();
                }
                catch { ;}
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
        /// 叫号8002 8002|操作员编号|窗口号|
        /// </summary>
        /// <param name="pIn"></param>
        /// <param name="pOut"></param>
        public static void CallTicket(cParamInfo pIn, out cParamInfo pOut)
        {
            pOut = new cParamInfo();
            SqlConnection con = new SqlConnection(SessionInfo.strConn);
            SqlTransaction tran = null;
            try
            {
                con.Open();
                tran = con.BeginTransaction();
                SqlCommand cmd = new SqlCommand("spCallerTicket", con, tran);
                cmd.CommandType = CommandType.StoredProcedure;
                 //@EmpNo 		varchar(20),	--操作员
                 //@FormNo        varchar(10), --登录窗口
                 //@ParamInfo     varchar(256),
                 //@OutInfo       varchar(256) output,
                 //@TicketNo      varchar(10) output, --票号
                 //@BusiNo        varchar(10) output,--业务编号
                 //@BusiName      varchar(60) output,--业务名称
                 //@RetCode		int output,		  
                 //@RetText		varchar(120) output 
                cmd.Parameters.Add("@EmpNo", SqlDbType.VarChar, 20);
                cmd.Parameters["@EmpNo"].Value = pIn.strEmpNo;

                cmd.Parameters.Add("@FormNo", SqlDbType.VarChar, 10);
                cmd.Parameters["@FormNo"].Value = pIn.strFormNo;

                cmd.Parameters.Add("@ParamInfo", SqlDbType.VarChar, 256);
                cmd.Parameters["@ParamInfo"].Value = pIn.strParamInfo;

                cmd.Parameters.Add("@OutInfo", SqlDbType.VarChar, 256);
                cmd.Parameters["@OutInfo"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@TicketNo", SqlDbType.VarChar, 10);
                cmd.Parameters["@TicketNo"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@BusiNo", SqlDbType.VarChar, 10);
                cmd.Parameters["@BusiNo"].Direction = ParameterDirection.Output; 

                cmd.Parameters.Add("@BusiName", SqlDbType.VarChar, 60);
                cmd.Parameters["@BusiName"].Direction = ParameterDirection.Output; 

                cmd.Parameters.Add("@RetCode", SqlDbType.Int);
                cmd.Parameters["@RetCode"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@RetText", SqlDbType.VarChar, 120);
                cmd.Parameters["@RetText"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                pOut.nRetCode = Convert.ToInt32(cmd.Parameters["@RetCode"].Value);
                pOut.strRetText = cmd.Parameters["@RetText"].Value.ToString();

                if (pOut.nRetCode == 0)
                {
                    pOut.strTicketNo = cmd.Parameters["@TicketNo"].Value.ToString();
                    pOut.strBusiNo = cmd.Parameters["@BusiNo"].Value.ToString();
                    pOut.strBusiName = cmd.Parameters["@BusiName"].Value.ToString();

                }
                if (pOut.nRetCode == 0)
                    tran.Commit();
                else
                    tran.Rollback(); 
            }
            catch (Exception se)
            {
                pOut.nRetCode = 99;
                pOut.strRetText = "系统忙,请稍后再试:" + se.Message;
                try
                {
                    if (tran != null)
                        tran.Rollback();
                }
                catch { ;}
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
        /// 重复叫号8012 8012|操作员编号|窗口号|呼叫号码|
        /// </summary>
        /// <param name="pIn"></param>
        /// <param name="pOut"></param>
        public static void CallTicketRepeat(cParamInfo pIn, out cParamInfo pOut)
        {
            pOut = new cParamInfo();
            SqlConnection con = new SqlConnection(SessionInfo.strConn);
            SqlTransaction tran = null;
            try
            {
                con.Open();
                tran = con.BeginTransaction();
                SqlCommand cmd = new SqlCommand("spCallerTicketRepeat", con, tran);
                cmd.CommandType = CommandType.StoredProcedure;
                //@EmpNo 		varchar(20),--操作员
                // @FormNo        varchar(10), --登录窗口
                // @TicketNo      varchar(10), --票号
                // @ParamInfo     varchar(256),
                // @OutInfo       varchar(256) output,
                // @BusiNo        varchar(10) output,--业务编号
                // @BusiName      varchar(60) output,--业务名称
                // @RetCode		int output,		  
                // @RetText		varchar(120) output 
                cmd.Parameters.Add("@EmpNo", SqlDbType.VarChar, 20);
                cmd.Parameters["@EmpNo"].Value = pIn.strEmpNo;

                cmd.Parameters.Add("@FormNo", SqlDbType.VarChar, 10);
                cmd.Parameters["@FormNo"].Value = pIn.strFormNo;

                cmd.Parameters.Add("@TicketNo", SqlDbType.VarChar, 10);
                cmd.Parameters["@TicketNo"].Value = pIn.strTicketNo;

                cmd.Parameters.Add("@ParamInfo", SqlDbType.VarChar, 256);
                cmd.Parameters["@ParamInfo"].Value = pIn.strParamInfo;

                cmd.Parameters.Add("@OutInfo", SqlDbType.VarChar, 256);
                cmd.Parameters["@OutInfo"].Direction = ParameterDirection.Output; 

                cmd.Parameters.Add("@BusiNo", SqlDbType.VarChar, 10);
                cmd.Parameters["@BusiNo"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@BusiName", SqlDbType.VarChar, 60);
                cmd.Parameters["@BusiName"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@RetCode", SqlDbType.Int);
                cmd.Parameters["@RetCode"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@RetText", SqlDbType.VarChar, 120);
                cmd.Parameters["@RetText"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                pOut.nRetCode = Convert.ToInt32(cmd.Parameters["@RetCode"].Value);
                pOut.strRetText = cmd.Parameters["@RetText"].Value.ToString();

                if (pOut.nRetCode == 0)
                {
                    pOut.strTicketNo = pIn.strTicketNo;
                    pOut.strBusiNo = cmd.Parameters["@BusiNo"].Value.ToString();
                    pOut.strBusiName = cmd.Parameters["@BusiName"].Value.ToString();

                }
                if (pOut.nRetCode == 0)
                    tran.Commit();
                else
                    tran.Rollback();
            }
            catch (Exception se)
            {
                pOut.nRetCode = 99;
                pOut.strRetText = "系统忙,请稍后再试:" + se.Message;
                try
                {
                    if (tran != null)
                        tran.Rollback();
                }
                catch { ;}
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
        /// 开始业务8003 8003|操作员编号|窗口号|业务编号|呼叫号码|
        /// </summary>
        /// <param name="pIn"></param>
        /// <param name="pOut"></param>
        public static void StartBusi(cParamInfo pIn, out cParamInfo pOut)
        {
            pOut = new cParamInfo();
            SqlConnection con = new SqlConnection(SessionInfo.strConn);
            SqlTransaction tran = null;
            try
            {
                con.Open();
                tran = con.BeginTransaction();
                SqlCommand cmd = new SqlCommand("spStartBusi", con, tran);
                cmd.CommandType = CommandType.StoredProcedure;
                //@EmpNo 		varchar(20), --操作员
                // @FormNo        varchar(10), --登录窗口
                // @TicketNo      varchar(10), --票号
                // @BusiNo        varchar(10), --业务编号
                // @ParamInfo     varchar(256),
                // @OutInfo       varchar(256) output,
                // @RetCode		int output,		  
                // @RetText		varchar(120) output 
                cmd.Parameters.Add("@EmpNo", SqlDbType.VarChar, 20);
                cmd.Parameters["@EmpNo"].Value = pIn.strEmpNo;

                cmd.Parameters.Add("@FormNo", SqlDbType.VarChar, 10);
                cmd.Parameters["@FormNo"].Value = pIn.strFormNo;

                cmd.Parameters.Add("@TicketNo", SqlDbType.VarChar, 10);
                cmd.Parameters["@TicketNo"].Value = pIn.strTicketNo;

                cmd.Parameters.Add("@BusiNo", SqlDbType.VarChar, 10);
                cmd.Parameters["@BusiNo"].Value = pIn.strBusiNo;

                cmd.Parameters.Add("@ParamInfo", SqlDbType.VarChar, 256);
                cmd.Parameters["@ParamInfo"].Value = pIn.strParamInfo;

                cmd.Parameters.Add("@OutInfo", SqlDbType.VarChar, 256);
                cmd.Parameters["@OutInfo"].Direction = ParameterDirection.Output; 

                cmd.Parameters.Add("@RetCode", SqlDbType.Int);
                cmd.Parameters["@RetCode"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@RetText", SqlDbType.VarChar, 120);
                cmd.Parameters["@RetText"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                pOut.nRetCode = Convert.ToInt32(cmd.Parameters["@RetCode"].Value);
                pOut.strRetText = cmd.Parameters["@RetText"].Value.ToString();

                if (pOut.nRetCode == 0)
                    tran.Commit();
                else
                    tran.Rollback();
            }
            catch (Exception se)
            {
                pOut.nRetCode = 99;
                pOut.strRetText = "系统忙,请稍后再试:" + se.Message;
                try
                {
                    if (tran != null)
                        tran.Rollback();
                }
                catch { ;}
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
        /// 结束业务8004 8004|操作员编号|窗口号|业务编号|呼叫号码|
        /// </summary>
        /// <param name="pIn"></param>
        /// <param name="pOut"></param>
        public static void EndBusi(cParamInfo pIn, out cParamInfo pOut)
        {
            pOut = new cParamInfo();
            SqlConnection con = new SqlConnection(SessionInfo.strConn);
            SqlTransaction tran = null;
            try
            {
                con.Open();
                tran = con.BeginTransaction();
                SqlCommand cmd = new SqlCommand("spEndBusi", con, tran);
                cmd.CommandType = CommandType.StoredProcedure;
                //@EmpNo 		varchar(20), --操作员
                // @FormNo        varchar(10), --登录窗口
                // @TicketNo      varchar(10), --票号
                // @BusiNo        varchar(10), --业务编号
                // @ParamInfo     varchar(256),
                // @OutInfo       varchar(256) output,
                // @RetCode		int output,		  
                // @RetText		varchar(120) output 
                cmd.Parameters.Add("@EmpNo", SqlDbType.VarChar, 20);
                cmd.Parameters["@EmpNo"].Value = pIn.strEmpNo;

                cmd.Parameters.Add("@FormNo", SqlDbType.VarChar, 10);
                cmd.Parameters["@FormNo"].Value = pIn.strFormNo;

                cmd.Parameters.Add("@TicketNo", SqlDbType.VarChar, 10);
                cmd.Parameters["@TicketNo"].Value = pIn.strTicketNo;

                cmd.Parameters.Add("@BusiNo", SqlDbType.VarChar, 10);
                cmd.Parameters["@BusiNo"].Value = pIn.strBusiNo;

                cmd.Parameters.Add("@ParamInfo", SqlDbType.VarChar, 256);
                cmd.Parameters["@ParamInfo"].Value = pIn.strParamInfo;

                cmd.Parameters.Add("@OutInfo", SqlDbType.VarChar, 256);
                cmd.Parameters["@OutInfo"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@RetCode", SqlDbType.Int);
                cmd.Parameters["@RetCode"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@RetText", SqlDbType.VarChar, 120);
                cmd.Parameters["@RetText"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                pOut.nRetCode = Convert.ToInt32(cmd.Parameters["@RetCode"].Value);
                pOut.strRetText = cmd.Parameters["@RetText"].Value.ToString();

                if (pOut.nRetCode == 0)
                    tran.Commit();
                else
                    tran.Rollback();
            }
            catch (Exception se)
            {
                pOut.nRetCode = 99;
                pOut.strRetText = "系统忙,请稍后再试:" + se.Message;
                try
                {
                    if (tran != null)
                        tran.Rollback();
                }
                catch { ;}
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
        /// 评价8005 8005|操作员编号|窗口号|业务编号|号码|评价分数|
        /// </summary>
        /// <param name="pIn"></param>
        /// <param name="pOut"></param>
        public static void Appraise(cParamInfo pIn, out cParamInfo pOut)
        {
            pOut = new cParamInfo();
            SqlConnection con = new SqlConnection(SessionInfo.strConn);
            SqlTransaction tran = null;
            try
            {
                con.Open();
                tran = con.BeginTransaction();
                SqlCommand cmd = new SqlCommand("spApprise", con, tran);
                cmd.CommandType = CommandType.StoredProcedure;
                //@EmpNo 		varchar(20),--操作员
                //@FormNo        varchar(10),--登录窗口
                //@TicketNo      varchar(10),--票号
                //@BusiNo        varchar(10),--业务编号
                //@Score         int, --评价分数
                //@ParamInfo     varchar(256),
                //@OutInfo       varchar(256) output,
                //@RetCode		int output,		  
                //@RetText		varchar(120) output 
                cmd.Parameters.Add("@EmpNo", SqlDbType.VarChar, 20);
                cmd.Parameters["@EmpNo"].Value = pIn.strEmpNo;

                cmd.Parameters.Add("@FormNo", SqlDbType.VarChar, 10);
                cmd.Parameters["@FormNo"].Value = pIn.strFormNo;

                cmd.Parameters.Add("@TicketNo", SqlDbType.VarChar, 10);
                cmd.Parameters["@TicketNo"].Value = pIn.strTicketNo;

                cmd.Parameters.Add("@BusiNo", SqlDbType.VarChar, 10);
                cmd.Parameters["@BusiNo"].Value = pIn.strBusiNo;

                cmd.Parameters.Add("@Score", SqlDbType.Int);
                cmd.Parameters["@Score"].Value = pIn.nScore;

                cmd.Parameters.Add("@ParamInfo", SqlDbType.VarChar, 256);
                cmd.Parameters["@ParamInfo"].Value = pIn.strParamInfo;

                cmd.Parameters.Add("@OutInfo", SqlDbType.VarChar, 256);
                cmd.Parameters["@OutInfo"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@RetCode", SqlDbType.Int);
                cmd.Parameters["@RetCode"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@RetText", SqlDbType.VarChar, 120);
                cmd.Parameters["@RetText"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                pOut.nRetCode = Convert.ToInt32(cmd.Parameters["@RetCode"].Value);
                pOut.strRetText = cmd.Parameters["@RetText"].Value.ToString();

                if (pOut.nRetCode == 0)
                    tran.Commit();
                else
                    tran.Rollback();
            }
            catch (Exception se)
            {
                pOut.nRetCode = 99;
                pOut.strRetText = "系统忙,请稍后再试:" + se.Message;
                try
                {
                    if (tran != null)
                        tran.Rollback();
                }
                catch { ;}
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
        /// 获取等待人数8013 8013|操作员编号|窗口号|
        /// </summary>
        /// <param name="pIn"></param>
        /// <param name="pOut"></param>
        public static void GetFormWaitNum(cParamInfo pIn, out cParamInfo pOut)
        {
            pOut = new cParamInfo();
            SqlConnection con = new SqlConnection(SessionInfo.strConn);
            SqlTransaction tran = null;
            try
            {
                con.Open();
                tran = con.BeginTransaction();
                SqlCommand cmd = new SqlCommand("spGetFormWait", con, tran);
                cmd.CommandType = CommandType.StoredProcedure;
                //@EmpNo 		varchar(20), --操作员
                //@FormNo        varchar(10), --登录窗口     
                //@ParamInfo     varchar(256),
                //@OutInfo       varchar(256) output,
                //@WaitNum       int  output,
                //@RetCode		int output,		  
                //@RetText		varchar(120) output 
                cmd.Parameters.Add("@EmpNo", SqlDbType.VarChar, 20);
                cmd.Parameters["@EmpNo"].Value = pIn.strEmpNo;

                cmd.Parameters.Add("@FormNo", SqlDbType.VarChar, 10);
                cmd.Parameters["@FormNo"].Value = pIn.strFormNo; 

                cmd.Parameters.Add("@ParamInfo", SqlDbType.VarChar, 256);
                cmd.Parameters["@ParamInfo"].Value = pIn.strParamInfo;

                cmd.Parameters.Add("@OutInfo", SqlDbType.VarChar, 256);
                cmd.Parameters["@OutInfo"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@WaitNum", SqlDbType.Int);
                cmd.Parameters["@WaitNum"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@RetCode", SqlDbType.Int);
                cmd.Parameters["@RetCode"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@RetText", SqlDbType.VarChar, 120);
                cmd.Parameters["@RetText"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                pOut.nRetCode = Convert.ToInt32(cmd.Parameters["@RetCode"].Value);
                pOut.strRetText = cmd.Parameters["@RetText"].Value.ToString();
                if (pOut.nRetCode == 0)
                {
                    pOut.nWaitNum = Convert.ToInt32(cmd.Parameters["@WaitNum"].Value);

                }
                if (pOut.nRetCode == 0)
                    tran.Commit();
                else
                    tran.Rollback();
            }
            catch (Exception se)
            {
                pOut.nRetCode = 99;
                pOut.strRetText = "系统忙,请稍后再试:" + se.Message;
                try
                {
                    if (tran != null)
                        tran.Rollback();
                }
                catch { ;}
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
        /// 弃号8014 8014|操作员编号|窗口号|业务编号|呼叫号码|
        /// </summary>
        /// <param name="pIn"></param>
        /// <param name="pOut"></param>
        public static void AbortBusi(cParamInfo pIn, out cParamInfo pOut)
        {
            pOut = new cParamInfo();
            SqlConnection con = new SqlConnection(SessionInfo.strConn);
            SqlTransaction tran = null;
            try
            {
                con.Open();
                tran = con.BeginTransaction();
                SqlCommand cmd = new SqlCommand("spAbortBusi", con, tran);
                cmd.CommandType = CommandType.StoredProcedure;
                //@EmpNo 		varchar(20),	--操作员
                //@FormNo        varchar(10), --登录窗口
                //@TicketNo      varchar(10), --票号
                //@BusiNo        varchar(10),--业务编号
                //@ParamInfo     varchar(256),
                //@OutInfo       varchar(256) output,
                //@RetCode		int output,		  
                //@RetText		varchar(120) output 
                cmd.Parameters.Add("@EmpNo", SqlDbType.VarChar, 20);
                cmd.Parameters["@EmpNo"].Value = pIn.strEmpNo;

                cmd.Parameters.Add("@FormNo", SqlDbType.VarChar, 10);
                cmd.Parameters["@FormNo"].Value = pIn.strFormNo;

                cmd.Parameters.Add("@TicketNo", SqlDbType.VarChar, 10);
                cmd.Parameters["@TicketNo"].Value = pIn.strTicketNo;

                cmd.Parameters.Add("@BusiNo", SqlDbType.VarChar, 10);
                cmd.Parameters["@BusiNo"].Value = pIn.strBusiNo;

                cmd.Parameters.Add("@ParamInfo", SqlDbType.VarChar, 256);
                cmd.Parameters["@ParamInfo"].Value = pIn.strParamInfo;

                cmd.Parameters.Add("@OutInfo", SqlDbType.VarChar, 256);
                cmd.Parameters["@OutInfo"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@RetCode", SqlDbType.Int);
                cmd.Parameters["@RetCode"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@RetText", SqlDbType.VarChar, 120);
                cmd.Parameters["@RetText"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                pOut.nRetCode = Convert.ToInt32(cmd.Parameters["@RetCode"].Value);
                pOut.strRetText = cmd.Parameters["@RetText"].Value.ToString();

                if (pOut.nRetCode == 0)
                    tran.Commit();
                else
                    tran.Rollback();
            }
            catch (Exception se)
            {
                pOut.nRetCode = 99;
                pOut.strRetText = "系统忙,请稍后再试:" + se.Message;
                try
                {
                    if (tran != null)
                        tran.Rollback();
                }
                catch { ;}
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
        /// 业务转移8015 8015|操作员编号|窗口号|业务编号|呼叫号码|新业务编号|
        /// </summary>
        /// <param name="pIn"></param>
        /// <param name="pOut"></param>
        public static void TransferBusi(cParamInfo pIn, out cParamInfo pOut)
        {
            pOut = new cParamInfo();
            SqlConnection con = new SqlConnection(SessionInfo.strConn);
            SqlTransaction tran = null;
            try
            {
                con.Open();
                tran = con.BeginTransaction();
                SqlCommand cmd = new SqlCommand("spTransferBusi", con, tran);
                cmd.CommandType = CommandType.StoredProcedure;
                //@EmpNo 		varchar(20),	--操作员
                //@FormNo        varchar(10), --登录窗口
                //@TicketNo      varchar(10), --票号
                //@BusiNo        varchar(10),--业务编号
                //@NewBusiNo     varchar(10),
                //@ParamInfo     varchar(256),
                //@OutInfo       varchar(256) output,
                //@RetCode		int output,		  
                //@RetText		varchar(120) output 
                cmd.Parameters.Add("@EmpNo", SqlDbType.VarChar, 20);
                cmd.Parameters["@EmpNo"].Value = pIn.strEmpNo;

                cmd.Parameters.Add("@FormNo", SqlDbType.VarChar, 10);
                cmd.Parameters["@FormNo"].Value = pIn.strFormNo;

                cmd.Parameters.Add("@TicketNo", SqlDbType.VarChar, 10);
                cmd.Parameters["@TicketNo"].Value = pIn.strTicketNo;

                cmd.Parameters.Add("@BusiNo", SqlDbType.VarChar, 10);
                cmd.Parameters["@BusiNo"].Value = pIn.strBusiNo;

                cmd.Parameters.Add("@NewBusiNo", SqlDbType.VarChar, 10);
                cmd.Parameters["@NewBusiNo"].Value = pIn.strNewBusiNo;

                cmd.Parameters.Add("@ParamInfo", SqlDbType.VarChar, 256);
                cmd.Parameters["@ParamInfo"].Value = pIn.strParamInfo;

                cmd.Parameters.Add("@OutInfo", SqlDbType.VarChar, 256);
                cmd.Parameters["@OutInfo"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@RetCode", SqlDbType.Int);
                cmd.Parameters["@RetCode"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@RetText", SqlDbType.VarChar, 120);
                cmd.Parameters["@RetText"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                pOut.nRetCode = Convert.ToInt32(cmd.Parameters["@RetCode"].Value);
                pOut.strRetText = cmd.Parameters["@RetText"].Value.ToString();

                if (pOut.nRetCode == 0)
                    tran.Commit();
                else
                    tran.Rollback();
            }
            catch (Exception se)
            {
                pOut.nRetCode = 99;
                pOut.strRetText = "系统忙,请稍后再试:" + se.Message;
                try
                {
                    if (tran != null)
                        tran.Rollback();
                }
                catch { ;}
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
        /// 暂停业务8010 8010|操作员编号|窗口号|
        /// </summary>
        /// <param name="pIn"></param>
        /// <param name="pOut"></param>
        public static void PauseBusi(cParamInfo pIn, out cParamInfo pOut)
        {
            pOut = new cParamInfo();
            SqlConnection con = new SqlConnection(SessionInfo.strConn);
            SqlTransaction tran = null;
            try
            {
                con.Open();
                tran = con.BeginTransaction();
                SqlCommand cmd = new SqlCommand("spPauseBusi", con, tran);
                cmd.CommandType = CommandType.StoredProcedure;
                //@EmpNo 		varchar(20),	--操作员
                //@FormNo        varchar(10), --登录窗口 
                //@ParamInfo     varchar(256),
                //@OutInfo       varchar(256) output,
                //@RetCode		int output,		  
                //@RetText		varchar(120) output 
                cmd.Parameters.Add("@EmpNo", SqlDbType.VarChar, 20);
                cmd.Parameters["@EmpNo"].Value = pIn.strEmpNo;

                cmd.Parameters.Add("@FormNo", SqlDbType.VarChar, 10);
                cmd.Parameters["@FormNo"].Value = pIn.strFormNo;

                cmd.Parameters.Add("@ParamInfo", SqlDbType.VarChar, 256);
                cmd.Parameters["@ParamInfo"].Value = pIn.strParamInfo;

                cmd.Parameters.Add("@OutInfo", SqlDbType.VarChar, 256);
                cmd.Parameters["@OutInfo"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@RetCode", SqlDbType.Int);
                cmd.Parameters["@RetCode"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@RetText", SqlDbType.VarChar, 120);
                cmd.Parameters["@RetText"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                pOut.nRetCode = Convert.ToInt32(cmd.Parameters["@RetCode"].Value);
                pOut.strRetText = cmd.Parameters["@RetText"].Value.ToString();

                if (pOut.nRetCode == 0)
                    tran.Commit();
                else
                    tran.Rollback();
            }
            catch (Exception se)
            {
                pOut.nRetCode = 99;
                pOut.strRetText = "系统忙,请稍后再试:" + se.Message;
                try
                {
                    if (tran != null)
                        tran.Rollback();
                }
                catch { ;}
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
        /// 恢复业务8011 8011|操作员编号|窗口号|
        /// </summary>
        /// <param name="pIn"></param>
        /// <param name="pOut"></param>
        public static void ResumeBusi(cParamInfo pIn, out cParamInfo pOut)
        {
            pOut = new cParamInfo();
            SqlConnection con = new SqlConnection(SessionInfo.strConn);
            SqlTransaction tran = null;
            try
            {
                con.Open();
                tran = con.BeginTransaction();
                SqlCommand cmd = new SqlCommand("spResumeBusi", con, tran);
                cmd.CommandType = CommandType.StoredProcedure;
                //@EmpNo 		varchar(20),	--操作员
                //@FormNo        varchar(10), --登录窗口 
                //@ParamInfo     varchar(256),
                //@OutInfo       varchar(256) output,
                //@RetCode		int output,		  
                //@RetText		varchar(120) output 
                cmd.Parameters.Add("@EmpNo", SqlDbType.VarChar, 20);
                cmd.Parameters["@EmpNo"].Value = pIn.strEmpNo;

                cmd.Parameters.Add("@FormNo", SqlDbType.VarChar, 10);
                cmd.Parameters["@FormNo"].Value = pIn.strFormNo;

                cmd.Parameters.Add("@ParamInfo", SqlDbType.VarChar, 256);
                cmd.Parameters["@ParamInfo"].Value = pIn.strParamInfo;

                cmd.Parameters.Add("@OutInfo", SqlDbType.VarChar, 256);
                cmd.Parameters["@OutInfo"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@RetCode", SqlDbType.Int);
                cmd.Parameters["@RetCode"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@RetText", SqlDbType.VarChar, 120);
                cmd.Parameters["@RetText"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                pOut.nRetCode = Convert.ToInt32(cmd.Parameters["@RetCode"].Value);
                pOut.strRetText = cmd.Parameters["@RetText"].Value.ToString();

                if (pOut.nRetCode == 0)
                    tran.Commit();
                else
                    tran.Rollback();
            }
            catch (Exception se)
            {
                pOut.nRetCode = 99;
                pOut.strRetText = "系统忙,请稍后再试:" + se.Message;
                try
                {
                    if (tran != null)
                        tran.Rollback();
                }
                catch { ;}
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
