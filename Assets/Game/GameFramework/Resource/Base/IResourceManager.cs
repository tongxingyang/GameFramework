namespace GameFramework.Res.Base
{
    public interface IResourceManager
    {
        void LoadAsset<T>(ResourceLoadInfo resourceLoadInfo, LoadAssetCallbacks loadAssetCallbacks,object userData = null);
    }
}