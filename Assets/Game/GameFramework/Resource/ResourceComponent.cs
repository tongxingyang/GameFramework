using System;
using System.Collections;
using System.IO;
using GameFramework.Base;
using GameFramework.Res.Base;
using GameFramework.Utility.File;
using GameFramework.Utility.Singleton;
using UnityEngine;
using UnityEngine.Networking;

namespace GameFramework.Res
{
    [DisallowMultipleComponent]
    public class ResourceComponent : GameFrameworkComponent
    {
        public override int Priority => SingletonMono<GameFramework>.GetInstance().ResPriority;
        
        public string PlatformName = String.Empty;
        public string AssetBundleVariant = String.Empty;
        
        private IResourceManager resourceManager;
        private bool isInit = false;
        public override void OnAwake()
        {
            base.OnAwake();
            resourceManager = new ResourceManager();
            resourceManager.LoadType = AppConst.ResourceConfig.IsUseAssetBundle ? enResouceLoadType.AssetBundle : enResouceLoadType.AssetDatabase;
            
        }

        public void InitResourceManager()
        {
            resourceManager.InitManager();
            resourceManager.PlatformName = PlatformName;
            resourceManager.LoadAllDependInfo();
            isInit = true;
        }
        
        public void RequestResource(string name, AssetBundleLoader.OnLoadAllBundle callback,
            enResourceLoadMode loadMode = enResourceLoadMode.Sync,
            enResourceLoadCache loadCache = enResourceLoadCache.NormalLoad,
            enResourceLoadMethod loadMethod = enResourceLoadMethod.LoadFromFile,object userdata = null)
        {
            if (isInit)
            {
                resourceManager.RequestResource(name, callback, loadMode, loadCache, loadMethod,userdata);
            }
        }

        public void StartResourceRecycling()
        {
            resourceManager.StartResourceRecycling();
        }
        
        public void AddToWhiteList(string name)
        {
            resourceManager.AddToWhiteList(name);
        }
        
        public IResourceManager GetResourceManager()
        {
            return resourceManager;
        }
        
        public void UnloadAllUnusedPreloadResources()
        {
            resourceManager.UnloadAllUnusedPreloadResources();
        }
        
        public void UnloadAllUnusedNormalResources()
        {
            resourceManager.UnloadAllUnusedNormalResources();
        }

        public void UnloadLoadTypeResourceByName(enResourceLoadCache loadCache, string name)
        {
            resourceManager.UnloadLoadTypeResourceByName(loadCache,name);
        }
        
        public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            if (isInit)
            {
                base.OnUpdate(elapseSeconds, realElapseSeconds);
                resourceManager.Update();  
            }
        }

        public void CopyFile(string srcFileUrl, string desFileUrl ,Action<bool,string> callBack)
        {
            StartCoroutine(CopyBinaryFileByCoroutine(srcFileUrl, desFileUrl, callBack));
        }
        
        public void LoadBinaryFile(string fileUrl,Action<string,byte[],string> callBack)
        {
            StartCoroutine(LoadBinaryFileByCoroutine(fileUrl, callBack));
        }
        
        public void ClearMemory()
        {
            GC.Collect();
//            LuaManager mgr = AppFacade.Instance.GetManager<LuaManager>(ManagerName.Lua);
//            if (mgr != null) mgr.LuaGC();
        }
        
        private IEnumerator LoadBinaryFileByCoroutine(string fileUri, Action<string,byte[],string> callBack)
        {
            byte[] bytes = null;
            string errorMessage = null;
            bool isError = false;
            UnityWebRequest unityWebRequest = UnityWebRequest.Get(fileUri);
            yield return unityWebRequest.SendWebRequest();
            isError = unityWebRequest.isNetworkError;
            errorMessage = isError ? unityWebRequest.error : null;
            bytes = unityWebRequest.downloadHandler.data;
            unityWebRequest.Dispose();
            callBack?.Invoke(fileUri, bytes, errorMessage);
        }
        
        private IEnumerator CopyBinaryFileByCoroutine(string srcFileUrl, string desFileUrl ,Action<bool,string> callBack)
        {
            byte[] bytes = null;
            string errorMessage = null;
            bool isError = false;
            UnityWebRequest unityWebRequest = UnityWebRequest.Get(srcFileUrl);
            yield return unityWebRequest.SendWebRequest();
            isError = unityWebRequest.isNetworkError;
            errorMessage = isError ? unityWebRequest.error : null;
            bytes = unityWebRequest.downloadHandler.data;
            unityWebRequest.Dispose();
            if (!isError)
            {
                string desPath = Path.GetDirectoryName(desFileUrl);
                if (!FileUtility.IsDirectoryExist(desPath))
                {
                    FileUtility.CreateDirectory(desPath);
                }
                if (FileUtility.IsFileExist(desFileUrl))
                {
                    FileUtility.DeleteFile(desFileUrl);
                }
                FileInfo fileInfo = new FileInfo(desFileUrl);
                using (Stream sw = fileInfo.Open(FileMode.Create,FileAccess.ReadWrite))
                {
                    if (bytes != null)
                    {
                        sw.Write(bytes,0,bytes.Length);
                    }
                }
            }
            callBack(isError, errorMessage);
        }
        
        #region Resources加载接口
        
        public IEnumerator IELoadResourceAsync<T>(string name,Action<UnityEngine.Object> callback) where T:UnityEngine.Object
        {
            ResourceRequest request = Resources.LoadAsync<T>(name);
            yield return request;
            callback?.Invoke(request.asset);
        }
        public void LoadResourceAsync<T>(string name, Action<UnityEngine.Object> callback) where T : UnityEngine.Object
        {
            StartCoroutine(IELoadResourceAsync<T>(name, callback));
        }
        public void LoadResource<T>(string name, Action<UnityEngine.Object> callback) where T : UnityEngine.Object
        {
            T t =Resources.Load<T>(name);
            callback?.Invoke(t);
        }
        public T LoadResource<T>(string name) where T : UnityEngine.Object
        {
            T t = Resources.Load<T>(name);
            return t;
        }
        
        #endregion
    }
}