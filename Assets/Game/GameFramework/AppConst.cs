using System;
using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    public class AppConst
    {
        public class GlobalCahce
        {
            public static Camera Main3DCamera;
            public static Camera UICamera;
            public static Transform UIRootCanvas;
            public static Transform PanelRoot;
        }
        
        public class Path
        {
            public static readonly string DebugerLogFilePath = Application.dataPath+"/Log/";
            public static readonly string ProfilerLogFilePath = "";
            public static readonly string InstallDataPath = Application.streamingAssetsPath;
            public static readonly string PresistentDataPath = Application.persistentDataPath;
        }
        
        public class GameConfig
        {
            public static readonly bool IsDebugMode = true;
            public static readonly bool IsShowDevelopInfo = true;
            public static readonly bool DebugerEnableLog = true;
            public static readonly bool DebugerEnableColor = true;
            public static readonly bool DebugerEnableSave = false;
        }
        
        public class UiConfig
        {
            public static readonly Vector2 GameResolution = new Vector2(1136, 640);
        }

        public class PlayerPrefsConfig
        {
            public static bool UseSecure = true;
            public static int Iterations = 555;
            public static string Password = "txy!123";
            public static string Salt = "txy";
        }
        
        public static class AssetBundleConfig
        {
            public static string BundleSuffix => "assetbundle";
            public static string AssetBundlePackageFile => "AssetBundle.zip";
            public static bool LuaEncrypt = true;
            public static string VersionFile => "version.dat";
            public static string FileListFile => "filelist.dat";
            public static bool ResetAssetBundleName = true;
            public static bool EnableAssetBundleRedundance = true;
        }

        public static class UpdateConfig
        {
            public static bool OpenHotUpdate = true;
        }
    }
}