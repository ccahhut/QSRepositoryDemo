using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
namespace QManage
{
    /// <summary>
    /// IniFile 的摘要说明。
    /// </summary>
    public class IniFile
    {
        private string fileName;
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileInt(
            string lpAppName,
            string lpKeyName,
            int nDefault,
            string lpFileName
            );
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(
            string lpAppName,
            string lpKeyName,
            string lpDefault,
            StringBuilder lpReturnedString,
            int nSize,
            string lpFileName
            );
        [DllImport("kernel32")]
        private static extern bool WritePrivateProfileString(
            string lpAppName,
            string lpKeyName,
            string lpString,
            string lpFileName
            );
        public IniFile(string filename)
        {
            fileName = filename;
        }
        public int GetInt(string section, string key, int def)
        {
            return GetPrivateProfileInt(section, key, def, fileName);
        }
        public string GetString(string section, string key, string def)
        {
            StringBuilder temp = new StringBuilder(1024);
            GetPrivateProfileString(section, key, def, temp, 1024, fileName);
            return temp.ToString();
        }
        public void WriteInt(string section, string key, int iVal)
        {
            WritePrivateProfileString(section, key, iVal.ToString(), fileName);
        }
        public void WriteString(string section, string key, string strVal)
        {
            WritePrivateProfileString(section, key, strVal, fileName);
        }
        public void DelKey(string section, string key)
        {
            WritePrivateProfileString(section, key, null, fileName);
        }
        public void DelSection(string section)
        {
            WritePrivateProfileString(section, null, null, fileName);
        }
    }
}
