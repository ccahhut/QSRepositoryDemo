using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
namespace QManage
{
    
    public static class BusiOp
    {   
        public static bool Logon(cOP pIn, cOP pOut)
        {
            pOut.nRetCode = 99;
            pOut.strRetText = "系统忙,请稍后再试";
            SqlConnection con = new SqlConnection(SessionInfo.strConn);
            SqlTransaction tran = null;
            try
            {
                con.Open();
                tran = con.BeginTransaction();
                SqlCommand cmd = new SqlCommand("procLogin", con, tran);
                cmd.CommandType = CommandType.StoredProcedure;
                // @OpNo 		varchar(20),	
                //@OpPwd       varchar(32),      
                //@OpName		varchar(60) output,   
                //@OpLevel		int output,  
                //@LastLoginTime  varchar(30) output, 
                //@RetCode		int output,		  
                //@RetText		varchar(120) output	
                //@RemoteDB      int output
                cmd.Parameters.Add("@OpNo", SqlDbType.VarChar,20);
                cmd.Parameters["@OpNo"].Value = pIn.strOpCode;

                cmd.Parameters.Add("@OpPwd", SqlDbType.VarChar,32);
                cmd.Parameters["@OpPwd"].Value = pIn.strOpPwd;

                cmd.Parameters.Add("@OpName", SqlDbType.VarChar,60);
                cmd.Parameters["@OpName"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@OpLevel", SqlDbType.Int);
                cmd.Parameters["@OpLevel"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@LastLoginTime", SqlDbType.VarChar,30);
                cmd.Parameters["@LastLoginTime"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@RetCode", SqlDbType.Int);
                cmd.Parameters["@RetCode"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@RetText", SqlDbType.VarChar,120);
                cmd.Parameters["@RetText"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@RemoteDB", SqlDbType.Int);
                cmd.Parameters["@RemoteDB"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                pOut.nRetCode = Convert.ToInt32(cmd.Parameters["@RetCode"].Value);
                pOut.strRetText = cmd.Parameters["@RetText"].Value.ToString();

                if (pOut.nRetCode == 0)
                {
                    pOut.strOpName = cmd.Parameters["@OpName"].Value.ToString();
                    pOut.strOutInfo = cmd.Parameters["@OpLevel"].Value.ToString();
                    pOut.strExtend1 = cmd.Parameters["@LastLoginTime"].Value.ToString();
                    pOut.strExtend2 = cmd.Parameters["@OpLevel"].Value.ToString();

                }
                if (pOut.nRetCode == 0)
                    tran.Commit();
                else
                    tran.Rollback();
                con.Close();

                return true;
            }
            catch (Exception se)
            {
                pOut.strRetText = "系统忙,请稍后再试:" + se.Message;
                try
                { 
                    if (tran != null)
                        tran.Rollback();
                }
                catch { ;}
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
        public static bool ModiOpPwd(cOP pIn, cOP pOut)
        {
            SqlConnection con = new SqlConnection(SessionInfo.strConn);
            SqlTransaction tran = null;
            try
            {
                con.Open();
                tran = con.BeginTransaction();
                SqlCommand cmd = new SqlCommand("procModiOpPwd", con, tran);
                cmd.CommandType = CommandType.StoredProcedure;
                //@OpNo 		varchar(20),	
                //@OpPwd       varchar(32),  
                //@OpNewPwd       varchar(32),    
                //@RetCode		int output,		  
                //@RetText		varchar(120) output	
                cmd.Parameters.Add("@OpNo", SqlDbType.VarChar, 20);
                cmd.Parameters["@OpNo"].Value = pIn.strOpCode;

                cmd.Parameters.Add("@OpPwd", SqlDbType.VarChar, 32);
                cmd.Parameters["@OpPwd"].Value = pIn.strOpPwd;

                cmd.Parameters.Add("@OpNewPwd", SqlDbType.VarChar, 32);
                cmd.Parameters["@OpNewPwd"].Value = pIn.strOpNewPwd;

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
                con.Close();

                return true;
            }
            catch (Exception se)
            {
                pOut.strRetText = "系统忙,请稍后再试:" + se.Message;
                try
                { 
                    if (tran != null)
                        tran.Rollback();
                }
                catch { ;}
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
        public static bool OpLogOut(cOP pIn, cOP pOut)
        {
            
            SqlConnection con = new SqlConnection(SessionInfo.strConn);
            SqlTransaction tran = null;
            try
            {
                con.Open();
                tran = con.BeginTransaction();
                SqlCommand cmd = new SqlCommand("procWriteOpLog", con, tran);
                cmd.CommandType = CommandType.StoredProcedure;
                //@OpNo 		varchar(20),
                //@TrCode  varchar(4),	
                //@Content varchar(1024) 
                cmd.Parameters.Add("@OpNo", SqlDbType.VarChar, 20);
                cmd.Parameters["@OpNo"].Value = pIn.strOpCode;

                cmd.Parameters.Add("@TrCode", SqlDbType.VarChar, 4);
                cmd.Parameters["@TrCode"].Value = pIn.strTrcode;

                cmd.Parameters.Add("@Content", SqlDbType.VarChar, 1024);
                cmd.Parameters["@Content"].Value = "操作员:" + pIn.strOpCode;

                cmd.ExecuteNonQuery();

                if (pOut.nRetCode == 0)
                    tran.Commit();
                else
                    tran.Rollback();
                con.Close();

                return true;
            }
            catch (Exception se)
            {
                pOut.strRetText = "系统忙,请稍后再试:" + se.Message;
                try
                { 
                    if (tran != null)
                        tran.Rollback();
                }
                catch { ;}
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
    }
}