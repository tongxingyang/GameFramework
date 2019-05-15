using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Object = UnityEngine.Object;

namespace GameFramework.Debug
{

    public enum LogColor
    {
        White,
        Red,
        Yellow,
        Blue,
        Green,
        Purple,
        Orange
    }
    
    public class UnityDebugerConsole
    {
        public void Log(string msg, Object context = null)
        {
            if(!Debuger.EnableLog) return;
            UnityEngine.Debug.Log(msg,context);
        }

        public void LogWarning(string msg, Object context = null)
        {           
            if(!Debuger.EnableLog) return;
            UnityEngine.Debug.LogWarning(msg,context);
        }

        public void LogError(string msg, Object context = null)
        {
            if(!Debuger.EnableLog) return;
            UnityEngine.Debug.LogError(msg,context);
        }
    }

    public interface IDebugLogTag
    {
        string LOGTAG { get; }
    }
    
    public static class Debuger
    {
        public static bool EnableLog = true;
        public static bool EnableTime = true;
        public static bool EnableColor = true;
        public static bool EnableSave = true;
        public static bool EnableStack = false;
        public static string LogFileDir = "";
        public static string LogFileName = "";
        public static string Prefix = " >>> ";
        public static StreamWriter LogFileWriter = null;
        private static UnityDebugerConsole m_console;
        private static Dictionary<LogColor, string> colors = new Dictionary<LogColor, string>();
        public static void Init(string logFileDir = null)
        {
            LogFileDir = logFileDir;
            m_console = new UnityDebugerConsole();
            if (string.IsNullOrEmpty(LogFileDir))
            {
                string path = System.AppDomain.CurrentDomain.BaseDirectory; 
                LogFileDir = path + "/DebugerLog/";
            }
            InitColor();
            LogLogHead();
        }
        public static void InitColor()
        {
            colors.Add(LogColor.White, "FFFFFF");
            colors.Add(LogColor.Green, "00FF00");
            colors.Add(LogColor.Blue, "99CCFF");
            colors.Add(LogColor.Red, "FF0000");
            colors.Add(LogColor.Yellow, "FFFF00");
            colors.Add(LogColor.Purple, "CC6699");
            colors.Add(LogColor.Orange, "FF9933");
        }
        private static void LogLogHead()
        {
            DateTime now = DateTime.Now;
            string timeStr = now.ToString("HH:mm:ss.fff") + " ";

            Internal_Log("================================================================================");
            Internal_Log("                                 GameFrameDebuger                               ");
            Internal_Log("--------------------------------------------------------------------------------");
            Internal_Log("Time:\t" + timeStr);
            Internal_Log("Path:\t" + LogFileDir);
            Internal_Log("================================================================================");
        }

        static void Internal_Log(string msg,LogColor logColor = LogColor.White)
        {
            if (Debuger.EnableTime)
            {
                DateTime now = DateTime.Now;
                msg = now.ToString("HH:mm:ss.fff") + " " + msg;
            }
            
            LogToFile("[I]" + msg);

            if (Debuger.EnableColor)
            {
                msg = string.Format("<color=#{0}>{1}</color>", colors[logColor], msg);
            }
            m_console?.Log(msg);
        }
        
        static void Internal_LogWarning(string msg,LogColor logColor = LogColor.Yellow)
        {
            if (Debuger.EnableTime)
            {
                DateTime now = DateTime.Now;
                msg = now.ToString("HH:mm:ss.fff") + " " + msg;
            }
            
            LogToFile("[W]" + msg);

            if (Debuger.EnableColor)
            {
                msg = string.Format("<color=#{0}>{1}</color>", colors[logColor], msg);
            }
            
            m_console?.LogWarning(msg);
        }

        static void Internal_LogError(string msg,LogColor logColor = LogColor.Red)
        {
            if (Debuger.EnableTime)
            {
                DateTime now = DateTime.Now;
                msg = now.ToString("HH:mm:ss.fff") + " " + msg;
            }
            
            LogToFile("[E]" + msg,true);

            if (Debuger.EnableColor)
            {
                msg = string.Format("<color=#{0}>{1}</color>", colors[logColor], msg);
            }
            
            m_console?.LogError(msg);

        }


