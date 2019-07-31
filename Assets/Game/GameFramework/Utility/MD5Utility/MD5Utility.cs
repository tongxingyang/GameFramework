using System.IO;
using System.Security.Cryptography;
using System.Text;
using GameFramework.Debug;
using GameFramework.Utility.File;

namespace GameFramework.Utility.MD5Utility
{
    public class MD5Utility
    {
        private static MD5CryptoServiceProvider md5 = null;
        public static MD5CryptoServiceProvider Md5 => md5 ?? (md5 = new MD5CryptoServiceProvider());

        private static StringBuilder builder = null;
        public static StringBuilder Builder => builder ?? (builder = new StringBuilder());
        
        private static string GetHexString(byte[] bytes)
        {
            Builder.Length = 0;
            Builder.Clear();
            for (int i = 0; i < bytes.Length; i++) {
                Builder.Append(bytes[i].ToString("x2"));
            }
            return Builder.ToString();
        }

        public static string ComputeHash(byte[] bytes)
        {
            byte[] buffer = Md5.ComputeHash(bytes);
            string hash = GetHexString(buffer);
            Md5.Clear();
            return hash;
        }
        
        public static string ComputeHash(Stream sm)
        {
            byte[] buffer = Md5.ComputeHash(sm);
            string hash = GetHexString(buffer);
            Md5.Clear();
            return hash;
        }

        public static string ComputeHashUtf8(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            string hash = ComputeHash(bytes);
            return hash;
        }

        public static string ComputeHashUnicode(string text)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(text);
            string hash = ComputeHash(bytes);
            return hash;
        }

        public static string ComputeFileHash(string filename)
        {
            try
            {
                using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    return ComputeHash(fs);
                }
            }
            catch (System.Exception ex)
            {
                Debuger.LogError(ex);
            }

            return string.Empty;
        }
        
        public static string GetFileMd5(string filePath)
        {
            if (FileUtility.IsFileExist(filePath))
            {
                return ComputeHash(FileUtility.ReadFile(filePath));
            }
            return string.Empty;
        }

        public static string GetMd5(byte[] data)
        {
            return ComputeHash(data);
        }

        public static string GetMd5Utf8(string data)
        {
            return ComputeHashUtf8(data);
        }

        public static string GetMd5Unicode(string data)
        {
            return ComputeHashUnicode(data);
        }
        
        public static string GetMd5Stream(Stream sm)
        {
            return ComputeHash(sm);
        }
        
        public static byte[] GetMd5Bytes(byte[] bytes)
        {
            return Md5.ComputeHash(bytes);
        }

        public static byte[] GetMd5Bytes(byte[] bytes, int offset, int length)
        {
            return Md5.ComputeHash(bytes, offset, length);
        }

        public static byte[] GetMd5Bytes(Stream stream)
        {
            return Md5.ComputeHash(stream);
        }
    }
}