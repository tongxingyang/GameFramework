using System.Collections;
using System.Collections.Generic;
using GameFramework.Pool.ReferencePool;
using GameFramework.Tool;
using GameFramework.Utility.PathUtility;
using UnityEngine;

namespace GameFramework.Res.Base
{
    public class AssetBundleModule : AbstractResourceModule
    {
        public delegate void OnLoadSelfBundle(AssetBundleLoader loader);
        
        public Dictionary<string, AssetBundleLoader> AssetBundleLoaders;
        public int MaxAsyncCoroutineCount;
        public List<AssetBundleAsyncQueue> AssetBundleAsyncQueues;
        public Dictionary<string, string[]> AssetBundleDepMap;
        
        private List<AbstractAssetInfo> UnusedAssetBundleInfos;
        private float CheckUnusedAssetBundleTimeInterval;
        private int MaxUnloadAssetBundleNumberPerFrame;
        private float AssetBundleMinLifeTime;
        private int AssetBundleRecycleFpsThreshold;

        public string[] GetAssetBundleDepInfo(string assetBundleName)
        {
            if (AssetBundleDepMap.ContainsKey(assetBundleName))
            {
                return AssetBundleDepMap[assetBundleName];
            }
            return null;
        }
        
        public override void InitModule()
        {
            base.InitModule();
            AssetBundleDepMap = new Dictionary<string, string[]>();
            LoadType = enResouceLoadType.AssetBundle;
            AssetBundleLoaders = new Dictionary<string, AssetBundleLoader>();
            UnusedAssetBundleInfos = new List<AbstractAssetInfo>();
            
            //todo 从设备能力设置参数
            CheckUnusedAssetBundleTimeInterval = 10f;
            MaxUnloadAssetBundleNumberPerFrame = 10;
            AssetBundleMinLifeTime = 20f;
            AssetBundleRecycleFpsThreshold = 20;
            MaxAsyncCoroutineCount = 3;
            AssetBundleAsyncQueues = new List<AssetBundleAsyncQueue>();
            for (int i = 0; i < MaxAsyncCoroutineCount; i++)
            {
                var queue = new AssetBundleAsyncQueue();
                AssetBundleAsyncQueues.Add(queue);
                queue.StartAssetBundleAsyncLoad();
            }
        }

        public override void LoadAllDependInfo()
        {
            AssetBundleDepMap.Clear();
            AssetBundle assetBundle = AssetBundle.LoadFromFile(PathUtility.GetCombinePath(AppConst.Path.PresistentDataPath,
                PlatformName));
            if (assetBundle != null)
            {
                AssetBundleManifest manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                if (manifest != null)
                {
                    var allabnames = manifest.GetAllAssetBundles();
                    foreach (var abname in allabnames)
                    {
                        var abdeparray = manifest.GetAllDependencies(abname);
                        AssetBundleDepMap[abname] = abdeparray;
                    }
                }
            }
        }

        protected override void RealRequestResource(string name, AssetBundleLoader.OnLoadAllBundle callback,
            enResourceLoadMode loadMode = enResourceLoadMode.Sync,
            enResourceLoadCache loadCache = enResourceLoadCache.NormalLoad,
            enResourceLoadMethod loadMethod = enResourceLoadMethod.LoadFromFile,object userData = null)
        {
            if (AllLoadedResourceInfos[enResourceLoadCache.NormalLoad].ContainsKey(name))
            {
                callback(AllLoadedResourceInfos[enResourceLoadCache.NormalLoad][name],userData);
                if (loadCache > enResourceLoadCache.NormalLoad)
                {
                    ChangeResourceInfoLoadType(name,enResourceLoadCache.NormalLoad, loadCache);
                }
            }else if (AllLoadedResourceInfos[enResourceLoadCache.Preload].ContainsKey(name))
            {
                callback(AllLoadedResourceInfos[enResourceLoadCache.Preload][name],userData);
                if (loadCache > enResourceLoadCache.Preload)
                {
                    ChangeResourceInfoLoadType(name,enResourceLoadCache.Preload, loadCache);
                }
            }else if (AllLoadedResourceInfos[enResourceLoadCache.PermanentLoad].ContainsKey(name))
            {
                callback(AllLoadedResourceInfos[enResourceLoadCache.PermanentLoad][name],userData);
            }
            else
            {
                AssetBundleLoader assetBundleLoader = null;
                if (AssetBundleLoaders.ContainsKey(name))
                {
                    assetBundleLoader = AssetBundleLoaders[name];
                    assetBundleLoader.UserData = userData;
                    assetBundleLoader.LoadCache = loadCache;
                    assetBundleLoader.LoadMode = loadMode;
                    assetBundleLoader.LoadMethod = loadMethod;
                    assetBundleLoader.OnLoadAllBundleAction += callback;
                    assetBundleLoader.OnLoadSelfBundleAction = OnLoadSelfBundleAction;
                    if (loadMode == enResourceLoadMode.Sync)
                    {
                        assetBundleLoader.LoadState = enLoadState.None;
                        assetBundleLoader.StartLoadBundle();
                    }
                }
                else
                {
                    assetBundleLoader = CreateAssetBundleLoader(name);
                    assetBundleLoader.LoadMethod = loadMethod;
                    assetBundleLoader.UserData = userData;
                    assetBundleLoader.LoadCache = loadCache;
                    assetBundleLoader.LoadMode = loadMode;
                    assetBundleLoader.OnLoadAllBundleAction = callback;
                    assetBundleLoader.OnLoadSelfBundleAction = OnLoadSelfBundleAction;
                    AssetBundleLoaders.Add(name,assetBundleLoader);
                    assetBundleLoader.StartLoadBundle();
                }
            }
        }

