using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using GameFramework.Utility.File;
using UnityEditor;
using UnityEngine;

namespace GameFramework.Editor.Core.AssetBundle
{
    public class AssetBundleBuildManager
    {
        public static string AssetBundleBuildRulePath =
            "Assets/Game/GameFramework/Editor/EditorAsset/AssetBundleBuildRules.asset";

//        public static Dictionary<string, HashSet<string>> AssetBundleNameMap = new Dictionary<string, HashSet<string>>();
        
        public class AssetBundleBuildRule : ScriptableObject
        {
            [Flags]
            public enum Platform
            {
                Undefined = 0,
                Windows64 = 1 << 0,
                MacOs = 1 << 1,
                Ios = 1 << 2,
                Android = 1 << 3,
            }
            
            public enum enRuleType
            {
                None,
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
            public bool DeterministicAssetBundle;
            public bool ForceRebuildAssetBundle;
            public bool IgnoreTypeTreeChanges;
            public bool AppendHashToAssetBundleName;
            public bool ChunkBasedCompression;
            public Platform Platforms;
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
            return (AssetBundleBuildManager.AssetBundleRule.Platforms & platform) != 0;
        }
        
        public static void SelectPlatform(AssetBundleBuildRule.Platform platform, bool selected)
        {
            if (selected)
            {
                AssetBundleBuildManager.AssetBundleRule.Platforms |= platform;
            }
            else
            {
                AssetBundleBuildManager.AssetBundleRule.Platforms &= ~platform;
            }
        }
        
        private static BuildAssetBundleOptions GetBuildAssetBundleOptions()
        {
            BuildAssetBundleOptions buildOptions = BuildAssetBundleOptions.None;

            if (AssetBundleBuildManager.AssetBundleRule.UncompressedAssetBundle)
            {
                buildOptions |= BuildAssetBundleOptions.UncompressedAssetBundle;
            }

            if (AssetBundleBuildManager.AssetBundleRule.DisableWriteTypeTree)
            {
                buildOptions |= BuildAssetBundleOptions.DisableWriteTypeTree;
            }

            if (AssetBundleBuildManager.AssetBundleRule.DeterministicAssetBundle)
            {
                buildOptions |= BuildAssetBundleOptions.DeterministicAssetBundle;
            }

            if (AssetBundleBuildManager.AssetBundleRule.ForceRebuildAssetBundle)
            {
                buildOptions |= BuildAssetBundleOptions.ForceRebuildAssetBundle;
            }

            if (AssetBundleBuildManager.AssetBundleRule.IgnoreTypeTreeChanges)
            {
                buildOptions |= BuildAssetBundleOptions.IgnoreTypeTreeChanges;
            }

            if (AssetBundleBuildManager.AssetBundleRule.AppendHashToAssetBundleName)
            {
                buildOptions |= BuildAssetBundleOptions.AppendHashToAssetBundleName;
            }

            if (AssetBundleBuildManager.AssetBundleRule.ChunkBasedCompression)
            {
                buildOptions |= BuildAssetBundleOptions.ChunkBasedCompression;
            }

            return buildOptions;
        }
        
        public static void BuildAssetBundle()
        {
            Reset();
            SetAllAssetBundleName(AppConst.AssetBundleConfig.EnableAssetBundleRedundance);
            AssetDatabase.RemoveUnusedAssetBundleNames();
            AssetDatabase.Refresh();
        }
        
        public static void Reset()
        {
            if (FileUtility.IsDirectoryExist(AppConst.AssetBundleConfig.AssetBundleOutputDir))
            {
                FileUtility.DeleteDirectory(AppConst.AssetBundleConfig.AssetBundleOutputDir);
            }
            if (AppConst.AssetBundleConfig.ResetAssetBundleName)
            {
                var assetBundleNames = AssetDatabase.GetAllAssetBundleNames();
                foreach (string assetBundleName in assetBundleNames)
                {
                    AssetDatabase.RemoveAssetBundleName(assetBundleName, true);
                }
//                AssetBundleNameMap.Clear();
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
                import.assetBundleVariant != AppConst.AssetBundleConfig.AssetBundleVariant)
            {
                import.SetAssetBundleNameAndVariant(abName,AppConst.AssetBundleConfig.AssetBundleVariant);
                import.SaveAndReimport();
            }
        }
        /// <summary>
        /// BundleData/BB/CC/DD.xx      AssetBundleName 文件的上一级目录为AssetBundleName
        /// </summary>
        /// <param name="inPath"></param>
        /// <param name="outPath"></param>
        /// <param name="exts"></param>
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
        /// <param name="inPath"></param>
        /// <param name="outPath"></param>
        /// <param name="exts"></param>
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
        /// <param name="path"></param>
        /// <param name="abName"></param>
        /// <param name="exts"></param>
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