using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using GameFramework.Utility;
using GameFramework.Utility.Compress;
using GameFramework.Utility.File;
using UnityEditor;
using UnityEngine;
using Version = GameFramework.Update.Version.Version;

namespace GameFramework.Editor.Core.AssetBundle
{
    public class AssetBundleBuildManager
    {
        public static string AssetBundleBuildRulePath =
            "Assets/Game/GameFramework/Editor/EditorAsset/AssetBundleBuildRules.asset";

        public static string VersionFile = "version.json";
        
        public static string OutputDirectory = "";
        public static string WorkingDirectory => $"{OutputDirectory}/Working/";
        public static string TempWorkingDirectory => $"{OutputDirectory}/TempWorking/";
        public static string UpdateFullDirectory => $"{OutputDirectory}/Update/{CurrentVersion.ToString()}/";
        public static string PackageDirectory => $"{OutputDirectory}/Package/{CurrentVersion.ToString()}/";
        public static Version CurrentVersion = new Version("0.0.1");

        public class AssetBundleBuildRule : ScriptableObject
        {
            [Flags]
            public enum Platform
            {
                Windows32 = 1 << 0,
                Windows64 = 1 << 1,
                MacOs = 1 << 2,
                Ios = 1 << 3,
                Android = 1 << 4,
            }
            
            public enum enRuleType
            {
                None,
                //路径下的所有文件使用相同的AssetBundleName
                AllFileWithSameBundleName,
                //路径下的所有文件使用上层目录路径的Name
                AllFileWithSamePathName,
                //一个文件打成一个AssetBundle
                SingleFileWithFileName,
            }
            
            [Serializable]
            public class AssetBundleBuildData
            {
                public enRuleType RuleType = enRuleType.None;
                public string AssetPath = String.Empty;
                public string AssetBundleName = String.Empty;
                public string FileFilter = String.Empty;
            }
            
            public List<AssetBundleBuildData> AssetBundleBuildDatas = new List<AssetBundleBuildData>();
            public HashSet<string> SceneBuildData = new HashSet<string>();
            public bool ZipSelected = true;
            public bool UncompressedAssetBundle;
            public bool DisableWriteTypeTree;
            public bool DeterministicAssetBundle = true;
            public bool ForceRebuildAssetBundle;
            public bool IgnoreTypeTreeChanges;
            public bool ChunkBasedCompression = true;
            public Platform Platforms;
            public string AssetBundleVariant = string.Empty;
        }
        
        private static AssetBundleBuildRule assetBundleRule;

        public static AssetBundleBuildRule AssetBundleRule
        {
            get
            {
                if (assetBundleRule == null)
                {
                    assetBundleRule = AssetDatabase.LoadAssetAtPath<AssetBundleBuildRule>(AssetBundleBuildRulePath);
                    if (assetBundleRule == null)
                    {
                        assetBundleRule = ScriptableObject.CreateInstance<AssetBundleBuildRule>();
                        AssetDatabase.CreateAsset(assetBundleRule, AssetBundleBuildRulePath);
                        AssetDatabase.SaveAssets();
                    }
                }
                return assetBundleRule;
            }
        }
        
        public static bool IsPlatformSelected(AssetBundleBuildRule.Platform platform)
        {
            return (AssetBundleRule.Platforms & platform) != 0;
        }
        
        public static void SelectPlatform(AssetBundleBuildRule.Platform platform, bool selected)
        {
            if (selected)
            {
                AssetBundleRule.Platforms |= platform;
            }
            else
            {
                AssetBundleRule.Platforms &= ~platform;
            }
        }
        
        private static BuildAssetBundleOptions GetBuildAssetBundleOptions()
        {
            BuildAssetBundleOptions buildOptions = BuildAssetBundleOptions.None;

            if (AssetBundleRule.UncompressedAssetBundle)
            {
                buildOptions |= BuildAssetBundleOptions.UncompressedAssetBundle;
            }

            if (AssetBundleRule.DisableWriteTypeTree)
            {
                buildOptions |= BuildAssetBundleOptions.DisableWriteTypeTree;
            }

            if (AssetBundleRule.DeterministicAssetBundle)
            {
                buildOptions |= BuildAssetBundleOptions.DeterministicAssetBundle;
            }

            if (AssetBundleRule.ForceRebuildAssetBundle)
            {
                buildOptions |= BuildAssetBundleOptions.ForceRebuildAssetBundle;
            }

            if (AssetBundleRule.IgnoreTypeTreeChanges)
            {
                buildOptions |= BuildAssetBundleOptions.IgnoreTypeTreeChanges;
            }

            if (AssetBundleRule.ChunkBasedCompression)
            {
                buildOptions |= BuildAssetBundleOptions.ChunkBasedCompression;
            }

            return buildOptions;
        }
        
