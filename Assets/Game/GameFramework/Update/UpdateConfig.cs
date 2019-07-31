using System;
using System.Security;
using Mono.Xml;

namespace GameFramework.Update
{
    public class UpdateStringConfig
    {
        #region StringValue

        public string SystemRequireMemory;
        public string SystemRequireSpace;
        public string UpdateStateSuccess;
        public string UpdateStateDiskSpaceFullErr;
        public string UpdateStateGetLocalVersionErr;
        public string UpdateStateGetServerVersionErr;
        public string UpdateStateNetworkErr;
        public string UpdateStateNetConnectionErr;
        public string UpdateStateNetUnstable;
        public string UpdateStateStartUpdateInfo;
        public string UpdateStateUnknownErr;
        public string UpdateStateDownloadingErrAutoRetry;
        public string UpdateStatusStart;
        public string UpdateStatusTryGetLocalVersion;
        public string UpdateStatusTryCheckFreeSpace;
        public string UpdateStatusTryGetNewVersion;
        public string UpdateStatusTryDnsResolving;
        public string UpdateStatusSuccessGetVersions;
        public string UpdateStatusBeginUpdate;
        public string UpdateStatusDownloading;
        public string UpdateStatusDownloadingServerList;
        public string UpdateStatusCompressPack;
        public string UpdateStringCurrentVersion;
        public string UpdateStringServerVersion;
        public string UpdateStringYes;
        public string UpdateStringNo;
        public string UpdateStringOk;
        public string UpdateStringCancel;
        public string UpdateStringHasErrorNotWifi;
        public string UpdateStringHasErrorRetry;
        public string UpdateStringHasError;
        public string UpdateStringHasFatalErrorNeedReinstall;
        public string UpdateStringPrepareForFirstTimeUse;
        public string UpdateStringEnsureEnoughSpace;
        public string UpdateStringTextUpdate;
        public string UpdateStringFileSize;
        public string UpdateStringDownloadSpeed;
        public string UpdateStatusWifiTips;

        #endregion

