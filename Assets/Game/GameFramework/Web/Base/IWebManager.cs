﻿using System;
using UnityEngine;

namespace GameFramework.Web.Base
{
    public interface IWebManager
    {
        int TotalAgentCount { get; }
        int FreeAgentCount { get; }
        int WorkingAgentCount { get; }
        int WaitTaskCount { get; }
        int AddGetWebRequest(string webUrl,float timeOut,Action<bool,string,byte[]> callAction);
        int AddPostWebRequest(string webUrl,float timeOut,byte[] postData, Action<bool,string,byte[]> callAction);
        int AddPostWebRequest(string webUrl,float timeOut,WWWForm wwwForm, Action<bool,string,byte[]> callAction);
        bool RemoveWebRequest(int serialId);
        void RemoveAllWebRequest();
    }
}