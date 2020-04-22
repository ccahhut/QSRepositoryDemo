using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Data;
using QManage.tools;
using System.Xml;
using Newtonsoft.Json;

namespace QManage
{
    class ClassAuthRemote
    {
        public int id;
        public int calltype;
        /// <summary>
        /// 状态。0：待发送  1：成功 2：失败 
        /// </summary>
        public int status = 0;
        public string sendinfo;
        public string recvinfo="";

        public int RetCode = 99;
        public string RetText = "系统忙，请稍后再试";

        public void callAuthRemote()
        {
            //calltype 接口类型 1：公安认证  2：电子税务局认证 3：存量房线上预审核 4：取号信息上传
            if (calltype == 4)
            {
                sendTicketInfo2Tax();
            }

            // 更新状态
            updateStatus();
        }

        public void insertTaxFlow(string trCode, string paramIn)
        {
            string dbTransDate = DateTime.Now.ToString("yyyyMMdd");
            string dbTrCode = trCode;
            string dbGenTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            int dbStatus = 0;
            string dbParamsIn = paramIn;
            int dbSortID = 1;

            DBUtils dbUtils = new DBUtils();
            string strSql = String.Format("insert into tbltaxflow (TransDate,TrCode,GenTime,Status,ParamsIn,SortID) VALUES ('{0}','{1}','{2}',{3},'{4}',{5})", dbTransDate, dbTrCode, dbGenTime, dbStatus, dbParamsIn, dbSortID);
            int resRows = dbUtils.ExcuSqlDB(strSql);
            dbUtils.CloseSqlDB();
        }

        public string getHallBusiInfo()
        {
            string hallBusiInfo = "";

            //string strSql = string.Format("Select BusiNo,BusiName,WaitNum,checkUser,BusiType,MenuType,IsHouseBusi,MetaBusiNo,AMGetNum,PMGetNum From tblBusi Where MenuType in (0,1) and Status=1 order by MenuType,SortID");
            //DBUtils dbUtils = new DBUtils();
            //DataTable dt = dbUtils.SelectSqlDB(strSql);
            //if (dt != null && dt.Rows.Count > 0)
            //{
            //    string uuid = dt.Rows[0]["QueueID"].ToString().Trim();
            //}

            JObject jsonBusiData = new JObject();
            JArray jsonBusi = new JArray();
            int busiCount = 0;

            DataTable dt = new DataTable();
            SqlConnection con = null;
            try
            {
                con = new SqlConnection(SessionInfo.strConn);
                con.Open();
                SqlCommand cmd = new SqlCommand("procGetBusiInfoEx", con);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter sqlda = new SqlDataAdapter(cmd);
                sqlda.Fill(dt);
                if (dt != null && dt.Rows.Count > 0)
                {
                    busiCount = dt.Rows.Count;
                    for (int idx = 0; idx < busiCount; idx++)
                    {
                        JObject jsonItem = new JObject();
                        jsonItem.Add("busino", dt.Rows[idx]["BusiNo"].ToString().Trim());
                        jsonItem.Add("businame", dt.Rows[idx]["BusiName"].ToString().Trim());

                        jsonBusi.Add(jsonItem);
                    }
                }
            }
            catch (Exception se)
            {
                FConst.WriteLog("获取业务列表信息procGetBusiInfoEx:" + se.Message);
                busiCount = 0;
                jsonBusi.Clear();
            }
            finally
            {
                try
                {
                    if (con != null)
                        con.Close();
                }
                catch { ;}
                try
                {
                    if (dt != null)
                    {
                        dt.Clear();
                        dt.Dispose();
                    }
                }
                catch { ;}
            }
            jsonBusiData.Add("rows", busiCount);
            jsonBusiData.Add("busi", jsonBusi);
            hallBusiInfo = JsonConvert.SerializeObject(jsonBusiData);

            return hallBusiInfo;
        }

