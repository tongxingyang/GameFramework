using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GameFramework.Base;
using GameFramework.Debug;
using GameFramework.Download;
using GameFramework.Download.Base;
using GameFramework.Res;
using GameFramework.Tool;
using GameFramework.Update.Version;
using GameFramework.Utility;
using GameFramework.Utility.Compress;
using GameFramework.Utility.File;
using GameFramework.Utility.MD5Utility;
using GameFramework.Utility.PathUtility;
using GameFramework.Utility.Singleton;
using UnityEngine;
using UnityEngine.Networking;

namespace GameFramework.Update
{
    public class UpdateComponent : GameFrameworkComponent
    {
        public override int Priority => 50;
        private UpdateStringConfig updateStringConfig = null;
        private byte[] updateFileCache;
        private int OneMegaBytes = 1024 * 1024;
        private Action updateSucces;
        private Action<string> updateError;
        private object lockObj = new object();

        public UpdateStringConfig UpdateStringConfig
        {
            get
            {
                if (updateStringConfig == null)
                {
                    updateStringConfig = new UpdateStringConfig();
                    string content = Resources
                        .Load<TextAsset>("Localization/" + SingletonMono<GameFramework>.GetInstance().Language).text;
                    updateStringConfig.ParseFromXmlString(content);
                }
                return updateStringConfig;
            }
        }

        private UpdateConfig updateConfig = null;

        public UpdateConfig UpdateConfig
        {
            get
            {
                if (updateConfig == null)
                {
                    updateConfig = new UpdateConfig();
                    string channel = Resources.Load<TextAsset>("ChannelConfig").text;
                    string content = Resources.Load<TextAsset>("UpdateConfig").text;
                    updateConfig.ParseFromXmlString(content, channel);
                }
                return updateConfig;
            }
        }

        private string platform;
        private Version.Version installVersion;
        private Version.Version firstDecompressVersion;
        private Version.Version currentVersion;
        private Version.Version serverVersion;

        private int fileCount = 0;

        private bool installVersionIsDone = false;
        private long installZipDecompressSize = 0;
        private int installZipDecompressFileCount = 0;
        private string installZipDecompressPassword = "";
        private bool isCopyVersionDone = false;
        private bool isCopyFileListDone = false;

        private bool serverBundleIsZip = false;
        private string serverBundleZipPassword = "";
        private Dictionary<string, UpdateFileInfo> oldFileInfoTable = new Dictionary<string, UpdateFileInfo>();
        private Dictionary<string, UpdateFileInfo> newFileInfoTable = new Dictionary<string, UpdateFileInfo>();
        private List<string> downloadList = new List<string>();
        private List<string> downloadErrorList = new List<string>();
        private int alreadyDownloadSuccessCount = 0;
        private int alreadyDownloadErrorCount = 0;
        private long totalDownloadSize = 0;
        private long alreadyDownloadSize = 0;
        private UpdatePanel updatePanel;
        private int updateUrlIsDoneIndex = -1;

        private bool isUpdateDone = false;

        public void Init(Action success, Action<string> error)
        {
            updateSucces = success;
            updateError = error;
            updatePanel = new UpdatePanel();
            updatePanel.InitPanel();
            installVersionIsDone = false;
            LoadInstallVersion();
        }

        /// <summary>
        /// 获取安装包原始的版本
        /// </summary>
        private void LoadInstallVersion()
        {
            Singleton<GameEntry>.GetInstance().GetComponent<ResourceComponent>().LoadBinaryFile(
                PathUtility.GetRemotePath(AppConst.Path.InstallDataPath, AppConst.AssetBundleConfig.VersionFile),
                InitInstallVersion);
        }