        public static void Log(object obj,LogColor logColor = LogColor.White)
        {
            if (!Debuger.EnableLog)
            {
                return;
            }

            string message = GetLogText(GetLogCaller(), obj);
            Internal_Log(Prefix + message,logColor);
        }

        public static void Log(string message = "",LogColor logColor = LogColor.White)
        {
            if (!Debuger.EnableLog)
            {
                return;
            }

            message = GetLogText(GetLogCaller(), message);
            Internal_Log(Prefix + message,logColor);
        }

        public static void Log(string format,LogColor logColor, params object[] args)
        {
            if (!Debuger.EnableLog)
            {
                return;
            }

            string message = GetLogText(GetLogCaller(), string.Format(format, args));
            Internal_Log(Prefix + message,logColor);
        }



        public static void Log(this IDebugLogTag obj, string message = "",LogColor logColor = LogColor.White)
        {
            if (!Debuger.EnableLog)
            {
                return;
            }

            message = GetLogText(GetLogTag(obj), GetLogCaller(), message);
            Internal_Log(Prefix + message,logColor);
            
        }

        public static void Log(this IDebugLogTag obj, string format,LogColor logColor , params object[] args)
        {
            if (!Debuger.EnableLog)
            {
                return;
            }

            string message = GetLogText(GetLogTag(obj), GetLogCaller(), string.Format(format, args));
            Internal_Log(Prefix + message,logColor);
            
        }

        public static void LogWarning(object obj,LogColor logColor = LogColor.Yellow )
        {
            string message = GetLogText(GetLogCaller(), obj);
            Internal_LogWarning(Prefix + message,logColor);
        }

        public static void LogWarning(string message,LogColor logColor = LogColor.Yellow )
        {
            message = GetLogText(GetLogCaller(), message);
            Internal_LogWarning(Prefix + message,logColor);
        }

        public static void LogWarning(string format,LogColor logColor, params object[] args)
        {
            string message = GetLogText(GetLogCaller(), string.Format(format, args));
            Internal_LogWarning(Prefix + message,logColor);
            
        }

        public static void LogWarning(this IDebugLogTag obj, string message,LogColor logColor = LogColor.Yellow )
        {
            message = GetLogText(GetLogTag(obj), GetLogCaller(), message);
            Internal_LogWarning(Prefix + message,logColor);
            
        }


        public static void LogWarning(this IDebugLogTag obj, string format, LogColor logColor, params object[] args)
        {
            string message = GetLogText(GetLogTag(obj), GetLogCaller(), string.Format(format, args));
            Internal_LogWarning(Prefix + message,logColor);
            
        }

        public static void LogError(object obj,LogColor logColor = LogColor.Red)
        {
            string message = GetLogText(GetLogCaller(), obj);
            Internal_LogError(Prefix + message,logColor);

        }

        public static void LogError(string message,LogColor logColor = LogColor.Red)
        {
            message = GetLogText(GetLogCaller(), message);
            Internal_LogError(Prefix + message,logColor);
            
        }

        public static void LogError(string format,LogColor logColor, params object[] args)
        {
            string message = GetLogText(GetLogCaller(), string.Format(format, args));
            Internal_LogError(Prefix + message,logColor);
            
        }


        public static void LogError(this IDebugLogTag obj, string message,LogColor logColor = LogColor.Red)
        {
            message = GetLogText(GetLogTag(obj), GetLogCaller(), message);
            Internal_LogError(Prefix + message,logColor);
            
        }


        public static void LogError(this IDebugLogTag obj, string format,LogColor logColor, params object[] args)
        {
            string message = GetLogText(GetLogTag(obj), GetLogCaller(), string.Format(format, args));
            Internal_LogError(Prefix + message,logColor);
            
        }


        private static string GetLogText(string tag, string methodName, string message)
        {
            return tag + "::" + methodName + "() " + message;
        }


        private static string GetLogText(string caller, string message)
        {
            return caller + "() " + message;
        }

        private static string GetLogText(string caller, object message)
        {
            return caller + "() " + (message != null? message.ToListString() :"null");
        }

        #region Object 2 ListString 
        /// <summary>
        /// 将容器序列化成字符串
        /// 格式：{a, b, c}
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        private static string ListToString<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                return "null";
            }

            if (source.Count() == 0)
            {
                return "[]";
            }

