using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
namespace QManage
{
    
    public static class BusiRole
    {
        public static bool RoleEdit(cBusiInfo pIn, cBusiInfo pOut)
        {
            SqlConnection con = new SqlConnection(SessionInfo.strConn);
            SqlTransaction tran = null;
            try
            {
                con.Open();
                tran = con.BeginTransaction();
                SqlCommand cmd = new SqlCommand("procEditRole", con,tran);
                cmd.CommandType = CommandType.StoredProcedure;
                //@OpNo 		varchar(20),	
                //@RoleNo       varchar(4),  
                //@RoleName       varchar(60),
                //@Right          varchar(800) ,
                //@Flag           int,   
                //@RetCode		int output,		  
                //@RetText		varchar(120) output
                string []tmp = pIn.strParamInfo.Split('|');
                cmd.Parameters.Add("@OpNo", SqlDbType.VarChar,20);
                cmd.Parameters["@OpNo"].Value = pIn.strOpCode;

                cmd.Parameters.Add("@RoleNo", SqlDbType.VarChar, 4);
                cmd.Parameters["@RoleNo"].Value = tmp[0];

                cmd.Parameters.Add("@RoleName", SqlDbType.VarChar, 60);
                cmd.Parameters["@RoleName"].Value = tmp[1];

                cmd.Parameters.Add("@Right", SqlDbType.VarChar,800);
                cmd.Parameters["@Right"].Value = pIn.strRight;

                cmd.Parameters.Add("@Flag", SqlDbType.Int);
                cmd.Parameters["@Flag"].Value = pIn.nFlag;

                cmd.Parameters.Add("@RetCode", SqlDbType.Int);
                cmd.Parameters["@RetCode"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@RetText", SqlDbType.VarChar,120);
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
        public static bool OpEdit(cBusiInfo pIn, cBusiInfo pOut)
        {
            SqlConnection con = new SqlConnection(SessionInfo.strConn);
            SqlTransaction tran = null;
            try
            {
                con.Open();
                tran = con.BeginTransaction();
                SqlCommand cmd = new SqlCommand("procEditOp", con, tran);
                cmd.CommandType = CommandType.StoredProcedure;
                //@OpNo 		varchar(20),
	            //@OpCode      varchar(20),
                //@OpName     varchar(60),
                //@OpPwd      varchar(32),
                //@RoleNo       varchar(4),  
                //@OpLevel       int,
                //@Status         int,
                //@Flag           int,   
                //@RetCode		int output,		  
                //@RetText		varchar(120) output	
                string[] tmp = pIn.strParamInfo.Split('|');
                cmd.Parameters.Add("@OpNo", SqlDbType.VarChar, 20);
                cmd.Parameters["@OpNo"].Value = pIn.strOpCode;

                cmd.Parameters.Add("@OpCode", SqlDbType.VarChar, 20);
                cmd.Parameters["@OpCode"].Value = tmp[0];

                cmd.Parameters.Add("@OpName", SqlDbType.VarChar, 60);
                cmd.Parameters["@OpName"].Value = tmp[1];

                cmd.Parameters.Add("@OpPwd", SqlDbType.VarChar, 32);
                cmd.Parameters["@OpPwd"].Value = tmp[2];

                cmd.Parameters.Add("@RoleNo", SqlDbType.VarChar, 4);
                cmd.Parameters["@RoleNo"].Value = tmp[3];

                cmd.Parameters.Add("@OpLevel", SqlDbType.Int);
                cmd.Parameters["@OpLevel"].Value = tmp[4];

                cmd.Parameters.Add("@Status", SqlDbType.Int);
                cmd.Parameters["@Status"].Value = tmp[5];

                cmd.Parameters.Add("@Flag", SqlDbType.Int);
                cmd.Parameters["@Flag"].Value = pIn.nFlag;

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
    }
}