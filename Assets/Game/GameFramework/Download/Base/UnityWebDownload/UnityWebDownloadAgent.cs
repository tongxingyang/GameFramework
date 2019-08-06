using System;
using System.Collections.Generic;
using System.IO;
using GameFramework.Pool.TaskPool;
using GameFramework.Utility.File;
using UnityEngine;
using UnityEngine.Networking;

namespace GameFramework.Download.Base.UnityWebDownload
{
    public class UnityWebDownloadAgent : MonoBehaviour, ITaskAgent<DownloadTask>
    {
        public DownloadTask Task { get; set; }
        private FileStream fileStream;
        private UnityWebRequest unityWebRequest;
        private int lastDownloadedSize = 0;
        public float WaitTime { get; set; }
        public int StartLength { get; set; }
        public int DownloadLength { get; set; }
        public int CurrentLength => StartLength + DownloadLength;
        public void Initialize()
        {
            Task = null;
            fileStream?.Dispose();
            fileStream?.Close();
            fileStream = null;
            WaitTime = 0;
            StartLength = 0;
            DownloadLength = 0;
        }

        public void OnStart(DownloadTask task)
        {
            Task = task;
            Task.DownloadState = enDownloadState.Doing;
            string downloadFile = string.Format("{0}.download", Task.DownloadPath);
            try
            {
                if (File.Exists(downloadFile))
                {
                    fileStream = File.OpenWrite(downloadFile);
                    fileStream.Seek(0, SeekOrigin.End);
                    StartLength = (int)fileStream.Length;
                    DownloadLength = 0;
                }
                else
                {
                    string directory = Path.GetDirectoryName(Task.DownloadPath);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    fileStream = new FileStream(downloadFile, FileMode.Create, FileAccess.Write);
                    StartLength = DownloadLength = 0;
                }

                if (StartLength > 0)
                {
                    Download(Task.DownloadUrl, StartLength);
                }
                else
                {
                    Download(Task.DownloadUrl);
                }
            }
            catch (Exception exception)
            {
                DownloadError(exception.ToString());
            }
        }

        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            if (Task.DownloadState == enDownloadState.Doing)
            {
                WaitTime += realElapseSeconds;
                if (WaitTime >= Task.TimeOut)
                {
                    DownloadError("time out");
                }
            }
            if (unityWebRequest == null)
            {
                return;
            }

            if (!unityWebRequest.isDone)
            {
                if (lastDownloadedSize < (int)unityWebRequest.downloadedBytes)
                {
                    WaitTime = 0;
                    var currentAdd = (uint)unityWebRequest.downloadedBytes - lastDownloadedSize;
                    lastDownloadedSize = (int)unityWebRequest.downloadedBytes;
                    Task.DownLoadUpdateAction(Task, (uint) lastDownloadedSize, (uint)currentAdd, unityWebRequest.downloadProgress);
                }

                return;
            }

            bool isError = false;
            isError = unityWebRequest.isNetworkError;
            if (isError)
            {
                DownloadError(unityWebRequest.error);
            }
            else if (unityWebRequest.downloadHandler.isDone)
            {

                
            }
        }

        private void SaveFile()
        {
            if(unityWebRequest.downloadHandler.data==null) return;
            try
            {
                int length = unityWebRequest.downloadHandler.data.Length;
                fileStream.Write(unityWebRequest.downloadHandler.data, 0, length);
                fileStream.Flush();
            }
            catch (Exception exception)
            {
                DownloadError(exception.ToString());
            }
        }


        private void DownloadError(string message)
        {
            unityWebRequest?.Dispose();
            unityWebRequest = null;
            lastDownloadedSize = 0;
            fileStream?.Dispose();
            fileStream?.Close();
            fileStream = null;
            Task.DownloadState = enDownloadState.Error;
            Task.Done = true;
            Task.DownloadErrorAction(Task, message);
        }

        private void DownloadSuccess()
        {
            WaitTime = 0;
            SaveFile();
            DownloadLength = unityWebRequest.downloadHandler.data.Length;
            unityWebRequest?.Dispose();
            unityWebRequest = null;
            lastDownloadedSize = 0;
            fileStream?.Dispose();
            fileStream?.Close();
            fileStream = null;
            if (FileUtility.IsFileExist(Task.DownloadPath))
            {
                FileUtility.DeleteFile(Task.DownloadPath);
            }
            File.Move(string.Format("{0}.download", Task.DownloadPath), Task.DownloadPath);
            Task.DownloadState = enDownloadState.Done;
            Task.DownloadDoneAction(Task, (uint) (StartLength + DownloadLength));
            Task.Done = true;
        }
        
        public void OnReset()
        {
            unityWebRequest?.Dispose();
            unityWebRequest = null;
            lastDownloadedSize = 0;
            fileStream?.Close();
            fileStream = null;
            Task = null;
            WaitTime = 0f;
            StartLength = 0;
            DownloadLength = 0;
        }

        public void ShotDown()
        {
            OnReset();
        }
        
        public void Download(string downloadUri)
        {
            unityWebRequest = UnityWebRequest.Get(downloadUri);
            unityWebRequest.SendWebRequest();
        }

        public void Download(string downloadUri, int fromPosition)
        {
            Dictionary<string, string> header = new Dictionary<string, string>
            {
                { "Range", string.Format("bytes={0}-", fromPosition.ToString()) }
            };

            unityWebRequest = UnityWebRequest.Post(downloadUri, header);
            unityWebRequest.SendWebRequest();
        }

        public void Download(string downloadUri, int fromPosition, int toPosition)
        {
            Dictionary<string, string> header = new Dictionary<string, string>
            {
                { "Range", string.Format("bytes={0}-{1}", fromPosition.ToString(), toPosition.ToString()) }
            };

            unityWebRequest = UnityWebRequest.Post(downloadUri, header);
            unityWebRequest.SendWebRequest();
        }
    }
}