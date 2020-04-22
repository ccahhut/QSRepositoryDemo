using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
namespace QManage
{
    
    public static class Busi
    {
        
        //反转字符串
        private static string ReverseString(string original)
        {
            char[] arr = original.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        } 
 
    }
}