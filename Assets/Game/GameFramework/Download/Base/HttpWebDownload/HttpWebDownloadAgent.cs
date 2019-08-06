using System.IO;
using System.Net;
using GameFramework.Pool.TaskPool;
using GameFramework.Utility.File;
using UnityEngine;

namespace GameFramework.Download.Base
{
    
    public class HttpWebDownloadAgent : MonoBehaviour,ITaskAgent<DownloadTask>
    {
        private ulong fileLength = 0;
        private ulong alreadyDownloadLength = 0;
        private HttpWebDownload[] httpWebDownloads;
        private string extension = "download";

        public bool IsOpenbrokenpointdownload { get; set; } = false;
        public int ThreadCount { get; set; } = 1;
        public DownloadTask Task { get; set; }
        
        public void Initialize()
        {
            httpWebDownloads = new HttpWebDownload[ThreadCount];
            for (int i = 0; i < ThreadCount; i++)
            {
                httpWebDownloads[i] = new HttpWebDownload();
            }
        }

        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            
        }

        public void ShotDown()
        {
            OnReset();
            foreach (HttpWebDownload httpWebDownload in httpWebDownloads)
            {
                httpWebDownload.Reset();
            }
            httpWebDownloads = null;
        }

        public void OnStart(DownloadTask task)
        {
            Task = task;
            if (Task.FileLength == 0)
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
                fileLength = Task.FileLength;
            }
            string dir = Path.GetDirectoryName(task.DownloadPath);
            if (!FileUtility.IsDirectoryExist(dir))
            {
                FileUtility.CreateDirectory(dir);
            }
            
            uint fileDataThread = (uint)fileLength / (uint)ThreadCount;
            var isDone = true;
            
            for (int i = 0; i < ThreadCount; i++)
            {
                uint start = (uint)i * fileDataThread;
                httpWebDownloads[i].StartPos = start;
                if (i == ThreadCount - 1)
                {
                    httpWebDownloads[i].EndPos = (uint)(fileLength - 1);
                }
                else
                {
                    httpWebDownloads[i].EndPos = ((uint)i + 1) * fileDataThread-1;
                }
                httpWebDownloads[i].IsDone = false;
                httpWebDownloads[i].TimeOut = task.TimeOut;
                string downloafFile = string.Format("{0}.{1}.{2}", task.DownloadPath, extension,i);
                httpWebDownloads[i].FileName = downloafFile;

                if (FileUtility.IsFileExist(downloafFile))
                {
                    if (!IsOpenbrokenpointdownload)
                    {
                        FileUtility.DeleteFile(downloafFile);
                        httpWebDownloads[i].FileStream =
                            new FileStream(downloafFile, FileMode.Create, FileAccess.ReadWrite);
                        isDone = false;
                    }
                    else
                    {
                        httpWebDownloads[i].FileStream = new FileStream(downloafFile,FileMode.Open,FileAccess.ReadWrite);
                        httpWebDownloads[i].FileStream.Seek(0, SeekOrigin.End);
                        uint alreadyLength = (uint) httpWebDownloads[i].FileStream.Length;
                        httpWebDownloads[i].StartPos = httpWebDownloads[i].StartPos + alreadyLength;
                        alreadyDownloadLength += alreadyLength;
                        if (httpWebDownloads[i].EndPos - httpWebDownloads[i].StartPos == alreadyLength)
                        {
                            httpWebDownloads[i].IsDone = true;
                        }
                        else
                        {
                            isDone = false;
                        }
                    }
                }
                else
                {
                    httpWebDownloads[i].FileStream = new FileStream(downloafFile,FileMode.Create,FileAccess.ReadWrite);
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
            for (int i = 0; i < ThreadCount; i++)
            {
                if (httpWebDownloads[i].IsDone == false)
                {
                    httpWebDownloads[i].ErrorCallback = DownloadPatrError;
                    httpWebDownloads[i].SucceseCallback = DownloadPartDone;
                    httpWebDownloads[i].UpdateCallback = DownloadPartUpdate;
                    httpWebDownloads[i].ThreadId = i;
                    httpWebDownloads[i].Url = Task.DownloadUrl;
                    httpWebDownloads[i].Download();
                }
            }
        }

        private void DownloadPartDone(int id)
        {
            var isalready = true;
            foreach (HttpWebDownload httpWebDownload in httpWebDownloads)
            {
                if (!httpWebDownload.IsDone)
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
            if (ThreadCount == 1)
            {
                FileUtility.Move(httpWebDownloads[0].FileName, Task.DownloadPath);
                DownloadDone();
            }
            else
            {
                using (FileStream fileStream = new FileStream(Task.DownloadPath,FileMode.CreateNew,FileAccess.Write))
                {
                    byte[] bytes = new byte[1024];
                    for (int i = 0; i < ThreadCount; i++)
                    {
                        using ( FileStream part = new FileStream(Task.DownloadPath,FileMode.Open,FileAccess.ReadWrite))
                        {
                            while (true)
                            {
                                var readLength = part.Read(bytes, 0, bytes.Length);
                                if (readLength > 0)
                                {
                                    fileStream.Write(bytes, 0, readLength);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                DownloadDone();
            }
        }
        
        private void DownloadDone()
        {
            Task.DownloadState = enDownloadState.Done;
            if (ThreadCount > 1)
            {
                DeleCacheFiles();
            }
            Task.DownloadDoneAction?.Invoke(Task,fileLength);
            Task.Done = true;
//            ResetHttpWebDownload();

        }

        private void DeleCacheFiles()
        {
            for (int i = 0; i < ThreadCount; i++)
            {
                httpWebDownloads[i]?.FileStream.Close();
                httpWebDownloads[i]?.FileStream.Dispose();
                FileUtility.DeleteFile(httpWebDownloads[i].FileName);
            }
        }
        
        private void DownloadError(string error)
        {
            if (ThreadCount > 1)
            {
                DeleCacheFiles();
            }
            Task.DownloadErrorAction?.Invoke(Task, error);
            Task.DownloadState = enDownloadState.Error;
            Task.Done = true;;
//            ResetHttpWebDownload();

        }

        private void DownloadUpdate(uint currentAdd)
        {
            var process = (float)alreadyDownloadLength/fileLength;
            Task.DownLoadUpdateAction?.Invoke(Task,alreadyDownloadLength,currentAdd,process);
        }

        public void ResetHttpWebDownload()
        {
            if (httpWebDownloads != null)
            {
                foreach (var threadDownload in httpWebDownloads)
                {
                    threadDownload.Reset();
                }
            }
        }        
        public void OnReset()
        {
            ResetHttpWebDownload();
            Task?.Clear();
            Task = null;
            fileLength = 0;
            alreadyDownloadLength = 0;
        }

        void OnDestroy()
        {
            ShotDown();
        }
    }
}
