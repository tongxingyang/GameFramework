namespace GameFramework.Res.Base
{
    public interface IResourceManager
    {
        void LoadAsset<T>(ResourceLoadInfo resourceLoadInfo, LoadCallback loadAssetCallbacks,object userData = null);
    }
}