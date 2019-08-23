namespace GameFramework.Res.Base
{
    public sealed class ResourceLoadInfo
    {
        private string assetName;
        private string assetBundleName;
        private enResourceLoadMode resourceLoadMode;
        private enResourceLoadMethod resourceLoadMethod;
        private enResourceLoadCache resourceLoadCache;

        public string AssetName => assetName;
        public string AssetBundleName => assetBundleName;
        public enResourceLoadMode ResourceLoadMode => resourceLoadMode;
        public enResourceLoadMethod ResourceLoadMethod => resourceLoadMethod;
        public enResourceLoadCache ResourceLoadCache => resourceLoadCache;
        public ResourceLoadInfo(string assetName,string assetBundleName ,enResourceLoadCache resourceLoadCache,enResourceLoadMode resourceLoadMode,enResourceLoadMethod resourceLoadMethod)
        {
            this.assetName = assetName;
            this.assetBundleName = assetBundleName;
            this.resourceLoadMode = resourceLoadMode;
            this.resourceLoadMethod = resourceLoadMethod;
            this.resourceLoadCache = resourceLoadCache;
        }
    }
}