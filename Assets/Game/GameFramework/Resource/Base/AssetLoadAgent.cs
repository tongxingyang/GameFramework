using GameFramework.Pool.TaskPool;
using GameFramework.Utility.EncryptUtility;
using GameFramework.Utility.File;
using UnityEditor;
using UnityEngine;

namespace GameFramework.Res.Base
{
    public class AssetLoadAgent : ITaskAgent<AssetLoadTask>
    {
        public enum  enLoadState
        {
            None,
            Loading,
            Complete
        }

        private int FrameCount = 0;
        public AssetLoadTask Task { get; set; }
        private ResourceRequest resourceRequest;
        private AssetBundleCreateRequest assetBundleCreateRequest;
        private AssetBundleRequest assetBundleRequest;
        private enLoadState LoadState = enLoadState.None;

        private AssetBundle assetBundle;
        private Object assetObject;

        private bool isLoadBundleSuccess = false;
        
        public void Initialize()
        {
            assetBundle = null;
            assetObject = null;
            resourceRequest = null;
            assetBundleCreateRequest = null;
            assetBundleRequest = null;
            LoadState = enLoadState.None;
            FrameCount = 0;
            isLoadBundleSuccess = false;
        }

        public void OnStart(AssetLoadTask task)
        {
            Task = task;
            if (Task.LoadType == enResouceLoadType.AssetBundle)
            {
                LoadAssetBundle();
                
            }else if (Task.LoadType == enResouceLoadType.Resources)
            {
                LoadResource();
            }
            LoadState = enLoadState.Loading;
        }

        private void LoadAssetBundle()
        {
            if (Task.LoadMode == enResourceLoadMode.Async)
            {
                if (AppConst.ResourceConfig.IsUseAssetBundle)
                {
                    AssetBundleLoadByTypeAsync();
                }
                else
                {
                    FrameCount = 0;
                }
                
            }else if (Task.LoadMode == enResourceLoadMode.Sync)
            {
                if (AppConst.ResourceConfig.IsUseAssetBundle)
                {
                    AssetBundleLoadByTypeSync();
                }
                else
                {
                    assetObject = AssetDatabase.LoadAssetAtPath(Task.AssetPath, Task.ResType);
                    Complete(assetObject);
                }
            }
        }

        private void AssetBundleLoadByTypeAsync()
        {
            if (Task.LoadMethod == enResourceLoadMethod.LoadFromFile)
            {
                assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(Task.BundlePath);
            }else if (Task.LoadMethod == enResourceLoadMethod.LoadFromMemory)
            {
                assetBundleCreateRequest = AssetBundle.LoadFromMemoryAsync(FileUtility.ReadAllBytes(Task.BundlePath));
            }else if (Task.LoadMethod == enResourceLoadMethod.LoadFromMemoryDecrypt)
            {
                assetBundleCreateRequest = AssetBundle.LoadFromMemoryAsync(EncryptUtility.AssetBundleDecrypt(FileUtility.ReadAllBytes(Task.BundlePath)));
            }else if (Task.LoadMethod == enResourceLoadMethod.LoadFromStream)
            {
                assetBundleCreateRequest = AssetBundle.LoadFromStreamAsync(FileUtility.Open(Task.BundlePath));
            }
        }
        
        private void AssetBundleLoadByTypeSync()
        {
            if (Task.LoadMethod == enResourceLoadMethod.LoadFromFile)
            {
                assetBundle = AssetBundle.LoadFromFile(Task.BundlePath);
                assetObject = assetBundle.LoadAsset(Task.AssetPath, Task.ResType);
                
            }else if (Task.LoadMethod == enResourceLoadMethod.LoadFromMemory)
            {
                assetBundle = AssetBundle.LoadFromMemory(FileUtility.ReadAllBytes(Task.BundlePath));
                assetObject = assetBundle.LoadAsset(Task.AssetPath, Task.ResType);
                
            }else if (Task.LoadMethod == enResourceLoadMethod.LoadFromMemoryDecrypt)
            {
                assetBundle = AssetBundle.LoadFromMemory(EncryptUtility.AssetBundleDecrypt(FileUtility.ReadAllBytes(Task.BundlePath)));
                assetObject = assetBundle.LoadAsset(Task.AssetPath, Task.ResType);
                
            }else if (Task.LoadMethod == enResourceLoadMethod.LoadFromStream)
            {
                assetBundle = AssetBundle.LoadFromStream(FileUtility.Open(Task.BundlePath));
                assetObject = assetBundle.LoadAsset(Task.AssetPath, Task.ResType);
            }
        }
        
        private void LoadResource()
        {
            if (Task.LoadMode == enResourceLoadMode.Async)
            {
                resourceRequest = Resources.LoadAsync(Task.AssetPath, Task.ResType);
                
            }else if (Task.LoadMode == enResourceLoadMode.Sync)
            {
                assetObject = Resources.Load(Task.AssetPath, Task.ResType);
                Complete(assetObject);
            }
        }
        
        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            if (LoadState == enLoadState.Loading)
            {
                if (Task.LoadType == enResouceLoadType.Resources)
                {
                    if (resourceRequest != null)
                    {
                        if (resourceRequest.isDone)
                        {
                            Complete(resourceRequest.asset);
                            resourceRequest = null;
                        }
                    }
                    else
                    {
                        Failed();
                    }
                }
                else if (Task.LoadType == enResouceLoadType.AssetBundle)
                {
                    if (AppConst.ResourceConfig.IsUseAssetBundle)
                    {
                        if (assetBundleCreateRequest != null)
                        {
                            if (!isLoadBundleSuccess)
                            {
                                if (assetBundleCreateRequest.isDone)
                                {
                                    isLoadBundleSuccess = true;
                                    assetBundle = assetBundleCreateRequest.assetBundle;
                                    assetBundleRequest = assetBundle.LoadAssetAsync(Task.AssetPath, Task.ResType);
                                }
                            }
                        }
                        else
                        {
                            Failed();
                        }
                        
                        if (assetBundleRequest != null)
                        {
                            if (assetBundleRequest.isDone)
                            {
                                assetObject = assetBundleRequest.asset;
                                Complete(assetObject);
                            }
                        }
                        else
                        {
                            Failed();
                        }
                    }
                    else
                    {
                        if (FrameCount >= AppConst.ResourceConfig.AssetDatabaseDelayFrame)
                        {
                            assetObject = AssetDatabase.LoadAssetAtPath(Task.AssetPath, Task.ResType);
                            Complete(assetObject);
                            FrameCount = 0;
                        }
                        FrameCount++;
                    }
                }
            }
        }

        public void Complete(Object data)
        {
            if (data == null)
            {
                Task.CallBack(false, null);
            }
            else
            {
                Task.CallBack(true, data);
            }
            Task.Done = true;
            LoadState = enLoadState.Complete;
        }

        public void Failed()
        {
            Task.CallBack(false, null);
            Task.Done = true;
            LoadState = enLoadState.Complete;
        }
        
        public void OnReset()
        {
            assetBundle = null;
            assetObject = null;
            isLoadBundleSuccess = false;
            Task = null;
            resourceRequest = null;
            assetBundleCreateRequest = null;
            assetBundleRequest = null;
            LoadState = enLoadState.None;
            FrameCount = 0;
        }

        public void ShotDown()
        {
            OnReset();
        }
    }
}