            if (source.Count() == 1)
            {
                return "[" + source.First() + "]";
            }

            var s = "";

            s += source.ButFirst().Aggregate(s, (res, x) => res + ", " + x.ToListString());
            s = "[" + source.First().ToListString() + s + "]";

            return s;
        }


        /// <summary>
        /// 将容器序列化成字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static string ToListString(this object obj)
        {
            if (obj is string)
            {
                return obj.ToString();
            }
            else
            {
                var objAsList = obj as IEnumerable;
                return objAsList == null ? obj.ToString() : objAsList.Cast<object>().ListToString();
            }
        }

        private static IEnumerable<T> ButFirst<T>(this IEnumerable<T> source)
        {
            return source.Skip(1);
        }

        #endregion 


        private static string GetLogTag(IDebugLogTag obj)
        {
            return obj.LOGTAG;
        }

        private static Assembly ms_Assembly;
        
        private static string GetLogCaller(bool bIncludeClassName = true)
        {
            StackTrace st = new StackTrace(2, false);
            if (st != null)
            {
                if (null == ms_Assembly)
                {
                    ms_Assembly = typeof(Debuger).Assembly;
                }

                int currStackFrameIndex = 0;
                while (currStackFrameIndex < st.FrameCount)
                {
                    StackFrame oneSf = st.GetFrame(currStackFrameIndex);
                    MethodBase oneMethod = oneSf.GetMethod();
                    

                    if (oneMethod.Module.Assembly != ms_Assembly)
                    {
                        if (bIncludeClassName)
                        {
                            return oneMethod.DeclaringType.Name + "::" + oneMethod.Name;
                        }
                        else
                        {
                            return oneMethod.Name;
                        }
                    }

                    currStackFrameIndex++;
                }

            }

            return "";
        }


        internal static string CheckLogFileDir()
        {
            if (string.IsNullOrEmpty(LogFileDir))
            {
                Internal_LogError("GameFrameDebuger:: LogFileDir is NULL!");
                return "";
            }

            try
            {
                if (!Directory.Exists(LogFileDir))
                {
                    Directory.CreateDirectory(LogFileDir);
                }
            }
            catch (Exception e)
            {
                Internal_LogError("GameFrameDebuger:: " + e.Message + e.StackTrace);
                return "";
            }

            return LogFileDir;
        }



        internal static string GenLogFileName()
        {
            DateTime now = DateTime.Now;
            string filename = now.GetDateTimeFormats('s')[0].ToString();
            filename = filename.Replace("-", "_");
            filename = filename.Replace(":", "_");
            filename = filename.Replace(" ", "");
            filename += ".log";

            return filename;
        }



        private static void LogToFile(string message, bool EnableStack = false)
        {
            if (!EnableSave)
            {
                return;
            }

            if (LogFileWriter == null)
            {
                LogFileName = GenLogFileName();
                LogFileDir = CheckLogFileDir();
                if (string.IsNullOrEmpty(LogFileDir))
                {
                    return;
                }

                string fullpath = LogFileDir + LogFileName;
                try
                {
                    LogFileWriter = File.AppendText(fullpath);
                    LogFileWriter.AutoFlush = true;
                }
                catch (Exception e)
                {
                    LogFileWriter = null;
                    Internal_LogError("GameFrameDebuger:: " + e.Message + e.StackTrace);
                    return;
                }
            }

            if (LogFileWriter != null)
            {
                try
                {
                    LogFileWriter.WriteLine(message);
                    if ((EnableStack || Debuger.EnableStack))
                    {
                        StackTrace st = new StackTrace(2, true);
                        if (st != null)
                        {
                            int currStackFrameIndex = 0;
                            while (currStackFrameIndex < st.FrameCount)
                            {
                                StackFrame oneSf = st.GetFrame(currStackFrameIndex);
                                MethodBase oneMethod = oneSf.GetMethod();
                                LogFileWriter.WriteLine("文件名: " + oneSf.GetFileName() + " 行号: " +
                                                        oneSf.GetFileLineNumber() + " 函数名: " + oneMethod.Name);
                                currStackFrameIndex++;
                            }

                        }
                    }
                }
                catch (Exception)
                {
                    return;
                }
            }
        }
    }
}