        public bool ParseFromXmlString(string content)
        {
            try
            {
                SecurityParser parser = new SecurityParser();
                parser.LoadXml(content); 

                SecurityElement root = parser.ToXml();
                for (int i = 0; i < (root.Children?.Count ?? 0); ++i)
                {
                    if (root.Children != null)
                    {
                        SecurityElement ele = (SecurityElement) root.Children[i];
                        switch (ele.Tag)
                        {
                            case "SystemRequireMemory":
                                this.SystemRequireMemory = ele.Attribute("text");
                                break;
                            case "SystemRequireSpace":
                                this.SystemRequireSpace = ele.Attribute("text");
                                break;
                            case "UpdateStateSuccess":
                                this.UpdateStateSuccess = ele.Attribute("text");
                                break;
                            case "UpdateStateDiskSpaceFullErr":
                                this.UpdateStateDiskSpaceFullErr = ele.Attribute("text");
                                break;
                            case "UpdateStateGetLocalVersionErr":
                                this.UpdateStateGetLocalVersionErr = ele.Attribute("text");
                                break;
                            case "UpdateStateGetServerVersionErr":
                                this.UpdateStateGetServerVersionErr = ele.Attribute("text");
                                break;
                            case "UpdateStateNetworkErr":
                                this.UpdateStateNetworkErr = ele.Attribute("text");
                                break;
                            case "UpdateStateNetConnectionErr":
                                this.UpdateStateNetConnectionErr = ele.Attribute("text");
                                break;
                            case "UpdateStateNetUnstable":
                                this.UpdateStateNetUnstable = ele.Attribute("text");
                                break;
                            case "UpdateStateStartUpdateInfo":
                                this.UpdateStateStartUpdateInfo = ele.Attribute("text");
                                break;
                            case "UpdateStateUnknownErr":
                                this.UpdateStateUnknownErr = ele.Attribute("text");
                                break;
                            case "UpdateStateDownloadingErrAutoRetry":
                                this.UpdateStateDownloadingErrAutoRetry = ele.Attribute("text");
                                break;
                            case "UpdateStatusStart":
                                this.UpdateStatusStart = ele.Attribute("text");
                                break;
                            case "UpdateStatusTryGetLocalVersion":
                                this.UpdateStatusTryGetLocalVersion = ele.Attribute("text");
                                break;
                            case "UpdateStatusTryCheckFreeSpace":
                                this.UpdateStatusTryCheckFreeSpace = ele.Attribute("text");
                                break;
                            case "UpdateStatusTryGetNewVersion":
                                this.UpdateStatusTryGetNewVersion = ele.Attribute("text");
                                break;
                            case "UpdateStatusTryDnsResolving":
                                this.UpdateStatusTryDnsResolving = ele.Attribute("text");
                                break;
                            case "UpdateStatusSuccessGetVersions":
                                this.UpdateStatusSuccessGetVersions = ele.Attribute("text");
                                break;
                            case "UpdateStatusBeginUpdate":
                                this.UpdateStatusBeginUpdate = ele.Attribute("text");
                                break;
                            case "UpdateStatusDownloading":
                                this.UpdateStatusDownloading = ele.Attribute("text");
                                break;
                            case "UpdateStatusDownloadingServerList":
                                this.UpdateStatusDownloadingServerList = ele.Attribute("text");
                                break;
                            case "UpdateStatusCompressPack":
                                this.UpdateStatusCompressPack = ele.Attribute("text");
                                break;
                            case "UpdateStringCurrentVersion":
                                this.UpdateStringCurrentVersion = ele.Attribute("text");
                                break;
                            case "UpdateStringServerVersion":
                                this.UpdateStringServerVersion = ele.Attribute("text");
                                break;
                            case "UpdateStringYes":
                                this.UpdateStringYes = ele.Attribute("text");
                                break;
                            case "UpdateStringNo":
                                this.UpdateStringNo = ele.Attribute("text");
                                break;
                            case "UpdateStringOk":
                                this.UpdateStringOk = ele.Attribute("text");
                                break;
                            case "UpdateStringCancel":
                                this.UpdateStringCancel = ele.Attribute("text");
                                break;
                            case "UpdateStringHasErrorNotWifi":
                                this.UpdateStringHasErrorNotWifi = ele.Attribute("text");
                                break;
                            case "UpdateStringHasErrorRetry":
                                this.UpdateStringHasErrorRetry = ele.Attribute("text");
                                break;
                            case "UpdateStringHasError":
                                this.UpdateStringHasError = ele.Attribute("text");
                                break;
                            case "UpdateStringHasFatalErrorNeedReinstall":
                                this.UpdateStringHasFatalErrorNeedReinstall = ele.Attribute("text");
                                break;
                            case "UpdateStringPrepareForFirstTimeUse":
                                this.UpdateStringPrepareForFirstTimeUse = ele.Attribute("text");
                                break;
                            case "UpdateStringEnsureEnoughSpace":
                                this.UpdateStringEnsureEnoughSpace = ele.Attribute("text");
                                break;
                            case "UpdateStringTextUpdate":
                                this.UpdateStringTextUpdate = ele.Attribute("text");
                                break;
                            case "UpdateStringFileSize":
                                this.UpdateStringFileSize = ele.Attribute("text");
                                break;
                            case "UpdateStringDownloadSpeed":
                                this.UpdateStringDownloadSpeed = ele.Attribute("text");
                                break;
                            case "UpdateStatusWifiTips":
                                this.UpdateStatusWifiTips = ele.Attribute("text");
                                break;
                            default:
                                break;
                        }
                    }
                }
                parser.Clear();
                return true;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(string.Format("Exception {0}", e.Message));
            }
            return false;
        }
    }

    public class UpdateConfig
    {
        public string UpdateServerUrl1;
        public string UpdateServerUrl2;
        public string UpdateServerUrl3;
        public string ClientServerUrl;

        public bool ParseFromXmlString(string content ,string channel)
        {
            bool bRet = false;
            try
            {
                SecurityParser parser = new SecurityParser();
                parser.LoadXml(content);

                SecurityElement root = parser.ToXml();
                for (int i = 0; i < (root.Children?.Count ?? 0); ++i)
                {
                    if (root.Children != null)
                    {
                        SecurityElement eleLocale = (SecurityElement)root.Children[i];
                        if (eleLocale.Tag != channel)
                            continue;
                        for (int j = 0; j < (eleLocale.Children?.Count ?? 0); ++j)
                        {
                            if (eleLocale.Children != null)
                            {
                                SecurityElement ele = (SecurityElement)eleLocale.Children[j];
                                switch (ele.Tag)
                                {
                                    case "UpdateServer1":
                                        UpdateServerUrl1 = ele.Attribute("url");
                                        break;
                                    case "UpdateServer2":
                                        UpdateServerUrl2 = ele.Attribute("url");
                                        break;
                                    case "UpdateServer3":
                                        UpdateServerUrl3 = ele.Attribute("url");
                                        break;
                                    case "ClientServer":
                                        ClientServerUrl = ele.Attribute("url");
                                        break;
                                }
                            }
                        }
                    }
                }
                parser.Clear();
                bRet = true;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(string.Format("Exception {0}", e.Message));
            }
            return bRet;
        }
    }
}