using System;
using GameFramework.Pool.TaskPool;

namespace GameFramework.Download.Base
{
    public class DownloadTask : ITask
    {
        private static int Serial = 0;

        public int SerialId { get; set; }
        public bool Done { get; set; } = false;
        public int Priority { get; set; }
        public ulong FileLength { get; set; }
        public enDownloadState DownloadState { get; set; } = enDownloadState.Todo;
        public string DownloadPath { get; set; }
        public string DownloadUrl { get; set; }
        public Action<DownloadTask,ulong> DownloadDoneAction { get; set; }
        public Action<DownloadTask,ulong,uint,float> DownLoadUpdateAction { get; set; }
        public Action<DownloadTask,string> DownloadErrorAction { get; set; }
        public int TimeOut { get; set; }
        public string FileName { get; set; }
        public DownloadTask(string fileName,string downloadPath, string downloadUrl, Action<DownloadTask,ulong>  doneCallback, Action<DownloadTask,ulong,uint,float> updateCallback,
            Action<DownloadTask,string> errorCallback, int priority, int timeout,ulong filelength = 0)
        {
            FileName = fileName;
            SerialId = Serial++;
            DownloadPath = downloadPath;
            DownloadUrl = downloadUrl;
            DownloadDoneAction = doneCallback;
            DownLoadUpdateAction = updateCallback;
            DownloadErrorAction = errorCallback;
            Priority = priority;
            TimeOut = timeout;
            FileLength = filelength;
            DownloadState = enDownloadState.Todo;
            Done = false;
        }

        public void Clear()
        {
            FileName = String.Empty;
            SerialId = -1;
            DownloadPath = String.Empty;
            DownloadUrl = String.Empty;
            DownloadDoneAction = null;
            DownLoadUpdateAction = null;
            DownloadErrorAction = null;
            Priority = 0;
            TimeOut = 0;
            FileLength = 0;
            DownloadState = enDownloadState.Todo;
            Done = false;
        }

    }
}