        /// <summary>
        /// 生成上传实名信息及照片的xml报文（航信）
        /// </summary>
        /// <param name="paramIn"></param>
        public string getRealNamePhotoPacket(string paramIn)
        {
            string xmlPacket = null;

            // @ParamIn = @UUID+'|'+@BusiNo+'|'+@TicketNo+'|'+@timestamp+'|'
            string[] tmp = paramIn.Split('|');

            string transDate = DateTime.Now.ToString("yyyyMMdd");
            string busiNo = tmp[1].Trim();
            string ticketNo = tmp[2].Trim();
            string strSql = string.Format("select f.FlowID,f.TransDate,f.BusiNo,f.TicketNo,f.PID,f.UserName,f.FormNo,form.DisplayName,f.EmpNo, rn.QueueID, rn.UserPhoto from tblFlow as f, tblForm as form, tblRealName as rn where f.status=1 and f.TransDate='{0}' and f.BusiNo='{1}' and f.TicketNo='{2}' and f.FormNo=form.FormNo and f.PID=rn.PID", transDate, busiNo, ticketNo);
            DBUtils dbUtils = new DBUtils();
            DataTable dt = dbUtils.SelectSqlDB(strSql);
            if (dt != null && dt.Rows.Count > 0)
            {
                string uuid = dt.Rows[0]["QueueID"].ToString().Trim();
                string FormNo = dt.Rows[0]["FormNo"].ToString().Trim();
                string DisplayName = dt.Rows[0]["DisplayName"].ToString().Trim();
                string EmpNo = dt.Rows[0]["EmpNo"].ToString().Trim();
                string PID = dt.Rows[0]["PID"].ToString().Trim();
                string UserName = dt.Rows[0]["UserName"].ToString().Trim();
                byte[] UserPhoto = (byte[])dt.Rows[0]["UserPhoto"];

                XmlDocument xmlDoc = new XmlDocument();
                XmlDeclaration xmlDecl = xmlDoc.CreateXmlDeclaration("1.0", "GB2312", null);
                xmlDoc.AppendChild(xmlDecl);

                XmlElement serviceElement = xmlDoc.CreateElement("", "service", "");
                serviceElement.SetAttribute("xmlns", "http://www.chinatax.gov.cn/spec/");
                serviceElement.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");

                XmlElement headElement = xmlDoc.CreateElement("", "head", "");
                XmlElement element = xmlDoc.CreateElement("", "tran_id", "");
                element.InnerText = "CQ.QT.DZSWJ.HXRLSB.PDJH";
                headElement.AppendChild(element);

                element = xmlDoc.CreateElement("", "channel_id", "");   //channel_id从配置文件读取
                element.InnerText = SessionInfo.channel_id_hx;
                headElement.AppendChild(element);

                string strBusinessDate = DateTime.Now.ToString("yyyyMMdd");
                string strBusinessTime = DateTime.Now.ToString("HHmmss");
                string sequenceID = string.Format("{0}{1}{2}{3}{4}", uuid, SessionInfo.strIpAddr, SessionInfo.strMac, strBusinessDate, strBusinessTime);
                element = xmlDoc.CreateElement("", "tran_seq", "");
                element.InnerText = FConst.GetMD5(sequenceID);
                headElement.AppendChild(element);

                element = xmlDoc.CreateElement("", "tran_date", "");
                element.InnerText = strBusinessDate;
                headElement.AppendChild(element);

                element = xmlDoc.CreateElement("", "tran_time", "");
                element.InnerText = strBusinessTime;
                headElement.AppendChild(element);

                serviceElement.AppendChild(headElement);

                XmlElement expandElement = xmlDoc.CreateElement("", "expand", "");
                serviceElement.AppendChild(expandElement);

                XmlElement bodyElement = xmlDoc.CreateElement("", "body", "");

                XmlDocument rootDoc = new XmlDocument();
                XmlElement rootElement = rootDoc.CreateElement("", "root", "");
                element = rootDoc.CreateElement("", "pdbh", "");
                element.InnerText = ticketNo;
                rootElement.AppendChild(element);

                element = rootDoc.CreateElement("", "dtbh", "");
                element.InnerText = uuid;
                rootElement.AppendChild(element);

                element = rootDoc.CreateElement("", "zxbh", "");
                element.InnerText = FormNo;
                rootElement.AppendChild(element);

                element = rootDoc.CreateElement("", "dzxbh", "");
                element.InnerText = DisplayName;
                rootElement.AppendChild(element);

                element = rootDoc.CreateElement("", "jhjbh", "");
                element.InnerText = uuid;
                rootElement.AppendChild(element);

                element = rootDoc.CreateElement("", "sfzhm", "");
                element.InnerText = PID;
                rootElement.AppendChild(element);

                element = rootDoc.CreateElement("", "sfzxm", "");
                element.InnerText = UserName;
                rootElement.AppendChild(element);

                element = rootDoc.CreateElement("", "sfztx", "");
                element.InnerText = Base64Util.Base64Encode(UserPhoto);
                rootElement.AppendChild(element);
                rootDoc.AppendChild(rootElement);

                bodyElement.AppendChild(xmlDoc.CreateCDataSection(rootDoc.InnerXml));
                serviceElement.AppendChild(bodyElement);

                xmlDoc.AppendChild(serviceElement);
                xmlPacket = xmlDoc.InnerXml;
            }
            if (dt != null)
            {
                dt.Clear();
                dt.Dispose();
            }
            dbUtils.CloseSqlDB();

            return xmlPacket;
        }

