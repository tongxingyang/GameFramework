using System;
using System.IO;
using System.Text;

namespace GameFramework.Editor.Core.AssetImportSetting
{
    public class TextLogger
    {
        public static TextLogger instance = null;
        private static readonly object _lock = new object();
        private StringBuilder stringBuilder = new StringBuilder();
        private static string strPath = "";

        private TextLogger()
        {
        }

        public string Path
        {
            set { strPath = value; }
            get { return strPath; }
        }

        public static TextLogger Instance => instance ?? (instance = new TextLogger());

        public void WriteLine(string message)
        {
            lock (_lock)
            {
                using (StreamWriter sw =  File.Exists(strPath) ? File.AppendText(strPath) : File.CreateText(strPath))
                {
                    stringBuilder.Length = 0;
                    stringBuilder.AppendFormat("[{0}]\t{1}", System.DateTime.Now.TimeOfDay, message);
                    sw.WriteLine(stringBuilder.ToString());
                }
            }
        }
    }
}