        private void InitInstallVersion(string fileUrl, byte[] bytes, string errorMessage)
        {
            if (bytes == null || bytes.Length == 0)
            {
                throw new Exception("local version.dat file is invalid");
            }
            using (ByteBuffer buffer = new ByteBuffer(bytes))
            {
                platform = Singleton<GameEntry>.GetInstance().GetComponent<ResourceComponent>().PlatformName =
                    ValueParse.ReadValue(buffer, ValueParse.StringParse);
                installVersion =
                    new Version.Version
                    {
                        MasterVersion = ValueParse.ReadValue(buffer, ValueParse.IntParse),
                        MinorVersion = ValueParse.ReadValue(buffer, ValueParse.IntParse),
                        RevisedVersion = ValueParse.ReadValue(buffer, ValueParse.IntParse)
                    };

                installZipDecompressFileCount = ValueParse.ReadValue(buffer, ValueParse.IntParse);
                installZipDecompressSize = ValueParse.ReadValue(buffer, ValueParse.LongParse);
                Singleton<GameEntry>.GetInstance().GetComponent<ResourceComponent>().AssetBundleVariant =
                    ValueParse.ReadValue(buffer, ValueParse.StringParse);
                ValueParse.ReadValue(buffer, ValueParse.BoolParse);
                installZipDecompressPassword = ValueParse.ReadValue(buffer, ValueParse.StringParse);
            }
            installVersionIsDone = true;
            RefreshInstallDataInfo();
        }

        private void RefreshInstallDataInfo()
        {
            if (!installVersionIsDone)
            {
                return;
            }
            CheckFirestDecompress();
        }

        /// <summary>
        /// 检查是否需要解压安装包中的文件
        /// </summary>
        private void CheckFirestDecompress()
        {
            bool isNeedDecompress = false;
            if (GetFirstDecompressVersion())
            {
                if (!Version.Version.VersionEqual(installVersion, firstDecompressVersion))
                {
                    isNeedDecompress = true;
                }
            }
            else
            {
                isNeedDecompress = true;
            }

            if (isNeedDecompress)
            {
                DeleAllPresistentData();
                DecompressInstallFile();
                WriteFirstDecompressVersion();
                Singleton<GameEntry>.GetInstance().GetComponent<ResourceComponent>().CopyFile(
                    PathUtility.GetRemotePath(AppConst.Path.InstallDataPath, AppConst.AssetBundleConfig.VersionFile),
                    PathUtility.GetCombinePath(AppConst.Path.PresistentDataPath,
                        AppConst.AssetBundleConfig.VersionFile),
                    (isError, errorMessage) =>
                    {
                        if (!isError)
                        {
                            isCopyVersionDone = true;
                        }
                        FirstDecompressCallBack();
                    }
                );
                Singleton<GameEntry>.GetInstance().GetComponent<ResourceComponent>().CopyFile(
                    PathUtility.GetRemotePath(AppConst.Path.InstallDataPath, AppConst.AssetBundleConfig.FileListFile),
                    PathUtility.GetCombinePath(AppConst.Path.PresistentDataPath,
                        AppConst.AssetBundleConfig.FileListFile),
                    (isError, errorMessage) =>
                    {
                        if (!isError)
                        {
                            isCopyFileListDone = true;
                        }
                        FirstDecompressCallBack();
                    }
                );
            }
            else
            {
                LoadPresistentVersionAndFileList();
            }
        }

        private void WriteFirstDecompressVersion()
        {
            string path = PathUtility.GetCombinePath(AppConst.Path.PresistentDataPath,
                AppConst.AssetBundleConfig.IsInstanllDecompressFileName);
            File.WriteAllText(path,
                string.Format("{0}.{1}.{2}", installVersion.MasterVersion, installVersion.MinorVersion,
                    installVersion.RevisedVersion));
        }

        /// <summary>
        /// 解压AssetBundle.zip文件到PresistentData
        /// </summary>
        private void DecompressInstallFile()
        {
            //检查磁盘空间是否满足大小 todo
            if ((long) Math.Ceiling(installZipDecompressSize * 1.1f) > MobileSystemInfo.GetFreeDiskSpace())
            {
                //磁盘空间不足
            }
            else
            {
                ZipUtility.DeCompressionFile(
                    PathUtility.GetCombinePath(AppConst.Path.InstallDataPath,
                        AppConst.AssetBundleConfig.AssetBundlePackageFile), AppConst.Path.PresistentDataPath,
                    installZipDecompressPassword);
            }
        }


        /// <summary>
        /// 获取沙河目录下的版本与文件列表
        /// </summary>
        private void LoadPresistentVersionAndFileList()
        {
            LoadPresistentVersion();
            GetServerGameVersion();
        }


        /// <summary>
        /// 更新资源
        /// </summary>
        private void UpdateResource()
        {
            LoadPresistentFilelist();
            LoadHasUpdateFileList();
            bool isCorruption = CheckClientCorruption();
            bool isVersionUpdate = currentVersion.RevisedVersion < serverVersion.RevisedVersion;
            if (!isCorruption && !isVersionUpdate)
            {
                isUpdateDone = true;
                updateSucces();
                return;
            }
            GetServerGameFileList();
        }

