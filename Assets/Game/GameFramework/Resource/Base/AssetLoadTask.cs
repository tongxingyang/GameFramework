using System;
using System.Collections.Generic;
using GameFramework.Pool.TaskPool;

namespace GameFramework.Res.Base
{
    public class AssetLoadTask : ITask
    {
        private static int Serial = 0;
        public int SerialId { get; set; }
        public bool Done { get; set; }
        public int Priority { get; set; }
        public enResouceLoadType LoadType = enResouceLoadType.AssetBundle;
        public enResourceLoadMethod LoadMethod = enResourceLoadMethod.LoadFromFile;
        public enResourceLoadMode LoadMode = enResourceLoadMode.Async;
        public Type ResType;
        public Action<bool, Object> CallBack = null;
        public string AssetPath = String.Empty;
        public string BundlePath = String.Empty;
        public List<string> Dependencies = null;
        public int LoadedDependenciesCount = 0;
        public void Clear()
        {
            SerialId = -1;
            Done = false;
            Priority = 0;
            ResType = null;
            CallBack = null;
            AssetPath = null;
            BundlePath = null;
            Dependencies?.Clear();
            Dependencies = null;
            LoadedDependenciesCount = 0;
        }

        public AssetLoadTask(string assetPath,string bundlePath, Type type,enResouceLoadType loadType,enResourceLoadMethod loadMethod,enResourceLoadMode loadMode,Action<bool,Object> callBack,int priority)
        {
            SerialId = Serial++;
            AssetPath = assetPath;
            BundlePath = bundlePath;
            ResType = type;
            LoadType = loadType;
            LoadMethod = loadMethod;
            LoadMode = loadMode;
            CallBack = callBack;
            Priority = priority;
            LoadedDependenciesCount = 0;
        }
        
        public AssetLoadTask(string assetPath,Type type,enResouceLoadType loadType,enResourceLoadMethod loadMethod,enResourceLoadMode loadMode,Action<bool,Object> callBack,int priority)
        {
            SerialId = Serial++;
            AssetPath = assetPath;
            BundlePath = String.Empty;
            ResType = type;
            LoadType = loadType;
            LoadMethod = loadMethod;
            LoadMode = loadMode;
            CallBack = callBack;
            Priority = priority;
            LoadedDependenciesCount = 0;
        }
    }
}