        private static BuildTarget GetBuildTarget(AssetBundleBuildRule.Platform platform)
        {
            switch (platform)
            {
                case AssetBundleBuildRule.Platform.Windows32:
                    return BuildTarget.StandaloneWindows;
                case AssetBundleBuildRule.Platform.Windows64:
                    return BuildTarget.StandaloneWindows64;
                case AssetBundleBuildRule.Platform.MacOs:
                    return BuildTarget.StandaloneOSX;
                case AssetBundleBuildRule.Platform.Ios:
                    return BuildTarget.iOS;
                case AssetBundleBuildRule.Platform.Android:
                    return BuildTarget.Android;
                default:
                    throw new Exception("打包平台不存在.... 请检查");
            }
        }
        
        public static bool BuildAssetBundle()
        {
            if (Directory.Exists(PackageDirectory))
            {
                Directory.Delete(PackageDirectory,true);
            }
            Directory.CreateDirectory(PackageDirectory);
            if (Directory.Exists(WorkingDirectory))
            {
                Directory.Delete(WorkingDirectory,true);
            }
            Directory.CreateDirectory(WorkingDirectory);
            if (Directory.Exists(TempWorkingDirectory))
            {
                Directory.Delete(TempWorkingDirectory,true);
            }
            Directory.CreateDirectory(TempWorkingDirectory);
            if (Directory.Exists(UpdateFullDirectory))
            {
                Directory.Delete(UpdateFullDirectory,true);
            }
            Directory.CreateDirectory(UpdateFullDirectory);
            
            Reset();
            SetAllAssetBundleName(AppConst.AssetBundleConfig.EnableAssetBundleRedundance);
            BuildAssetBundleOptions buildAssetBundleOptions = GetBuildAssetBundleOptions();
            bool isSuccess = false;
            isSuccess = BuildAssetBundles(AssetBundleBuildRule.Platform.Windows32, buildAssetBundleOptions);

            if (isSuccess)
            {
                isSuccess = BuildAssetBundles(AssetBundleBuildRule.Platform.Windows64, buildAssetBundleOptions);
            }

            if (isSuccess)
            {
                isSuccess = BuildAssetBundles(AssetBundleBuildRule.Platform.MacOs, buildAssetBundleOptions);
            }

            if (isSuccess)
            {
                isSuccess = BuildAssetBundles(AssetBundleBuildRule.Platform.Ios, buildAssetBundleOptions);
            }

            if (isSuccess)
            {
                isSuccess = BuildAssetBundles(AssetBundleBuildRule.Platform.Android, buildAssetBundleOptions);
            }
            
            AssetDatabase.RemoveUnusedAssetBundleNames();
            AssetDatabase.Refresh();
            return isSuccess;
        }

        public static bool BuildAssetBundles(AssetBundleBuildRule.Platform platform,BuildAssetBundleOptions buildAssetBundleOptions)
        {
            if (!IsPlatformSelected(platform))
            {
                return true;
            }
            string platformName = platform.ToString();
            string workingPath = $"{WorkingDirectory}{platformName}/";
            string tempworkingPath = $"{TempWorkingDirectory}{platformName}/";
            string packagePath = $"{PackageDirectory}{platformName}/";
            string updatePath = $"{UpdateFullDirectory}{platformName}/";
            if (Directory.Exists(workingPath))
            {
                Directory.Delete(workingPath,true);
            }
            Directory.CreateDirectory(workingPath);
            
            if (Directory.Exists(packagePath))
            {
                Directory.Delete(packagePath,true);
            }
            Directory.CreateDirectory(packagePath);
            
            if (Directory.Exists(updatePath))
            {
                Directory.Delete(updatePath,true);
            }
            Directory.CreateDirectory(updatePath);

            AssetBundleManifest assetBundleManifest =
                BuildPipeline.BuildAssetBundles(workingPath, buildAssetBundleOptions, GetBuildTarget(platform));
            if (assetBundleManifest == null)
            {
                return false;
            }
            ProcessUpdate(workingPath,updatePath,platform);
            ProcessPackage(workingPath, tempworkingPath, packagePath, platform);
            return true;
        }

