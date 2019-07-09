using UnityEngine;

namespace GameFramework.Tool.NativeCall
{
    public static class InvokeAndroid
    {
        public static void CallAndroidStaticMethod(string className, string methodName, params object[] methodParams)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaClass androidJavaClass = null;
            using (androidJavaClass = new AndroidJavaClass(className))
            {
                androidJavaClass.CallStatic(methodName,methodParams);
            }
            androidJavaClass.Dispose();
#endif
        }
        
        public static T CallAndroidStaticMethod<T>(string className,string methodName,params object[] methodParams)
        {
            T ret = default(T);
#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaClass androidJavaClass = null;
            using (androidJavaClass = new AndroidJavaClass(className))
            {
                ret = androidJavaClass.CallStatic<T>(methodName,methodParams);
            }
            androidJavaClass.Dispose();
#endif
            return ret;
        }

        public static void CallAndroidMethod(string className, string methodName, params object[] methodParams)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaObject androidJavaObject = null;
            using (androidJavaObject = new AndroidJavaObject(className))
            {
                androidJavaObject.Call(methodName,methodParams);
            }
            androidJavaObject.Dispose();
#endif
        }

        public static T CallAndroidMethod<T>(string className, string methodName, params object[] methodParams)
        {
            T ret = default(T);
#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaObject androidJavaObject = null;
            using (androidJavaObject = new AndroidJavaObject(className))
            {
                ret = androidJavaObject.Call<T>(methodName,methodParams);
            }
            androidJavaObject.Dispose();
#endif
            return ret;
        }
        
    }
}