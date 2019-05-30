using System;
using GameFramework.Base;
using GameFramework.Download.Base;
using GameFramework.Utility.Singleton;
using UnityEngine;

namespace GameFramework.Download
{
    [DisallowMultipleComponent]
    public class DownloadComponent : GameFrameworkComponent
    {
        private DownloadManager downloadManager;

        private Transform instanceRoot;
        public override int Priority => SingletonMono<GameFramework>.GetInstance().DownloadPriority;

        public int TotalAgentCount => downloadManager.TotalAgentCount;
        public int FreeAgentCount => downloadManager.FreeAgentCount;
        public int WorkingAgentCount => downloadManager.WorkingAgentCount;
        public int WaitingTaskCount => downloadManager.WaitingTaskCount;
        public float CurrentSpeed => downloadManager.CurrentSpeed;
        public int DefaultTimeout
        {
            get { return downloadManager.DefaultTimeout; }
            set { downloadManager.DefaultTimeout = value; }
        }

        public int RetryCount
        {
            get { return downloadManager.RetryCount; }
            set { downloadManager.RetryCount = value; }
        }

        public bool IsOpenMultiThread
        {
            get { return downloadManager.IsOpenMultiThread; }
            set { downloadManager.IsOpenMultiThread = value; }
        }
        
        public override void OnAwake()
        {
            base.OnAwake();
            downloadManager = new DownloadManager();
            if (instanceRoot == null)
            {
                instanceRoot = (new GameObject("Download Root")).transform;
                instanceRoot.SetParent(gameObject.transform);
                instanceRoot.localScale = Vector3.one;
                instanceRoot.localPosition = Vector3.zero;
            }
            for (int i = 0; i < SingletonMono<GameFramework>.GetInstance().DownloadCount; i++)
            {
                DownloadAgent downloadAgent = new GameObject("Download "+(i+1)).AddComponent<DownloadAgent>();
                downloadAgent.transform.SetParent(instanceRoot);
                downloadAgent.transform.localPosition = Vector3.zero;
                downloadAgent.transform.localScale = Vector3.one;
                downloadManager.AddDownloadAgent(downloadAgent);
            }
        }

        public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            downloadManager.OnUpdate(elapseSeconds, realElapseSeconds);
        }

        public override void Shutdown()
        {
            base.Shutdown();
            downloadManager.Shutdown();
        }

        public int AddDownload(string downloadPath, string downloadUri, Action<DownloadTask, ulong> doneCallback,
            Action<DownloadTask, ulong, float> updateCallback,
            Action<DownloadTask, string> errorCallback)
        {
            return downloadManager.AddDownload(downloadPath, downloadUri, doneCallback, updateCallback, errorCallback);
        }

        public int AddDownload(string downloadPath, string downloadUri, Action<DownloadTask, ulong> doneCallback,
            Action<DownloadTask, ulong, float> updateCallback,
            Action<DownloadTask, string> errorCallback, int priority)
        {
            return downloadManager.AddDownload(downloadPath, downloadUri, doneCallback, updateCallback, errorCallback,
                priority);
        }

        public int AddDownload(string downloadPath, string downloadUrl, Action<DownloadTask, ulong> doneCallback,
            Action<DownloadTask, ulong, float> updateCallback,
            Action<DownloadTask, string> errorCallback, int priority, int timeout, ulong filelength = 0)
        {
            return downloadManager.AddDownload(downloadPath, downloadUrl, doneCallback, updateCallback, errorCallback,
                priority, timeout, filelength);
        }

        public bool RemoveDownload(int serialId)
        {
            return downloadManager.RemoveDownload(serialId);
        }

        public void RemoveAllDownload()
        {
            downloadManager.RemoveAllDownload();
        }
    }
}