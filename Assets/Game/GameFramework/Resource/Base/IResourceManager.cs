namespace GameFramework.Res.Base
{
    public interface IResourceManager
    {
        string PlatformName { get; set; }
        enResouceLoadType LoadType { get; set; }
        void InitManager();
        void LoadAllDependInfo();
        void StartResourceRecycling();
        void AddToWhiteList(string name);
        void RequestResource(string name, AssetBundleLoader.OnLoadAllBundle callback,
            enResourceLoadMode loadMode = enResourceLoadMode.Sync,
            enResourceLoadCache loadCache = enResourceLoadCache.NormalLoad,
            enResourceLoadMethod loadMethod = enResourceLoadMethod.LoadFromFile,object userdata = null);
        void UnloadAllUnusedPreloadResources();
        void UnloadAllUnusedNormalResources();
        void UnloadLoadTypeResourceByName(enResourceLoadCache loadCache, string name);
        void Update();
    }
}