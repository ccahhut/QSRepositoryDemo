using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Globalization;
namespace Common
{
    /**//// <summary>
    /// DESEncrypt加密解密算法。
    /// </summary>
    public sealed class DESEncrypt
    {
        private DESEncrypt()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        private static string key = "CQWeIZhe";

        /**//// <summary>
        /// 对称加密解密的密钥
        /// </summary>
        public static string Key
        {
            get
            {
                return key;
            }
            set
            {
                key = value;
            }
        }

        /**//// <summary>
        /// DES加密
        /// </summary>
        /// <param name="encryptString"></param>
        /// <returns></returns>
        public static string DesEncrypt(string encryptString)
        {
            try
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(key.Substring(0, 8));
                byte[] keyIV = keyBytes;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
                 
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, provider.CreateEncryptor(keyBytes, keyIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                BitConverter.ToString(mStream.ToArray());
                return BitConverter.ToString(mStream.ToArray()).Replace("-", "");
            }
            catch
            {
                return "";
            }

			//string.
        }

        /**//// <summary>
        /// DES解密
        /// </summary>
        /// <param name="decryptString"></param>
        /// <returns></returns>
        public static string DesDecrypt(string decryptString)
        {
            try
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(key.Substring(0, 8));
                byte[] keyIV = keyBytes;
                string strTmp = "";
                for (int i = 0; i < decryptString.Length / 2; i++)
                {
                    strTmp += decryptString.Substring(i * 2, 2) + "-";
                }
                strTmp = strTmp.Substring(0, strTmp.Length - 1);
                string[] sInput = strTmp.Split("-".ToCharArray());
                byte[] inputByteArray = new byte[sInput.Length];
                for (int i = 0; i < sInput.Length; i++)
                {
                    inputByteArray[i] = byte.Parse(sInput[i], NumberStyles.HexNumber);
                }

                //byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, provider.CreateDecryptor(keyBytes, keyIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return "";
            }
        }
    }
}