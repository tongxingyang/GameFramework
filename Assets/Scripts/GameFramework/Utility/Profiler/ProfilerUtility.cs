using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace GameFramework.Utility.Profiler
{
    public class ProfilerUtility
    {
        
        [Conditional("DEBUG")]
        public static void BeginSample(string name)
        {
#if UNITY_5_5_OR_NEWER
            UnityEngine.Profiling.Profiler.BeginSample(name);
#else
            UnityEngine.Profiler.BeginSample(name);
#endif
        }
                
        [Conditional("DEBUG")]
        public static void BeginSample(string formatName,params object[] args)
        {
#if UNITY_5_5_OR_NEWER
            UnityEngine.Profiling.Profiler.BeginSample(string.Format(formatName,args));
#else
            UnityEngine.Profiler.BeginSample(name);
#endif
        }
        
        [Conditional("DEBUG")]
        public static void EndSample()
        {
#if UNITY_5_5_OR_NEWER
            UnityEngine.Profiling.Profiler.EndSample();
#else
            UnityEngine.Profiler.EndSample();
#endif
        }
                
        public static IEnumerator SaveProfileFile(float time)
        {
             string file = AppConst.Path.ProfilerLogFilePath + "/profilerfiles_" + DateTime.Now.ToString("yyyyMMddhhmmss") +".log";
             UnityEngine.Profiling.Profiler.logFile = file;
             UnityEngine.Profiling.Profiler.enabled = true;
             UnityEngine.Profiling.Profiler.enableBinaryLog = true;
             yield return new WaitForSecondsRealtime(time);
             UnityEngine.Profiling.Profiler.enableBinaryLog = false;
        } 
        
    }
}