using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Threading;
namespace QManage
{
    public class cEmp
    { 
        public string result { get; set; }
        public string photo { get; set; }
        public string dtckmc { get; set; }
        public string pjnr { get; set; }
        public string username { get; set; }
        public cEmp()
        { 
            //TODO: 在此处添加构造函数逻辑  
            //  
            result = "0";
            photo = "";
            dtckmc = "";
            pjnr = "";
            username = "";
        }
    }
    public class cForm
    {
        public string dtckDm { get; set; } 
        public string dtckmc { get; set; }
        public cForm()
        {
            //TODO: 在此处添加构造函数逻辑  
            //   
            dtckDm = ""; 
            dtckmc = "";
        }
    }
    public class cBusi
    {
        public string ywflDm { get; set; }
        public string ywflmc { get; set; }
        public string xh { get; set; }
        public string sfzjy { get; set; }
        public string ywfllx { get; set; }
        public string sjywfldm { get; set; } 
        public cBusi()
        {
            //TODO: 在此处添加构造函数逻辑  
            //   
            ywflDm = "";
            ywflmc = "";
            xh = "";
            sfzjy = "N";
            ywfllx = "";
            sjywfldm = "";
        }
    }
    public class cGetTkInfo
    {
        public string qhhm { get; set; }
        public string ywflDm { get; set; } 
        public cGetTkInfo()
        {
            //TODO: 在此处添加构造函数逻辑  
            //   
            qhhm = "";
            ywflDm = ""; 
        }
    }
    public class cCallTkInfo
    {
        public string jhhm { get; set; }
        public string ywflDm { get; set; } 
        public cCallTkInfo()
        {
            //TODO: 在此处添加构造函数逻辑  
            //   
            jhhm = "";
            ywflDm = ""; 
        }
    }
    public class cResult
    {
        public string result { get; set; }
        public cResult()
        {
            //TODO: 在此处添加构造函数逻辑  
            //  
            result = "0"; 
        }
    }
    public class cTime
    {
        public string timestamp { get; set; }
        public cTime()
        {
            //TODO: 在此处添加构造函数逻辑  
            //  
            timestamp = "0";
        }
    }
   // "result":"1","bbbh":"2","downUrl
    public class cGGInfo
    {
        public string result { get; set; }
        public string bbbh { get; set; }
        public string downUrl { get; set; } 
        public cGGInfo()
        {
            //TODO: 在此处添加构造函数逻辑  
            //  
            result = "0";
            bbbh = "";
            downUrl = ""; 
        }
    }
    //{"result":"1","ywflDm":"00000001","timeD":"AM"}
    public class cYYInfo
    {
        public string result { get; set; }
        public string ywflDm { get; set; }
        public string timeD { get; set; }
        public string resultMsg { get; set; }
        public cYYInfo()
        {
            //TODO: 在此处添加构造函数逻辑  
            //  
            result = "0";
            ywflDm = "";
            timeD = "";
            resultMsg = "预约失败";
        }
    }
    public class cTkDiffInfo
    {
        public string result { get; set; }
        public string jghs { get; set; }
        public cTkDiffInfo()
        {
            //TODO: 在此处添加构造函数逻辑  
            //   
            result = "";
            jghs = "999";
        }
    }
    public class cResultMsg
    {
        public string result { get; set; }
        public string resultMsg { get; set; }
        public cResultMsg()
        {
            //TODO: 在此处添加构造函数逻辑  
            //  
            result = "0";
            resultMsg = "系统忙,请稍后再试...";
        }
    }
    public class cDMResultMsg
    {
        public int ret_code { get; set; }
        public string ret_text { get; set; }
        public cDMResultMsg()
        {
            //TODO: 在此处添加构造函数逻辑  
            //  
            ret_code = 1;
            ret_text = "系统忙,请稍后再试...";
        }
    }
}
