using System;
using GameFramework.Base;
using GameFramework.Res;
using GameFramework.Utility;
using GameFramework.Utility.PathUtility;
using GameFramework.Utility.Singleton;
using UnityEngine;

namespace GameFramework.Update
{
    public class UpdateComponent : GameFrameworkComponent
    {
        public override int Priority => 50;
        private UpdateStringConfig updateStringConfig = null;
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
        private Version.Version currentVersion;
        private Version.Version onlineVersion;

        private bool installVersionIsDone = false;
        private bool installFileListIsDone = false;
        private long installZipDecompressSize = 0;
        private string installZipDecompressPassword = "";
        
        private UpdatePanel updatePanel;
        
        public void Init()
        {
            updatePanel = new UpdatePanel();
            updatePanel.InitPanel();
            installVersionIsDone = false;
            installFileListIsDone = false;
            Singleton<GameEntry>.GetInstance().GetComponent<ResourceComponent>().LoadBinaryFile(
                PathUtility.GetRemotePath(AppConst.Path.InstallDataPath, AppConst.AssetBundleConfig.VersionFile),
                InitInstallVersion);
            Singleton<GameEntry>.GetInstance().GetComponent<ResourceComponent>().LoadBinaryFile(
                PathUtility.GetRemotePath(AppConst.Path.InstallDataPath, AppConst.AssetBundleConfig.FileListFile),
                InitInstallFileList);
        }
        
        public void InitInstallVersion(string fileUrl,byte[] bytes,string errorMessage)
        {
            if (bytes == null || bytes.Length == 0)
            {
                throw new Exception("local version.dat file is invalid");
            }
            using (ByteBuffer buffer = new ByteBuffer(bytes))
            {
//                ValueParse.WriteValue(buffer, platform.ToString(), ValueParse.StringParse);
//                ValueParse.WriteValue(buffer, CurrentVersion.MasterVersion, ValueParse.IntParse);
//                ValueParse.WriteValue(buffer, CurrentVersion.MinorVersion, ValueParse.IntParse);
//                ValueParse.WriteValue(buffer, CurrentVersion.RevisedVersion, ValueParse.IntParse);
//                ValueParse.WriteValue(buffer, sizeCount, ValueParse.LongParse);
//                ValueParse.WriteValue(buffer,
//                    string.IsNullOrEmpty(AssetBundleRule.AssetBundleVariant) ? "" : AssetBundleRule.AssetBundleVariant,
//                    ValueParse.StringParse);
//                ValueParse.WriteValue(buffer, AssetBundleRule.ZipSelected, ValueParse.BoolParse);
//                ValueParse.WriteValue(buffer,
//                    string.IsNullOrEmpty(AssetBundleRule.ZipPassWord) ? "" : AssetBundleRule.ZipPassWord,
//                    ValueParse.StringParse);
                platform = Singleton<GameEntry>.GetInstance().GetComponent<ResourceComponent>().PlatformName =
                    ValueParse.ReadValue(buffer, ValueParse.StringParse);
                installVersion =
                    new Version.Version
                    {
                        MasterVersion = ValueParse.ReadValue(buffer, ValueParse.IntParse),
                        MinorVersion = ValueParse.ReadValue(buffer, ValueParse.IntParse),
                        RevisedVersion = ValueParse.ReadValue(buffer, ValueParse.IntParse)
                    };
                
            }
            RefreshInstallDataInfo();
        }
        public void InitInstallFileList(string fileUrl,byte[] bytes,string errorMessage)
        {
            
        }

        private void RefreshInstallDataInfo()
        {
            if (!installVersionIsDone || !installFileListIsDone)
            {
                return;
            }

        }
    }
}