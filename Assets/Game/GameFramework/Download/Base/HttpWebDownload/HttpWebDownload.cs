using System;
using System.IO;
using System.Net;
using System.Threading;

namespace GameFramework.Download.Base
{
    public class HttpWebDownload
    {
        private object lockObj = new object();
        private byte[] buffer;
        public Action<string, int> ErrorCallback;
        public Action<int> SucceseCallback;
        public Action<int, uint> UpdateCallback;
        public string FileName { get; set; }
        public int TimeOut { get; set; }
        public int ThreadId { get; set; }
        public string Url { get; set; }
        public bool IsDone { get; set; }
        public FileStream FileStream { get; set; }
        public uint StartPos { get; set; }
        public uint EndPos { get; set; }
        public HttpWebRequest HttpWebRequest { get; set; }
        private HttpWebResponse httpWebResponse = null;
        private Stream responseStream = null;
        public void Reset()
        {            
            responseStream?.Dispose();
            responseStream?.Close();
            responseStream = null;
            httpWebResponse?.Dispose();
            httpWebResponse?.Close();
            httpWebResponse = null;
            HttpWebRequest?.Abort();
            HttpWebRequest = null;
            TimeOut = 0;
            ThreadId = 0;
            Url = String.Empty;
            IsDone = true;
            StartPos = 0;
            EndPos = 0;
            registerWaitHandle = null;
            waitHandle = null;
            buffer = null;
            FileStream?.Dispose();
            FileStream?.Close();
            FileStream = null;
        }

        private void ClearCallBack()
        {
            ErrorCallback = null;
            SucceseCallback = null;
            UpdateCallback = null;
        }
        
        private void Init()
        {
            responseStream?.Dispose();
            responseStream?.Close();
            responseStream = null;
            httpWebResponse?.Dispose();
            httpWebResponse?.Close();
            httpWebResponse = null;
            HttpWebRequest?.Abort();
            HttpWebRequest = null;
            buffer = null;
        }
        
        public void Download()
        {
            try
            {
                Init();
                HttpWebRequest = WebRequest.CreateHttp(Url) as HttpWebRequest;
                HttpWebRequest.AddRange(StartPos,EndPos);
                HttpWebRequest.Timeout = TimeOut;
                HttpWebRequest.KeepAlive = false;
                var result =
                    (IAsyncResult) HttpWebRequest.BeginGetResponse(OnDownloadCallBack, HttpWebRequest);
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
                    if(IsDone) return;
                    HttpWebRequest = ar.AsyncState as HttpWebRequest;
                    if (HttpWebRequest == null) return;
                    httpWebResponse = GetWebResponse(HttpWebRequest, ar);
                    responseStream = httpWebResponse.GetResponseStream();
                    BeginReadDownloadData(OnReadCallback);
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("download file error "+e.Message);
                UnRegisterTimeOut();
                DownloadError(e.Message);
            } 
        }

        private void BeginReadDownloadData(AsyncCallback callback)
        {
            buffer = new byte[1024];
            responseStream.BeginRead(buffer, 0, buffer.Length, callback, null);
        }
        private void OnReadCallback(IAsyncResult ar)
        {
            
            try
            {
                if(IsDone) return;
                if(responseStream==null)
                    return;
                int readLength = responseStream.EndRead(ar);
                if (readLength > 0)
                {
                    FileStream.Write(buffer,0,readLength);
                    FileStream.Flush();
                    StartPos += (uint)readLength;
                    DownloadUpdate((uint)readLength);
                    BeginReadDownloadData(OnReadCallback);
                }
                else
                {
                    FileStream.Flush();
                    if (StartPos -1 != EndPos)
                    {
                        DownloadError("下载文件不完整");
                    }
                    else
                    {
                        DownloadDone();
                    }
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
            IsDone = true;
            Reset();
            SucceseCallback?.Invoke(ThreadId);
            ClearCallBack();
        }

        private void DownloadError(string error)
        {
            Reset();
            ErrorCallback?.Invoke(error,ThreadId);
            ClearCallBack();
        }

        private void DownloadUpdate(uint read)
        {
            UpdateCallback?.Invoke(ThreadId,read);
        }

        #region TimeOut

        private RegisteredWaitHandle registerWaitHandle;
        private WaitHandle waitHandle;
        private void RegisterTimeOut(WaitHandle handle)
        {
            waitHandle = handle;
            registerWaitHandle = ThreadPool.RegisterWaitForSingleObject(handle,
                (OnTimeoutCallback), HttpWebRequest, TimeOut, true);
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