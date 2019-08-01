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

        public void CopyFile(string srcFileUrl, string desFileUrl ,Action<bool,string> callBack)
        {
            StartCoroutine(CopyBinaryFileByCoroutine(srcFileUrl, desFileUrl, callBack));
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
    }
}