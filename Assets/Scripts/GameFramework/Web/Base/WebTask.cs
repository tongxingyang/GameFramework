using System;
using GameFramework.Pool.TaskPool;
using UnityEngine;

namespace GameFramework.Web.Base
{
    public class WebTask :ITask
    {
        private static int Serial = 0;
        
        private int serialid = -1;
        private bool done = false;
        private int priority = 0;
        private enWebState state;
        private float timeOut = 0;
        private string webUrl;
        private WWWForm wwwForm;
        private byte[] postBytes = null;

        public Action<bool,byte[]> callBack;

        public byte[] GetPostBytes()
        {
            return postBytes;
        }
        
        public WWWForm WwwForm
        {
            get { return wwwForm; }
            set { wwwForm = value; }
        }
        
        public float TimeOut
        {
            get { return timeOut; }
            set { timeOut = value; }
        }
        
        public enWebState State
        {
            get { return state; }
            set { state = value; }
        }
        
        public string WebUrl
        {
            get { return webUrl; }
            set { webUrl = value; }
        }
        
        public int SerialId
        {
            get { return serialid; }
            set { serialid = value; }
        }

        public bool Done
        {
            get { return done; }
            set { done = value; }
        }

        public int Priority
        {
            get { return priority; }
            set { priority = value; }
        }

        public void Clear()
        {
            webUrl = string.Empty;
            callBack = null;
            wwwForm = null;
            postBytes = null;
        }
        
        public WebTask(string webUrl, int priority, float timeout,Action<bool,byte[]> callAction)
        {
            serialid = Serial++;
            this.priority = priority;
            done = false;
            state = enWebState.Todo;
            this.webUrl = webUrl;
            postBytes = null;
            this.timeOut = timeout;
            this.wwwForm = null;
            callBack = callAction;
        }
        
        public WebTask(string webUrl, int priority, float timeout,byte[] postdata,Action<bool,byte[]> callAction)
        {
            serialid = Serial++;
            this.priority = priority;
            done = false;
            state = enWebState.Todo;
            this.webUrl = webUrl;
            postBytes = postdata;
            this.timeOut = timeout;
            this.wwwForm = null;
            callBack = callAction;
        }
        
        public WebTask(string webUrl, int priority, float timeout,WWWForm wwwForm,Action<bool,byte[]> callAction)
        {
            serialid = Serial++;
            this.priority = priority;
            done = false;
            state = enWebState.Todo;
            this.webUrl = webUrl;
            postBytes = null;
            this.timeOut = timeout;
            this.wwwForm = wwwForm;
            callBack = callAction;
        }
    }
}