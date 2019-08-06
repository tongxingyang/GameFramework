using System;
using GameFramework.Pool.TaskPool;

namespace GameFramework.Download.Base
{
    public class DownloadManager : IDownloadManager
    {
        
        private readonly TaskPool<DownloadTask> taskPool;
        private int defaultPriority = 10;
        public int TotalAgentCount => taskPool.TotalAgentsCount;
        public int FreeAgentCount => taskPool.FreeAgentsCount;
        public int WorkingAgentCount => taskPool.WorkingAgentsCount;
        public int WaitingTaskCount => taskPool.WaitAgentsCount;
        public float CurrentSpeed { get; } = 0;
        public int DefaultTimeout { get; set; }
        public bool IsOpenbrokenpointdownload { get; set; }
        public int ThreadCount { get; set; }

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
        
        public void AddHttpWebDownloadAgent(HttpWebDownloadAgent downloadAgent)
        {
            downloadAgent.IsOpenbrokenpointdownload = IsOpenbrokenpointdownload;
            downloadAgent.ThreadCount = ThreadCount;
            taskPool.AddAgent(downloadAgent);
        }

        public int AddDownload(string fileName,string downloadPath, string downloadUri, Action<DownloadTask, ulong> doneCallback, Action<DownloadTask, ulong,uint, float> updateCallback,
            Action<DownloadTask, string> errorCallback)
        {
            return AddDownload(fileName,downloadPath, downloadUri, doneCallback, updateCallback, errorCallback, defaultPriority, DefaultTimeout);
        }

        public int AddDownload(string fileName,string downloadPath, string downloadUri,Action<DownloadTask, ulong> doneCallback, Action<DownloadTask, ulong, uint,float> updateCallback,
            Action<DownloadTask, string> errorCallback,  int priority)
        {
            return AddDownload(fileName,downloadPath, downloadUri, doneCallback, updateCallback, errorCallback, priority, DefaultTimeout);
        }

        public int AddDownload(string fileName,string downloadPath, string downloadUrl, Action<DownloadTask, ulong> doneCallback, Action<DownloadTask, ulong,uint, float> updateCallback,
            Action<DownloadTask, string> errorCallback, int priority, int timeout, ulong filelength = 0)
        {
            DownloadTask downloadTask = new DownloadTask(fileName,downloadPath, downloadUrl, doneCallback, updateCallback,
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