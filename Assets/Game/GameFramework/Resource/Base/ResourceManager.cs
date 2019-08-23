namespace GameFramework.Res.Base
{
    public class ResourceManager : IResourceManager
    {
        public AbstractResourceModule CurrentResourceModule;

        public string PlatformName { get; set; }
        
        public enResouceLoadType LoadType { get; set; }

        public void InitManager()
        {
            if (LoadType == enResouceLoadType.AssetBundle)
            {
                CurrentResourceModule = new AssetBundleModule();
            }else if (LoadType == enResouceLoadType.AssetDatabase)
            {
                CurrentResourceModule = new AssetDatabaseModule();
            }
            CurrentResourceModule.InitModule();
        }

        public void LoadAllDependInfo()
        {
            CurrentResourceModule.PlatformName = PlatformName;
            CurrentResourceModule.LoadAllDependInfo();
        }

        public void StartResourceRecycling()
        {
            CurrentResourceModule.StartResourceRecycling();
        }

        public void AddToWhiteList(string name)
        {
            CurrentResourceModule.AddToWhiteList(name);
        }
        
        public void RequestResource(string name, AssetBundleLoader.OnLoadAllBundle callback,
            enResourceLoadMode loadMode = enResourceLoadMode.Sync,
            enResourceLoadCache loadCache = enResourceLoadCache.NormalLoad,
            enResourceLoadMethod loadMethod = enResourceLoadMethod.LoadFromFile,object userdata = null)
        {
            CurrentResourceModule.RequestResource(name,callback,loadMode,loadCache,loadMethod,userdata);
        }
        
        public void UnloadAllUnusedPreloadResources()
        {
            CurrentResourceModule.UnloadAllUnusedPreloadResources();
        }
        
        public void UnloadAllUnusedNormalResources()
        {
            CurrentResourceModule.UnloadAllUnusedNormalResources();
        }

        public void UnloadLoadTypeResourceByName(enResourceLoadCache loadCache, string name)
        {
            CurrentResourceModule.UnloadSpecificLoadTypeResourceByName(loadCache,name);
        }
        
        public void Update()
        {
            CurrentResourceModule.Update();
        }
        
    }
}