        private static void ProcessUpdate(string workingPath ,string updatePath, AssetBundleBuildRule.Platform platform)
        {
            string updateVersionPath = $"{updatePath}{AppConst.AssetBundleConfig.VersionFile}";
            using (ByteBuffer buffer = new ByteBuffer())
            {
                ValueParse.WriteValue(buffer, CurrentVersion.MasterVersion, ValueParse.IntParse);
                ValueParse.WriteValue(buffer, CurrentVersion.MinorVersion, ValueParse.IntParse);
                ValueParse.WriteValue(buffer, CurrentVersion.RevisedVersion, ValueParse.IntParse);
                ValueParse.WriteValue(buffer,
                    string.IsNullOrEmpty(AssetBundleRule.AssetBundleVariant) ? "" : AssetBundleRule.AssetBundleVariant,
                    ValueParse.StringParse);
                File.WriteAllBytes(updateVersionPath,buffer.ToBytes());
            }
            string[] allFiles = Directory.GetFiles(workingPath, "*", SearchOption.AllDirectories);
            List<string> allAssetBundleFiles = new List<string>();
            foreach (string tempFile in allFiles)
            {
                string path = tempFile.Replace("\\", "/");
                if(Path.GetExtension(path) == ".mainfest") continue;
                allAssetBundleFiles.Add(path);
            }
            EditorUtility.ClearProgressBar();
            int index = 0;
            int count = allAssetBundleFiles.Count;
            foreach (string assetBundleFile in allAssetBundleFiles)
            {
                index++;
                EditorUtility.DisplayProgressBar("压缩AssetBundle", "压缩AssetBundle文件", (float)index / (float)count);
                string target = assetBundleFile.Replace(workingPath, updatePath);
                string targetPath = Path.GetDirectoryName(target);
                if (!Directory.Exists(targetPath))
                {
                    if (targetPath != null) Directory.CreateDirectory(targetPath);
                    //Compress
                    ZIPUtility.ZipCompress(assetBundleFile,new FileInfo[]{new FileInfo(assetBundleFile), },target+".zip",4);
                }
            }
            EditorUtility.ClearProgressBar();
        }
        
        private static void ProcessPackage(string workingPath ,string tempworkingPath ,string packagePath, AssetBundleBuildRule.Platform platform)
        {
            string packageVersionPath = $"{packagePath}{AppConst.AssetBundleConfig.VersionFile}";
            using (ByteBuffer buffer = new ByteBuffer())
            {
                ValueParse.WriteValue(buffer, CurrentVersion.MasterVersion, ValueParse.IntParse);
                ValueParse.WriteValue(buffer, CurrentVersion.MinorVersion, ValueParse.IntParse);
                ValueParse.WriteValue(buffer, CurrentVersion.RevisedVersion, ValueParse.IntParse);
                ValueParse.WriteValue(buffer,
                    string.IsNullOrEmpty(AssetBundleRule.AssetBundleVariant) ? "" : AssetBundleRule.AssetBundleVariant,
                    ValueParse.StringParse);
                File.WriteAllBytes(packageVersionPath,buffer.ToBytes());
            }
            //Compress
            string[] allFiles = Directory.GetFiles(workingPath, "*", SearchOption.AllDirectories);
            List<string> allAssetBundleFiles = new List<string>();
            foreach (string tempFile in allFiles)
            {
                string path = tempFile.Replace("\\", "/");
                if(Path.GetExtension(path) == ".mainfest") continue;
                allAssetBundleFiles.Add(path);
            }
            EditorUtility.ClearProgressBar();
            int index = 0;
            int count = allAssetBundleFiles.Count;
            foreach (string assetBundleFile in allAssetBundleFiles)
            {
                index++;
                EditorUtility.DisplayProgressBar("复制AssetBundle", "复制AssetBundle文件", (float)index / (float)count);
                string target = assetBundleFile.Replace(workingPath, tempworkingPath);
                string targetPath = Path.GetDirectoryName(target);
                if (!Directory.Exists(targetPath))
                {
                    if (targetPath != null) Directory.CreateDirectory(targetPath);
                    File.Copy(assetBundleFile, target);
                }
            }
            EditorUtility.ClearProgressBar();
            ZIPUtility.ZipCompress(tempworkingPath,packageVersionPath,"*",4);
            EditorUtility.DisplayDialog("压缩AssetBundle", "压缩AssetBundle文件夹完成", "确定");
            Directory.Delete(tempworkingPath,true);
        }

        
        public static void Reset()
        {
            if (AppConst.AssetBundleConfig.ResetAssetBundleName)
            {
                var assetBundleNames = AssetDatabase.GetAllAssetBundleNames();
                foreach (string assetBundleName in assetBundleNames)
                {
                    AssetDatabase.RemoveAssetBundleName(assetBundleName, true);
                }
            }
            AssetDatabase.Refresh();
        }

        public static void SetAllAssetBundleName(bool checkRedundance = true)
        {
            
        }

