#if UNITY_EDITOR

using System;
using GameFramework.Pool.ReferencePool;
using UnityEditor;

namespace GameFramework.Res.Base
{
    public class AssetDatabaseLoader : IReference
    {
        public AssetDatabaseModule.OnLoadSelfBundle OnLoadSelfBundleAction;
        public AssetBundleLoader.OnLoadAllBundle OnLoadAllBundleAction;
        
        public enResourceLoadMode LoadMode;
        public enResourceLoadCache LoadCache;
        public enResourceLoadMethod LoadMethod;
        public enLoadState LoadState;
        public object UserData;
        
        public AssetDatabaseInfo BundleInfo;
        public string AssetBundleName { get; set; }

        public AssetDatabaseLoader()
        {
            AssetBundleName = String.Empty;
            OnLoadSelfBundleAction = null;
            OnLoadAllBundleAction = null;
            LoadMethod = enResourceLoadMethod.LoadFromFile;
            LoadState = enLoadState.None;
            LoadMode = enResourceLoadMode.Sync;
            BundleInfo = null;
        }

        public void StartLoadResource()
        {
            if (LoadState == enLoadState.None)
            {
                LoadState = enLoadState.Waiting;
                LoadAsset();
            }
        }

        public void LoadAsset()
        {
            LoadState = enLoadState.Loading;
            LoadAssetSync();
        }

        public void LoadAssetSync()
        {
            var assetPath = AssetDatabase.GetAssetPathsFromAssetBundle(AssetBundleName);
            if (assetPath.Length != 0)
            {
                BundleInfo = CreateAssetDatabaseInfo(AssetBundleName, assetPath);
                BundleInfo.UpdateLastUsedTime();
            }
            LoadState = enLoadState.SelfComplete;
            OnLoadSelfBundleAction?.Invoke(this);
            LoadState = enLoadState.AllComplete;
            BundleInfo.IsReady = true;
            OnLoadAllBundleAction?.Invoke(BundleInfo,UserData);
            OnLoadAllBundleAction = null;
            ReferencePool.Release<AssetDatabaseLoader>(this);
        }

        private AssetDatabaseInfo CreateAssetDatabaseInfo(string name, string[] assetPath)
        {
            var abinfo = ReferencePool.Acquire<AssetDatabaseInfo>();
            abinfo.AssetBundleName = name;
            abinfo.AssetsPath = assetPath;
            abinfo.OnBundleUnloadAction = OnBundleUnloadAction;
            return abinfo;
        }
        
        public  void OnBundleUnloadAction(AbstractAssetInfo info)
        {
            var abinfo = info as AssetDatabaseInfo;
            ReferencePool.Release<AssetDatabaseInfo>(abinfo);
        }
        
        public void Reset()
        {
            AssetBundleName = String.Empty;
            OnLoadAllBundleAction = null;
            OnLoadSelfBundleAction = null;
            LoadMode = enResourceLoadMode.Sync;
            LoadCache = enResourceLoadCache.NormalLoad;
            LoadMethod = enResourceLoadMethod.LoadFromFile;
            BundleInfo = null;
            UserData = null;
        }
    }
}

#endif