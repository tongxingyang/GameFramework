using System.Collections;
using System.Collections.Generic;
using GameFramework.Base;
using GameFramework.Utility.Singleton;
using UnityEngine;

namespace GameFramework.Res.Base
{
    public abstract class AbstractResourceModule
    {
        public enResouceLoadType LoadType;
        public bool EnableRecycleUnused;
        public HashSet<string> ResourceWhiteList;
        public Dictionary<enResourceLoadCache, Dictionary<string, AbstractAssetInfo>> AllLoadedResourceInfos;
        public int CurrentFPS;
        private float totalDeltaTime;
        private int frameCount;
        private float fpsUpdateInterval;
        public string PlatformName;
        public virtual void InitModule()
        {
            LoadType = enResouceLoadType.Invalide;
            EnableRecycleUnused = true;
            ResourceWhiteList = new HashSet<string>();
            AllLoadedResourceInfos = new Dictionary<enResourceLoadCache, Dictionary<string, AbstractAssetInfo>>
            {
                {enResourceLoadCache.Preload, new Dictionary<string, AbstractAssetInfo>()},
                {enResourceLoadCache.NormalLoad, new Dictionary<string, AbstractAssetInfo>()},
                {enResourceLoadCache.PermanentLoad, new Dictionary<string, AbstractAssetInfo>()}
            };
            fpsUpdateInterval = 1.0f;
        }

        public void AddToWhiteList(string name)
        {
            if (!ResourceWhiteList.Contains(name))
            {
                ResourceWhiteList.Add(name);
            }
        }

        public void RequestResource(string name, AssetBundleLoader.OnLoadAllBundle callback,
            enResourceLoadMode loadMode = enResourceLoadMode.Sync,
            enResourceLoadCache loadCache = enResourceLoadCache.NormalLoad,
            enResourceLoadMethod loadMethod = enResourceLoadMethod.LoadFromFile,object userData = null)
        {
            if (ResourceWhiteList.Contains(name))
            {
                loadCache = enResourceLoadCache.Preload;
            }
            RealRequestResource(name, callback,loadMode, loadCache, loadMethod,userData);
        }
        
        protected abstract void RealRequestResource(string name, AssetBundleLoader.OnLoadAllBundle callback,
            enResourceLoadMode loadMode = enResourceLoadMode.Sync,
            enResourceLoadCache loadCache = enResourceLoadCache.NormalLoad,
            enResourceLoadMethod loadMethod = enResourceLoadMethod.LoadFromFile,object userData = null);
        
        public virtual void Update()
        {
            totalDeltaTime += Time.deltaTime;
            frameCount++;
            if (totalDeltaTime >= fpsUpdateInterval)
            {
                CurrentFPS = (int)(frameCount / totalDeltaTime);
                totalDeltaTime = 0f;
                frameCount = 0;
            }
        }

        public void StartResourceRecycling()
        {
            Singleton<GameEntry>.GetInstance().GetComponent<ResourceComponent>().StartCoroutine(CheckUnsedResource());
        }
        
        protected abstract IEnumerator CheckUnsedResource();
        public abstract void LoadAllDependInfo();
        
        public void UnloadAllUnusedPreloadResources()
        {
            UnloadSpecificLoadTypeResource(enResourceLoadCache.Preload);
        }
        
        public void UnloadAllUnusedNormalResources()
        {
            UnloadSpecificLoadTypeResource(enResourceLoadCache.NormalLoad);
        }
        
        protected void UnloadSpecificLoadTypeResource(enResourceLoadCache loadCache)
        {
            if (loadCache == enResourceLoadCache.PermanentLoad)
            {
                return;
            }
            RealUnloadSpecificLoadTypeResource(loadCache);
        }
        
        protected abstract void RealUnloadSpecificLoadTypeResource(enResourceLoadCache loadCache);

        public void UnloadSpecificLoadTypeResourceByName(enResourceLoadCache loadCache,string name)
        {
            RealUnloadSpecificLoadTypeResourceByName(loadCache,name);
        }
        
        protected abstract void RealUnloadSpecificLoadTypeResourceByName(enResourceLoadCache loadCache,string name);
        
        protected void ChangeResourceInfoLoadType(string name, enResourceLoadCache oldloadCache,
            enResourceLoadCache newloadCache)
        {
            if (AllLoadedResourceInfos[oldloadCache].ContainsKey(name))
            {
                var info = AllLoadedResourceInfos[oldloadCache][name];
                AllLoadedResourceInfos[newloadCache].Add(name,info);
                AllLoadedResourceInfos[oldloadCache].Remove(name);
            }
        }
        
    }
}