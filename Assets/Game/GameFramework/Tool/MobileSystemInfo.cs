using UnityEngine;

namespace GameFramework.Tool
{
    public class MobileSystemInfo
    {
        public static bool NetAvailable => Application.internetReachability != NetworkReachability.NotReachable;

        public static bool IsWifi => Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;

        public static bool IsCarrier => Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork;
        
        public static long GetFreeDiskSpace()
        {
#if UNITY_ANDROID

#elif UNITY_IPHONE && !UNITY_EDITOR

#endif
            return 1024 * 1024 * 1024;
        }
    }
}