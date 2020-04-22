using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace QManage.tools
{
    /// <summary>
    /// 数据库访问工具类
    /// </summary>
    class DBUtils
    {
        public const int DB_CODE_SUCCESS = 0;
        public const string DB_CODE_SUCCESS_TEXT = "成功";

        public const int DB_CODE_ERROR = -1;
        public const string DB_CODE_ERROR_TEXT = "未知错误";

        //SessionInfo.strConn = string.Format("server={0};database={1};uid={2};pwd={3}", SessionInfo.strServer, SessionInfo.strDB, SessionInfo.strUser, SessionInfo.strPwd);
        public int DBRetCode = -1;
        public string DBRetMessage = "未知错误";

        private SqlConnection sqlcn;

        public bool OpenSqlDB()
        {
            return OpenSqlDB(SessionInfo.strConn);
        }
        /// <summary>
        /// 打开数据库
        /// </summary>
        /// <param name="ConnectString">数据库连接串</param>
        /// <returns></returns>
        public bool OpenSqlDB(string ConnectString)
        {
            bool retValue = false;

            CloseSqlDB();
            try
            {
                sqlcn = new SqlConnection(ConnectString);
                sqlcn.Open();

                retValue = true;
                DBRetCode = DB_CODE_SUCCESS;
                DBRetMessage = string.Format("打开数据库成功：[{0}]", ConnectString);
            }
            catch (Exception ex)
            {
                DBRetCode = DB_CODE_ERROR;
                DBRetMessage = string.Format("打开数据库失败：[{0}]{1}", ConnectString, ex.ToString());
                retValue = false;
            }
            return retValue;
        }

        /// <summary>
        /// 关闭数据库
        /// </summary>
        public void CloseSqlDB()
        {
            try
            {
                if (sqlcn != null)
                {
                    sqlcn.Close();
                    sqlcn.Dispose();
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            finally
            {
                sqlcn = null;
            }
        }

        /// <summary>
        /// 查询并返回结果集
        /// </summary>
        /// <param name="SelectString"></param>
        /// <returns></returns>
        public DataTable SelectSqlDB(string SelectString)
        {
            if (sqlcn == null)
            {
                if (!OpenSqlDB())
                {
                    return null;
                }
            }

            DataTable dt = new DataTable();
            try
            {
                SqlDataAdapter sqlda = new SqlDataAdapter(SelectString, sqlcn);
                sqlda.Fill(dt);

                DBRetCode = DB_CODE_SUCCESS;
                DBRetMessage = DB_CODE_SUCCESS_TEXT;
            }
            catch (Exception ex)
            {
                DBRetCode = DB_CODE_ERROR;
                DBRetMessage = string.Format("执行查询语句异常：{0} 原因：{1}", SelectString, ex.ToString());
            }
            return dt;
        }

        public int ExcuSqlDB(string SqlString)
        {
            int row = 0;
            if (sqlcn == null)
            {
                if (!OpenSqlDB())
                {
                    return row;
                }
            }
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = SqlString;
                cmd.Connection = sqlcn;
                row = cmd.ExecuteNonQuery();

                DBRetCode = DB_CODE_SUCCESS;
                DBRetMessage = DB_CODE_SUCCESS_TEXT;
            }
            catch (Exception ex)
            {
                DBRetCode = DB_CODE_ERROR;
                DBRetMessage = string.Format("执行Sql语句异常：{0} 原因：{1}", SqlString, ex.ToString());
            }
            return row;
        }
    }
}
