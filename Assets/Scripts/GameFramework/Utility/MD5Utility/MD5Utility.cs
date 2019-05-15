using System.IO;
using System.Security.Cryptography;
using System.Text;
using GameFramework.Debug;
using GameFramework.Utility.File;

namespace GameFramework.Utility.MD5Utility
{
    public class MD5Utility
    {
        public static string GetHexString(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++) {
                sb.Append(bytes[i].ToString("x2"));
            }
            return sb.ToString();
        }

        public static string ComputeHash(byte[] bytes)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] buffer = md5.ComputeHash(bytes);
            string hash = GetHexString(buffer);
            md5.Clear();
            return hash;
        }

        public static string ComputeHashUTF8(string text)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            string hash = ComputeHash(bytes);
            md5.Clear();
            return hash;
        }

        public static string ComputeHashUnicode(string text)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] bytes = Encoding.Unicode.GetBytes(text);
            string hash = ComputeHash(bytes);
            md5.Clear();
            return hash;
        }

        public static string ComputeHash(Stream sm)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] buffer = md5.ComputeHash(sm);
            string hash = GetHexString(buffer);
            md5.Clear();
            return hash;
        }

        public static string ComputeFileHash(string filename)
        {
            try
            {
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    byte[] retVal = md5.ComputeHash(fs);
                    string str = GetHexString(retVal);
                    return str;
                }
            }
            catch (System.Exception ex)
            {
                Debuger.LogError(ex);
            }

            return string.Empty;
        }
        
        public static string GetFileMD5(string filePath)
        {
            if (FileUtility.IsFileExist(filePath))
            {
                return ComputeHash(FileUtility.ReadFile(filePath));
            }
            return string.Empty;
        }

        public static string GetMD5(byte[] data)
        {
            return ComputeHash(data);
        }

        public static string GetMd5UTF8(string data)
        {
            return ComputeHashUTF8(data);
        }

        public static string GetMd5Unicode(string data)
        {
            return ComputeHashUnicode(data);
        }
        
    }
}