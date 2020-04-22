using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace QManage
{
    public  class cParamInfo
    {
        public string strEmpNo;
        public string strEmpName;
        public string strEmpPwd;
        public string strEmpNewPwd;
        public string strEmpLevel;
        public string strFormNo;
        public string strFormName;
        public string strBusiNo;
        public string strNewBusiNo;
        public string strBusiName;
        public string strCallerAddr;
        public string strIPAddr;
        public string strParamInfo;
        public string strTicketNo;
        public int    nTaxFlowID;
        public int nScore;
        public int nWaitNum;
        public string strOutInfo;
        public string strExtend1;
        public string strExtend2;
        public string strExtend3;
        public string strTrcode;
        public int nRetCode;
        public string strRetText;
        public cParamInfo()
        {
            strEmpNo = "";
            strEmpName = "";
            strEmpPwd = "";
            strEmpNewPwd = "";
            strEmpLevel = "";
            strFormNo = "";
            strFormName = "";
            strBusiNo = "";
            strNewBusiNo = "";
            strBusiName = "";
            strCallerAddr = "";
            strIPAddr = "";
            strParamInfo = "";
            strTicketNo = "";
            nTaxFlowID = 0;
            nScore=0;
            nWaitNum=0;
            strOutInfo = "";
            strExtend1 = "";
            strExtend2 = "";
            strExtend3 = "";
            strTrcode = "";
            nRetCode = 99;
            strRetText = "系统忙,请稍后再试!";
        }
    }
 
}
