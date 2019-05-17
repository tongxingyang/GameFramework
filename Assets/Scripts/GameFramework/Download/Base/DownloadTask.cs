using System;
using GameFramework.Pool.TaskPool;

namespace GameFramework.Download.Base
{
    public class DownloadTask : ITask
    {
        private static int Serial = 0;
        
        private int serialid = -1;
        private bool done = false;
        private int priority = 0;
        private ulong fileLength;
        private enDownloadState downloadState = enDownloadState.Todo;
        private string downloadPath = string.Empty;
        private string downloadUrl = String.Empty;
        private Action<DownloadTask,ulong> downloadDoneAction = null;
        private Action<DownloadTask,string> downloadErrorAction = null;
        private Action<DownloadTask,ulong,float> downloadUpdateAction = null;
        private int timeOut;
        public int SerialId => serialid;
        public string DownloadPath => downloadPath;
        public string DownloadUrl => downloadUrl;
        public ulong FileLength => fileLength;
        public Action<DownloadTask,ulong> DownloadDoneAction => downloadDoneAction;
        public Action<DownloadTask,ulong,float> DownLoadUpdateAction => downloadUpdateAction;
        public Action<DownloadTask,string> DownloadErrorAction => downloadErrorAction;
        public int TimeOut => timeOut;
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
        public enDownloadState DownloadState
        {
            get { return downloadState; }
            set { downloadState = value; }
        }
        public DownloadTask(string path, string url, Action<DownloadTask,ulong>  done, Action<DownloadTask,ulong,float> update,
            Action<DownloadTask,string> error, int priority, int timeout,ulong filelength = 0)
        {
            serialid = Serial++;
            downloadPath = path;
            downloadUrl = url;
            downloadDoneAction = done;
            downloadUpdateAction = update;
            downloadErrorAction = error;
            this.priority = priority;
            this.timeOut = timeout;
            this.fileLength = filelength;
            downloadState = enDownloadState.Todo;
            this.done = false;
        }

        public void Clear()
        {
            downloadPath = string.Empty;
            downloadUrl = String.Empty;
            downloadDoneAction = null;
            downloadUpdateAction = null;
            downloadErrorAction = null;
            fileLength = 0;
            done = true;
        }

    }
}