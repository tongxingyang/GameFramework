using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameFramework.Res.Base
{
    public class AssetBundleInfo : AbstractAssetInfo
    {
        public AssetBundle Bundle { get; set; }

        public HashSet<AssetBundleInfo> DepAssetBundleInfos;

        public AssetBundleInfo()
        {
            Bundle = null;
            AssetBundleName = String.Empty;
            LastUsedTime = 0.0f;
            IsReady = false;
            DepAssetBundleInfos = new HashSet<AssetBundleInfo>();
            ReferenceCount = 0;
            ReferenceList = new List<WeakReference>();
            LoadedAssetMap = new Dictionary<string, Object>();
            OnBundleUnloadAction = null;
        }

        public void AddDependency(AssetBundleInfo info)
        {
            if (info != null && DepAssetBundleInfos.Add(info))
            {
                info.Retain();
            }
        }
        
        public override void LoadAllAsset<T>()
        {
            if (IsReady)
            {
                var allassets = Bundle.LoadAllAssets<T>();
                foreach (var asset in allassets)
                {
                    var assetName = asset.name;
                    if (!LoadedAssetMap.ContainsKey(assetName))
                    {
                        LoadedAssetMap.Add(assetName, asset);
                    }
                }
            }
        }

        public override T LoadAsset<T>(string assetName)
        {
            if (IsReady)
            {
                if (LoadedAssetMap.ContainsKey(assetName))
                {
                    return LoadedAssetMap[assetName] as T;
                }
                if (Bundle == null)
                {
                    return null;
                }
                var asset = Bundle.LoadAsset<T>(assetName);
                if (asset != null)
                {
                    LoadedAssetMap.Add(assetName,asset);
                    return asset;
                }
            }
            return null;
        }

        public override GameObject Instantiate(string assetName)
        {
            var go = LoadAsset<GameObject>(assetName);
            if (go != null)
            {
                var goIns = Object.Instantiate(go);
                RetainInfoOwner(goIns);
                UpdateLastUsedTime();
                return goIns;
            }
            return null;
        }

        public override T GetAsset<T>(Object owner, string assetName)
        {
            if (owner != null)
            {
                var asset = LoadAsset<T>(assetName);
                if (asset != null)
                {
                    RetainInfoOwner(owner);
                    UpdateLastUsedTime();
                    return asset;
                }
                return null;
            }
            return null;
        }

        public override void Dispose()
        {
            UnloadAssetBundle();
            LastUsedTime = 0.0f;
            foreach (AssetBundleInfo depAssetBundleInfo in DepAssetBundleInfos)
            {
                depAssetBundleInfo.Release();
            }
            DepAssetBundleInfos.Clear();
            OnBundleUnloadAction?.Invoke(this);
            OnBundleUnloadAction = null;
        }

        public override void Reset()
        {
            Bundle = null;
            AssetBundleName = String.Empty;
            LastUsedTime = 0.0f;
            IsReady = false;
            DepAssetBundleInfos.Clear();
            ReferenceCount = 0;
            ReferenceList.Clear();
            LoadedAssetMap.Clear();
            OnBundleUnloadAction = null;
        }

        private void UnloadAssetBundle()
        {
            if (Bundle != null)
            {
                Bundle.Unload(true);
            }
            Bundle = null;
            IsReady = false;
        }
    }
}