using System;
using System.Text;
using GameFramework.Pool.TaskPool;
using UnityEngine;
using UnityEngine.Networking;

namespace GameFramework.Web.Base
{
    public class WebAgent : MonoBehaviour,ITaskAgent<WebTask>
    {
        private WebTask webTask;
        private UnityWebRequest unityWebRequest = null;
        private float waitTime;  

        public WebTask Task => webTask;
        public UnityWebRequest UnityWebRequest => unityWebRequest;
        public float WaitTime => waitTime;
        
        public void Awake()
        {

        }
        
        public void Initialize()
        {
            webTask = null;
            waitTime = 0;
        }

        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            if (webTask.State == enWebState.Doing)
            {
                waitTime += realElapseSeconds;
                if (waitTime >= webTask.TimeOut)
                {
                    RequestError("timeout");
                }
            }
            
            if (unityWebRequest == null || !unityWebRequest.isDone)
            {
                return;
            }
            bool isError = false;
            isError = unityWebRequest.isNetworkError;

            if (isError)
            {
                RequestError(unityWebRequest.error);
            }
            else if (unityWebRequest.downloadHandler.isDone)
            {
                RequestSuccese();
            }

        }

        public void ShotDown()
        {
            OnReset();
        }

        public void OnStart(WebTask task)
        {
            webTask = task;
            webTask.State = enWebState.Doing;
            if (webTask.WwwForm == null && webTask.GetPostBytes() == null)
            {
                GetRequest(webTask.WebUrl);
            }
            else
            {
                if (webTask.WwwForm != null)
                {
                    PostRequest(webTask.WebUrl,webTask.WwwForm);
                }
                else
                {
                    PostRequest(webTask.WebUrl,webTask.GetPostBytes());
                }
            }
            waitTime = 0;
        }

        public void GetRequest(string webUrl)
        {
            unityWebRequest = UnityEngine.Networking.UnityWebRequest.Get(webUrl);
            unityWebRequest.SendWebRequest();

        }

        public void PostRequest(string webUrl, byte[] postBytes)
        {
            unityWebRequest = UnityWebRequest.Post(webUrl, Encoding.UTF8.GetString(postBytes));
            unityWebRequest.SendWebRequest();

        }

        public void PostRequest(string webUrl, WWWForm wwwForm)
        {
            unityWebRequest = UnityWebRequest.Post(webUrl, wwwForm);
            unityWebRequest.SendWebRequest();
        }
        
        public void OnReset()
        {
            webTask.Clear();
            webTask = null;
            waitTime = 0;
            unityWebRequest.Dispose();
            unityWebRequest = null;
        }

        private void RequestError(string error)
        {
            webTask.State = enWebState.Error;
            webTask.callBack?.Invoke(false,error, null);
            webTask.Done = true;
        }

        private void RequestSuccese()
        {
            webTask.State = enWebState.Doing;
            webTask.callBack?.Invoke(true,unityWebRequest.downloadHandler.text, unityWebRequest.downloadHandler.data);
            webTask.Done = true;
        }
    }
}