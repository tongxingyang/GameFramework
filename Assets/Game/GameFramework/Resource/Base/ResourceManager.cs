
using System.Collections.Generic;

namespace GameFramework.Res.Base
{
    public class ResourceManager : IResourceManager
    {
        public Dictionary<string ,string> AssetToMap = new Dictionary<string, string>();
        
        public void LoadAssetFromAssetBundleAsync<T>(string assetName, int priority,enResourceLoadMethod method, LoadCallback loadCallback,
            object userData)
        {
            
        }

        public void LoadAssetFromAssetBundleSync<T>(string assetName, int priority, enResourceLoadMethod method,
            LoadCallback loadCallback, object userData)
        {
            
        }
        
        public void LoadAssetFromResourceAsync<T>(string assetName, LoadCallback loadCallback ,object userData)
        {
            
        }

        public void LoadAssetFromResourceSync<T>(string assetName, LoadCallback loadCallback, object userData)
        {
            
        }

        public void LoadAssetFromResource<T>(string assetName, enResourceLoadMode mode,
            LoadCallback loadCallback, object userData)
        {
            if (mode == enResourceLoadMode.Async)
            {
                LoadAssetFromResourceAsync<T>(assetName,loadCallback,userData);
            }
            else
            {
                LoadAssetFromResourceSync<T>(assetName,loadCallback,userData);
            }
        }

        public void LoadAssetFromAssetBundle<T>(string assetName, enResourceLoadMode mode, int priority, enResourceLoadMethod method,
            LoadCallback loadCallback, object userData)
        {
            if (mode == enResourceLoadMode.Async)
            {
                LoadAssetFromAssetBundleAsync<T>(assetName, priority, method, loadCallback, userData);
            }
            else
            {
                LoadAssetFromAssetBundleSync<T>(assetName, priority, method, loadCallback, userData);
            }
        }

        public void LoadAsset<T>(ResourceLoadInfo resourceLoadInfo, LoadCallback loadCallback, object userData = null)
        {
            throw new System.NotImplementedException();
        }
    }
}