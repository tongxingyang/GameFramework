using UnityEngine;

namespace GameFramework.Tool
{
    public class MobileSystemInfo
    {
        public static bool NetAvailable => Application.internetReachability != NetworkReachability.NotReachable;

        public static bool IsWifi => Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;

        public static bool IsCarrier => Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork;
    }
}