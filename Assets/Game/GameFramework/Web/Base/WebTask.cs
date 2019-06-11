using System;
using GameFramework.Pool.TaskPool;
using UnityEngine;

namespace GameFramework.Web.Base
{
    public class WebTask : ITask
    {
        private static int Serial = 0;
        private int serialId = -1;
        private bool done = false;
        private int priority = 0;
        private enWebState state;
        private float timeOut = 0;
        private string webUrl;
        private WWWForm wwwForm;
        private byte[] postBytes = null;

        public Action<bool,string,byte[]> callBack;

        public byte[] GetPostBytes()
        {
            return postBytes;
        }
        
        public WWWForm WwwForm
        {
            get => wwwForm;
            set => wwwForm = value;
        }
        
        public float TimeOut
        {
            get => timeOut;
            set => timeOut = value;
        }
        
        public enWebState State
        {
            get => state;
            set => state = value;
        }
        
        public string WebUrl
        {
            get => webUrl;
            set => webUrl = value;
        }
        
        public int SerialId
        {
            get => serialId;
            set => serialId = value;
        }

        public bool Done
        {
            get => done;
            set => done = value;
        }

        public int Priority
        {
            get => priority;
            set => priority = value;
        }

        public void Clear()
        {
            webUrl = string.Empty;
            callBack = null;
            wwwForm = null;
            postBytes = null;
        }
        
        public WebTask(string webUrl, int priority, float timeout,Action<bool,string,byte[]> callAction)
        {
            serialId = Serial++;
            this.priority = priority;
            done = false;
            state = enWebState.Todo;
            this.webUrl = webUrl;
            postBytes = null;
            this.timeOut = timeout;
            this.wwwForm = null;
            callBack = callAction;
        }
        
        public WebTask(string webUrl, int priority, float timeout,byte[] postdata,Action<bool,string,byte[]> callAction)
        {
            serialId = Serial++;
            this.priority = priority;
            done = false;
            state = enWebState.Todo;
            this.webUrl = webUrl;
            postBytes = postdata;
            this.timeOut = timeout;
            this.wwwForm = null;
            callBack = callAction;
        }
        
        public WebTask(string webUrl, int priority, float timeout,WWWForm wwwForm,Action<bool,string,byte[]> callAction)
        {
            serialId = Serial++;
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