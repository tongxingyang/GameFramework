#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameFramework.Res.Base
{
    public class AssetDatabaseInfo : AbstractAssetInfo
    {
        public string[] AssetsPath;

        public AssetDatabaseInfo()
        {
            AssetBundleName = String.Empty;
            LastUsedTime = 0.0f;
            IsReady = false;
            ReferenceCount = 0;
            ReferenceList = new List<WeakReference>();
            LoadedAssetMap = new Dictionary<string, Object>();
            AssetsPath = null;
        }
        
        public override void LoadAllAsset<T>()
        {
            if (IsReady)
            {
                if (AssetsPath != null && AssetsPath.Length > 0)
                {
                    foreach (string path in AssetsPath)
                    {
                        var allAssets = AssetDatabase.LoadAllAssetsAtPath(path);
                        foreach (var asset in allAssets)
                        {
                            if (asset.GetType() == typeof(T))
                            {
                                var assetName = asset.name;
                                if (!LoadedAssetMap.ContainsKey(assetName))
                                {
                                    LoadedAssetMap.Add(assetName,asset);
                                } 
                            }
                        }
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
                if (AssetsPath == null) return null;
                var assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(AssetBundleName, assetName);
                if (assetPaths.Length == 1)
                {
                    T asset = AssetDatabase.LoadAssetAtPath<T>(assetPaths[0]);
                    if (asset != null)
                    {
                        LoadedAssetMap.Add(assetName,asset);
                        return asset;
                    }
                    return null;
                }
                if (assetPaths.Length > 1)
                {
                    UnityEngine.Debug.LogError(string.Format("AssetBundle: {0} 存在同名Asset: {1}资源", AssetBundleName,
                        assetName));
                    return null;
                }
                return null;
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
            UnloadResource();
            LastUsedTime = 0.0f;
            OnBundleUnloadAction?.Invoke(this);
            OnBundleUnloadAction = null;
        }

        public override void Reset()
        {
            AssetBundleName = String.Empty;
            LastUsedTime = 0.0f;
            IsReady = false;
            ReferenceCount = 0;
            ReferenceList.Clear();
            LoadedAssetMap.Clear();
            AssetsPath = null;
        }

        public void UnloadResource()
        {
            foreach (KeyValuePair<string,Object> loadAsset in LoadedAssetMap)
            {
                var asset = loadAsset.Value;
                if (asset is GameObject || asset is Component)
                {

                }
                else
                {
                    Resources.UnloadAsset(asset);
                }
            }
            LoadedAssetMap.Clear();
            IsReady = false;
        }
    }
}
#endif