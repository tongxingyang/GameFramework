using System;
using GameFramework.Pool.TaskPool;

namespace GameFramework.Download.Base
{
    public class DownloadManager : IDownloadManager
    {
        
        private TaskPool<DownloadTask> taskPool;
        private int defaultTimeOut = 10000;
        private int retryCount = 3;
        private bool isOpenMultiThread = false;
        private float currentSpeed = 0;
        private int defaultPriority = 10;
        public int TotalAgentCount => taskPool.TotalAgentsCount;
        public int FreeAgentCount => taskPool.FreeAgentsCount;
        public int WorkingAgentCount => taskPool.WorkingAgentsCount;
        public int WaitingTaskCount => taskPool.WaitAgentsCount;
        public float CurrentSpeed => currentSpeed;

        public int DefaultTimeout
        {
            get { return defaultTimeOut; }
            set { defaultTimeOut = value; }
        }

        public int RetryCount
        {
            get { return retryCount; }
            set { retryCount = value; }
        }

        public bool IsOpenMultiThread
        {
            get { return isOpenMultiThread; }
            set { isOpenMultiThread = value; }
        }

        public DownloadManager()
        {
            taskPool = new TaskPool<DownloadTask>();
        }
        
        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            taskPool.OnUpdate(elapseSeconds,realElapseSeconds);
        }

        public void Shutdown()
        {
            taskPool.ShotDown();
        }
        
        public void AddDownloadAgent(DownloadAgent downloadAgent)
        {
            taskPool.AddAgent(downloadAgent);
        }

        public int AddDownload(string downloadPath, string downloadUri, Action<DownloadTask, ulong> doneCallback, Action<DownloadTask, ulong, float> updateCallback,
            Action<DownloadTask, string> errorCallback)
        {
            return AddDownload(downloadPath, downloadUri, doneCallback, updateCallback, errorCallback, defaultPriority, defaultTimeOut);
        }

        public int AddDownload(string downloadPath, string downloadUri,Action<DownloadTask, ulong> doneCallback, Action<DownloadTask, ulong, float> updateCallback,
            Action<DownloadTask, string> errorCallback,  int priority)
        {
            return AddDownload(downloadPath, downloadUri, doneCallback, updateCallback, errorCallback, priority, defaultTimeOut);
        }

        public int AddDownload(string downloadPath, string downloadUrl, Action<DownloadTask, ulong> doneCallback, Action<DownloadTask, ulong, float> updateCallback,
            Action<DownloadTask, string> errorCallback, int priority, int timeout, ulong filelength = 0)
        {
            DownloadTask downloadTask = new DownloadTask(downloadPath, downloadUrl, doneCallback, updateCallback,
                errorCallback, priority, timeout, filelength);
            taskPool.AddTask(downloadTask);
            return downloadTask.SerialId;
        }

        public bool RemoveDownload(int serialId)
        {
            return taskPool.RemoveTask(serialId) != null;
        }

        public void RemoveAllDownload()
        {
            taskPool.RemoveAllTasks();
        }
    }
}