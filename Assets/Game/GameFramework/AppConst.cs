﻿using UnityEngine;

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
            public static readonly bool IsDebugMode = true;
            public static readonly bool IsShowFpsCounter = true;
            public static readonly bool IsShowMemoryDetector = true;
            public static readonly bool DebugerEnableLog = true;
            public static readonly bool DebugerEnableColor = true;
            public static readonly bool DebugerEnableSave = false;
        }
        public class UIConfig
        {
            public static readonly Vector2 GameResolution = new Vector2(1136, 640);
        }
    }
}