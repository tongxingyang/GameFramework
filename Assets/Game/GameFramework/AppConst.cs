using UnityEngine;

namespace GameFramework
{
    public class AppConst
    {
        public class Path
        {
            public static readonly string DebugerLogFilePath = Application.dataPath+"/Log/";
            public static readonly string ProfilerLogFilePath = "";
        }
        public class GameConfig
        {
            public static readonly bool DebugerEnableLog = true;
            public static readonly bool DebugerEnableColor = true;
            public static readonly bool DebugerEnableSave = false;
        }
    }
}