        public static void SetAssetBundleName(string assetPath, string abName)
        {
            var import = AssetImporter.GetAtPath(assetPath);
            if (import == null)
            {
                return;
            }
            abName = abName.ToLower();
            abName += AppConst.AssetBundleConfig.BundleSuffix;
            if (import.assetBundleName != abName ||
                import.assetBundleVariant != AssetBundleRule.AssetBundleVariant)
            {
                import.SetAssetBundleNameAndVariant(abName,AssetBundleRule.AssetBundleVariant);
                import.SaveAndReimport();
            }
        }
        /// <summary>
        /// BundleData/BB/CC/DD.xx      AssetBundleName 文件的上一级目录为AssetBundleName
        /// </summary>
        public static void SetAssetBundleNameUsePathName(string inPath, string outPath, List<string> exts)
        {
            if (inPath.EndsWith("/"))
            {
                inPath = inPath.Substring(0, inPath.Length - 1);
            } 
            if (outPath.EndsWith("/"))
            {
                outPath = outPath.Substring(0, outPath.Length - 1);
            }
            List<string> files = new List<string>();
            FileUtility.RecursiveFile(inPath, files, exts);
            for (int i = 0; i < files.Count; i++)
            {
                string filePath = files[i];
                filePath = filePath.Replace(inPath, outPath);
                filePath = filePath.Substring(0, filePath.LastIndexOf('/'));
                SetAssetBundleName(files[i],filePath);
            }
        }

        /// <summary>
        /// 文件夹下每个资源独自打成一个AssetBundle
        /// </summary>
        public static void SetAssetBundleNamePathSingle(string inPath, string outPath, List<string> exts)
        {
            if (inPath.EndsWith("/"))
            {
                inPath = inPath.Substring(0, inPath.Length - 1);
            } 
            if (outPath.EndsWith("/"))
            {
                outPath = outPath.Substring(0, outPath.Length - 1);
            }
            List<string> files = new List<string>();
            FileUtility.RecursiveFile(inPath, files, exts);
            for (int i = 0; i < files.Count; i++)
            {
                string filePath = files[i];
                string abName = filePath.Replace(inPath, "");
                abName = abName.Substring(0, abName.LastIndexOf('.'));
                abName = $"{outPath}{abName}";
                SetAssetBundleName(filePath,abName);
            }
        }

        /// <summary>
        /// 设置文件夹下所有指定后缀文件相同的 AssetBundle名
        /// </summary>
        public static void SetAssetBundleNameAll(string path, string abName, List<string> exts)
        {
            if (path.EndsWith("/"))
            {
                path = path.Substring(0, path.Length - 1);
            } 
            if (abName.EndsWith("/"))
            {
                abName = abName.Substring(0, abName.Length - 1);
            }
            List<string> files = new List<string>();
            FileUtility.RecursiveFile(path, files, exts);
            for (int i = 0; i < files.Count; i++)
            {
                SetAssetBundleName(files[i],abName);
            }
        }

        public static void SetScenceAssetBundleName()
        {
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            if (scenes.Length > 0)
            {
                for (int i = 0; i < scenes.Length; i++)
                {
                    EditorBuildSettingsScene scene = scenes[i];
                    if (string.IsNullOrEmpty(scene.path))
                    {
                        continue;
                    }
                    string abName = scene.path.Substring(scene.path.LastIndexOf('/') + 1);
                    abName = abName.Substring(0, abName.LastIndexOf('.')).ToLower();
                    if (AssetBundleBuildManager.AssetBundleRule.SceneBuildData.Contains(abName))
                    {
                        continue;
                    }
                    SetAssetBundleName(scene.path, $"scene{abName}");
                }
            }
        }

        public static void SetLuaAssetBundleName(string path,string abName,List<string> exts)
        {
            if (path.EndsWith("/"))
            {
                path = path.Substring(0, path.Length - 1);
            } 
            if (abName.EndsWith("/"))
            {
                abName = abName.Substring(0, abName.Length - 1);
            }
            string tempPath = "Assets/Temp/lua";
            FileUtility.DeleteDirectory(tempPath);
            List<string> files = new List<string>();
            FileUtility.RecursiveFile(path, files, exts);
            for (int i = 0; i < files.Count; i++)
            {
                string srcPath = files[i];
                string desPath = srcPath.Replace(path, tempPath).Replace(".lua", ".bytes");
                FileUtility.CopyFile(srcPath,desPath);
            }
            
            AssetDatabase.Refresh();
            SetAssetBundleNameAll(tempPath, abName, new List<string>() {"bytes"});
        }
        
    }
}