        private AssetBundleLoader CreateAssetBundleLoader(string name)
        {
            var depNames = GetAssetBundleDepInfo(name);
            var loader = ReferencePool.Acquire<AssetBundleLoader>();
            loader.AssetBundleName = name;
            loader.DepAssetBundleNames = depNames;
            return loader;
        }

        private void OnLoadSelfBundleAction(AssetBundleLoader abl)
        {
            var abName = abl.AssetBundleName;
            if (AssetBundleLoaders.ContainsKey(abName))
            {
                AssetBundleLoaders.Remove(abName);
                AllLoadedResourceInfos[abl.LoadCache].Add(abName,abl.BundleInfo);
            }
        }

        protected override IEnumerator CheckUnsedResource()
        {
            while (true)
            {
                if (EnableRecycleUnused && CurrentFPS >= AssetBundleRecycleFpsThreshold &&
                    AssetBundleLoaders.Count == 0)
                {
                    float time = Time.time;
                    foreach (KeyValuePair<string,AbstractAssetInfo> abstractAssetInfo in AllLoadedResourceInfos[enResourceLoadCache.NormalLoad])
                    {
                        if (abstractAssetInfo.Value.IsUnused)
                        {
                            if (time - abstractAssetInfo.Value.LastUsedTime > AssetBundleMinLifeTime)
                            {
                                UnusedAssetBundleInfos.Add(abstractAssetInfo.Value);
                            }
                        }
                    }
                    if (UnusedAssetBundleInfos.Count > 0)
                    {
                        UnusedAssetBundleInfos.Sort(LastUsedTimeSort);
                        for (int i = 0; i < UnusedAssetBundleInfos.Count; i++)
                        {
                            if (i < MaxUnloadAssetBundleNumberPerFrame)
                            {
                                AllLoadedResourceInfos[enResourceLoadCache.NormalLoad]
                                    .Remove(UnusedAssetBundleInfos[i].AssetBundleName);
                                UnusedAssetBundleInfos[i].Dispose();
                            }
                            else
                            {
                                break;
                            }
                        }
                        UnusedAssetBundleInfos.Clear();
                    }
                }
                yield return Yielders.GetWaitForSeconds(CheckUnusedAssetBundleTimeInterval);
            }
        }
        
        private int LastUsedTimeSort(AbstractAssetInfo a, AbstractAssetInfo b)
        {
            return a.LastUsedTime.CompareTo(b.LastUsedTime);
        }

        protected override void RealUnloadSpecificLoadTypeResource(enResourceLoadCache loadCache)
        {
            foreach (KeyValuePair<string,AbstractAssetInfo> assetInfo in AllLoadedResourceInfos[loadCache])
            {
                if (assetInfo.Value.IsUnused)
                {
                    UnusedAssetBundleInfos.Add(assetInfo.Value);
                }
            }
            if (UnusedAssetBundleInfos.Count == 0)
            {
                return;
            }
            foreach (AbstractAssetInfo info in UnusedAssetBundleInfos)
            {
                AllLoadedResourceInfos[loadCache].Remove(info.AssetBundleName);
                info.Dispose();
            }
            UnusedAssetBundleInfos.Clear();
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
    }
}