        /// <summary>
        /// 获取沙河目录下面的文件列表信息
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void LoadPresistentFilelist()
        {
            oldFileInfoTable.Clear();
            string filePath = PathUtility.GetCombinePath(AppConst.Path.PresistentDataPath,
                AppConst.AssetBundleConfig.FileListFile);
            byte[] bytes = FileUtility.ReadAllBytes(filePath);
            if (bytes == null || bytes.Length == 0)
            {
                isUpdateDone = true;
                updateError("获取当前客户端的文件列表信息失败");
                return;
            }

            using (ByteBuffer buffer = new ByteBuffer(bytes))
            {
                fileCount = ValueParse.ReadValue(buffer, ValueParse.IntParse);
                for (int i = 0; i < fileCount; i++)
                {
                    UpdateFileInfo updateFileInfo = new UpdateFileInfo
                    {
                        AssetBundleName = ValueParse.ReadValue(buffer, ValueParse.StringParse),
                        Length = ValueParse.ReadValue(buffer, ValueParse.IntParse),
                        Md5Code = ValueParse.ReadValue(buffer, ValueParse.IntParse),
                        ZipLength = ValueParse.ReadValue(buffer, ValueParse.IntParse),
                        ZipMd5Code = ValueParse.ReadValue(buffer, ValueParse.IntParse)
                    };
                    oldFileInfoTable.Add(updateFileInfo.AssetBundleName, updateFileInfo);
                }
            }
        }

