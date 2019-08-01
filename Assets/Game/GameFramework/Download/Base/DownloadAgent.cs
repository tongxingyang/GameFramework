using System.IO;
using System.Net;
using System.Threading;
using GameFramework.Pool.TaskPool;
using GameFramework.Utility.File;
using UnityEngine;
using ThreadPriority = System.Threading.ThreadPriority;

namespace GameFramework.Download.Base
{
    
    public class DownloadAgent : MonoBehaviour,ITaskAgent<DownloadTask>
    {
        private int threadCount = 1;
        private int retryCount = 1;
        private bool isOpenbrokenpointdownload = false;

        private ulong fileLength = 0;
        private DownloadTask task;
        private ulong alreadyDownloadLength = 0;
        private HttpMultiThreadDownload[] httpMultiThreadDownloads;
        private string extension = "download";

        public bool IsOpenbrokenpointdownload
        {
            get => isOpenbrokenpointdownload;
            set => isOpenbrokenpointdownload = value;
        }
        
        public int RetryCount
        {
            get => retryCount;
            set => retryCount = value;
        }
        
        public int ThreadCount
        {
            get => threadCount;
            set => threadCount = value;
        }
        public DownloadTask Task => task;
        
        public void Initialize()
        {
            httpMultiThreadDownloads = new HttpMultiThreadDownload[threadCount];
            for (int i = 0; i < threadCount; i++)
            {
                httpMultiThreadDownloads[i] = new HttpMultiThreadDownload();
            }
        }

        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            
        }

        public void ShotDown()
        {
            OnReset();
            httpMultiThreadDownloads = null;
        }

        public void OnStart(DownloadTask task)
        {
            this.task = task;
            if (this.task.FileLength == 0)
            {
                HttpWebRequest httpWebRequest = WebRequest.CreateHttp(task.DownloadUrl);
                httpWebRequest.Method = "head";
                httpWebRequest.Timeout = 5000;
                using (HttpWebResponse httpWebResponse = (HttpWebResponse) httpWebRequest.GetResponse())
                {
                    if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                    {
                        fileLength = (ulong) httpWebResponse.ContentLength;
                    }
                    httpWebResponse.Close();
                    httpWebResponse.Dispose();
                }
                httpWebRequest.Abort();
            }
            else
            {
                fileLength = this.task.FileLength;
            }
            string dir = Path.GetDirectoryName(task.DownloadPath);
            if (!FileUtility.IsDirectoryExist(dir))
            {
                FileUtility.CreateDirectory(dir);
            }
            if (!isOpenbrokenpointdownload)
            {
                DeleCacheFiles();
            }
            uint fileDataThread = (uint)fileLength / (uint)threadCount;
            var isDone = true;
            
            for (int i = 0; i < threadCount; i++)
            {
                uint start = (uint)i * fileDataThread;
                httpMultiThreadDownloads[i].StartPos = start;
                if (i == threadCount - 1)
                {
                    httpMultiThreadDownloads[i].EndPos = (uint)(fileLength - 1);
                }
                else
                {
                    httpMultiThreadDownloads[i].EndPos = ((uint)i + 1) * fileDataThread-1;
                }
                httpMultiThreadDownloads[i].IsDone = false;
                httpMultiThreadDownloads[i].TimeOut = task.TimeOut;
                string downloafFile = string.Format("{0}.{1}.{2}", task.DownloadPath, extension,i);
                httpMultiThreadDownloads[i].FileName = downloafFile;
                if (FileUtility.IsFileExist(downloafFile))
                {
                    httpMultiThreadDownloads[i].FileStream = new FileStream(downloafFile,FileMode.Open,FileAccess.ReadWrite);
                    httpMultiThreadDownloads[i].FileStream.Seek(0, SeekOrigin.End);
                    uint alreadyLength = (uint) httpMultiThreadDownloads[i].FileStream.Length;
                    httpMultiThreadDownloads[i].StartPos = httpMultiThreadDownloads[i].StartPos + alreadyLength;
                    alreadyDownloadLength += alreadyLength;
                    if (httpMultiThreadDownloads[i].EndPos - httpMultiThreadDownloads[i].StartPos == alreadyLength)
                    {
                        httpMultiThreadDownloads[i].IsDone = true;
                    }
                    else
                    {
                        isDone = false;
                    }
                }
                else
                {
                    httpMultiThreadDownloads[i].FileStream = new FileStream(downloafFile,FileMode.Create,FileAccess.ReadWrite);
                    isDone = false;
                    
                }
            }
            
            if (isDone)
            {
                MergeData();
            }
            else
            {
                DownloadFile();
            }
        }

