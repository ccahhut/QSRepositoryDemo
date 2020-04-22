using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
namespace QManage
{
    
    public static class BusiInfo
    {
        /// <summary>
        /// 通用过程调用
        /// </summary>
        /// <param name="pIn"></param>
        /// <param name="pOut"></param>
        /// <returns></returns>
        public static bool CommonEdit(cBusiInfo pIn, cBusiInfo pOut,string strProcName)
        {
            SqlConnection con = new SqlConnection(SessionInfo.strConn);
            SqlTransaction tran = null;
            try
            {
                con.Open();
                tran = con.BeginTransaction();
                SqlCommand cmd = new SqlCommand(strProcName, con, tran);
                cmd.CommandType = CommandType.StoredProcedure;
                //@OpNo 		varchar(20),	
                //@ParamsInfo    varchar(8000),
                //@Flag           int,   
                //@RetCode		int output,		  
                //@RetText		varchar(120) output	
                cmd.Parameters.Add("@OpNo", SqlDbType.VarChar, 20);
                cmd.Parameters["@OpNo"].Value = pIn.strOpCode;

                cmd.Parameters.Add("@ParamsInfo", SqlDbType.VarChar, 8000);
                cmd.Parameters["@ParamsInfo"].Value = pIn.strParamInfo;

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
        public static bool BusiEdit(cBusiInfo pIn, cBusiInfo pOut)
        {
            SqlConnection con = new SqlConnection(SessionInfo.strConn);
            SqlTransaction tran = null;
            try
            {
                con.Open();
                tran = con.BeginTransaction();
                SqlCommand cmd = new SqlCommand("procEditBusi", con, tran);
                cmd.CommandType = CommandType.StoredProcedure;
                //@OpNo 		varchar(20),	
                //@ParamsInfo    varchar(4000), 
                //@strExtend1    varchar(512),
                //@Flag           int,   
                //@RetCode		int output,		  
                //@RetText		varchar(120) output
                cmd.Parameters.Add("@OpNo", SqlDbType.VarChar, 20);
                cmd.Parameters["@OpNo"].Value = pIn.strOpCode;

                cmd.Parameters.Add("@ParamsInfo", SqlDbType.VarChar, 4000);
                cmd.Parameters["@ParamsInfo"].Value = pIn.strParamInfo;

                cmd.Parameters.Add("@strExtend1", SqlDbType.VarChar,512);
                cmd.Parameters["@strExtend1"].Value = pIn.strExtend1;

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
        public static bool FormEdit(cBusiInfo pIn, cBusiInfo pOut)
        {
            SqlConnection con = new SqlConnection(SessionInfo.strConn);
            SqlTransaction tran = null;
            try
            {
                con.Open();
                tran = con.BeginTransaction();
                SqlCommand cmd = new SqlCommand("procEditForm", con, tran);
                cmd.CommandType = CommandType.StoredProcedure;
                //@OpNo 		varchar(20),	
                //@ParamsInfo    varchar(4000), 
                //@strExtend1    varchar(512),
                //@Flag           int,   
                //@RetCode		int output,		  
                //@RetText		varchar(120) output
                cmd.Parameters.Add("@OpNo", SqlDbType.VarChar, 20);
                cmd.Parameters["@OpNo"].Value = pIn.strOpCode;

                cmd.Parameters.Add("@ParamsInfo", SqlDbType.VarChar, 4000);
                cmd.Parameters["@ParamsInfo"].Value = pIn.strParamInfo;

                cmd.Parameters.Add("@strExtend1", SqlDbType.VarChar, 512);
                cmd.Parameters["@strExtend1"].Value = pIn.strExtend1;

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
        public static bool DeptEdit(cBusiInfo pIn, cBusiInfo pOut)
        {
            SqlConnection con = new SqlConnection(SessionInfo.strConn);
            SqlTransaction tran = null;
            try
            {
                con.Open();
                tran = con.BeginTransaction();
                SqlCommand cmd = new SqlCommand("procEditDept", con, tran);
                cmd.CommandType = CommandType.StoredProcedure;
                //@OpNo 		varchar(20),	
                //@DeptNo       varchar(4),  
                //@DeptName       varchar(60), 
                //@Flag           int,   
                //@RetCode		int output,		  
                //@RetText		varchar(120) output	
                string []tmp = pIn.strParamInfo.Split('|');
                cmd.Parameters.Add("@OpNo", SqlDbType.VarChar,20);
                cmd.Parameters["@OpNo"].Value = pIn.strOpCode;

                cmd.Parameters.Add("@DeptNo", SqlDbType.VarChar, 4);
                cmd.Parameters["@DeptNo"].Value = tmp[0];

                cmd.Parameters.Add("@DeptName", SqlDbType.VarChar, 60);
                cmd.Parameters["@DeptName"].Value = tmp[1];

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

        public static bool EmpEdit(cBusiInfo pIn, cBusiInfo pOut)
        {
            SqlConnection con = new SqlConnection(SessionInfo.strConn);
            SqlTransaction tran = null;
            try
            {
                con.Open();
                tran = con.BeginTransaction();
                SqlCommand cmd = new SqlCommand("procEditEmp", con, tran);
                cmd.CommandType = CommandType.StoredProcedure;
                //@OpNo 		varchar(20),	
                //@ParamsInfo    varchar(4000),
                //@Photo          image,
                //@Flag           int,   
                //@RetCode		int output,		  
                //@RetText		varchar(120) output	 
                cmd.Parameters.Add("@OpNo", SqlDbType.VarChar, 20);
                cmd.Parameters["@OpNo"].Value = pIn.strOpCode;

                cmd.Parameters.Add("@ParamsInfo", SqlDbType.VarChar, 4000);
                cmd.Parameters["@ParamsInfo"].Value =pIn.strParamInfo;

                cmd.Parameters.Add("@Photo", SqlDbType.Image);
                cmd.Parameters["@Photo"].Value = pIn.bStream;

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
        public static bool AppraiseEdit(cBusiInfo pIn, cBusiInfo pOut)
        {
            SqlConnection con = new SqlConnection(SessionInfo.strConn);
            SqlTransaction tran = null;
            try
            {
                con.Open();
                tran = con.BeginTransaction();
                SqlCommand cmd = new SqlCommand("procEditAppraise", con, tran);
                cmd.CommandType = CommandType.StoredProcedure;
                //@OpNo 		varchar(20),	
                //@ParamsInfo    varchar(4000),
                //@Flag           int,   
                //@RetCode		int output,		  
                //@RetText		varchar(120) output	
                cmd.Parameters.Add("@OpNo", SqlDbType.VarChar, 20);
                cmd.Parameters["@OpNo"].Value = pIn.strOpCode;

                cmd.Parameters.Add("@ParamsInfo", SqlDbType.VarChar, 4000);
                cmd.Parameters["@ParamsInfo"].Value = pIn.strParamInfo; 

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

        public static bool PrintPageEdit(cBusiInfo pIn, cBusiInfo pOut)
        {
            SqlConnection con = new SqlConnection(SessionInfo.strConn);
            SqlTransaction tran = null;
            try
            { 
                con.Open();
                tran = con.BeginTransaction();
                SqlCommand cmd = new SqlCommand("procEditPrintPage", con, tran);
                cmd.CommandType = CommandType.StoredProcedure;
                //@OpNo 		varchar(20),	
                //@ParamsInfo    varchar(8000),
                //@Extend1        varchar(10),
                //@Extend2        varchar(10),  
                //@Flag           int,   
                //@RetCode		int output,		  
                //@RetText		varchar(120) output	
                cmd.Parameters.Add("@OpNo", SqlDbType.VarChar, 20);
                cmd.Parameters["@OpNo"].Value = pIn.strOpCode;

                cmd.Parameters.Add("@ParamsInfo", SqlDbType.VarChar, 8000);
                cmd.Parameters["@ParamsInfo"].Value = pIn.strParamInfo;

                cmd.Parameters.Add("@Extend1", SqlDbType.VarChar, 10);
                cmd.Parameters["@Extend1"].Value = pIn.strExtend1;

                cmd.Parameters.Add("@Extend2", SqlDbType.VarChar, 10);
                cmd.Parameters["@Extend2"].Value = pIn.strExtend2;

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
        public static bool PrintPagePhotoEdit(cBusiInfo pIn, cBusiInfo pOut)
        {
            SqlConnection con = new SqlConnection(SessionInfo.strConn);
            SqlTransaction tran = null;
            try
            {
                con.Open();
                tran = con.BeginTransaction();
                SqlCommand cmd = new SqlCommand("procEditPrintPagePhoto", con, tran);
                cmd.CommandType = CommandType.StoredProcedure;
                //@OpNo 		varchar(20),	
                //@ParamsInfo    varchar(8000),
                //@Photo
                //@Flag           int,   
                //@RetCode		int output,		  
                //@RetText		varchar(120) output	
                cmd.Parameters.Add("@OpNo", SqlDbType.VarChar, 20);
                cmd.Parameters["@OpNo"].Value = pIn.strOpCode;

                cmd.Parameters.Add("@ParamsInfo", SqlDbType.VarChar, 8000);
                cmd.Parameters["@ParamsInfo"].Value = pIn.strParamInfo;

                cmd.Parameters.Add("@Photo", SqlDbType.Image);
                cmd.Parameters["@Photo"].Value = pIn.bStream;

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
        public static bool TimeLimitEdit(cBusiInfo pIn, cBusiInfo pOut)
        {
            SqlConnection con = new SqlConnection(SessionInfo.strConn);
            SqlTransaction tran = null;
            try
            {
                con.Open();
                tran = con.BeginTransaction();
                SqlCommand cmd = new SqlCommand("procEditTimeLimit", con, tran);
                cmd.CommandType = CommandType.StoredProcedure;
                //@OpNo 		    varchar(20),	
                //@ParamsInfo    varchar(4000), 
                //@BusiNo         varchar(10),
                //@Flag           int,   
                //@RetCode		int output,		  
                //@RetText		varchar(120) output	
                cmd.Parameters.Add("@OpNo", SqlDbType.VarChar, 20);
                cmd.Parameters["@OpNo"].Value = pIn.strOpCode;

                cmd.Parameters.Add("@ParamsInfo", SqlDbType.VarChar, 4000);
                cmd.Parameters["@ParamsInfo"].Value = pIn.strParamInfo;

                cmd.Parameters.Add("@BusiNo", SqlDbType.VarChar, 10);
                cmd.Parameters["@BusiNo"].Value = pIn.strBatchNo;

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