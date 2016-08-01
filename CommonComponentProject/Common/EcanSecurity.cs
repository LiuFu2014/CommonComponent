using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// 一些加密算法
    /// </summary>
    public class EcanSecurity
    {
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="str">要加密的字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string MD5_Encode(string str)
        {
            MD5 m = new MD5CryptoServiceProvider();
            byte[] data = Encoding.Default.GetBytes(str);
            byte[] result = m.ComputeHash(data);
            string ret1 = string.Empty;
            try
            {
                for (int j = 0; j < result.Length; j++)
                {
                    ret1 += result[j].ToString("x").PadLeft(2, '0');
                }
                return ret1;

            }
            catch (Exception)
            {
                return str;
            }

        }

        /// <summary>
        /// 简单加密函数
        /// </summary>
        /// <param name="str">要加密的字符串</param>
        /// <returns>返回加密后的字符串</returns>
        /// 
        public static string Simple_Encode(string str)
        {
            string s = "";
            try
            {
                for (int i = 0; i < str.Length; i++)
                {
                    s += (char)(str[i] + 10 - 1 * 2);
                }
                return s;
            }
            catch
            {
                return str;
            }
        }

        /// <summary>
        /// 简单解密函数
        /// </summary>
        /// <param name="str">要解密的字符串</param>
        /// <returns>返回解密后的字符串</returns>
        /// 
        public static string Simple_Decode(string str)
        {
            string s = "";
            try
            {
                for (int i = 0; i < str.Length; i++)
                {
                    s += (char)(str[i] - 10 + 1 * 2);
                }
                return s;
            }
            catch
            {
                return str;
            }
        }

        //默认密钥向量
        private static byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        /// <summary>
        /// 对称加密法加密函数
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥,要求为8位</param>
        /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
        public static string Symmetry_Encode(string encryptString, string encryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray());
            }
            catch
            {
                return "加密密钥有误，无法加密！";
            }
        }

        /// <summary>
        /// 对称加密法解密函数
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
        /// <returns>解密成功返回解密后的字符串，失败返源串</returns>
        public static string Symmetry_Decode(string decryptString, string decryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey);
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return "解密密钥有误，无法解密！";
            }
        }

    }
}
