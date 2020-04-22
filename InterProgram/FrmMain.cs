using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using Common;
using System.Threading;
using System.IO;
using System.Net.Sockets;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Xml.Linq;
using QManage.tools; 

namespace QManage
{
    public partial class FrmMain : Form
    {
        //收到的消息
        /****工作线程更新主界面*****/
        public delegate void MyInvoke(string strLog);//使用托管方法更新界面
        public void UpdateLog(string strLog)
        {
            if (rText.Lines.Length > 300)
                rText.Clear();
            rText.AppendText(System.DateTime.Now.ToString("HH:mm:ss") +strLog + "\r\n");
        }
         
        /*****/
        private Thread thAuthRemote;
        private Thread thInterFace;
        private Thread thCheckNet;
        private Thread thAppraiseNull;
        //private Thread thGInterFace;
        public FrmMain()
        { 
            InitializeComponent();
            StartUp();
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            base.UpdateStyles(); 
            //解决闪烁
            BackgroundNoSplash(); 
        }
        //只启动1个进程
        private void StartUp()
        {
            Process currentProcess = Process.GetCurrentProcess();

            foreach (Process item in Process.GetProcessesByName(currentProcess.ProcessName))
            {
                if (item.Id != currentProcess.Id &&
                (item.StartTime - currentProcess.StartTime).TotalMilliseconds <= 0)
                {
                    item.Kill();

                    item.WaitForExit();

                    break;
                }
            }
        }  
        #region  "解决闪烁"
        MdiClient mdiClient = new MdiClient();    
        private void BackgroundNoSplash()
        {
            foreach (Control var in this.Controls)
            {
                if (var is MdiClient)
                {
                    mdiClient = var as MdiClient;
                    break;
                }
            }
            if (mdiClient != null)
            {
                mdiClient.Paint += new PaintEventHandler(OnMdiClientPaint);
                System.Reflection.MethodInfo mi = (mdiClient as Control).GetType().GetMethod("SetStyle", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                mi.Invoke(mdiClient, new object[] { ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer
                 | ControlStyles.ResizeRedraw, true });

            }
        } 
        private void OnMdiClientPaint(object sender, PaintEventArgs e)
         {
             if (!File.Exists(".\\img\\BackGroundImage.bmp"))
                 return;
            Image image = Image.FromFile(".\\img\\BackGroundImage.bmp");
             //g.DrawImage(Properties.Resources.BackGroundImage, new Rectangle(0, 0, mdiClient.Width, mdiClient.Height));
            Graphics g = e.Graphics;
            g.DrawImage(image, new Rectangle(0, 0, mdiClient.Width, mdiClient.Height));
             string msg = "";
             SizeF size = e.Graphics.MeasureString(msg, this.Font);
             g.DrawString(msg, this.Font, new SolidBrush(Color.White), mdiClient.Width - size.Width, mdiClient.Height - size.Height);

         }
        #endregion
        //读取配置文件信息
        private void LoadConfig()
        {  
            SessionInfo.strServer = ConfigurationSettings.AppSettings["strServer"];
            SessionInfo.strDB = ConfigurationSettings.AppSettings["strDB"];
            SessionInfo.strUser = ConfigurationSettings.AppSettings["strUser"];
            SessionInfo.strPwd = ConfigurationSettings.AppSettings["strPwd"];
            SessionInfo.strConn = string.Format("server={0};database={1};uid={2};pwd={3}", SessionInfo.strServer, SessionInfo.strDB, SessionInfo.strUser, SessionInfo.strPwd);
            SessionInfo.nDelay = Convert.ToInt32(ConfigurationSettings.AppSettings["nDelay"]);
            
            SessionInfo.nTimeOut = Convert.ToInt32(ConfigurationSettings.AppSettings["nTimeOut"]);

            SessionInfo.DEBUG = Convert.ToInt32(ConfigurationSettings.AppSettings["DEBUG"]);

            SessionInfo.channel_id_hx = ConfigurationSettings.AppSettings["channel_id_hx"];
            SessionInfo.strUrl_hx = ConfigurationSettings.AppSettings["strUrl_hx"];


            //暂时不进行加密处理
            
            SessionInfo.strIpAddr = FConst.GetIpAddr();
            SessionInfo.strHostName = FConst.GetHostName();
            SessionInfo.strMac = FConst.GetMac();
            Thread.Sleep(SessionInfo.nDelay);  
            //获取系统信息
            getSysParams();
             
        }
        
        //获取系统参数信息
        private void getSysParams()
        {
            int nSQLID = 303;
            string strParam = "";
            byte[] bParamInfo;
            int nRetCode;
            string strRetText;
            bParamInfo = Encoding.Default.GetBytes(strParam);
            DataTable dt;
            DataInfo.getTable(nSQLID, bParamInfo, out dt, out nRetCode, out strRetText);
            if (nRetCode == 0)
            { 
                SessionInfo.strUUID = dt.Rows[0][0].ToString();
                SessionInfo.strUrl = dt.Rows[0][1].ToString();
                SessionInfo.nSystemType = Convert.ToInt32(dt.Rows[0][2]); 
                //SessionInfo.nSystemType = 1;
                if (dt.Columns.Count >= 4)
                {
                    SessionInfo.strDMServerUrl = dt.Rows[0][3].ToString();  // 增加中心接口Url地址（预约取号） 2020-02-29 
                }
            } 
        }
        
         
        private void FrmMain_Load(object sender, EventArgs e)
        {
            FConst.SetColor(this);  
            LoadConfig();
            this.notifyIcon1.Text = "排队叫号通讯系统";
            // 启动线程
            //thAuthRemote = new Thread(new ThreadStart(AuthRemoteProc));
            //thAuthRemote.Start();
            //if (SessionInfo.nSystemType != 1)
            //    return;
            thInterFace = new Thread(new ThreadStart(SendAndReceive));
            thInterFace.Start();
            thCheckNet = new Thread(new ThreadStart(this.checkNetStatus));
            thCheckNet.Start();
            thAppraiseNull = new Thread(new ThreadStart(this.AppraiseNull));
            thAppraiseNull.Start();

            //
            //thGInterFace = new Thread(new ThreadStart(SendAndReceiveGTax));
            //thGInterFace.Start();

            FConst.WriteLog("地税接口通讯系统启动");
            MyInvoke mi = new MyInvoke(UpdateLog);
            string strLog = "<=>地税接口通讯系统启动";
            BeginInvoke(mi, new Object[] { strLog });
            WindowState = FormWindowState.Minimized;
            SessionInfo.bLogin = true;
             
        }


        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (SessionInfo.bLogin)
            {
                //if (MessageBox.Show("确实要退出吗?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No)
                //{
                //    e.Cancel = true;
                //}
                //else
                //{
                FConst.WriteLog("地税接口通讯系统退出");
                    System.Environment.Exit(0);
                //}
            }
            else
            {
                System.Environment.Exit(0);
            }
        }

      
 
        public void rptCache() 
        {
            #region 缓存文件清单
            ////为了解决水晶报表第一次打印慢的问题
            //DataTable dt = new DataTable();
            //dt.Columns.Add("temp");
            ////水晶报表对象
            //QManage.rpt.SettRpt1 rpt = new QManage.rpt.SettRpt1();
            //rpt.SetDataSource(dt);
            #endregion 缓存文件清单
        }

