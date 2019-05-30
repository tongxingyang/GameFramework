using System;
using GameFramework.Pool.TaskPool;
using UnityEngine;

namespace GameFramework.Web.Base
{
    public class WebManager : IWebManager
    {
        private TaskPool<WebTask> taskPool;

        public int DefaultPriority = 50;
        
        public int TotalAgentCount => taskPool.TotalAgentsCount;
        public int FreeAgentCount => taskPool.FreeAgentsCount;
        public int WorkingAgentCount => taskPool.WorkingAgentsCount;
        public int WaitTaskCount => taskPool.WaitAgentsCount;

        public WebManager()
        {
            taskPool = new TaskPool<WebTask>();
        }

        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            taskPool.OnUpdate(elapseSeconds,realElapseSeconds);
        }

        public void Shutdown()
        {
            taskPool.ShotDown();
        }
        
        public int AddGetWebRequest(string webUrl, float timeOut, Action<bool,string, byte[]> callAction)
        {
            WebTask webTask = new WebTask(webUrl,DefaultPriority,timeOut,callAction);
            taskPool.AddTask(webTask);
            return webTask.SerialId;
        }

        public int AddPostWebRequest(string webUrl, float timeOut, byte[] postData, Action<bool,string, byte[]> callAction)
        {
            WebTask webTask = new WebTask(webUrl,DefaultPriority,timeOut,postData,callAction);
            taskPool.AddTask(webTask);
            return webTask.SerialId;
        }

        public int AddPostWebRequest(string webUrl, float timeOut, WWWForm wwwForm, Action<bool,string, byte[]> callAction)
        {
            WebTask webTask = new WebTask(webUrl,DefaultPriority,timeOut,wwwForm,callAction);
            taskPool.AddTask(webTask);
            return webTask.SerialId;
        }

        public bool RemoveWebRequest(int serialId)
        {
            return taskPool.RemoveTask(serialId).SerialId != -1;
        }

        public void RemoveAllWebRequest()
        {
            taskPool.RemoveAllTaks();
        }

        public void AddWebRequestAgent(WebAgent webAgent)
        {
            taskPool.AddAgent(webAgent);
        }
        
    }
}