        private void LoadHasUpdateFileList()
        {
            string filePath = PathUtility.GetCombinePath(AppConst.Path.PresistentDataPath,
                AppConst.AssetBundleConfig.HasUpdateFileName);
            if (FileUtility.IsFileExist(filePath))
            {
                using (StreamReader sr = File.OpenText(filePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] pair = line.Split(',');
                        UpdateFileInfo fileInfo = new UpdateFileInfo
                        {
                            AssetBundleName = pair[0],
                            Length = int.Parse(pair[1]),
                            Md5Code = int.Parse(pair[2]),
                            ZipLength = int.Parse(pair[3]),
                            ZipMd5Code = int.Parse(pair[4])
                        };

                        if (oldFileInfoTable.ContainsKey(fileInfo.AssetBundleName))
                        {
                            oldFileInfoTable[fileInfo.AssetBundleName] = fileInfo;
                        }
                        else
                        {
                            oldFileInfoTable.Add(fileInfo.AssetBundleName, fileInfo);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 检查本地客户端文件是否丢失
        /// </summary>
        /// <returns></returns>
        private bool CheckClientCorruption()
        {
            List<string> lostFiles = new List<string>();
            foreach (KeyValuePair<string, UpdateFileInfo> keyValuePair in oldFileInfoTable)
            {
                UpdateFileInfo fileInfo = keyValuePair.Value;
                string filename =
                    PathUtility.GetCombinePath(AppConst.Path.PresistentDataPath, fileInfo.AssetBundleName);
                if (FileUtility.IsFileExist(filename))
                {
                    continue;
                }
                lostFiles.Add(fileInfo.AssetBundleName);
            }
            foreach (var file in lostFiles)
            {
                oldFileInfoTable.Remove(file);
            }
            if (lostFiles.Count > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取初始化要下载的文件列表
        /// </summary>
        private void GetDownloadFileList()
        {
            downloadList.Clear();
            totalDownloadSize = 0;
            alreadyDownloadSize = 0;
            alreadyDownloadSuccessCount = 0;
            alreadyDownloadErrorCount = 0;
            foreach (KeyValuePair<string, UpdateFileInfo> updateFileInfo in newFileInfoTable)
            {
                UpdateFileInfo fileInfo = null;
                oldFileInfoTable.TryGetValue(updateFileInfo.Key, out fileInfo);
                if (fileInfo?.Md5Code == updateFileInfo.Value.Md5Code)
                {
                    continue;
                }
                downloadList.Add(updateFileInfo.Key);
                totalDownloadSize += updateFileInfo.Value.ZipLength;
            }
            if (downloadList.Count == 0)
            {
                //更新结束
                isUpdateDone = true;
                updateSucces();
                return;
            }
        }

        /// <summary>
        /// 删除无效的文件
        /// </summary>
        private void DeleteUselessFiles()
        {
            if (newFileInfoTable.Count == 0) return;
            List<string> deleteFiles = new List<string>();
            foreach (KeyValuePair<string, UpdateFileInfo> updateFileInfo in oldFileInfoTable)
            {
                if (newFileInfoTable.ContainsKey(updateFileInfo.Key))
                {
                    continue;
                }
                deleteFiles.Add(updateFileInfo.Key);
            }
            foreach (string deleteFile in deleteFiles)
            {
                oldFileInfoTable.Remove(deleteFile);
                string filePath = PathUtility.GetCombinePath(AppConst.Path.PresistentDataPath, deleteFile);
                if (FileUtility.IsFileExist(filePath))
                {
                    FileUtility.DeleteFile(filePath);
                }
            }
        }

        /// <summary>
        /// 加载server上的filelist callback
        /// </summary>
        /// <param name="isOk"></param>
        /// <param name="datas"></param>
        /// <param name="errorMessage"></param>
        private void LoadServerFileListCallback(bool isOk, byte[] datas, string errorMessage)
        {
            newFileInfoTable.Clear();
            if (isOk && datas != null && datas.Length > 0)
            {
                using (ByteBuffer buffer = new ByteBuffer(datas))
                {
                    fileCount = ValueParse.ReadValue(buffer, ValueParse.IntParse);
                    for (int i = 0; i < fileCount; i++)
                    {
                        UpdateFileInfo updateFileInfo = new UpdateFileInfo
                        {
                            AssetBundleName = ValueParse.ReadValue(buffer, ValueParse.StringParse),
                            Length = ValueParse.ReadValue(buffer, ValueParse.IntParse),
                            Md5Code = ValueParse.ReadValue(buffer, ValueParse.IntParse),
                            ZipLength = ValueParse.ReadValue(buffer, ValueParse.IntParse),
                            ZipMd5Code = ValueParse.ReadValue(buffer, ValueParse.IntParse)
                        };
                        newFileInfoTable.Add(updateFileInfo.AssetBundleName, updateFileInfo);
                    }
                }
                DeleteUselessFiles();
                GetDownloadFileList();
                if (!isUpdateDone)
                {
                    BeginDownFile();
                    return;
                    //检查磁盘空间是否满足大小 todo
                    if ((long) Math.Ceiling(totalDownloadSize * 1.1f) > MobileSystemInfo.GetFreeDiskSpace())
                    {
                        //磁盘空间不足
                    }
                    else
                    {
                        if (MobileSystemInfo.NetAvailable)
                        {
                            //运营商网络
                            if (MobileSystemInfo.IsCarrier)
                            {
                                updatePanel.ShowMessageBox(UpdatePanel.enMessageBoxType.CancelOk,
                                    UpdateStringConfig.UpdateStatusBeginUpdate,
                                    UpdateStringConfig.UpdateStringHasErrorNotWifi,
                                    BeginDownFile, () =>
                                    {
                                        if (Application.isPlaying)
                                        {
                                            Application.Quit();
                                        }
                                    }, UpdateStringConfig.UpdateStatusWifiTips, FormatDownloadSize(totalDownloadSize));
                            }
                            // wi-fi 网络
                            else if (MobileSystemInfo.IsWifi)
                            {
                                //弹出提示框询问是否下载
                                updatePanel.ShowMessageBox(UpdatePanel.enMessageBoxType.CancelOk,
                                    UpdateStringConfig.UpdateStatusBeginUpdate,
                                    UpdateStringConfig.UpdateStateStartUpdateInfo,
                                    BeginDownFile, () =>
                                    {
                                        if (Application.isPlaying)
                                        {
                                            Application.Quit();
                                        }
                                    }, UpdateStringConfig.UpdateStatusWifiTips, FormatDownloadSize(totalDownloadSize));
                            }
                        }
                    }
                }
            }
            else
            {
                //获取服务器的FileList失败
                isUpdateDone = true;
                updateError("获取服务器最新版本的文件列表信息失败");
            }
        }

        private void BeginDownFile()
        {
            downloadErrorList.Clear();
            foreach (string file in downloadList)
            {
                string fileName = serverBundleIsZip ? file + ".zip" : file;
                string fileUrl = PathUtility.GetCombinePath(UpdateConfig.UpdateServerUrls[updateUrlIsDoneIndex],
                    platform, fileName);
                string savePath = PathUtility.GetCombinePath(AppConst.Path.HotUpdateDownloadDataPath, fileName);
                string saveDir = Path.GetDirectoryName(savePath);
                if (!Directory.Exists(saveDir))
                {
                    Directory.CreateDirectory(saveDir);
                }
                Singleton<GameEntry>.GetInstance().GetComponent<DownloadComponent>().AddDownload(file, savePath,
                    fileUrl,
                    DownloadDoneCallbcak, DownloadUpdateCallbcak, DownloadErrorCallbcak);
            }
        }

        private void DownloadDoneCallbcak(DownloadTask downloadTask, ulong size)
        {
            UpdateFileInfo updateFileInfo = newFileInfoTable[downloadTask.FileName];
            lock (lockObj)
            {
                if (VerificationFile(updateFileInfo, downloadTask.DownloadPath))
                {
                    alreadyDownloadSuccessCount++;
                    AppendHasUpdateFile(updateFileInfo);
                    RefreshDownloadState();
                }
                else
                {
                    DownloadErrorCallbcak(downloadTask, String.Empty);
                }
            }
        }

        private void DownloadErrorCallbcak(DownloadTask downloadTask, string message)
        {
            alreadyDownloadErrorCount++;
            UpdateFileInfo updateFileInfo = newFileInfoTable[downloadTask.FileName];
            downloadErrorList.Add(updateFileInfo.AssetBundleName);
            RefreshDownloadState();
        }

        private void DownloadUpdateCallbcak(DownloadTask downloadTask, ulong size, uint currentAdd, float progress)
        {
            alreadyDownloadSize += currentAdd;
        }

        private bool VerificationFile(UpdateFileInfo updateFileInfo, string filePath)
        {
//            return true;
            var length = 0;
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite))
            {
                length = (int) fileStream.Length;
                if (length != updateFileInfo.ZipLength)
                {
                    FileUtility.DeleteFile(filePath);
                    return false;
                }
                if (updateFileCache == null || updateFileCache.Length < length)
                {
                    updateFileCache = new byte[(length / OneMegaBytes + 1) * OneMegaBytes];
                }
                int offset = 0;
                int count = length;
                while (count > 0)
                {
                    int bytesRead = fileStream.Read(updateFileCache, offset, count);
                    offset += bytesRead;
                    count -= bytesRead;
                }
            }

            if (serverBundleIsZip)
            {
                byte[] zipMd5Bytes = MD5Utility.GetMd5Bytes(updateFileCache, 0, length);
                int zipMd5Code = BitConverter.ToInt32(zipMd5Bytes, 0);
                if (zipMd5Code != updateFileInfo.ZipMd5Code)
                {
                    FileUtility.DeleteFile(filePath);
                    return false;
                }
                int decompressSize = ZipUtility.DeCompressionFileInSamePath(filePath, serverBundleZipPassword);
                if (decompressSize != updateFileInfo.Length)
                {
                    FileUtility.DeleteFile(filePath);
                    return false;
                }
            }
            else
            {
                byte[] md5Bytes = MD5Utility.GetMd5Bytes(updateFileCache, 0, length);
                int md5Code = BitConverter.ToInt32(md5Bytes, 0);
                if (md5Code != updateFileInfo.Md5Code)
                {
                    FileUtility.DeleteFile(filePath);
                    return false;
                }
            }
            string srcPath = PathUtility.GetCombinePath(AppConst.Path.HotUpdateDownloadDataPath,
                updateFileInfo.AssetBundleName);
            string desPath = PathUtility.GetCombinePath(AppConst.Path.PresistentDataPath,
                updateFileInfo.AssetBundleName);
            if(FileUtility.IsFileExist(desPath))
            {
                FileUtility.DeleteFile(desPath);
            }
            FileUtility.Move(srcPath,desPath);
            if (FileUtility.IsFileExist(filePath))
            {
                FileUtility.DeleteFile(filePath);
            }
            return true;
        }

        private void RefreshDownloadState()
        {
            if (alreadyDownloadSuccessCount + alreadyDownloadErrorCount == downloadList.Count)
            {
                if (alreadyDownloadSuccessCount == downloadList.Count)
                {
                    UnityEngine.Debug.LogError("下载成功 没有失败的文件");
                    SaveServerVersionToPersistent();
                    SaveServerFileListToPersistent();
                    ClearHasUpdateInfo();
                    FileUtility.DeleteDirectory(AppConst.Path.HotUpdateDownloadDataPath);
                    isUpdateDone = true;
                    updateSucces();
                }
                else
                {
                    UnityEngine.Debug.LogError("下载成功 但是有失败的文件");
                    isUpdateDone = true;
                    updateError("下载更新文件出现异常");
                }
            }
        }

        private void SaveServerVersionToPersistent()
        {
            string filePath = PathUtility.GetCombinePath(AppConst.Path.PresistentDataPath,
                AppConst.AssetBundleConfig.VersionFile);
            using (ByteBuffer buffer = new ByteBuffer())
            {
                ValueParse.WriteValue(buffer, platform.ToString(), ValueParse.StringParse);
                ValueParse.WriteValue(buffer, serverVersion.MasterVersion, ValueParse.IntParse);
                ValueParse.WriteValue(buffer, serverVersion.MinorVersion, ValueParse.IntParse);
                ValueParse.WriteValue(buffer, serverVersion.RevisedVersion, ValueParse.IntParse);
                ValueParse.WriteValue(buffer, 0, ValueParse.IntParse);
                ValueParse.WriteValue(buffer, 0, ValueParse.LongParse);
                ValueParse.WriteValue(buffer,
                    string.IsNullOrEmpty(Singleton<GameEntry>.GetInstance().GetComponent<ResourceComponent>()
                        .AssetBundleVariant)
                        ? ""
                        : Singleton<GameEntry>.GetInstance().GetComponent<ResourceComponent>().AssetBundleVariant,
                    ValueParse.StringParse);
                ValueParse.WriteValue(buffer, true, ValueParse.BoolParse);
                ValueParse.WriteValue(buffer, "", ValueParse.StringParse);
                if (FileUtility.IsFileExist(filePath))
                {
                    FileUtility.DeleteFile(filePath);
                }
                File.WriteAllBytes(filePath, buffer.ToBytes());
            }
        }

        private void SaveServerFileListToPersistent()
        {
            string filePath = PathUtility.GetCombinePath(AppConst.Path.PresistentDataPath,
                AppConst.AssetBundleConfig.FileListFile);
            if (newFileInfoTable.Count > 0)
            {
                using (ByteBuffer buffer = new ByteBuffer())
                {
                    ValueParse.WriteValue(buffer, newFileInfoTable.Count, ValueParse.IntParse);
                    foreach (KeyValuePair<string, UpdateFileInfo> updateFileInfo in newFileInfoTable)
                    {
                        ValueParse.WriteValue(buffer, updateFileInfo.Value.AssetBundleName, ValueParse.StringParse);
                        ValueParse.WriteValue(buffer, updateFileInfo.Value.Length, ValueParse.IntParse);
                        ValueParse.WriteValue(buffer, updateFileInfo.Value.Md5Code, ValueParse.IntParse);
                        ValueParse.WriteValue(buffer, updateFileInfo.Value.ZipLength, ValueParse.IntParse);
                        ValueParse.WriteValue(buffer, updateFileInfo.Value.ZipMd5Code, ValueParse.IntParse);
                    }
                    if (FileUtility.IsFileExist(filePath))
                    {
                        FileUtility.DeleteFile(filePath);
                    }
                    File.WriteAllBytes(filePath, buffer.ToBytes());
                }
            }
        }

        private void ClearHasUpdateInfo()
        {
            string filePath = PathUtility.GetCombinePath(AppConst.Path.PresistentDataPath,
                AppConst.AssetBundleConfig.HasUpdateFileName);
            if (FileUtility.IsFileExist(filePath))
            {
                FileUtility.DeleteFile(filePath);
            }
        }

        public void AppendHasUpdateFile(UpdateFileInfo updateFileInfo)
        {
            string filePath = PathUtility.GetCombinePath(AppConst.Path.PresistentDataPath,
                AppConst.AssetBundleConfig.HasUpdateFileName);
            try
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Append))
                {
                    using (var write = new StreamWriter(fileStream))
                    {
                        write.WriteLine(updateFileInfo.AssetBundleName + "," + updateFileInfo.Length + "," +
                                        updateFileInfo.Md5Code + "," + updateFileInfo.ZipLength + "," +
                                        updateFileInfo.ZipMd5Code);
                    }
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
            }
        }

        private string FormatDownloadSize(long size)
        {
            string num = String.Empty;
            float MB = size / (1024.0f * 1024.0f);
            if (MB >= 1f)
            {
                MB = ((MB * 10) - (MB * 10) % 1) / 10;
                num = MB % 1 > 0 ? string.Format("{0:0.0} MB", MB) : string.Format("{0:0} MB", MB);
            }
            else
            {
                float kb = size / 1024.0f;
                num = string.Format("{0:0} KB", kb);
            }
            return num;
        }

        /// <summary>
        /// 获取server上的FileList
        /// </summary>
        private void GetServerGameFileList()
        {
            StartCoroutine(GetServerFileListCo(LoadServerFileListCallback));
        }

        private IEnumerator GetServerFileListCo(Action<bool, byte[], string> callBack)
        {
            string url = PathUtility.GetCombinePath(UpdateConfig.UpdateServerUrls[updateUrlIsDoneIndex], platform,
                AppConst.AssetBundleConfig.FileListFile);
            byte[] bytes = null;
            bool isError = false;
            UnityWebRequest unityWebRequest = UnityWebRequest.Get(url);
            yield return unityWebRequest.SendWebRequest();
            isError = unityWebRequest.isNetworkError;
            var errorMessage = isError ? unityWebRequest.error : null;
            bytes = unityWebRequest.downloadHandler.data;
            unityWebRequest.Dispose();
            callBack?.Invoke(!isError, bytes, errorMessage);
        }

        private void LoadServerVersionCallback(bool isOk, byte[] datas, string errorMessage)
        {
            if (isOk && datas != null && datas.Length > 0)
            {
                using (ByteBuffer buffer = new ByteBuffer(datas))
                {
                    platform = Singleton<GameEntry>.GetInstance().GetComponent<ResourceComponent>().PlatformName =
                        ValueParse.ReadValue(buffer, ValueParse.StringParse);
                    serverVersion = new Version.Version
                    {
                        MasterVersion = ValueParse.ReadValue(buffer, ValueParse.IntParse),
                        MinorVersion = ValueParse.ReadValue(buffer, ValueParse.IntParse),
                        RevisedVersion = ValueParse.ReadValue(buffer, ValueParse.IntParse)
                    };
                    Singleton<GameEntry>.GetInstance().GetComponent<ResourceComponent>().AssetBundleVariant =
                        ValueParse.ReadValue(buffer, ValueParse.StringParse);
                    serverBundleIsZip = ValueParse.ReadValue(buffer, ValueParse.BoolParse);
                    serverBundleZipPassword = ValueParse.ReadValue(buffer, ValueParse.StringParse);
                }
                VersionCompareResult compareResult =
                    Version.Version.CompareResult(serverVersion, currentVersion, false);
                switch (compareResult)
                {
                    case VersionCompareResult.Greater:
                        updatePanel.ShowMessageBox(UpdatePanel.enMessageBoxType.YesNo,
                            UpdateStringConfig.UpdateStatusBeginUpdate,
                            UpdateStringConfig.UpdateStringHasFatalErrorNeedReinstall,
                            () =>
                            {
                                OpenAppStore(SingletonMono<GameFramework>.GetInstance().GetAppId());
                                if (Application.isPlaying)
                                {
                                    Application.Quit();
                                }
                            }, () =>
                            {
                                if (Application.isPlaying)
                                {
                                    Application.Quit();
                                }
                            });
                        break;
                    case VersionCompareResult.Equal:
                    case VersionCompareResult.Less:
                        UpdateResource();
                        break;
                }
            }
            else
            {
                //获取服务器的版本信息失败
                isUpdateDone = true;
                updateError("获取服务器的版本信息失败");
            }
        }

        private void GetServerGameVersion()
        {
            StartCoroutine(GetServerVersionCo(LoadServerVersionCallback));
        }

        private IEnumerator GetServerVersionCo(Action<bool, byte[], string> callBack)
        {
            int urlIndex = UpdateConfig.UpdateServerUrls.Count - 1;
            string errorMessage = String.Empty;
            while (urlIndex > 0)
            {
                string url = PathUtility.GetCombinePath(UpdateConfig.UpdateServerUrls[urlIndex], platform,
                    AppConst.AssetBundleConfig.VersionFile);

                byte[] bytes = null;
                bool isError = false;
                UnityWebRequest unityWebRequest = UnityWebRequest.Get(url);
                yield return unityWebRequest.SendWebRequest();
                isError = unityWebRequest.isNetworkError;
                errorMessage = isError ? unityWebRequest.error : null;
                bytes = unityWebRequest.downloadHandler.data;
                unityWebRequest.Dispose();
                if (isError)
                {
                    urlIndex--;
                    continue;
                }
                updateUrlIsDoneIndex = urlIndex;
                callBack?.Invoke(true, bytes, null);
                yield break;
            }
            callBack?.Invoke(false, null, errorMessage);
        }

        /// <summary>
        /// 加载沙河目录的版本文件
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void LoadPresistentVersion()
        {
            string filePath = PathUtility.GetCombinePath(AppConst.Path.PresistentDataPath,
                AppConst.AssetBundleConfig.VersionFile);
            if (FileUtility.IsFileExist(filePath))
            {
                try
                {
                    using (ByteBuffer buffer = new ByteBuffer(FileUtility.ReadFile(filePath)))
                    {
                        platform = Singleton<GameEntry>.GetInstance().GetComponent<ResourceComponent>().PlatformName =
                            ValueParse.ReadValue(buffer, ValueParse.StringParse);
                        currentVersion =
                            new Version.Version
                            {
                                MasterVersion = ValueParse.ReadValue(buffer, ValueParse.IntParse),
                                MinorVersion = ValueParse.ReadValue(buffer, ValueParse.IntParse),
                                RevisedVersion = ValueParse.ReadValue(buffer, ValueParse.IntParse)
                            };
                        ValueParse.ReadValue(buffer, ValueParse.IntParse);
                        ValueParse.ReadValue(buffer, ValueParse.LongParse);
                        Singleton<GameEntry>.GetInstance().GetComponent<ResourceComponent>().AssetBundleVariant =
                            ValueParse.ReadValue(buffer, ValueParse.StringParse);
                        ValueParse.ReadValue(buffer, ValueParse.BoolParse);
                        ValueParse.ReadValue(buffer, ValueParse.StringParse);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            else
            {
                isUpdateDone = true;
                updateError("获取获取当前客户端版本信息失败");
            }
        }

        /// <summary>
        /// 第一次处理完成的回调
        /// </summary>
        private void FirstDecompressCallBack()
        {
            if (!isCopyFileListDone || !isCopyVersionDone)
            {
                return;
            }
            LoadPresistentVersionAndFileList();
        }

        /// <summary>
        /// 清理沙河目录下的所有文件
        /// </summary>
        private void DeleAllPresistentData()
        {
            foreach (var directory in Directory.GetDirectories(AppConst.Path.PresistentDataPath))
            {
                FileUtility.DeleteDirectory(directory, true);
            }
            foreach (var file in Directory.GetFiles(AppConst.Path.PresistentDataPath))
            {
                FileUtility.DeleteFile(file);
            }
        }

        /// <summary>
        /// 获取解压原始zip的应用版本
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private bool GetFirstDecompressVersion()
        {
            string path = PathUtility.GetCombinePath(AppConst.Path.PresistentDataPath,
                AppConst.AssetBundleConfig.IsInstanllDecompressFileName);
            if (FileUtility.IsFileExist(path))
            {
                try
                {
                    string text = FileUtility.ReadAllText(path);
                    if (!string.IsNullOrEmpty(text))
                    {
                        firstDecompressVersion = new Version.Version(text);
                        return true;
                    }
                    return false;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            return false;
        }

        /// <summary>
        /// 打开应用商店
        /// </summary>
        /// <param name="appId"></param>
        public void OpenAppStore(string appId)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            Application.OpenURL("market://details?id=" + appId);
#elif UNITY_IOS && !UNITY_EDITOR
            Application.OpenURL("itms-apps://itunes.apple.com/app/id" + appId);
#endif
        }

        private void ClearHotUpdate()
        {
            oldFileInfoTable.Clear();
            newFileInfoTable.Clear();
            downloadList.Clear();
            downloadErrorList.Clear();
            updateStringConfig = null;
            updateConfig = null;
            updateFileCache = null;
            installVersion = null;
            firstDecompressVersion = null;
            currentVersion = null;
            serverVersion = null;
            updatePanel.ClearResource();
            updatePanel = null;
        }
    }
}