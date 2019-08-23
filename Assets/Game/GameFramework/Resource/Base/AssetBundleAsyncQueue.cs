using System.Collections;
using System.Collections.Generic;
using GameFramework.Base;
using GameFramework.Utility.EncryptUtility;
using GameFramework.Utility.File;
using GameFramework.Utility.Singleton;
using UnityEngine;

namespace GameFramework.Res.Base
{
    public class AssetBundleAsyncQueue
    {
        public static Queue<AssetBundleLoader> AssetBundleAsyncLoadQueue = new Queue<AssetBundleLoader>();

        public bool IsLoading;

        public AssetBundleLoader CurrentLoader;
        public string CurrentLoaderName;

        public AssetBundleAsyncQueue()
        {
            IsLoading = false;
        }
        
        public void StartAssetBundleAsyncLoad()
        {
            if (IsLoading == false)
            {
                Singleton<GameEntry>.GetInstance().GetComponent<ResourceComponent>().StartCoroutine(AssetBundleLoadAsync());
                IsLoading = true;
            }
        }

        public static void AddToAsyncLoadQueue(AssetBundleLoader loader)
        {
            if (loader.LoadMode == enResourceLoadMode.Async)
            {
                AssetBundleAsyncLoadQueue.Enqueue(loader);
            }
        }

        private IEnumerator AssetBundleLoadAsync()
        {
            while (true)
            {
                if (AssetBundleAsyncLoadQueue.Count > 0)
                {
                    CurrentLoader = AssetBundleAsyncLoadQueue.Dequeue();
                    CurrentLoaderName = CurrentLoader.AssetBundleName;
                    if (CurrentLoader.LoadState == enLoadState.None)
                    {

                    }
                    else
                    {
                        CurrentLoader.LoadState = enLoadState.Loading;
                        AssetBundleCreateRequest assetBundleCreateRequest = null;
                        if (CurrentLoader.LoadMethod == enResourceLoadMethod.LoadFromFile)
                        {
                            assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(CurrentLoader.AssetBundleName);
                        }else if (CurrentLoader.LoadMethod == enResourceLoadMethod.LoadFromMemory)
                        {
                            assetBundleCreateRequest =
                                AssetBundle.LoadFromMemoryAsync(
                                    FileUtility.ReadAllBytes(CurrentLoader.AssetBundleName));
                        }else if (CurrentLoader.LoadMethod == enResourceLoadMethod.LoadFromMemoryDecrypt)
                        {
                            assetBundleCreateRequest = AssetBundle.LoadFromMemoryAsync(
                                EncryptUtility.AssetBundleDecrypt(
                                    FileUtility.ReadAllBytes(CurrentLoader.AssetBundleName)));
                        }else if (CurrentLoader.LoadMethod == enResourceLoadMethod.LoadFromStream)
                        {
                            assetBundleCreateRequest =
                                AssetBundle.LoadFromStreamAsync(FileUtility.Open(CurrentLoader.AssetBundleName));
                        }
                        yield return assetBundleCreateRequest;
                        if (CurrentLoaderName == CurrentLoader.AssetBundleName && CurrentLoader.LoadState != enLoadState.None)
                        {
                            if (assetBundleCreateRequest != null)
                            {
                                CurrentLoader?.OnSelfAssetBundleLoadComplete(assetBundleCreateRequest.assetBundle);
                            }
                        }
                    }
                    CurrentLoader = null;
                }
                else
                {
                    yield return null;
                }
            }
        }
    }
}