using System.Runtime.InteropServices;

namespace GameFramework.Tool.NativeCall
{
    public static class InvokeIphone
    {
        
        [DllImport("__Internal")]
        private static extern void CallIphoneMethord(string methodName, string[] str, int length);

        [DllImport("__Internal")]
        private static extern string CallIphoneMethordStr(string methodName, string[] str, int length);

        public static void CallIphoneStaticMethod(string className, string methodName, params string[] methodParams)
        {
#if UNITY_IOS
            int length = methodParams.Length;
            CallIphoneMethord(methodName, methodParams, length);
#endif
        }

        public static string CallIphoneStrStaticMethod(string className, string methodName, params string[] methodParams)
        {
            string str = string.Empty;
#if UNITY_IOS
            int length = methodParams.Length;
            str = CallIphoneMethordStr(methodName, methodParams, length);
#endif
            return str;
        }
    }
}