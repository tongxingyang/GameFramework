#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using GameFramework.Pool.ReferencePool;
using GameFramework.Tool;
using UnityEngine;

namespace GameFramework.Res.Base
{
    public class AssetDatabaseModule : AbstractResourceModule
    {
        public delegate void OnLoadSelfBundle(AssetDatabaseLoader loader);

        public Dictionary<string, AssetDatabaseLoader> AssetDatabaseLoaders;
        
        private List<AbstractAssetInfo> UnusedAssetDatabaseInfos;
        private float CheckUnusedAssetBundleTimeInterval;
        private int MaxUnloadAssetBundleNumberPerFrame;
        private float AssetBundleMinLifeTime;
        private int AssetBundleRecycleFpsThreshold;

        public override void InitModule()
        {
            base.InitModule();
            LoadType = enResouceLoadType.AssetDatabase;
            AssetDatabaseLoaders = new Dictionary<string, AssetDatabaseLoader>();
            UnusedAssetDatabaseInfos = new List<AbstractAssetInfo>();
            CheckUnusedAssetBundleTimeInterval = 10f;
            MaxUnloadAssetBundleNumberPerFrame = 10;
            AssetBundleMinLifeTime = 20f;
            AssetBundleRecycleFpsThreshold = 20;
        }

        protected override void RealRequestResource(string name, AssetBundleLoader.OnLoadAllBundle callback, enResourceLoadMode loadMode = enResourceLoadMode.Sync,
            enResourceLoadCache loadCache = enResourceLoadCache.NormalLoad,
            enResourceLoadMethod loadMethod = enResourceLoadMethod.LoadFromFile, object userdata = null)
        {
            if (AllLoadedResourceInfos[enResourceLoadCache.NormalLoad].ContainsKey(name))
            {
                callback(AllLoadedResourceInfos[enResourceLoadCache.NormalLoad][name],userdata);
                if (loadCache > enResourceLoadCache.NormalLoad)
                {
                    ChangeResourceInfoLoadType(name,enResourceLoadCache.NormalLoad, loadCache);
                }
            }else if (AllLoadedResourceInfos[enResourceLoadCache.Preload].ContainsKey(name))
            {
                callback(AllLoadedResourceInfos[enResourceLoadCache.Preload][name],userdata);
                if (loadCache > enResourceLoadCache.Preload)
                {
                    ChangeResourceInfoLoadType(name,enResourceLoadCache.Preload, loadCache);
                }
            }else if (AllLoadedResourceInfos[enResourceLoadCache.PermanentLoad].ContainsKey(name))
            {
                callback(AllLoadedResourceInfos[enResourceLoadCache.PermanentLoad][name],userdata);
            }
            else
            {
                AssetDatabaseLoader assetDatabaseLoader = CreateAssetDatabaseLoader(name);
                assetDatabaseLoader.LoadCache = loadCache;
                assetDatabaseLoader.UserData = userdata;
                assetDatabaseLoader.LoadMode = loadMode;
                assetDatabaseLoader.LoadMethod = loadMethod;
                assetDatabaseLoader.OnLoadAllBundleAction = callback;
                assetDatabaseLoader.OnLoadSelfBundleAction = OnLoadSelfBundleAction;
                AssetDatabaseLoaders.Add(name, assetDatabaseLoader);
                assetDatabaseLoader.StartLoadResource();
            }
        }
        public override void LoadAllDependInfo()
        {

        }
        private void OnLoadSelfBundleAction(AssetDatabaseLoader adl)
        {
            var abname = adl.AssetBundleName;
            if (AssetDatabaseLoaders.ContainsKey(abname))
            {
                AssetDatabaseLoaders.Remove(abname);
                AllLoadedResourceInfos[adl.LoadCache].Add(abname, adl.BundleInfo);
            }
        }

        private AssetDatabaseLoader CreateAssetDatabaseLoader(string name)
        {
            var loader = ReferencePool.Acquire<AssetDatabaseLoader>();
            loader.AssetBundleName = name;
            return loader;
        }
        
        protected override IEnumerator CheckUnsedResource()
        {
            while (true)
            {
                if (EnableRecycleUnused && CurrentFPS >= AssetBundleRecycleFpsThreshold &&
                    AssetDatabaseLoaders.Count == 0)
                {
                    float time = Time.time;
                    foreach (KeyValuePair<string,AbstractAssetInfo> assetInfo in AllLoadedResourceInfos[enResourceLoadCache.NormalLoad])
                    {
                        if (assetInfo.Value.IsUnused)
                        {
                            if (time - assetInfo.Value.LastUsedTime > AssetBundleMinLifeTime)
                            {
                                UnusedAssetDatabaseInfos.Add(assetInfo.Value);
                            }
                        }
                    }
                    if (UnusedAssetDatabaseInfos.Count > 0)
                    {
                        UnusedAssetDatabaseInfos.Sort(LastUsedTimeSort);
                        for (int i = 0; i < UnusedAssetDatabaseInfos.Count; i++)
                        {
                            if (i < MaxUnloadAssetBundleNumberPerFrame)
                            {
                                AllLoadedResourceInfos[enResourceLoadCache.NormalLoad]
                                    .Remove(UnusedAssetDatabaseInfos[i].AssetBundleName);
                                UnusedAssetDatabaseInfos[i].Dispose();
                            }
                            else
                            {
                                break;
                            }
                        }
                        UnusedAssetDatabaseInfos.Clear();
                        Resources.UnloadUnusedAssets();
                    }
                }
                yield return Yielders.GetWaitForSeconds(CheckUnusedAssetBundleTimeInterval);
            }
        }

        protected override void RealUnloadSpecificLoadTypeResource(enResourceLoadCache loadCache)
        {
            foreach (KeyValuePair<string,AbstractAssetInfo> assetInfo in AllLoadedResourceInfos[loadCache])
            {
                if (assetInfo.Value.IsUnused)
                {
                    UnusedAssetDatabaseInfos.Add(assetInfo.Value);
                }
            }
            if (UnusedAssetDatabaseInfos.Count == 0)
            {
                return;
            }
            foreach (AbstractAssetInfo info in UnusedAssetDatabaseInfos)
            {
                AllLoadedResourceInfos[loadCache].Remove(info.AssetBundleName);
                info.Dispose();
            }
            UnusedAssetDatabaseInfos.Clear();
            Resources.UnloadUnusedAssets();
        }

        protected override void RealUnloadSpecificLoadTypeResourceByName(enResourceLoadCache loadCache, string name)
        {
            AbstractAssetInfo item;
            foreach (KeyValuePair<string,AbstractAssetInfo> assetInfo in AllLoadedResourceInfos[loadCache])
            {
                if (assetInfo.Value.AssetBundleName == name)
                {
                    item = assetInfo.Value;
                    AllLoadedResourceInfos[loadCache].Remove(item.AssetBundleName);
                    item.Dispose();
                    break;
                }
            }
        }

        private int LastUsedTimeSort(AbstractAssetInfo a, AbstractAssetInfo b)
        {
            return a.LastUsedTime.CompareTo(b.LastUsedTime);
        }
    }
}

#endif
