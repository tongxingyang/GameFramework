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
    
    public interface IDebugLogTag
    {
        string LOGTAG { get; }
    }
    
    public interface IDebugerConsole
    {
        void Log(string msg, object context = null);
        void LogWarning(string msg, object context = null);
        void LogError(string msg, object context = null);
    }
    
    public class UnityDebugerConsole : IDebugerConsole
    {
        
        private readonly object[] args = { "" };
        private readonly MethodInfo log;
        private readonly MethodInfo logWarning;
        private readonly MethodInfo logError;
        
        public UnityDebugerConsole()
        {
            Type type = Type.GetType("UnityEngine.Debug, UnityEngine");
            log = type.GetMethod("Log", new Type[] { typeof(object) });
            logWarning = type.GetMethod("LogWarning", new Type[] { typeof(object) });
            logError = type.GetMethod("LogError", new Type[] { typeof(object) });

        }
        
        public void Log(string msg, object context = null)
        {
            args[0] = msg;
            log.Invoke(null, args);
        }

        public void LogWarning(string msg, object context = null)
        {           
            args[0] = msg;
            logWarning.Invoke(null, args);
        }

        public void LogError(string msg, object context = null)
        {
            args[0] = msg;
            logError.Invoke(null, args);
        }
    }
    
    public static class Debuger
    {
        public static bool EnableLog;
        public static bool EnableTime = true;
        public static bool EnableColor = true;
        public static bool EnableSave = true;
        public static bool EnableStack = false;
        public static string LogFileDir = "";
        public static string LogFileName = "";
        public static string Prefix = " >>>  ";
        public static StreamWriter LogFileWriter = null;
        private static IDebugerConsole console;
        private static readonly Dictionary<LogColor, string> colors = new Dictionary<LogColor, string>();
        public static void Init(string logFileDir = null ,IDebugerConsole console = null)
        {
            LogFileDir = logFileDir;
            Debuger.console = console;
            if (string.IsNullOrEmpty(LogFileDir))
            {
                string path = System.AppDomain.CurrentDomain.BaseDirectory; 
                LogFileDir = path + "/DebugerLog/";
            }
            InitColor();
            LogLogHead();
        }
        private static void InitColor()
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
            if (EnableTime)
            {
                DateTime now = DateTime.Now;
                msg = now.ToString("HH:mm:ss.fff") + " " + msg;
            }

            if (console is UnityDebugerConsole && EnableColor)
            {
                msg = string.Format("<color=#{0}>{1}</color>", colors[logColor], msg);
            }
            if (console != null)
            {
                console.Log(msg);
            }
            else
            {
                var old = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(msg);
                Console.ForegroundColor = old;
            }
            
            LogToFile(" [N]: " + msg);

        }
        
        static void Internal_LogWarning(string msg,LogColor logColor = LogColor.Yellow)
        {
            if (EnableTime)
            {
                DateTime now = DateTime.Now;
                msg = now.ToString("HH:mm:ss.fff") + " " + msg;
            }

            if (console is UnityDebugerConsole && EnableColor)
            {
                msg = string.Format("<color=#{0}>{1}</color>", colors[logColor], msg);
            }
            
            if (console != null)
            {
                console.LogWarning(msg);
            }
            else
            {
                var old = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(msg);
                Console.ForegroundColor = old;
            }
            
            LogToFile(" [W]: " + msg);
        }

        static void Internal_LogError(string msg,LogColor logColor = LogColor.Red)
        {
            if (EnableTime)
            {
                DateTime now = DateTime.Now;
                msg = now.ToString("HH:mm:ss.fff") + " " + msg;
            }
            
            if (console is UnityDebugerConsole && EnableColor)
            {
                msg = string.Format("<color=#{0}>{1}</color>", colors[logColor], msg);
            }
            
            if (console != null)
            {
                console.LogError(msg);
            }
            else
            {
                var old = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(msg);
                Console.ForegroundColor = old;
                
            }

            LogToFile(" [E]: " + msg ,true);
        }

        [Conditional("ENABLELOG")]
        public static void Log(object obj,LogColor logColor = LogColor.White)
        {
            if (!EnableLog)
            {
                return;
            }
            string message = GetLogText(GetLogCaller(), obj);
            Internal_Log(Prefix + message,logColor);
        }
        
        [Conditional("ENABLELOG")]
        public static void Log(string message = "",LogColor logColor = LogColor.White)
        {
            if (!EnableLog)
            {
                return;
            }
            message = GetLogText(GetLogCaller(), message);
            Internal_Log(Prefix + message,logColor);
        }

        [Conditional("ENABLELOG")]
        public static void Log(string format,LogColor logColor, params object[] args)
        {
            if (!EnableLog)
            {
                return;
            }

            string message = GetLogText(GetLogCaller(), string.Format(format, args));
            Internal_Log(Prefix + message,logColor);
        }
        
        [Conditional("ENABLELOG")]
        public static void Log(string format, params object[] args)
        {
            if (!EnableLog)
            {
                return;
            }

            string message = GetLogText(GetLogCaller(), string.Format(format, args));
            Internal_Log(Prefix + message);
        }

        [Conditional("ENABLELOG")]
        public static void Log(this IDebugLogTag obj, string message = "",LogColor logColor = LogColor.White)
        {
            if (!EnableLog)
            {
                return;
            }

            message = GetLogText(GetLogTag(obj), GetLogCaller(), message);
            Internal_Log(Prefix + message,logColor);
        }

        [Conditional("ENABLELOG")]
        public static void Log(this IDebugLogTag obj, string format,LogColor logColor , params object[] args)
        {
            if (!EnableLog)
            {
                return;
            }

            string message = GetLogText(GetLogTag(obj), GetLogCaller(), string.Format(format, args));
            Internal_Log(Prefix + message,logColor);
        }
        
        [Conditional("ENABLELOG")]
        public static void Log(this IDebugLogTag obj, string format , params object[] args)
        {
            if (!EnableLog)
            {
                return;
            }

            string message = GetLogText(GetLogTag(obj), GetLogCaller(), string.Format(format, args));
            Internal_Log(Prefix + message);
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
        
        public static void LogWarning(string format, params object[] args)
        {
            string message = GetLogText(GetLogCaller(), string.Format(format, args));
            Internal_LogWarning(Prefix + message);
            
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
        
        public static void LogWarning(this IDebugLogTag obj, string format, params object[] args)
        {
            string message = GetLogText(GetLogTag(obj), GetLogCaller(), string.Format(format, args));
            Internal_LogWarning(Prefix + message);
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

        public static void LogError(string format, params object[] args)
        {
            string message = GetLogText(GetLogCaller(), string.Format(format, args));
            Internal_LogError(Prefix + message);
            
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
        
        public static void LogError(this IDebugLogTag obj, string format, params object[] args)
        {
            string message = GetLogText(GetLogTag(obj), GetLogCaller(), string.Format(format, args));
            Internal_LogError(Prefix + message);
        }


        private static string GetLogText(string tag, string caller, string message)
        {
            return tag + "::" + caller + "() " + message;
        }


        private static string GetLogText(string caller, string message)
        {
            return caller + "() " + message;
        }

        private static string GetLogText(string caller, object message)
        {
            return caller + "() " + (message != null? message.ToListString() :"null");
        }

        private static string ListToString<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                return "null";
            }

            var enumerable = source as T[] ?? source.ToArray();
            if (enumerable.Count() == 0)
            {
                return "[]";
            }

            if (enumerable.Count() == 1)
            {
                return "[" + enumerable.First() + "]";
            }

            var s = "";

            s += enumerable.ButFirst().Aggregate(s, (res, x) => res + ", " + x.ToListString());
            s = "[" + enumerable.First().ToListString() + s + "]";

            return s;
        }

        private static string ToListString(this object obj)
        {
            if (obj is string)
            {
                return obj.ToString();
            }
            var objAsList = obj as IEnumerable;
            return objAsList == null ? obj.ToString() : objAsList.Cast<object>().ListToString();
        }

        private static IEnumerable<T> ButFirst<T>(this IEnumerable<T> source)
        {
            return source.Skip(1);
        }

        private static string GetLogTag(IDebugLogTag obj)
        {
            return obj.LOGTAG;
        }

        private static Assembly debugerAssembly;
        
        private static string GetLogCaller(bool bIncludeClassName = true)
        {
            StackTrace st = new StackTrace(2, false);
            {
                if (null == debugerAssembly)
                {
                    debugerAssembly = typeof(Debuger).Assembly;
                }

                int currStackFrameIndex = 0;
                while (currStackFrameIndex < st.FrameCount)
                {
                    StackFrame oneSf = st.GetFrame(currStackFrameIndex);
                    MethodBase oneMethod = oneSf.GetMethod();
                    
                    if (oneMethod.Module.Assembly != debugerAssembly)
                    {
                        if (bIncludeClassName)
                        {
                            return oneMethod.DeclaringType.Name + "::" + oneMethod.Name;
                        }
                        return oneMethod.Name;
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
                Internal_LogError("GameFrameDebuger :: LogFileDir is NULL!");
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
                Internal_LogError("GameFrameDebuger :: " + e.Message + e.StackTrace);
                return "";
            }

            return LogFileDir;
        }



        internal static string GenLogFileName()
        {
            DateTime now = DateTime.Now;
            string filename = now.GetDateTimeFormats('s')[0];
            filename = filename.Replace("-", "_");
            filename = filename.Replace(":", "_");
            filename = filename.Replace(" ", "");
            filename += ".log";
            return filename;
        }

        private static void LogToFile(string message, bool enableStack = false)
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
                    Internal_LogError("GameFrameDebuger :: " + e.Message + e.StackTrace);
                    return;
                }
            }

            if (LogFileWriter != null)
            {
                try
                {
                    LogFileWriter.WriteLine(message);
                    if (enableStack || EnableStack)
                    {
                        StackTrace st = new StackTrace(2, true);
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