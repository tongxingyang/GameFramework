using System;
using System.Text;

namespace GameFramework.Utility
{
    public class StringUtility
    {
        [ThreadStatic]
        private static StringBuilder s_CachedStringBuilder = new StringBuilder(1024);

        public static string Format(string format, object arg0)
        {
            s_CachedStringBuilder.Length = 0;
            s_CachedStringBuilder.AppendFormat(format, arg0);
            return s_CachedStringBuilder.ToString();
        }

        public static string Format(string format, object arg0, object arg1)
        {
            s_CachedStringBuilder.Length = 0;
            s_CachedStringBuilder.AppendFormat(format, arg0, arg1);
            return s_CachedStringBuilder.ToString();
        }

        public static string Format(string format, object arg0, object arg1, object arg2)
        {
            s_CachedStringBuilder.Length = 0;
            s_CachedStringBuilder.AppendFormat(format, arg0, arg1, arg2);
            return s_CachedStringBuilder.ToString();
        }

        public static string Format(string format, params object[] args)
        {
            s_CachedStringBuilder.Length = 0;
            s_CachedStringBuilder.AppendFormat(format, args);
            return s_CachedStringBuilder.ToString();
        }

        public static string GetFullName<T>(string name)
        {
            return GetFullName(typeof(T), name);
        }

        public static string GetFullName(Type type, string name)
        {
            string typeName = type.FullName;
            return string.IsNullOrEmpty(name) ? typeName : Format("{0}.{1}", typeName, name);
        }
    }
}