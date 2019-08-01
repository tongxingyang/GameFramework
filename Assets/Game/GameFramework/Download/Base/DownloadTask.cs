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
        private Action<DownloadTask,ulong,uint,float> downloadUpdateAction = null;
        private int timeOut;
        public int SerialId => serialid;
        public string DownloadPath => downloadPath;
        public string DownloadUrl => downloadUrl;
        public ulong FileLength => fileLength;
        public Action<DownloadTask,ulong> DownloadDoneAction => downloadDoneAction;
        public Action<DownloadTask,ulong,uint,float> DownLoadUpdateAction => downloadUpdateAction;
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

        public string FileName;
        public DownloadTask(string fileName,string downloadPath, string downloadUrl, Action<DownloadTask,ulong>  doneCallback, Action<DownloadTask,ulong,uint,float> updateCallback,
            Action<DownloadTask,string> errorCallback, int priority, int timeout,ulong filelength = 0)
        {
            FileName = fileName;
            serialid = Serial++;
            this.downloadPath = downloadPath;
            this.downloadUrl = downloadUrl;
            downloadDoneAction = doneCallback;
            downloadUpdateAction = updateCallback;
            downloadErrorAction = errorCallback;
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