using System.Text;
using GameFramework.Pool.TaskPool;
using UnityEngine;
using UnityEngine.Networking;

namespace GameFramework.Web.Base
{
    public class WebAgent : MonoBehaviour,ITaskAgent<WebTask>
    {
        private WebTask webTask;
        
        public WebTask Task => webTask;

        private UnityWebRequest unityWebRequest = null;

        public UnityWebRequest UnityWebRequest => unityWebRequest;

        private float waitTime;  

        public float WaitTime => waitTime;
        
        void Awake()
        {
            webTask = null;
            waitTime = 0;
            
        }
        
        public void Initialize()
        {
            
        }

        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            if (webTask.State == enWebState.Doing)
            {
                waitTime += realElapseSeconds;
                if (waitTime >= webTask.TimeOut)
                {
                    RequestError();
                }
            }
            
            if (unityWebRequest == null || !unityWebRequest.isDone)
            {
                return;
            }
            bool isError = false;
#if UNITY_2017_1_OR_NEWER
            isError = unityWebRequest.isNetworkError;
#else
            isError = m_UnityWebRequest.isError;
#endif
            if (isError)
            {
                RequestError();
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
#if UNITY_2017_2_OR_NEWER
            unityWebRequest.SendWebRequest();
#else
            unityWebRequest.Send();
#endif
        }

        public void PostRequest(string webUrl, byte[] postBytes)
        {
            unityWebRequest = UnityWebRequest.Post(webUrl, Encoding.UTF8.GetString(postBytes));
#if UNITY_2017_2_OR_NEWER
            unityWebRequest.SendWebRequest();
#else
            unityWebRequest.Send();
#endif
        }

        public void PostRequest(string webUrl, WWWForm wwwForm)
        {
            unityWebRequest = UnityWebRequest.Post(webUrl, wwwForm);
#if UNITY_2017_2_OR_NEWER
            unityWebRequest.SendWebRequest();
#else
            unityWebRequest.Send();
#endif
        }
        
        public void OnReset()
        {
            webTask.Clear();
            webTask = null;
            waitTime = 0;
            unityWebRequest.Dispose();
            unityWebRequest = null;
        }

        private void RequestError()
        {
            webTask.State = enWebState.Error;
            webTask.callBack?.Invoke(false, null);
            webTask.Done = true;
            OnReset();
        }

        private void RequestSuccese()
        {
            webTask.State = enWebState.Doing;
            webTask.callBack?.Invoke(true, unityWebRequest.downloadHandler.data);
            webTask.Done = true;
            OnReset();
        }
    }
}