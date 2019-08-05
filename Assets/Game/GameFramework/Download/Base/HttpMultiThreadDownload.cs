using System;
using System.IO;
using System.Net;
using System.Threading;

namespace GameFramework.Download.Base
{
    public class HttpMultiThreadDownload
    {
        private int timeout;
        private string fileName;
        private int threadId;
        private string url;
        private bool isDone = false;
        private object lockObj = new object();
        private uint startPos;
        private uint endPos;
        private FileStream fileStream;
        private HttpWebRequest httpWebRequest;
        private HttpWebResponse httpWebResponse;
        private Stream httpwebStream;
        private byte[] buffer;

   
        public Action<string, int> ErrorCallback;
        public Action<int> SucceseCallback;
        public Action<int, uint> UpdateCallback;
        public Thread CurrentThread;
        
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        public int TimeOut
        {
            get { return timeout; }
            set { timeout = value; }
        }
        public int ThreadId
        {
            get { return threadId; }
            set { threadId = value; }
        }
        public string Url
        {
            get { return url; }
            set { url = value; }
        }
        public bool IsDone
        {
            get { return isDone; }
            set { isDone = value; }
        }
        public FileStream FileStream
        {
            get { return fileStream; }
            set { fileStream = value; }
        }
        public uint StartPos
        {
            get { return startPos; }
            set { startPos = value; }
        }
        public uint EndPos
        {
            get { return endPos; }
            set { endPos = value; }
        }
        public HttpWebRequest HttpWebRequest
        {
            get { return httpWebRequest; }
            set { httpWebRequest = value; }
        }
        
        public void Reset()
        {
            timeout = 0;
            threadId = 0;
            url = String.Empty;
            isDone = true;
            startPos = 0;
            endPos = 0;
            fileStream?.Dispose();
            fileStream?.Close();
            fileStream = null;
            httpwebStream?.Dispose();
            httpwebStream?.Close();
            httpwebStream = null;
            httpWebResponse?.Dispose();
            httpWebResponse?.Close();
            httpWebResponse = null;
            httpWebRequest?.Abort();
            httpWebRequest = null;

            CurrentThread?.Abort();
            CurrentThread = null;
            ErrorCallback = null;
            SucceseCallback = null;
            UpdateCallback = null;
            registerWaitHandle = null;
            waitHandle = null;
            buffer = null;
        }

        public void StopThread()
        {
            CurrentThread?.Abort();
            CurrentThread = null;
        }
        
        public void Download()
        {
            try
            {
                httpWebRequest?.Abort();
                httpWebRequest = WebRequest.CreateHttp(url);
                httpWebRequest.AddRange(startPos,endPos);
                httpWebRequest.Timeout = timeout;
                var result =
                    (IAsyncResult) httpWebRequest.BeginGetResponse(new AsyncCallback(OnDownloadCallBack), httpWebRequest);
                RegisterTimeOut(result.AsyncWaitHandle);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("download file error "+e.Message);
                DownloadError(e.Message);
                UnRegisterTimeOut();
            }
        }

        public HttpWebResponse GetWebResponse(HttpWebRequest webRequest,IAsyncResult ar)
        {
            try
            {
                return webRequest.EndGetResponse(ar) as HttpWebResponse;
            }
            catch (WebException e)
            {
                if (e.Response != null)
                {
                    return e.Response as HttpWebResponse;
                }
                throw;
            }
        }
        
        private void OnDownloadCallBack(IAsyncResult ar)
        {
            try
            {
                UnRegisterTimeOut();
                lock (lockObj)
                {
                    if(isDone) return;
                    httpWebRequest = ar.AsyncState as HttpWebRequest;
                    if (httpWebRequest == null) return;
                    httpWebResponse = GetWebResponse(httpWebRequest, ar);
                    buffer = new byte[1024];
                    httpwebStream = httpWebResponse.GetResponseStream();
                    httpwebStream.BeginRead(buffer, 0, buffer.Length, ReadCallback, httpwebStream);
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("download file error "+e.Message);
                UnRegisterTimeOut();
                DownloadError(e.Message);
            }
        }


        private void ReadCallback(IAsyncResult ar)
        {
            try
            {
                lock (lockObj)
                {
                    if(isDone) return;
                    httpwebStream = ar.AsyncState as Stream;
                    int read = httpwebStream.EndRead(ar);
                    if (read > 0)
                    {
                        DownloadUpdate((uint)read);
                        fileStream.Write(buffer, 0, read);
                        fileStream.Flush();
                        startPos += (uint) read;
                    }
                    else
                    {
                        fileStream.Flush();
                        httpwebStream.Dispose();
                        if (startPos -1 != endPos)
                        {
                            DownloadError("length error " + startPos + "   :   " + endPos);
                        }
                        else
                        {
                            DownloadDone();
                        }
                        return;
                    }
                    if(isDone) return;
                    httpwebStream.BeginRead(buffer, 0, buffer.Length, ReadCallback, httpwebStream);

                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("download file error "+e.Message);
                DownloadError(e.Message);
            }
        }
        
        private void DownloadDone()
        {
            isDone = true;
            SucceseCallback?.Invoke(threadId);
        }

        private void DownloadError(string error)
        {
            ErrorCallback?.Invoke(error,threadId);
        }

        private void DownloadUpdate(uint read)
        {
            UpdateCallback?.Invoke(threadId,read);
        }

        #region TimeOut

        private RegisteredWaitHandle registerWaitHandle;
        private WaitHandle waitHandle;
        private void RegisterTimeOut(WaitHandle handle)
        {
            waitHandle = handle;
            registerWaitHandle = ThreadPool.RegisterWaitForSingleObject(handle,
                (OnTimeoutCallback), httpWebRequest, timeout, true);
        }

        private void UnRegisterTimeOut()
        {
            if (waitHandle != null && registerWaitHandle != null)
            {
                registerWaitHandle.Unregister(waitHandle);
            }
        }
        
        void OnTimeoutCallback(object stat,bool timeout)
        {
            lock(lockObj)
            {
                if(timeout)
                {
                    DownloadError("timeout");
                }

                UnRegisterTimeOut();
            }
        }

        #endregion
        
    }
}