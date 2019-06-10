namespace GameFramework.Res.Base
{
    public class ResourceManager : IResourceManager
    {
        public void LoadAssetFromAssetBundleAsync<T>(string assetName, int priority,enResourceLoadMethod method, LoadAssetCallbacks loadAssetCallbacks,
            object userData)
        {
            
        }

        public void LoadAssetFromAssetBundleSync<T>(string assetName, int priority, enResourceLoadMethod method,
            LoadAssetCallbacks loadAssetCallbacks, object userData)
        {
            
        }
        
        public void LoadAssetFromResourceAsync<T>(string assetName, LoadAssetCallbacks loadAssetCallbacks ,object userData)
        {
            
        }

        public void LoadAssetFromResourceSync<T>(string assetName, LoadAssetCallbacks loadAssetCallbacks, object userData)
        {
            
        }

        public void LoadAssetFromResource<T>(string assetName, enResourceLoadMode mode,
            LoadAssetCallbacks loadAssetCallbacks, object userData)
        {
            if (mode == enResourceLoadMode.Async)
            {
                LoadAssetFromResourceAsync<T>(assetName,loadAssetCallbacks,userData);
            }
            else
            {
                LoadAssetFromResourceSync<T>(assetName,loadAssetCallbacks,userData);
            }
        }

        public void LoadAssetFromAssetBundle<T>(string assetName, enResourceLoadMode mode, int priority, enResourceLoadMethod method,
            LoadAssetCallbacks loadAssetCallbacks, object userData)
        {
            if (mode == enResourceLoadMode.Async)
            {
                LoadAssetFromAssetBundleAsync<T>(assetName, priority, method, loadAssetCallbacks, userData);
            }
            else
            {
                LoadAssetFromAssetBundleSync<T>(assetName, priority, method, loadAssetCallbacks, userData);
            }
        }

        public void LoadAsset<T>(ResourceLoadInfo resourceLoadInfo, LoadAssetCallbacks loadAssetCallbacks, object userData = null)
        {
            throw new System.NotImplementedException();
        }
    }
}