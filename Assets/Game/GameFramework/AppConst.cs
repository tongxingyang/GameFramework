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
        
        public class LaunchConfig
        {
            public static bool IsShowGameSplash = true;
            public static bool IsShowGameMonition = true;
            public static bool IsPlayGameSplashVideo = true;
            public static string GameSplashVideoName = String.Empty;
        }
        
        public class Path
        {
            public static readonly string DebugerLogFilePath = Application.dataPath+"/Log/";
            public static readonly string ProfilerLogFilePath = "";
            public static readonly string InstallDataPath = Application.streamingAssetsPath;
            public static readonly string PresistentDataPath = Application.persistentDataPath;
            public static readonly string HotUpdateDownloadDataPath = Application.temporaryCachePath;
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
        
        
        public class VideoConfig
        {
            public static int CGVideoWidth = 1280;
            public static int CGVideoHeight = 720;
            public static int UIVideoWidth = 1280;
            public static int UIVideoHeight = 960;
        }

        public class PlayerPrefsConfig
        {
            public static bool UseSecure = true;
            public static int Iterations = 555;
            public static string Password = "txy!1234";
            public static string Salt = "txy!1234";
        }
        
        public static class AssetBundleConfig
        {
            public static string BundleSuffix = "assetbundle";
            public static string AssetBundlePackageFile = "AssetBundle.zip";
            public static bool LuaEncrypt = true;
            public static string VersionFile = "version.dat";
            public static string FileListFile = "filelist.dat";
            public static bool ResetAssetBundleName = true;
            public static bool EnableAssetBundleRedundance = true;
            public static string IsInstanllDecompressFileName = "UpdateFirstDecompressVersion.txt";
            public static string HasUpdateFileName = "HasUpdate.txt";
        }

        public static class UpdateConfig
        {
            public static bool OpenHotUpdate = true;
        }
        
        public static class ResourceConfig
        {
            public static bool IsUseAssetBundle = false;
            public static int AssetDatabaseDelayFrame = 10;
        }
    }
}