        private void updateStatus()
        {
            SqlConnection con = null;
            try
            {
                //@Id			int,
                //@Status		int,
                //@RecvInfo	varchar(1024),
                //@RetCode		int output,
                //@RetText		varchar(120) output	

                con = new SqlConnection(SessionInfo.strConn);
                con.Open();
                SqlCommand cmd = new SqlCommand("procAuthRemoteUpdate", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@Id", SqlDbType.Int);
                cmd.Parameters["@Id"].Value = id;

                cmd.Parameters.Add("@Status", SqlDbType.Int);
                cmd.Parameters["@Status"].Value = status;

                cmd.Parameters.Add("@RecvInfo", SqlDbType.VarChar, 1024);
                cmd.Parameters["@RecvInfo"].Value = recvinfo;

                cmd.Parameters.Add("@RetCode", SqlDbType.Int);
                cmd.Parameters["@RetCode"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@RetText", SqlDbType.VarChar, 120);
                cmd.Parameters["@RetText"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                RetCode = Convert.ToInt32(cmd.Parameters["@RetCode"].Value);
                RetText = cmd.Parameters["@RetText"].Value.ToString();
            }
            catch (Exception se)
            {
                FConst.WriteErrLog("updateStatus:" + se.Message);
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
        /// 发送取票信息
        /// </summary>
        public void sendTicketInfo2Tax()
        {
            status = 2;

            //0:uuid, 1:busiNo, 2:ticketNo, 3:timestamp, 4:IDCard, 5:IDName, 6:sfzjlxDm
            String[] detail = sendinfo.Split(new string[]{ "|" }, StringSplitOptions.None );

            String strUrl = string.Format("http://{0}/services/pdjhjWebService_updateQhhm?wsdl", SessionInfo.strUrl);
            String method = "updateQhhmNew";
            String uuid = detail[0];
            String busiNo = detail[1];
            String ticketNo = detail[2];
            long timestamp = GetTimeStamp();
            String IDCard = detail[4];
            String IDName = detail[5];
            String sfzjlxDm = IDCard.Length>0 ? "201" : "";
            FConst.WriteLog(String.Format("传入参数：大厅id=[{0}] 业务编码=[{1}] 取号号码=[{2}] 时戳=[{3}] 身份证=[{4}] 姓名=[{5}] 身份证类型=[{6}]",
                                                uuid, busiNo, ticketNo, timestamp, IDCard, IDName, sfzjlxDm));

            try
            {
                FConst.WriteLog(String.Format("开始调用接口[{0}]{1}", method, strUrl));
                if (SessionInfo.nSystemType == 1)
                {
                    WebServiceProxy wsdl = new WebServiceProxy(strUrl, "pdjh");
                    FConst.WriteLog(String.Format("开始调用接口[{0}]{1}，初始化完成", method, strUrl));
                    String result = wsdl.ExecuteQuery(method, new object[] { uuid, busiNo, ticketNo, timestamp, IDCard, IDName, sfzjlxDm }).ToString();

                    FConst.WriteLog(String.Format("调用方法{0}返回值：[{1}]", method, result));
                    String resultValue = JObject.Parse(result)["result"].ToString();
                    if (resultValue.Equals("1"))
                    {
                        status = 1;
                    }
                    FConst.WriteLog(String.Format("调用方法{0}结果状态：[{1}]", method, resultValue.Equals("1") ? "调用成功" : "调用失败"));
                }
                else
                {
                    FConst.WriteLog(String.Format("系统处于脱机方式，无法调用接口[SystemType={0}]", SessionInfo.nSystemType));
                }

            }
            catch (Exception ex)
            {
                FConst.WriteLog(String.Format("调用方法{0}时发生异常：{1}", method, ex.ToString()));
            }

        }

        /// <summary>
        /// 取时间戳
        /// </summary>
        /// <returns></returns>
        public static long GetTimeStamp()
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            DateTime nowTime = DateTime.Now;
            TimeSpan timeSpan = nowTime - startTime;

            return (long)timeSpan.TotalMilliseconds;
        }

    }
}
