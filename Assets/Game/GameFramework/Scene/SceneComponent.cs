using System;
using GameFramework.Base;
using GameFramework.Res;
using GameFramework.Res.Base;
using GameFramework.Utility.Extension;
using GameFramework.Utility.Singleton;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameFramework.Scene
{
    public class SceneComponent : GameFrameworkComponent
    {
        public override int Priority => SingletonMono<GameFramework>.GetInstance().ScenePriority;
        
        private int SceneIndex = -1;
        private string lastSceneName;
        private string targetScene;
        private string loadingScene = "loading";
        private bool loadSceneComplete = false;
        private float loadingProgress = 0f;
        private AsyncOperation loadingBarAsyncOperation = null;
        private AsyncOperation loadingSceneAsyncOperation = null;
        private AsyncOperation unloadAssetsAsyncOperation = null;
        private Action sceneLoadedCallback = null;
        private Action<float> sceneLoadingProgress = null;
        private bool isBundleLoaded = false;

        public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            if (loadSceneComplete)
            {
                return;
            }
            if (null != loadingBarAsyncOperation)
            {
                sceneLoadingProgress?.Invoke(0.0f);
                if (loadingBarAsyncOperation.isDone)
                {
                    loadingBarAsyncOperation = null;
                    sceneLoadingProgress?.Invoke(0.0f);
                    unloadAssetsAsyncOperation = Resources.UnloadUnusedAssets();
                }
            }else if (unloadAssetsAsyncOperation != null)
            {
                if (unloadAssetsAsyncOperation.isDone)
                {
                    unloadAssetsAsyncOperation = null;
                    Singleton<GameEntry>.GetInstance().GetComponent<ResourceComponent>().ClearMemory();
                    if (!AppConst.ResourceConfig.IsUseAssetBundle)
                    {
                        isBundleLoaded = true;
                    }
                    else
                    {
                        Singleton<GameEntry>.GetInstance().GetComponent<ResourceComponent>().RequestResource(GetSceneName(targetScene),
                            (a, b) =>
                            {
                                isBundleLoaded = true;
                            }, enResourceLoadMode.Async, enResourceLoadCache.PermanentLoad);
                    }
                }
                else
                {
                    sceneLoadingProgress(0.0f + unloadAssetsAsyncOperation.progress * 0.15f);
                }
            }else if (isBundleLoaded)
            {
                loadingSceneAsyncOperation = SceneManager.LoadSceneAsync(GetSceneName(targetScene));
                loadingSceneAsyncOperation.allowSceneActivation = false;
                isBundleLoaded = false;
                
            }else if (null != loadingSceneAsyncOperation)
            {
                if (loadingSceneAsyncOperation.isDone)
                {
                    loadingSceneAsyncOperation.allowSceneActivation = true;
                    var obj = GameObject.Find("Scene");
                    if (obj)
                    {
                        obj.StaticBatching();
                    }
                    sceneLoadedCallback?.Invoke();
                    sceneLoadedCallback = null;
                    loadingSceneAsyncOperation = null;
                    Singleton<GameEntry>.GetInstance().GetComponent<ResourceComponent>().UnloadLoadTypeResourceByName(enResourceLoadCache.PermanentLoad, GetSceneName(targetScene));
                }
                else
                {
                    sceneLoadingProgress(0.15f + loadingSceneAsyncOperation.progress * 0.85f);
                }
            }
        }

        public string GetSceneName(string name)
        {
            return name;
        }
        
        public void LoadScene(string name, Action onFinish = null , Action<float> onProgress = null)
        {
            LoadSceneImpl(name, onFinish, onProgress);
        }

        private void LoadSceneImpl(string name, Action onFinish = null ,Action<float> onProgress = null)
        {
            targetScene = name;
            BeginLoadingScene(name);
            if (null == loadingBarAsyncOperation)
            {
                loadingBarAsyncOperation = SceneManager.LoadSceneAsync(loadingScene);
                sceneLoadedCallback = onFinish;
                sceneLoadingProgress = onProgress;
            }
        }

        private void BeginLoadingScene(string name)
        {
            loadingProgress = 0.0f;
            loadSceneComplete = false;
            isBundleLoaded = false;
        }

        public bool IsLoading()
        {
            return loadingProgress > 0 && loadingProgress < 0.999f;
        }
    }
}