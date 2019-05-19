using System;

namespace GameFramework.Download.Base
{
    public class DownloadManager : IDownloadManager
    {
        public int TotalAgentCount { get; }
        public int FreeAgentCount { get; }
        public int WorkingAgentCount { get; }
        public int WaitingTaskCount { get; }
        public float Timeout { get; set; }
        public float RetryCount { get; set; }
        public float CurrentSpeed { get; }
        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            
        }

        public void Shutdown()
        {
            
        }
        public void AddDownloadAgent(DownloadAgent downloadAgent)
        {
            throw new NotImplementedException();
        }

        public int AddDownload(string downloadPath, string downloadUri)
        {
            throw new NotImplementedException();
        }

        public int AddDownload(string downloadPath, string downloadUri, int priority)
        {
            throw new NotImplementedException();
        }

        public int AddDownload(string downloadPath, string downloadUri, object userData)
        {
            throw new NotImplementedException();
        }

        public int AddDownload(string downloadPath, string downloadUrl, Action<DownloadTask, ulong> doneCallback, Action<DownloadTask, ulong, float> updateCallback,
            Action<DownloadTask, string> errorCallback, int priority, int timeout, ulong filelength = 0)
        {
            throw new NotImplementedException();
        }

        public bool RemoveDownload(int serialId)
        {
            throw new NotImplementedException();
        }

        public void RemoveAllDownload()
        {
            throw new NotImplementedException();
        }
    }
}