        private void DownloadFile()
        {
            for (int i = 0; i < threadCount; i++)
            {
                if (httpMultiThreadDownloads[i].IsDone == false)
                {
                    httpMultiThreadDownloads[i].ErrorCallback = DownloadPatrError;
                    httpMultiThreadDownloads[i].SucceseCallback = DownloadPartDone;
                    httpMultiThreadDownloads[i].UpdateCallback = DownloadPartUpdate;
                    Thread current =
                        new Thread(httpMultiThreadDownloads[i].Download)
                        {
                            Name = string.Format("Download Thread {0}", i),
                            Priority = ThreadPriority.AboveNormal
                        };
                    httpMultiThreadDownloads[i].CurrentThread = current;
                    httpMultiThreadDownloads[i].ThreadId = i;
                    httpMultiThreadDownloads[i].Url = task.DownloadUrl;
                    current.Start();
                    Thread.Sleep(50);
                }
            }
        }

        private void DownloadPartDone(int id)
        {
            UnityEngine.Debug.LogError("threadid  "+id+" download sucess");
            var isalready = true;
            foreach (HttpMultiThreadDownload httpMultiThreadDownload in httpMultiThreadDownloads)
            {
                if (!httpMultiThreadDownload.IsDone)
                {
                    isalready = false;
                    break;
                }
            }
            if (isalready)
            {
                MergeData();
            }
        }

        private void DownloadPatrError(string error,int id)
        {
            UnityEngine.Debug.LogError("threadid  "+id+" download error "+error);
            DownloadError(error);
        }

        private void DownloadPartUpdate(int id, uint currentAdd)
        {
            alreadyDownloadLength += currentAdd;
            DownloadUpdate(currentAdd);
        }
        
        private void MergeData()
        {
            if (alreadyDownloadLength != fileLength)
            {
                DownloadError("download size error");
                return;
            }
            if (threadCount == 1)
            {
                httpMultiThreadDownloads[0].FileStream.Dispose();
                FileUtility.Move(httpMultiThreadDownloads[0].FileName, task.DownloadPath);
                DownloadDone();
            }
            else
            {
                using (FileStream fileStream = new FileStream(task.DownloadPath,FileMode.CreateNew,FileAccess.Write))
                {
                    byte[] bytes = new byte[1024];
                    for (int i = 0; i < threadCount; i++)
                    {
                        httpMultiThreadDownloads[i].FileStream.Seek(0, SeekOrigin.Begin);
                        while (true)
                        {
                            var readLength = httpMultiThreadDownloads[i].FileStream.Read(bytes, 0, bytes.Length);
                            if (readLength > 0)
                            {
                                fileStream.Write(bytes, 0, readLength);
                            }
                            else
                            {
                                break;
                            }
                        }
                        httpMultiThreadDownloads[i].FileStream.Close();
                        httpMultiThreadDownloads[i].FileStream.Dispose();
                        FileUtility.DeleteFile(httpMultiThreadDownloads[i].FileName);
                    }
                    fileStream.Close();
                    fileStream.Dispose();
                    DownloadDone();
                }
            }
        
        }
        
        private void DownloadDone()
        {
            task.DownloadDoneAction?.Invoke(task,fileLength);
            task.DownloadState = enDownloadState.Done;
            task.Done = true;
        }

        private void DeleCacheFiles()
        {
            for (int i = 0; i < threadCount; i++)
            {
                httpMultiThreadDownloads[i]?.FileStream.Close();
                httpMultiThreadDownloads[i]?.FileStream.Dispose();
                FileUtility.DeleteFile(httpMultiThreadDownloads[i].FileName);
            }
        }
        
        private void DownloadError(string error)
        {
            retryCount--;
            if (retryCount > 0)
            {
                DeleCacheFiles();
                for (int i = 0; i < threadCount; i++)
                {
                    httpMultiThreadDownloads[i].Reset();
                }
                fileLength = 0;
                alreadyDownloadLength = 0;
                OnStart(this.task);
            }
            else
            {
                DeleCacheFiles();
                task.DownloadErrorAction?.Invoke(task, error);
                task.DownloadState = enDownloadState.Error;
                task.Done = true;;
            }
        }

        private void DownloadUpdate(uint currentAdd)
        {
            var process = (float)alreadyDownloadLength/fileLength;
            task.DownLoadUpdateAction?.Invoke(task,alreadyDownloadLength,currentAdd,process);
        }
        
        public void OnReset()
        {
            if (httpMultiThreadDownloads != null)
            {
                foreach (var threadDownload in httpMultiThreadDownloads)
                {
                    threadDownload.Reset();
                }
            }
            task?.Clear();
            task = null;
            fileLength = 0;
            alreadyDownloadLength = 0;
        }

        void OnDestroy()
        {
            ShotDown();
        }
    }
}
