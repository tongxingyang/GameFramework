using System;
using System.Collections;
using GameFramework.Base;
using GameFramework.Res.Base;
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

        public override void OnAwake()
        {
            base.OnAwake();
            
            if (SingletonMono<GameFramework>.GetInstance().EditorResourceMode)
            {
                resourceManager = new EditorResourceManager();
            }
            else
            {
                resourceManager = new ResourceManager();
            }
        }

        public ResourceComponent()
        {
           
        }

        public IResourceManager GetResourceManager()
        {
            return resourceManager;
        }

        public void LoadBinaryFile(string fileUrl,Action<string,byte[],string> callBack)
        {
            StartCoroutine(LoadBinaryFileByCoroutine(fileUrl, callBack));
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
    }
}