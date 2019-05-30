using System;

namespace GameFramework.Download.Base
{
    public interface IDownloadManager
    {
        int TotalAgentCount{ get; }
        int FreeAgentCount{ get; }
        int WorkingAgentCount{ get; }
        int WaitingTaskCount{ get; }
        int DefaultTimeout{ get; set;}
        int RetryCount{ get; set;}
        bool IsOpenMultiThread { get; set; }
        float CurrentSpeed{ get; }
        void AddDownloadAgent(DownloadAgent downloadAgent);
        int AddDownload(string downloadPath, string downloadUri, Action<DownloadTask, ulong> doneCallback,
            Action<DownloadTask, ulong, float> updateCallback,
            Action<DownloadTask, string> errorCallback);
        int AddDownload(string downloadPath, string downloadUri, Action<DownloadTask, ulong> doneCallback,
            Action<DownloadTask, ulong, float> updateCallback,
            Action<DownloadTask, string> errorCallback, int priority);
        int AddDownload(string downloadPath, string downloadUrl, Action<DownloadTask,ulong>  doneCallback, Action<DownloadTask,ulong,float> updateCallback,
            Action<DownloadTask,string> errorCallback, int priority, int timeout,ulong filelength = 0);
        bool RemoveDownload(int serialId);
        void RemoveAllDownload();
    }
}