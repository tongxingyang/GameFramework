namespace GameFramework.Res.Base
{
    public sealed class ResourceLoadInfo
    {
        private string assetName;
        private int priority;
        private enResouceLoadType resouceLoadType;
        private enResourceLoadMode resourceLoadMode;
        private enResourceLoadMethod resourceLoadMethod;

        public string AssetName => assetName;
        public int Priority => priority;
        public enResouceLoadType ResouceLoadType => resouceLoadType;
        public enResourceLoadMode ResourceLoadMode => resourceLoadMode;
        public enResourceLoadMethod ResourceLoadMethod => resourceLoadMethod;
        public ResourceLoadInfo(string assetName,int priority,enResouceLoadType resouceLoadType,enResourceLoadMode resourceLoadMode,enResourceLoadMethod resourceLoadMethod)
        {
            this.assetName = assetName;
            this.priority = priority;
            this.resouceLoadType = resouceLoadType;
            this.resourceLoadMode = resourceLoadMode;
            this.resourceLoadMethod = resourceLoadMethod;
        }
    }
}