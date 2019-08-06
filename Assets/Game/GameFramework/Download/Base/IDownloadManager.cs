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
        int ThreadCount{ get; set;}
        bool IsOpenbrokenpointdownload { get; set; }
        float CurrentSpeed{ get; }
        int AddDownload(string fileName,string downloadPath, string downloadUri, Action<DownloadTask, ulong> doneCallback,
            Action<DownloadTask, ulong,uint, float> updateCallback,
            Action<DownloadTask, string> errorCallback);
        int AddDownload(string fileName,string downloadPath, string downloadUri, Action<DownloadTask, ulong> doneCallback,
            Action<DownloadTask, ulong,uint, float> updateCallback,
            Action<DownloadTask, string> errorCallback, int priority);
        int AddDownload(string fileName,string downloadPath, string downloadUrl, Action<DownloadTask,ulong>  doneCallback, Action<DownloadTask,ulong,uint,float> updateCallback,
            Action<DownloadTask,string> errorCallback, int priority, int timeout,ulong filelength = 0);
        bool RemoveDownload(int serialId);
        void RemoveAllDownload();
    }
}