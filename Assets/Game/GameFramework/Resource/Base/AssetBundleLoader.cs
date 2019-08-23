using System;
using System.Collections.Generic;
using GameFramework.Base;
using GameFramework.Pool.ReferencePool;
using GameFramework.Utility.EncryptUtility;
using GameFramework.Utility.File;
using GameFramework.Utility.Singleton;
using UnityEngine;

namespace GameFramework.Res.Base
{
    public class AssetBundleLoader : IReference
    {   
        public delegate void OnLoadAllBundle(AbstractAssetInfo info ,object userData);
        
        public AssetBundleModule.OnLoadSelfBundle OnLoadSelfBundleAction;
        public OnLoadAllBundle OnLoadAllBundleAction;
        
        public enResourceLoadMode LoadMode;
        public enResourceLoadCache LoadCache;
        public enResourceLoadMethod LoadMethod;
        public enLoadState LoadState;
        public object UserData;

        public AssetBundleInfo BundleInfo;
        public string AssetBundleName { get; set; }
        public List<AssetBundleInfo> DepAssetBundleInfoList;

        private int AlreadyLoadCount;
        private int DepAssetBundleCount;
        private string[] depAssetBundleNames;
        public string[] DepAssetBundleNames
        {
            get => depAssetBundleNames;
            set
            {
                depAssetBundleNames = value;
                DepAssetBundleCount = depAssetBundleNames?.Length ?? 0;
                if (DepAssetBundleInfoList == null)
                {
                    DepAssetBundleInfoList = new List<AssetBundleInfo>();
                }
                else
                {
                    DepAssetBundleInfoList.Clear();
                }
            }
        }

        public AssetBundleLoader()
        {
            AssetBundleName = String.Empty;
            DepAssetBundleNames = null;
            OnLoadAllBundleAction = null;
            OnLoadSelfBundleAction = null;
            LoadMethod = enResourceLoadMethod.LoadFromFile;
            LoadState = enLoadState.None;
            LoadMode = enResourceLoadMode.Sync;
            AlreadyLoadCount = 0;
            BundleInfo = null;
        }

        public AssetBundleLoader(string assetBundleName, string[] depNames)
        {
            AssetBundleName = assetBundleName;
            DepAssetBundleNames = depNames;
            OnLoadAllBundleAction = null;
            OnLoadSelfBundleAction = null;
            LoadMethod = enResourceLoadMethod.LoadFromFile;
            LoadState = enLoadState.None;
            LoadMode = enResourceLoadMode.Sync;
            AlreadyLoadCount = 0;
            BundleInfo = null;
        }

        public void StartLoadBundle()
        {
            if (LoadState == enLoadState.None)
            {
                LoadState = enLoadState.Waiting;
                LoadDepAssetBundle();
            }
        }
        
        public void Reset()
        {
            AssetBundleName = String.Empty;
            DepAssetBundleCount = 0;
            DepAssetBundleNames = null;
            OnLoadAllBundleAction = null;
            OnLoadSelfBundleAction = null;
            LoadMode = enResourceLoadMode.Sync;
            LoadCache = enResourceLoadCache.NormalLoad;
            LoadMethod = enResourceLoadMethod.LoadFromFile;
            AlreadyLoadCount = 0;
            BundleInfo = null;
            LoadState = enLoadState.None;
            DepAssetBundleInfoList.Clear();
            UserData = null;
        }

        public void LoadSelfAssetBundle()
        {
            if (LoadMode == enResourceLoadMode.Sync)
            {
                LoadState = enLoadState.Loading;
                LoadAssetBundleSync();
            }
            else
            {
                AssetBundleAsyncQueue.AddToAsyncLoadQueue(this);
            }
        }

        private void LoadAssetBundleSync()
        {
            AssetBundle assetBundle = null;
            
            if (LoadMethod == enResourceLoadMethod.LoadFromFile)
            {
                assetBundle = AssetBundle.LoadFromFile(AssetBundleName);
                
            }else if (LoadMethod == enResourceLoadMethod.LoadFromMemory)
            {
                assetBundle = AssetBundle.LoadFromMemory(FileUtility.ReadAllBytes(AssetBundleName));
                
            }else if (LoadMethod == enResourceLoadMethod.LoadFromMemoryDecrypt)
            {
                assetBundle = AssetBundle.LoadFromMemory(EncryptUtility.AssetBundleDecrypt(FileUtility.ReadAllBytes(AssetBundleName)));
                
            }else if (LoadMethod == enResourceLoadMethod.LoadFromStream)
            {
                assetBundle = AssetBundle.LoadFromStream(FileUtility.Open(AssetBundleName));
            }
            OnSelfAssetBundleLoadComplete(assetBundle);

        }
        
        public void LoadDepAssetBundle()
        {
            if (DepAssetBundleCount == 0)
            {
                LoadSelfAssetBundle();
            }
            else
            {
                foreach (string depAssetBundleName in DepAssetBundleNames)
                {
                    Singleton<GameEntry>.GetInstance().GetComponent<ResourceComponent>().RequestResource(depAssetBundleName,OnDepAssetBundleLoadComplete,LoadMode,enResourceLoadCache.NormalLoad,LoadMethod, null);
                }
            }
        }

        public void OnSelfAssetBundleLoadComplete(AssetBundle assetBundle)
        {
            BundleInfo = CreateAssetBundleInfo(AssetBundleName, assetBundle);
            BundleInfo.UpdateLastUsedTime();
            LoadState = enLoadState.SelfComplete;
            OnLoadSelfBundleAction?.Invoke(this);
            OnLoadSelfBundleAction = null;
            AllAssetBundleLoadedComplete();
        }

        public  AssetBundleInfo CreateAssetBundleInfo(string name, AssetBundle assetBundle)
        {
            var abinfo = ReferencePool.Acquire<AssetBundleInfo>();
            abinfo.AssetBundleName = name;
            abinfo.Bundle = assetBundle;
            abinfo.OnBundleUnloadAction = OnBundleUnloadAction;
            return abinfo;
        }

        public  void OnBundleUnloadAction(AbstractAssetInfo info)
        {
            var abinfo = info as AssetBundleInfo;
            ReferencePool.Release<AssetBundleInfo>(abinfo);
        }
        
        
        private void OnDepAssetBundleLoadComplete(AbstractAssetInfo info , object data)
        {
            var abinfo = info as AssetBundleInfo;
            DepAssetBundleInfoList.Add(abinfo);
            AlreadyLoadCount++;
            abinfo?.UpdateLastUsedTime();
            if (AlreadyLoadCount == DepAssetBundleCount)
            {
                LoadSelfAssetBundle();
            }
        }

        private void AllAssetBundleLoadedComplete()
        {
            foreach (AssetBundleInfo assetBundleInfo in DepAssetBundleInfoList)
            {
                BundleInfo.AddDependency(assetBundleInfo);
            }
            LoadState = enLoadState.AllComplete;
            BundleInfo.IsReady = true;
            OnLoadAllBundleAction?.Invoke(BundleInfo,UserData);
            OnLoadAllBundleAction = null;
            ReferencePool.Release<AssetBundleLoader>(this);
        }

    }
}