using System;
using GameFramework.Base;
using GameFramework.Utility.Singleton;
using GameFramework.Web.Base;
using UnityEngine;

namespace GameFramework.Web
{
    public class WebComponent : GameFrameworkComponent
    {
        private WebManager webManager;
        private Transform instanceRoot;
        public override int Priority => SingletonMono<GameFramework>.GetInstance().WebRequestPriority;

        public int TotalAgentCount => webManager.TotalAgentCount;
        public int FreeAgentCount => webManager.FreeAgentCount;
        public int WorkingAgentCount => webManager.WorkingAgentCount;
        public int WaitTaskCount => webManager.WaitTaskCount;

        public override void OnAwake()
        {
            base.OnAwake();
            webManager = new WebManager();
            if (instanceRoot == null)
            {
                instanceRoot = (new GameObject("Web Request Root")).transform;
                instanceRoot.SetParent(gameObject.transform);
                instanceRoot.localScale = Vector3.one;
                instanceRoot.localPosition = Vector3.zero;
            }

            for (int i = 0; i < SingletonMono<GameFramework>.GetInstance().WebRequestCount; i++)
            {
                WebAgent webAgent = new GameObject("Web Request "+(i+1)).AddComponent<WebAgent>();
                webAgent.transform.SetParent(instanceRoot);
                webAgent.transform.localPosition = Vector3.zero;
                webAgent.transform.localScale = Vector3.one;
                webManager.AddWebRequestAgent(webAgent);
            }
            
        }
        
        public override void Shutdown()
        {
            base.Shutdown();
            webManager.Shutdown();
        }

        public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            webManager.OnUpdate(elapseSeconds,realElapseSeconds);
        }

        public int AddGetWebRequest(string webUrl, float timeOut, Action<bool,string, byte[]> callAction)
        {
            return webManager.AddGetWebRequest(webUrl, timeOut, callAction);
        }

        public int AddPostWebRequest(string webUrl, float timeOut, byte[] postData, Action<bool,string, byte[]> callAction)
        {
            return webManager.AddPostWebRequest(webUrl, timeOut, postData, callAction);
        }

        public int AddPostWebRequest(string webUrl, float timeOut, WWWForm wwwForm, Action<bool,string, byte[]> callAction)
        {
            return webManager.AddPostWebRequest(webUrl, timeOut, wwwForm, callAction);
        }

        public bool RemoveWebRequest(int serialId)
        {
            return webManager.RemoveWebRequest(serialId);
        }

        public void RemoveAllWebRequest()
        {
            webManager.RemoveAllWebRequest();
        }
    }
}