        private void menuExit1_Click(object sender, EventArgs e)
        {
            Close();
        }
        /// <summary>
        /// 远程处理任务
        /// </summary>
        private void AuthRemoteProc()
        {
            int LOOPTIME = 1000;//延迟时间
            while (true)
            {
                try
                {
                    DataTable dt = Fun.getAuthRemoteRecord();
                    for (int index = 0; index < dt.Rows.Count; index++)
                    {
                        ClassAuthRemote clsAuthRemote = new ClassAuthRemote();
                        clsAuthRemote.id = Convert.ToInt32(dt.Rows[index]["id"].ToString() + "");
                        clsAuthRemote.calltype = Convert.ToInt32(dt.Rows[index]["calltype"].ToString() + "");
                        clsAuthRemote.sendinfo = dt.Rows[index]["sendinfo"].ToString() + "";

                        Thread thProc = new Thread(new ThreadStart(clsAuthRemote.callAuthRemote));
                        thProc.Start();
                    }
                }
                catch { ;}
                Thread.Sleep(LOOPTIME);
            }
        }

        /// <summary>  
        /// 通讯
        /// </summary>  
        private void SendAndReceive()
        {
            /*8001	员工登录
            8002	叫号
            8003	开始办理
            8004	结束办理
            8005	评价
            --8006	退出登录
            --8010	暂停服务
            --8011	恢复服务
            --8012	重复呼叫
            --8013	获取窗口等待人数
            8014	弃号
            --8015	业务转移
            8101	获取大厅窗口信息
            8102	获取大厅业务信息
            8103	获取窗口业务信息
            8104	排队号码清零
            8105	获取当前排队取号，叫号信息
            8106	获取系统时间
            8107	服务启动
            8108	叫号机心跳
            8109	叫号窗口心跳
            8110	广告信息同步
            8201	取号(同步排队取号)
            8202	预约取号 
            8203	提醒间隔号数	spTaxGetDiffNum
            8204	短信提醒	spTaxSmsAlert
            8205	大厅业务分类信息(新)	spTaxGetBusiInfoNew
            8206	实名校验	spTaxVerifyPID

            */
            int LOOPDELAYTIME = 120;
            int LOOPTIME = 30000;//断网状态延迟时间
            //前一个流水号
            int nPreviousFlowID = 0;
            int nTaxFlowID = 0;
            string strTrCode = "";
            string strParams = ""; 
            //string url = "http://162.20.22.200:8002/services/pdjhjWebService?wsdl";//wsdl地址
            //string strUrl = SessionInfo.strUrl;
            string strUrl = "";
            string method = "";//javaWebService开放的接口  
            //WebServiceProxy wsd = new WebServiceProxy(url, name); 
            string strLog = "";
            string strParamsOut = "";
            int nBusiStatus = 0;
            string strBusiParams = "";
            MyInvoke mi = new MyInvoke(UpdateLog);
            int isRecall = 0;
            while (true)
            {
                try
                {
                    isRecall = 0;
                    //获取1条信息\
                    strParamsOut = "";
                    nBusiStatus = 0;
                    strBusiParams = "";
                    method = ""; 
                    object[] str = null;

                    //if (SessionInfo.nSystemType == 0)
                    //{
                    //    Thread.Sleep(LOOPTIME);
                    //    continue;
                    //}

                    Fun.GetSendInfo(out nTaxFlowID,out strTrCode,out strParams);
                    if (nTaxFlowID == 0)
                    {
                        Thread.Sleep(LOOPDELAYTIME);
                        continue;
                    }

                    if (strTrCode == "3301")    //大厅业务信息同步到中心服务器
                    {
                        isRecall = 1;
                        //在8205取业务信息列表返回（spTaxSetInfo）后生成该指令
                        strUrl = string.Format("http://{0}/service/hall?wsdl", SessionInfo.strDMServerUrl);
                        method = "setHallBusi".Trim();
                        ClassAuthRemote authRemote = new ClassAuthRemote();
                        string busiData = authRemote.getHallBusiInfo();
                        str = new object[2] { SessionInfo.strUUID, busiData };
                    }
                    else if (strTrCode == "3302")    //上传预约叫号流水，由8002发起。
                    {
                        isRecall = 1;
                        strUrl = string.Format("http://{0}/service/hall?wsdl", SessionInfo.strDMServerUrl);
                        method = "syncCall".Trim();
                        string[] tmp = strParams.Split('|');
                        if (tmp.Length < 6) continue;
                        //@UUID+'|'+@RemoteNoBusi+'|'+@RemoteNoForm+'|'+@EmpNo+'|'+@TicketNo+@TransInfo+'|'+@CallTime+'|'
                        //25FAA763E9C95681E053CA1614A2D27C | 10000403 | 10000039 | 15001133818 | B001 | 1558400334000 |
                        //if (tmp[5].Length > 10) tmp[5] = tmp[5].Substring(0, 10);
                        //str = new object[6] { tmp[0], tmp[1], tmp[2], tmp[3], tmp[4], Convert.ToInt64(tmp[5]) };
                        str = new object[6] { tmp[0], tmp[1], tmp[2], tmp[3], tmp[4], tmp[5] };
                    }
                    else if (strTrCode == "3303")    //上传预约开始办理流水，由8003发起
                    {
                        isRecall = 1;
                        strUrl = string.Format("http://{0}/service/hall?wsdl", SessionInfo.strDMServerUrl);
                        method = "syncInit".Trim();
                        string[] tmp = strParams.Split('|');
                        if (tmp.Length < 6) continue;
                        //@UUID+'|'+@RemoteNoBusi+'|'+@RemoteNoForm+'|'+@EmpNo+'|'+@TicketNo+@TransInfo+'|'+@CallTime+'|'
                        //25FAA763E9C95681E053CA1614A2D27C | 10000403 | 10000039 | 15001133818 | B001 | 1558400354000 |
                        //if (tmp[5].Length > 10) tmp[5] = tmp[5].Substring(0, 10);
                        //str = new object[6] { tmp[0], tmp[1], tmp[2], tmp[3], tmp[4], Convert.ToInt64(tmp[5]) };
                        str = new object[6] { tmp[0], tmp[1], tmp[2], tmp[3], tmp[4], tmp[5] };
                    }
                    else if (strTrCode == "3304")    //上传预约办理结束流水，由8004发起
                    {
                        isRecall = 1;
                        strUrl = string.Format("http://{0}/service/hall?wsdl", SessionInfo.strDMServerUrl);
                        method = "syncTail".Trim();
                        string[] tmp = strParams.Split('|');
                        //@UUID+'|'+@RemoteNoBusi+'|'+@RemoteNoForm+'|'+@EmpNo+'|'+@TicketNo+@TransInfo+'|'+@CallTime+'|'
                        //25FAA763E9C95681E053CA1614A2D27C | 10000403 | 10000039 | 15001133818 | B001 | 1558400354000 |
                        //if (tmp[5].Length > 10) tmp[5] = tmp[5].Substring(0, 10);

                        if (tmp.Length < 6) continue;
                        str = new object[6] { tmp[0], tmp[1], tmp[2], tmp[3], tmp[4], tmp[5] };

                        //if (tmp.Length < 7) continue;
                        //str = new object[7] { tmp[0], tmp[1], tmp[2], tmp[3], tmp[4], tmp[5], Convert.ToInt64(tmp[6]) };

                    }
                    else if (strTrCode == "3305")    //上传预约评价流水，由8005发起
                    {
                        isRecall = 1;
                        strUrl = string.Format("http://{0}/service/hall?wsdl", SessionInfo.strDMServerUrl);
                        method = "syncEval".Trim();
                        string[] tmp = strParams.Split('|');
                        //@UUID+'|'+@RemoteNoBusi+'|'+@RemoteNoForm+'|'+@EmpNo+'|'+@TicketNo+@TransInfo+'|'+Convert(varchar(10),@Score)+'|'+@CallTime+'|'
                        //25FAA763E9C95681E053CA1614A2D27C | 10000403 | 10000039 | 15001133818 | B001 | 0 | 1558400502000 |

                        if (tmp.Length < 7) continue;
                        //if (tmp[6].Length > 10) tmp[6] = tmp[6].Substring(0, 10);
                        //str = new object[7] { tmp[0], tmp[1], tmp[2], tmp[3], tmp[4], tmp[5], Convert.ToInt64(tmp[6]) };
                        str = new object[7] { tmp[0], tmp[1], tmp[2], tmp[3], tmp[4], tmp[5], tmp[6] };

                    }
                    else if (strTrCode == "3203")    //上传预约取票信息，由3201发起。(暂未用procUpdateAppointmentToTax)
                    {
                        isRecall = 1;
                        strUrl = string.Format("http://{0}/service/hall?wsdl", SessionInfo.strDMServerUrl);
                        method = "syncTake".Trim();
                        string qhlx = "0";
                        string[] tmp = strParams.Split('|');
                        //if (tmp.Length < 5) continue;
                        if (tmp.Length >= 8)
                        {
                            //取票类型(0.普通取票 9.预约取票)
                            if (tmp[7].Trim().Equals("2"))
                            {
                                qhlx = "9";
                            }
                        }
                        //@UUID+'|'+@BusiNo+'|'+@TicketNo+'|'+@timestamp+'|'+@IDCard+'|'+@IDName+'|'+@sfzjlxDm+'|'+@channeltype | yy_flow_id(dycode)
                        //55BCB64036335282E053B7010CA275BF | 33 | Z002 | 1583503590000 | 50024119930906473X | 田锋             | 201 | 0 | dycode
                        //public String syncTake(String jhjDm,String ywflDm, String qhhm,String qhlx,Long timestamp);
                        //if (tmp[3].Length > 10) tmp[3] = tmp[3].Substring(0, 10);
                        //str = new object[5] { tmp[0], tmp[1], tmp[2], qhlx, Convert.ToInt64(tmp[3]) };
                        str = new object[6] { tmp[0], tmp[1], tmp[2], qhlx, tmp[8], tmp[3] };
                        StringBuilder builder = new StringBuilder();
                        foreach (object item in str)
                        {
                            builder.AppendFormat("{0}|", item.ToString());
                        }
                        FConst.WriteLog(String.Format("上传预约取票信息的参数[{0}]", builder));
                    }
                    else if (strTrCode == "3202")    // 叫号后上传取票实名信息及照片 procUpdateRealNameToTax
                    {
                        //登录String userId,String password,String dtckDm
                        strUrl = string.Format("http://{0}/smbs/hxws/service?wsdl", SessionInfo.strUrl_hx);
                        method = "transServices".Trim();
                        // @ParamIn = @UUID+'|'+@BusiNo+'|'+@TicketNo+'|'+@timestamp+'|'
                        string[] tmp = strParams.Split('|');
                        if (tmp.Length < 3) continue;
                        ClassAuthRemote authRemote = new ClassAuthRemote();
                        string xmlPacket = authRemote.getRealNamePhotoPacket(strParams);
                        if (xmlPacket == null) continue;
                        str = new object[1] { xmlPacket };
                    }
                    else if (strTrCode == "3201")    // 上传取票信息procUpdateTicketInfoToTax
                    {
                        //25FAA763E9C95681E053CA1614A2D27C| 33| A379| 1583206949000 | userIdCode | userName | 201 | 0 | dycode
                        //@UUID+'|'+@BusiNo+'|'+@TicketNo+'|'+@timestamp+'|'+@IDCard+'|'+@IDName+'|'+@sfzjlxDm+'|'+@channeltype | yy_flow_id(dycode)
                        strUrl = string.Format("http://{0}/services/pdjhjWebService_updateQhhm?wsdl", SessionInfo.strUrl);
                        method = "updateQhhmNew";
                        string[] tmp = strParams.Split('|');
                        //if (tmp.Length < 7) continue;
                        str = new object[7] { tmp[0], tmp[1], tmp[2], Convert.ToInt64(tmp[3]), tmp[4], tmp[5], tmp[6] };

                        //生成向中心数据管理服务器上传取号流水
                        if (nPreviousFlowID != nTaxFlowID)
                        {
                            ClassAuthRemote authRemote = new ClassAuthRemote();
                            authRemote.insertTaxFlow("3203", strParams);
                            nPreviousFlowID = nTaxFlowID;
                        }
                    }
                    else if (strTrCode == "8001")
                    {
                        //登录String userId,String password,String dtckDm
                        strUrl = string.Format("http://{0}/services/pdjhjWebService_login?wsdl", SessionInfo.strUrl);
                        method = "login";
                        string[] tmp = strParams.Split('|');
                        //if (tmp.Length < 3) continue;
                        str = new object[3] { tmp[0], DESEncrypt.DesDecrypt(tmp[1]), tmp[2] };
                    }
                    else if (strTrCode == "8101")
                    {
                        //获取大厅窗口信息String jhjDm
                        strUrl = string.Format("http://{0}/services/pdjhjWebService_getDtCkxx?wsdl", SessionInfo.strUrl);
                        method = "getDtCkxx";
                        string[] tmp = strParams.Split('|');
                        //if (tmp.Length < 1) continue;
                        str = new object[1] { tmp[0] };
                    }
                    else if (strTrCode == "8102")
                    {
                        //获取大厅业务信息String jhjDm
                        strUrl = string.Format("http://{0}/services/pdjhjWebService_getDtYwflxx?wsdl", SessionInfo.strUrl);
                        method = "getDtYwflxx";
                        string[] tmp = strParams.Split('|');
                        //if (tmp.Length < 1) continue;
                        str = new object[1] { tmp[0] };
                    }
                    else if (strTrCode == "8103")
                    {
                        //获取窗口业务信息String jhjDm,String dtckDm
                        strUrl = string.Format("http://{0}/services/pdjhjWebService_getYwflxx?wsdl", SessionInfo.strUrl);
                        method = "getYwflxx";
                        string[] tmp = strParams.Split('|');
                        //if (tmp.Length < 2) continue;
                        str = new object[2] { tmp[0], tmp[1] };
                    }
                    else if (strTrCode == "8201")
                    {
                        //取号(同步排队取号)String jhjDm,String ywflDm,String qhhm,Long timestamp
                        strUrl = string.Format("http://{0}/services/pdjhjWebService_updateQhhm?wsdl", SessionInfo.strUrl);
                        method = "updateQhhm";
                        string[] tmp = strParams.Split('|');
                        if (tmp.Length < 4) continue;
                        str = new object[4] { tmp[0], tmp[1], tmp[2], Convert.ToInt64(tmp[3]) };

                        continue;   //取消取号信息上传同步（改到3201完成）2018-11-12
                    }
                    else if (strTrCode == "8002")
                    {
                        //叫号String jhjDm,String ywflDm,String dtckDm,String swryDm,String jhhm,Long timestamp
                        strUrl = string.Format("http://{0}/services/pdjhjWebService_updateJhhm?wsdl", SessionInfo.strUrl);
                        method = "updateJhhm";
                        string[] tmp = strParams.Split('|');
                        if (tmp.Length < 6) continue;
                        str = new object[6] { tmp[0], tmp[1], tmp[2], tmp[3], tmp[4], Convert.ToInt64(tmp[5]) };

                        //生成向中心数据管理服务器上传叫号流水
                        if (nPreviousFlowID != nTaxFlowID)
                        {
                            ClassAuthRemote authRemote = new ClassAuthRemote();
                            authRemote.insertTaxFlow("3302", strParams);
                            nPreviousFlowID = nTaxFlowID;
                        }
                    }
                    else if (strTrCode == "8003")
                    {
                        //开始办理String jhjDm,String ywflDm,String dtckDm,String swryDm,String hs,Long timestamp
                        strUrl = string.Format("http://{0}/services/pdjhjWebService_updateKsywblsj?wsdl", SessionInfo.strUrl);
                        method = "updateKsywblsj";
                        string[] tmp = strParams.Split('|');
                        if (tmp.Length < 6) continue;
                        str = new object[6] { tmp[0], tmp[1], tmp[2], tmp[3], tmp[4], Convert.ToInt64(tmp[5]) };

                        //生成向中心数据管理服务器上传开始办理流水
                        if (nPreviousFlowID != nTaxFlowID)
                        {
                            ClassAuthRemote authRemote = new ClassAuthRemote();
                            authRemote.insertTaxFlow("3303", strParams);
                            nPreviousFlowID = nTaxFlowID;
                        }
                    }
                    else if (strTrCode == "8004")
                    {
                        //结束办理String jhjDm,String ywflDm,String dtckDm,String swryDm,String hs,Long timestamp
                        strUrl = string.Format("http://{0}/services/pdjhjWebService_updateJsywblsj?wsdl", SessionInfo.strUrl);
                        method = "updateJsywblsj";
                        string[] tmp = strParams.Split('|');
                        if (tmp.Length < 6) continue;
                        str = new object[6] { tmp[0], tmp[1], tmp[2], tmp[3], tmp[4], Convert.ToInt64(tmp[5]) };

                        //生成向中心数据管理服务器上传办理结束流水
                        if (nPreviousFlowID != nTaxFlowID)
                        {
                            ClassAuthRemote authRemote = new ClassAuthRemote();
                            authRemote.insertTaxFlow("3304", strParams);
                            nPreviousFlowID = nTaxFlowID;
                        }
                    }
                    else if (strTrCode == "8005")
                    {
                        //评价String jhjDm,String ywflDm,String dtckxx,String swryDm,String hs,String pjxx,Long timestamp
                        strUrl = string.Format("http://{0}/services/pdjhjWebService_addPjxx?wsdl", SessionInfo.strUrl);
                        method = "addPjxx";
                        string[] tmp = strParams.Split('|');
                        if (tmp.Length < 7) continue;
                        str = new object[7] { tmp[0], tmp[1], tmp[2], tmp[3], tmp[4], tmp[5], Convert.ToInt64(tmp[6]) };

                        //生成向中心数据管理服务器上传评价流水
                        if (nPreviousFlowID != nTaxFlowID)
                        {
                            ClassAuthRemote authRemote = new ClassAuthRemote();
                            authRemote.insertTaxFlow("3305", strParams);
                            nPreviousFlowID = nTaxFlowID;
                        }
                    }
                    else if (strTrCode == "8014")
                    {
                        //弃号 String jhjDm,String ywflDm,String dtckDm,String swryDm,String hs,Long timestamp
                        strUrl = string.Format("http://{0}/services/pdjhjWebService_updateKhsj?wsdl", SessionInfo.strUrl);
                        method = "updateKhsj";
                        string[] tmp = strParams.Split('|');
                        if (tmp.Length < 6) continue;
                        str = new object[6] { tmp[0], tmp[1], tmp[2], tmp[3], tmp[4], Convert.ToInt64(tmp[5]) };
                    }
                    else if (strTrCode == "8104")
                    {
                        //排队号码清零String jhjDm
                        strUrl = string.Format("http://{0}/services/pdjhjWebService_resetJhjhm?wsdl", SessionInfo.strUrl);
                        method = "resetJhjhm";
                        string[] tmp = strParams.Split('|');
                        if (tmp.Length < 1) continue;
                        str = new object[1] { tmp[0] };
                    }
                    else if (strTrCode == "8105")
                    {
                        //获取当前排队取号，叫号信息String jhjDm
                        strUrl = string.Format("http://{0}/services/pdjhjWebService_getQhhmAndJhhm?wsdl", SessionInfo.strUrl);
                        method = "getQhhmAndJhhm";
                        string[] tmp = strParams.Split('|');
                        if (tmp.Length < 2) continue;
                        str = new object[2] { tmp[0], tmp[1] };
                    }
                    else if (strTrCode == "8106")
                    {
                        //获取系统时间 
                        strUrl = string.Format("http://{0}/services/pdjhjWebService_getSystemTime?wsdl", SessionInfo.strUrl);
                        method = "getSystemTime";

                    }
                    else if (strTrCode == "8107")
                    {
                        //服务启动 String jhjDm,Long timestamp
                        strUrl = string.Format("http://{0}/services/pdjhjWebService_startPdjhj?wsdl", SessionInfo.strUrl);
                        method = "startPdjhj";
                        string[] tmp = strParams.Split('|');
                        //if (tmp.Length < 2) continue;
                        str = new object[2] { tmp[0], Convert.ToInt64(tmp[1]) };
                    }
                    else if (strTrCode == "8108")
                    {
                        //叫号机心跳 String jhjDm,Long timestamp
                        strUrl = string.Format("http://{0}/services/pdjhjWebService_pdjhjStarting?wsdl", SessionInfo.strUrl);
                        method = "pdjhjStarting";
                        string[] tmp = strParams.Split('|');
                        if (tmp.Length < 2) continue;
                        str = new object[2] { tmp[0], Convert.ToInt64(tmp[1]) };
                    }
                    else if (strTrCode == "8109")
                    {
                        //叫号窗口心跳 String jhjDm,String dtckDm,String swryDm,Long timestamp
                        strUrl = string.Format("http://{0}/services/pdjhjWebService_jhxtStarting?wsdl", SessionInfo.strUrl);
                        method = "jhxtStarting";
                        string[] tmp = strParams.Split('|');
                        if (tmp.Length < 4) continue;
                        str = new object[4] { tmp[0], tmp[1], tmp[2], Convert.ToInt64(tmp[3]) };
                    }
                    else if (strTrCode == "8110")
                    {
                        //广告信息同步 String jhjDm,String dtckDm,String bbbh
                        strUrl = string.Format("http://{0}/services/pdjhjWebService_queryGgwj?wsdl", SessionInfo.strUrl);
                        method = "queryGgwj";
                        string[] tmp = strParams.Split('|');
                        if (tmp.Length < 3) continue;
                        str = new object[3] { tmp[0], tmp[1], tmp[2] };
                    }
                    else if (strTrCode == "8202")
                    {
                        //预约取号 String jhjDm,string sfzjhm String yzm
                        strUrl = string.Format("http://{0}/services/pdjhjWebService_getYyxxByYzm?wsdl", SessionInfo.strUrl);
                        method = "getYyxxByYzm";
                        string[] tmp = strParams.Split('|');
                        if (tmp.Length < 3) continue;
                        str = new object[3] { tmp[0], tmp[1], tmp[2] };
                    }
                    else if (strTrCode == "8203")
                    {
                        //提醒间隔号数 String jhjDm
                        strUrl = string.Format("http://{0}/services/pdjhjWebService_getDtdxtxjghs?wsdl", SessionInfo.strUrl);
                        method = "getDtdxtxjghs";
                        string[] tmp = strParams.Split('|');
                        if (tmp.Length < 1) continue;
                        str = new object[1] { tmp[0] };
                    }
                    else if (strTrCode == "8204")
                    {
                        //短信提醒 String jhjDm,String phone,String dt_hs,String jghm
                        strUrl = string.Format("http://{0}/services/pdjhjWebService_sendMessage?wsdl", SessionInfo.strUrl);
                        method = "sendMessageByPhone";
                        string[] tmp = strParams.Split('|');
                        if (tmp.Length < 4) continue;
                        str = new object[4] { tmp[0], tmp[1], tmp[2], tmp[3] };
                    }
                    else if (strTrCode == "8205")
                    {
                        //获取大厅业务分类信息（新） String jhjDm
                        strUrl = string.Format("http://{0}/services/pdjhjWebService_getDtYwflxxNew?wsdl", SessionInfo.strUrl);
                        method = "getDtYwflxxNew";
                        string[] tmp = strParams.Split('|');
                        //if (tmp.Length < 1) continue;
                        str = new object[1] { tmp[0] };
                    }
                    else if (strTrCode == "_8206")  //智能办税厅接口改造报文规范-20190416 改为调用新接口，smxxjy弃用
                    {
                        //1.21.	实名信息校验 String sfzhm,String xm
                        strUrl = string.Format("http://{0}/services/pdjhjWebService_smxxjy?wsdl", SessionInfo.strUrl);
                        method = "smxxjy";
                        string[] tmp = strParams.Split('|');
                        if (tmp.Length < 2) continue;
                        str = new object[2] { tmp[0], tmp[1] };
                    }
                    else if (strTrCode == "9006")
                    {
                        //暂停办理String jhjDm,String ywflDm,String dtckDm,String swryDm,String hs,Long timestamp
                        strUrl = string.Format("http://{0}/services/pdjhjWebService_updateZtywblsj?wsdl", SessionInfo.strUrl);
                        method = "updateZtywblsj";
                        string[] tmp = strParams.Split('|');
                        if (tmp.Length < 6) continue;
                        str = new object[6] { tmp[0], tmp[1], tmp[2], tmp[3], tmp[4], Convert.ToInt64(tmp[5]) };
                    }
                    else if (strTrCode == "9007")
                    {
                        //恢复办理String jhjDm,String ywflDm,String dtckDm,String swryDm,String hs,Long timestamp
                        strUrl = string.Format("http://{0}/services/pdjhjWebService_updateXbywblsj?wsdl", SessionInfo.strUrl);
                        method = "updateXbywblsj";
                        string[] tmp = strParams.Split('|');
                        if (tmp.Length < 6) continue;
                        str = new object[6] { tmp[0], tmp[1], tmp[2], tmp[3], tmp[4], Convert.ToInt64(tmp[5]) };
                    }
                    else
                    {
                        strBusiParams = "不支持的功能";
                        Fun.WriteRcvInfo(nTaxFlowID, strParamsOut, nBusiStatus, strBusiParams);
                        Thread.Sleep(LOOPDELAYTIME);
                        continue;
                    }
                    FConst.WriteLog(string.Format("发送报文[{0}][{1}]:{2}", strUrl, method, strParams));
                    strLog = string.Format("=>发送报文[{0}]{1}]:{2}", strUrl, method, strParams);
                    BeginInvoke(mi, new Object[] { strLog });
                    WebServiceProxy wsd = new WebServiceProxy(strUrl, "pdjh"); 
                    strParamsOut = (string)wsd.ExecuteQuery(method, str);
                    FConst.WriteLog(string.Format("接收报文[{0}][{1}]:{2}", strUrl, method, strParamsOut));
                    strLog = string.Format("<=接收报文[{0}][{1}]:{2}", strUrl, method, strParamsOut);
                    BeginInvoke(mi, new Object[] { strLog });
                    //开始后续处理 
                    try
                    {
                        if (strTrCode == "3301")    //大厅业务信息同步到中心服务器
                        {
                            nBusiStatus = 1;
                            //cDMResultMsg result = (cDMResultMsg)JsonConvert.DeserializeObject(strParamsOut, typeof(cDMResultMsg));

                        }
                        else if (strTrCode == "3302")    //上传预约叫号流水，由8002发起。
                        {
                            nBusiStatus = 1;
                            //cDMResultMsg result = (cDMResultMsg)JsonConvert.DeserializeObject(strParamsOut, typeof(cDMResultMsg));
                        }
                        else if (strTrCode == "3303")    //上传预约开始办理流水，由8003发起
                        {
                            nBusiStatus = 1;
                            //cDMResultMsg result = (cDMResultMsg)JsonConvert.DeserializeObject(strParamsOut, typeof(cDMResultMsg));
                        }
                        else if (strTrCode == "3304")    //上传预约办理结束流水，由8004发起
                        {
                            nBusiStatus = 1;
                            //cDMResultMsg result = (cDMResultMsg)JsonConvert.DeserializeObject(strParamsOut, typeof(cDMResultMsg));
                        }
                        else if (strTrCode == "3305")    //上传预约评价流水，由8005发起
                        {
                            nBusiStatus = 1;
                            //cDMResultMsg result = (cDMResultMsg)JsonConvert.DeserializeObject(strParamsOut, typeof(cDMResultMsg));
                        }
                        else if (strTrCode == "3203")    //上传预约取票信息，由3201发起。
                        {
                            nBusiStatus = 1;
                            //cDMResultMsg result = (cDMResultMsg)JsonConvert.DeserializeObject(strParamsOut, typeof(cDMResultMsg));
                        }
                        else if (strTrCode == "3202")
                        {
                            //上传实名信息及照片返回结果处理
                            nBusiStatus = 1;
                            XDocument xDocument = XDocument.Parse(strParamsOut);
                            XNamespace xmlns = "http://www.chinatax.gov.cn/spec/";
                            XName xname = xmlns + "service";
                            XElement xmlService = xDocument.Element(xmlns + "service");
                            XElement xmlHead = xmlService.Element(xmlns + "head");
                            XElement xmlRtnMsg = xmlHead.Element(xmlns + "rtn_msg");
                            string head_code = xmlRtnMsg.Element(xmlns + "Code").Value;
                            string head_message = xmlRtnMsg.Element(xmlns + "Message").Value;
                            if (Convert.ToInt32(head_code) == 0)
                            {
                                XElement xmlBody = xmlService.Element(xmlns + "body");

                                XElement xmlCData = XDocument.Parse(xmlBody.Value).Element("root");
                                string body_code = xmlCData.Element("code").Value;
                                string body_message = xmlCData.Element("message").Value;
                            }
                        }
                        else if (strTrCode == "3201")
                        {
                            //上传取票信息
                            //返回结果
                            cResult result = (cResult)JsonConvert.DeserializeObject(strParamsOut, typeof(cResult));
                            if (result.result == "1")
                                nBusiStatus = 1;
                        }
                        else if (strTrCode == "8001")
                        {
                            //登录
                            //返回结果 照片路径url 大厅窗口名称 平均分 税务人员名称
                            cEmp emp = (cEmp)JsonConvert.DeserializeObject(strParamsOut, typeof(cEmp));
                            if (emp.result == "1")
                                nBusiStatus = 1;
                            strBusiParams = string.Format("{0}|{1}|{2}|{3}|{4}|", emp.result, emp.photo, emp.dtckmc, emp.pjnr, emp.username);
                        }
                        else if (strTrCode == "8101")
                        {
                            //获取大厅窗口信息 
                            //dtckList dtckDm(8位窗口代码)，dtckmc(窗口名称)
                            //{"dtckList":[{"dtckmc":"窗口1","dtckDm":"00000001"}]}
                            JObject ja = (JObject)JsonConvert.DeserializeObject(strParamsOut);
                            List<cForm> lForm = JsonConvert.DeserializeObject<List<cForm>>(ja.First.First.ToString());
                            foreach (cForm frm in lForm)
                            {
                                frm.dtckmc = frm.dtckmc.Replace("|", "");
                                strBusiParams += frm.dtckDm + "|" + frm.dtckmc + "|;";
                            }
                            nBusiStatus = 1;
                        }
                        else if (strTrCode == "8102")
                        {
                            //获取大厅业务信息String jhjDm
                            //{"ywflList":[{"xh":1,"ywflmc":"分类1","ywflDm":"00000001"，”sfzjy”:”Y”}]}
                            JObject ja = (JObject)JsonConvert.DeserializeObject(strParamsOut);
                            //List<cBusi> lBusi = JsonConvert.DeserializeObject<List<cBusi>>(ja[0]["ywflList"].ToString());
                            List<cBusi> lBusi = JsonConvert.DeserializeObject<List<cBusi>>(ja.First.First.ToString());
                            foreach (cBusi busi in lBusi)
                            {
                                busi.ywflmc = busi.ywflmc.Replace("|", "");
                                strBusiParams += busi.ywflDm + "|" + busi.ywflmc + "|" + busi.xh + "|" + busi.sfzjy + "|;";
                            }
                            nBusiStatus = 1;
                        }
                        else if (strTrCode == "8103")
                        {
                            //获取窗口业务信息String jhjDm,String dtckDm
                            //{"ywflList":[{"xh":1,"ywflmc":"分类1","ywflDm":"00000001"}]}
                            JObject ja = (JObject)JsonConvert.DeserializeObject(strParamsOut);
                            //List<cBusi> lBusi = JsonConvert.DeserializeObject<List<cBusi>>(ja[0]["ywflList"].ToString());
                            List<cBusi> lBusi = JsonConvert.DeserializeObject<List<cBusi>>(ja.First.First.ToString());
                            foreach (cBusi busi in lBusi)
                            {
                                busi.ywflmc = busi.ywflmc.Replace("|", "");
                                strBusiParams += busi.ywflDm + "|" + busi.ywflmc + "|" + busi.xh + "|;";
                            }
                            nBusiStatus = 1;
                        }
                        else if (strTrCode == "8201")
                        {
                            //取号(同步排队取号)String jhjDm,String ywflDm,String qhhm,Long timestamp
                            //返回结果
                            cResult result = (cResult)JsonConvert.DeserializeObject(strParamsOut, typeof(cResult));
                            if (result.result == "1")
                                nBusiStatus = 1;
                        }
                        else if (strTrCode == "8002")
                        {
                            //叫号String jhjDm,String ywflDm,String dtckDm,String swryDm,String jhhm,Long timestamp
                            //返回结果
                            cResult result = (cResult)JsonConvert.DeserializeObject(strParamsOut, typeof(cResult));
                            if (result.result == "1")
                                nBusiStatus = 1;
                        }
                        else if (strTrCode == "8003")
                        {
                            //开始办理String jhjDm,String ywflDm,String dtckDm,String swryDm,String hs,Long timestamp
                            //返回结果
                            cResult result = (cResult)JsonConvert.DeserializeObject(strParamsOut, typeof(cResult));
                            if (result.result == "1")
                                nBusiStatus = 1;
                        }
                        else if (strTrCode == "8004")
                        {
                            //结束办理String jhjDm,String ywflDm,String dtckDm,String swryDm,String hs,Long timestamp
                            //返回结果
                            cResult result = (cResult)JsonConvert.DeserializeObject(strParamsOut, typeof(cResult));
                            if (result.result == "1")
                                nBusiStatus = 1;
                        }
                        else if (strTrCode == "8005")
                        {
                            //评价String jhjDm,String ywflDm,String dtckxx,String swryDm,String hs,String pjxx,Long timestamp
                            //返回结果
                            cResult result = (cResult)JsonConvert.DeserializeObject(strParamsOut, typeof(cResult));
                            if (result.result == "1")
                                nBusiStatus = 1;
                        }
                        else if (strTrCode == "8014")
                        {
                            //弃号 String jhjDm,String ywflDm,String dtckDm,String swryDm,String hs,Long timestamp
                            //返回结果
                            cResult result = (cResult)JsonConvert.DeserializeObject(strParamsOut, typeof(cResult));
                            if (result.result == "1")
                                nBusiStatus = 1;
                        }
                        else if (strTrCode == "8104")
                        {
                            //排队号码清零String jhjDm
                            //返回结果
                            cResult result = (cResult)JsonConvert.DeserializeObject(strParamsOut, typeof(cResult));
                            if (result.result == "1")
                                nBusiStatus = 1;
                        }
                        else if (strTrCode == "8105")
                        {
                            //获取当前排队取号，叫号信息String jhjDm
                            //{"jhxxList":[{"qhhm":10,"ywflDm":"00000001"}],"qhxxList":[{"jhhm":28,"ywflDm":"00000001"}]}
                            JObject ja = (JObject)JsonConvert.DeserializeObject(strParamsOut);
                            List<cGetTkInfo> lGetInfo = JsonConvert.DeserializeObject<List<cGetTkInfo>>(ja.First.First.ToString());
                            foreach (cGetTkInfo ginfo in lGetInfo)
                            {
                                strBusiParams += ginfo.qhhm + "|" + ginfo.ywflDm + "|;";
                            }
                            strBusiParams += "#";
                            List<cCallTkInfo> lCallInfo = JsonConvert.DeserializeObject<List<cCallTkInfo>>(ja.Next.First.ToString());
                            foreach (cCallTkInfo cinfo in lCallInfo)
                            {
                                strBusiParams += cinfo.jhhm + "|" + cinfo.ywflDm + "|;";
                            }
                            nBusiStatus = 1;
                        }
                        else if (strTrCode == "8106")
                        {
                            //获取系统时间 
                            //{"timestamp":1443173069210}
                            cTime time = (cTime)JsonConvert.DeserializeObject(strParamsOut, typeof(cTime));
                            if (time.timestamp != "")
                                nBusiStatus = 1;
                            strBusiParams = string.Format("{0}|", time.timestamp);
                        }
                        else if (strTrCode == "8107")
                        {
                            //服务启动 String jhjDm,Long timestamp
                            //返回结果
                            cResult result = (cResult)JsonConvert.DeserializeObject(strParamsOut, typeof(cResult));
                            if (result.result == "1")
                                nBusiStatus = 1;
                        }
                        else if (strTrCode == "8108")
                        {
                            //叫号机心跳 String jhjDm,Long timestamp
                            //返回结果
                            cResult result = (cResult)JsonConvert.DeserializeObject(strParamsOut, typeof(cResult));
                            if (result.result == "1")
                                nBusiStatus = 1;
                        }
                        else if (strTrCode == "8109")
                        {
                            //叫号窗口心跳 String jhjDm,String dtckDm,String swryDm,Long timestamp
                            //返回结果
                            cResult result = (cResult)JsonConvert.DeserializeObject(strParamsOut, typeof(cResult));
                            if (result.result == "1")
                                nBusiStatus = 1;
                        }
                        else if (strTrCode == "8110")
                        {
                            //广告信息同步 String jhjDm,String dtckDm,String bbbh
                            //{"result":"1","bbbh":"2","downUrl":"http://127.0.0.1:8080//download.sword?ctrl=Zndt001Ctrl_downLoadGgwj"}
                            cGGInfo gg = (cGGInfo)JsonConvert.DeserializeObject(strParamsOut, typeof(cGGInfo));
                            if (gg.result == "1" || gg.result == "2")
                                nBusiStatus = 1;
                            strBusiParams = string.Format("{0}|{1}|{2}|", gg.result, gg.bbbh, gg.downUrl);
                        }
                        else if (strTrCode == "8202")
                        {
                            //预约取号 String jhjDm,String yzm
                            //{"result":"1","ywflDm":"00000001","timeD":"10001100",” resultMsg”:”预约时间未到”}
                            cYYInfo yy = (cYYInfo)JsonConvert.DeserializeObject(strParamsOut, typeof(cYYInfo));
                            if (yy.result == "1")
                                nBusiStatus = 1;
                            strBusiParams = string.Format("{0}|{1}|{2}|{3}|", yy.result, yy.ywflDm, yy.timeD, yy.resultMsg);
                        }
                        else if (strTrCode == "8203")
                        {
                            //获取叫号提醒间隔号数 
                            //{"result":"1","jghs":3}
                            cTkDiffInfo diff = (cTkDiffInfo)JsonConvert.DeserializeObject(strParamsOut, typeof(cTkDiffInfo));
                            if (diff.result == "1")
                                nBusiStatus = 1;
                            strBusiParams = string.Format("{0}|", diff.jghs);
                        }
                        else if (strTrCode == "8204")
                        {
                            //短信提醒纳税人
                            //{"result":"1"}
                            cResult result = (cResult)JsonConvert.DeserializeObject(strParamsOut, typeof(cResult));
                            if (result.result == "1")
                                nBusiStatus = 1;
                        }
                        else if (strTrCode == "8205")
                        {
                            //获取大厅业务信息String jhjDm
                            //{"ywflList":[{"xh":1,"ywflmc":"分类1","ywflDm":"00000001"，”sfzjy”:”Y”}]}
                            JObject ja = (JObject)JsonConvert.DeserializeObject(strParamsOut);
                            //List<cBusi> lBusi = JsonConvert.DeserializeObject<List<cBusi>>(ja[0]["ywflList"].ToString());
                            List<cBusi> lBusi = JsonConvert.DeserializeObject<List<cBusi>>(ja.First.First.ToString());
                            foreach (cBusi busi in lBusi)
                            {
                                busi.ywflmc = busi.ywflmc.Replace("|", "");
                                strBusiParams += busi.ywflDm + "|" + busi.ywflmc + "|" + busi.xh + "|" + busi.sfzjy + "|" + busi.ywfllx + "|" + busi.sjywfldm + "|;";
                            }
                            nBusiStatus = 1;
                        }
                        else if (strTrCode == "8206")
                        {
                            //实名信息校验
                            //{"result":"1"，”resultMsg”:””未进行实名认证””}
                            cResultMsg result = (cResultMsg)JsonConvert.DeserializeObject(strParamsOut, typeof(cResultMsg));
                            if (result.result == "1")
                                nBusiStatus = 1;
                            strBusiParams = string.Format("{0}|", result.resultMsg);

                        }
                        else if (strTrCode == "9006")
                        {
                            //暂停办理String jhjDm,String ywflDm,String dtckDm,String swryDm,String hs,Long timestamp
                            //返回结果
                            cResult result = (cResult)JsonConvert.DeserializeObject(strParamsOut, typeof(cResult));
                            if (result.result == "1")
                                nBusiStatus = 1;
                        }
                        else if (strTrCode == "9007")
                        {
                            //恢复办理String jhjDm,String ywflDm,String dtckDm,String swryDm,String hs,Long timestamp
                            //返回结果
                            cResult result = (cResult)JsonConvert.DeserializeObject(strParamsOut, typeof(cResult));
                            if (result.result == "1")
                                nBusiStatus = 1;
                        }
                    }
                    catch (Exception se)
                    {
                        strLog = string.Format("<=处理失败1[{0}]:【{1}】{2}【strParams={3}】【{4}】", method, se.Message, se.ToString(), strParams, strUrl);
                        FConst.WriteLog(strLog);
                        BeginInvoke(mi, new Object[] { strLog });
                        nBusiStatus = 99;//网络错误中断

                        if (isRecall == 1) nBusiStatus = 1;
                    }
                    
                }
                catch (Exception se)
                {
                    strLog = string.Format("<=处理失败2[{0}]:【{1}】{2}【strParams={3}】【{4}】", method, se.Message, se.ToString(), strParams, strUrl);
                    FConst.WriteLog(strLog);
                    BeginInvoke(mi, new Object[] { strLog });
                    nBusiStatus = 99;//网络错误中断

                    if (isRecall == 1) nBusiStatus = 1;
                }
                finally
                {
                    try
                    {
                        //回写数据库中的记录
                        Fun.WriteRcvInfo(nTaxFlowID, strParamsOut, nBusiStatus, strBusiParams);
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
                Thread.Sleep(LOOPDELAYTIME);
            }
        }

        /// <summary>  
        /// 国税通讯
        /// </summary>  
        private void SendAndReceiveGTax()
        {
            /* 
            8002	叫号
            8003	开始办理
            8004	结束办理
            8005	评价
            8014	弃号
            8015	业务转移
            8201	取号(同步排队取号)
            8202	预约取号 
            */
            int LOOPDELAYTIME = 1000;
            int nTaxFlowID = 0;
            string strParams = "";
            string strTrCode = "";
            string strLog = "";
            MyInvoke mi = new MyInvoke(UpdateLog);
            while (true)
            {
                try
                {
                    //获取1条信息\
                    Fun.GetSendInfoGTax(out nTaxFlowID, out strTrCode, out strParams);
                    if (nTaxFlowID == 0)
                    {
                        Thread.Sleep(LOOPDELAYTIME);
                        continue;
                    }

                    strLog = string.Format("=>发送国税报文[{0}:{0}]", strTrCode,nTaxFlowID);
                    BeginInvoke(mi, new Object[] { strLog }); 
                    //开始发送
                    StringBuilder responseData = new StringBuilder(); 
                    Encoding code = Encoding.GetEncoding("UTF-8");  
                    //构造请求地址
                    //string strResult = "";
                    //请求远程HTTP 
                    string strUrl = strParams;
                        //设置HttpWebRequest基本信息
                    HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(strUrl);
                    myReq.Timeout = 20000;
                    myReq.Method = "get";
                    myReq.ContentType = "application /x-www-form-urlencoded";

                    //发送POST数据请求服务器
                    HttpWebResponse HttpWResp;

                    HttpWResp = (HttpWebResponse)myReq.GetResponse();
                        
                    
                }
                catch (Exception se)
                {
                    strLog = string.Format("<=发送国税报文失败[{0}:{0}]", strTrCode, nTaxFlowID);
                    BeginInvoke(mi, new Object[] { strLog });
                }
                Thread.Sleep(LOOPDELAYTIME);
            }
        } 

        private void FrmMain_SizeChanged(object sender, EventArgs e)
        {
            //判断是否选择的是最小化按钮 
            if (WindowState == FormWindowState.Minimized)
            {
                //托盘显示图标等于托盘图标对象 
                //注意notifyIcon1是控件的名字而不是对象的名字 
                //隐藏任务栏区图标 
                this.ShowInTaskbar = false;
                //图标显示在托盘区  
                notifyIcon1.Visible = true;
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //判断是否已经最小化于托盘 
            if (WindowState == FormWindowState.Minimized)
            {
                //还原窗体显示 
                WindowState = FormWindowState.Normal;
                //激活窗体并给予它焦点 
                this.Activate();
                //任务栏区显示图标 
                this.ShowInTaskbar = true;
                //托盘区图标隐藏  
                notifyIcon1.Visible = false;
            }
        }


        /// <summary>  
        /// 检查网络状态
        /// </summary>  
        private void checkNetStatusOld()
        {
            /* 
            8106	获取系统时间  
            */ 
            int LOOPTIME = 55000;//断网状态延迟时间   
            string strLog = "";
            int nBusiStatus = 0; 
            MyInvoke mi = new MyInvoke(UpdateLog);
            while (true)
            {
                try
                {
                    //获取1条信息\ 
                    nBusiStatus = 0;
                    //获取系统时间
                    int nTaxFlowID = Fun.getSystemTime(); 
                    string strResult = "";
                    if (nTaxFlowID > 0)
                    {
                        bool bRet = Fun.getTaxResult(nTaxFlowID, out nBusiStatus, out strResult);
                        if (bRet)
                        {
                            if (nBusiStatus == 1)
                            {
                                FConst.WriteLog("网络监控:连接正常");
                                strLog = "<=>网络监控:连接正常";
                                BeginInvoke(mi, new Object[] { strLog });
                            }
                            else
                            {
                                FConst.WriteLog("网络监控:连接异常");
                                strLog = "<=>网络监控:连接异常";
                                BeginInvoke(mi, new Object[] { strLog });
                            }
                            if (nBusiStatus == 1)
                                SessionInfo.nSystemType = 1;
                            else
                                SessionInfo.nSystemType = 0;
                            //更新数据库中的状态
                            Fun.ModifySystemType();
                        }
                    }
                }
                catch (Exception se)
                {
                    //说明网络错误了
                    FConst.WriteErrLog(string.Format("处理失败(网络监测):{0}", se.Message));
                    strLog = string.Format("<=>网络监控失败[{0}]", se.Message);
                    BeginInvoke(mi, new Object[] { strLog });
                }
                 
                Thread.Sleep(LOOPTIME);
            }
        }

        /// <summary>  
        /// 检查网络状态
        /// </summary>  
        private void checkNetStatus()
        {
            /* 
            8106	获取系统时间  
            */
            int LOOPTIME = 55000;//断网状态延迟时间 
            string strParams = "";
            string strUrl = "";
            string method = "";//javaWebService开放的接口   
            string strLog = "";
            string strParamsOut = "";
            int nBusiStatus = 0;
            MyInvoke mi = new MyInvoke(UpdateLog);
            while (true)
            {
                try
                {
                    //获取1条信息\ 
                    nBusiStatus = 0;
                    method = "";
                    object[] str = null;
                    //获取系统时间 
                    strUrl = string.Format("http://{0}/services/pdjhjWebService_getSystemTime?wsdl", SessionInfo.strUrl);
                    method = "getSystemTime";
                    FConst.WriteLog(string.Format("发送报文(网络监测)[{0}][{1}]:{2}", strUrl, method, strParams));
                    strLog = string.Format("=>发送报文(网络监测)[{0}][{1}]:{2}", strUrl, method, strParams);
                    BeginInvoke(mi, new Object[] { strLog });
                    WebServiceProxy wsd = new WebServiceProxy(strUrl, "pdjh");
                    strParamsOut = (string)wsd.ExecuteQuery(method, str);
                    FConst.WriteLog(string.Format("接收报文(网络监测)[{0}][{1}]:{2}", strUrl, method, strParamsOut));
                    strLog = string.Format("<=接收报文(网络监测)[{0}][{1}]:{2}", strUrl, method, strParamsOut);
                    BeginInvoke(mi, new Object[] { strLog });
                    //开始后续处理 
                    try
                    {
                        //获取系统时间 
                        //{"timestamp":1443173069210}
                        cTime time = (cTime)JsonConvert.DeserializeObject(strParamsOut, typeof(cTime));
                        if (time.timestamp != "")
                            nBusiStatus = 1;
                    }
                    catch (Exception se)
                    {
                        FConst.WriteErrLog(string.Format("处理失败(网络监测)[{0}][{1}]:{2}", strUrl, method, se.Message));
                        strLog = string.Format("<=处理失败(网络监测)[{0}][{1}]:{2}", strUrl, method, se.Message);
                        BeginInvoke(mi, new Object[] { strLog });
                    }
                }
                catch (Exception se)
                {
                    //说明网络错误了
                    FConst.WriteErrLog(string.Format("处理失败(网络监测)[{0}][{1}]:{2}", strUrl, method, se.Message));
                    strLog = string.Format("<=处理失败(网络监测)[{0}][{1}]:{2}]", strUrl, method, se.Message);
                    BeginInvoke(mi, new Object[] { strLog });
                }
                finally
                {
                    try
                    {

                        if (nBusiStatus == 1)
                            SessionInfo.nSystemType = 1;
                        else
                            SessionInfo.nSystemType = 0;
                        //更新数据库中的状态
                        Fun.ModifySystemType();
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
                Thread.Sleep(LOOPTIME);
            }
        }
        /// <summary>  
        /// 发送空评价
        /// </summary>  
        private void AppraiseNull()
        {
            /* 
            8005	获取系统时间  
            */
            int LOOPTIME = 50000;//延迟时间  
            string strLog = "";  
            MyInvoke mi = new MyInvoke(UpdateLog);
            while (true)
            {
                try
                {
                    FConst.WriteLog(string.Format("处理未评价信息[]"));
                    strLog = string.Format("<=>处理未评价信息[]");
                    BeginInvoke(mi, new Object[] { strLog });
                    //更新数据库中的状态
                    Fun.ModifyAppraise();
                }
                catch (Exception se)
                {
                    //说明网络错误了
                    FConst.WriteErrLog(string.Format("处理未评价信息[失败]:{0}", se.Message));
                    strLog = string.Format("<=>处理未评价信息[失败]:{0}", se.Message);
                    BeginInvoke(mi, new Object[] { strLog });
                }
                 
                Thread.Sleep(LOOPTIME);
            }
        }
    }
}
