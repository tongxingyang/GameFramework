using System;
using System.IO;
using GameFramework.Utility.Compress;
using GameFramework.Utility.File;
using UnityEditor;
using UnityEngine;
using Version = GameFramework.Update.Version.Version;

namespace GameFramework.Editor.Core.AssetBundle
{
    public class AssetBundleEditor : EditorWindow
    {
        [MenuItem("Tools/ClearProgress")]
        public static void ClearProgress()
        {
            EditorUtility.ClearProgressBar();
        }
        
        [MenuItem("Tools/AssetBunle/Build AssetBundle设置")]
        public static void BuildAssetBundleEditor()
        {
            AssetBundleEditor assetBundleEditor = GetWindow<AssetBundleEditor>();
            assetBundleEditor.Show();
            assetBundleEditor.position = new Rect(100, 100, 1300, 400);
            assetBundleEditor.minSize = new Vector2(1300, 600);
            GetVersion();
        }

        private Vector2 scrollPosition;
        private int CurrentIndex = -1;
        private string scenceName = String.Empty;

        private AssetBundleBuildManager.AssetBundleBuildRule.AssetBundleBuildData currentSelectData =
            new AssetBundleBuildManager.AssetBundleBuildRule.AssetBundleBuildData();

        private void OnGUI()
        {
            EditorGUILayout.LabelField("设置场景随包发布,不需要打成ab的名字:",EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal("box");
            {
                EditorGUILayout.LabelField("SceneName:", GUILayout.MinWidth(100));
                scenceName = EditorGUILayout.TextField(scenceName);
                if (GUILayout.Button("添加场景", GUILayout.MinWidth(100)))
                {
                    if (string.IsNullOrEmpty(scenceName))
                    {
                        return;
                    }
                    AssetBundleBuildManager.AssetBundleRule.SceneBuildData.Add(scenceName);
                }
                if (GUILayout.Button("移除场景", GUILayout.MinWidth(100)))
                {
                    if (string.IsNullOrEmpty(scenceName))
                    {
                        return;
                    }
                    AssetBundleBuildManager.AssetBundleRule.SceneBuildData.Remove(scenceName);
                }
            }
            EditorGUILayout.EndHorizontal();
            foreach (string senceName in AssetBundleBuildManager.AssetBundleRule.SceneBuildData)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("随包发布的场景名 :");
                    EditorGUILayout.LabelField(senceName);
                }
                EditorGUILayout.EndHorizontal();
            }
            GUILayout.Space(15);
            EditorGUILayout.BeginHorizontal("box");
            {
                AssetBundleBuildManager.AssetBundleRule.AssetBundleVariant = EditorGUILayout.TextField(
                    "AssetBundle变体名称", AssetBundleBuildManager.AssetBundleRule.AssetBundleVariant,
                    GUILayout.Width(500));
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("AssetPath:", GUILayout.MinWidth(100));
                currentSelectData.AssetPath = EditorGUILayout.TextField(currentSelectData.AssetPath);
                EditorGUILayout.LabelField("AssetBundleName:", GUILayout.MinWidth(100));
                currentSelectData.AssetBundleName = EditorGUILayout.TextField(currentSelectData.AssetBundleName);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("FileFilter:", GUILayout.MinWidth(100));
                currentSelectData.FileFilter = EditorGUILayout.TextField(currentSelectData.FileFilter);
                EditorGUILayout.LabelField("RuleType:", GUILayout.MinWidth(100));
                currentSelectData.RuleType =
                    (AssetBundleBuildManager.AssetBundleBuildRule.enRuleType) EditorGUILayout.EnumPopup("",
                        currentSelectData.RuleType);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Save", GUILayout.MinWidth(100)))
                {
                    SetCurrentValue(AssetBundleBuildManager.AssetBundleRule.AssetBundleBuildDatas[CurrentIndex],
                        currentSelectData);
                }
                if (GUILayout.Button("Delete", GUILayout.MinWidth(100)))
                {
                    if (CurrentIndex < 0) return;
                    AssetBundleBuildManager.AssetBundleRule.AssetBundleBuildDatas.RemoveAt(CurrentIndex);
                }
                if (GUILayout.Button("New Data", GUILayout.MinWidth(100)))
                {
                    if (string.IsNullOrEmpty(currentSelectData.AssetPath)
                        || string.IsNullOrEmpty(currentSelectData.AssetBundleName)
                        || string.IsNullOrEmpty(currentSelectData.FileFilter)
                        || currentSelectData.RuleType == AssetBundleBuildManager.AssetBundleBuildRule.enRuleType.None)
                    {
                        return;
                    }
                    AssetBundleBuildManager.AssetBundleBuildRule.AssetBundleBuildData data =
                        new AssetBundleBuildManager.AssetBundleBuildRule.AssetBundleBuildData
                        {
                            AssetPath = currentSelectData.AssetPath,
                            AssetBundleName = currentSelectData.AssetBundleName,
                            FileFilter = currentSelectData.FileFilter,
                            RuleType = currentSelectData.RuleType
                        };
                    AssetBundleBuildManager.AssetBundleRule.AssetBundleBuildDatas.Add(data);
                    CurrentIndex = AssetBundleBuildManager.AssetBundleRule.AssetBundleBuildDatas.Count - 1;
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            GUILayout.Space(5f);
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.LabelField("Platform", EditorStyles.boldLabel);
                    EditorGUILayout.BeginHorizontal("box");
                    {
                        EditorGUILayout.BeginVertical();
                        {
                            AssetBundleBuildManager.SelectPlatform(
                                AssetBundleBuildManager.AssetBundleBuildRule.Platform.Windows32,
                                EditorGUILayout.ToggleLeft("Windows x86",
                                    AssetBundleBuildManager.IsPlatformSelected(AssetBundleBuildManager
                                        .AssetBundleBuildRule.Platform.Windows32)));
                            AssetBundleBuildManager.SelectPlatform(
                                AssetBundleBuildManager.AssetBundleBuildRule.Platform.Windows64,
                                EditorGUILayout.ToggleLeft("Windows x64",
                                    AssetBundleBuildManager.IsPlatformSelected(AssetBundleBuildManager
                                        .AssetBundleBuildRule.Platform.Windows64)));
                            AssetBundleBuildManager.SelectPlatform(
                                AssetBundleBuildManager.AssetBundleBuildRule.Platform.MacOs,
                                EditorGUILayout.ToggleLeft("Apple MacOS",
                                    AssetBundleBuildManager.IsPlatformSelected(AssetBundleBuildManager
                                        .AssetBundleBuildRule.Platform.MacOs)));
                            AssetBundleBuildManager.SelectPlatform(
                                AssetBundleBuildManager.AssetBundleBuildRule.Platform.Ios,
                                EditorGUILayout.ToggleLeft("iPhone",
                                    AssetBundleBuildManager.IsPlatformSelected(AssetBundleBuildManager
                                        .AssetBundleBuildRule.Platform.Ios)));
                            AssetBundleBuildManager.SelectPlatform(
                                AssetBundleBuildManager.AssetBundleBuildRule.Platform.Android,
                                EditorGUILayout.ToggleLeft("Android",
                                    AssetBundleBuildManager.IsPlatformSelected(AssetBundleBuildManager
                                        .AssetBundleBuildRule.Platform.Android)));
                        }
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginVertical();
                    {
                        AssetBundleBuildManager.AssetBundleRule.ZipSelected =
                            EditorGUILayout.ToggleLeft("Zip 压缩 AssetBundle",
                                AssetBundleBuildManager.AssetBundleRule.ZipSelected);

                        AssetBundleBuildManager.AssetBundleRule.ZipPassWord = EditorGUILayout.TextField("Zip PassWord",
                            AssetBundleBuildManager.AssetBundleRule.ZipPassWord);

                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.LabelField("AssetBundle Options", EditorStyles.boldLabel);
                    EditorGUILayout.BeginVertical("box");
                    {
                        AssetBundleBuildManager.AssetBundleRule.UncompressedAssetBundle = EditorGUILayout.ToggleLeft("Uncompressed AssetBundle",
                            AssetBundleBuildManager.AssetBundleRule.UncompressedAssetBundle);
                        if (AssetBundleBuildManager.AssetBundleRule.UncompressedAssetBundle)
                        {
                            AssetBundleBuildManager.AssetBundleRule.ChunkBasedCompression = false;
                        }

                        AssetBundleBuildManager.AssetBundleRule.DisableWriteTypeTree = EditorGUILayout.ToggleLeft("Disable Write TypeTree",
                            AssetBundleBuildManager.AssetBundleRule.DisableWriteTypeTree);

                        if (AssetBundleBuildManager.AssetBundleRule.DisableWriteTypeTree)
                        {
                            AssetBundleBuildManager.AssetBundleRule.IgnoreTypeTreeChanges = false;
                        }

                        AssetBundleBuildManager.AssetBundleRule.DeterministicAssetBundle =
                            EditorGUILayout.ToggleLeft("Deterministic AssetBundle",
                                AssetBundleBuildManager.AssetBundleRule.DeterministicAssetBundle);
                        AssetBundleBuildManager.AssetBundleRule.ForceRebuildAssetBundle =
                            EditorGUILayout.ToggleLeft("Force Rebuild AssetBundle",
                                AssetBundleBuildManager.AssetBundleRule.ForceRebuildAssetBundle);

                        AssetBundleBuildManager.AssetBundleRule.IgnoreTypeTreeChanges = EditorGUILayout.ToggleLeft("Ignore TypeTree Changes",
                            AssetBundleBuildManager.AssetBundleRule.IgnoreTypeTreeChanges);
                        if (AssetBundleBuildManager.AssetBundleRule.IgnoreTypeTreeChanges)
                        {
                            AssetBundleBuildManager.AssetBundleRule.DisableWriteTypeTree = false;
                        }

                        AssetBundleBuildManager.AssetBundleRule.ChunkBasedCompression = EditorGUILayout.ToggleLeft("Chunk Based Compression",
                            AssetBundleBuildManager.AssetBundleRule.ChunkBasedCompression);
                        if (AssetBundleBuildManager.AssetBundleRule.ChunkBasedCompression)
                        {
                            AssetBundleBuildManager.AssetBundleRule.UncompressedAssetBundle = false;
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal("box");
            {
                EditorGUILayout.LabelField("版本设置 :", GUILayout.Width(100));
                AssetBundleBuildManager.CurrentVersion.MasterVersion = int.Parse(EditorGUILayout.TextField("主版本号: ",AssetBundleBuildManager.CurrentVersion.MasterVersion.ToString(),GUILayout.Width(300)));
                AssetBundleBuildManager.CurrentVersion.MinorVersion = int.Parse(EditorGUILayout.TextField("次版本号: ",AssetBundleBuildManager.CurrentVersion.MinorVersion.ToString(),GUILayout.Width(300)));
                AssetBundleBuildManager.CurrentVersion.RevisedVersion = int.Parse(EditorGUILayout.TextField("修订版本号: ",AssetBundleBuildManager.CurrentVersion.RevisedVersion.ToString(),GUILayout.Width(300)));
                if (GUILayout.Button("保存版本信息", GUILayout.MinWidth(100)))
                {
                    SaveVersion();
                }
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("保存打包路径:", GUILayout.Width(100));
                    if (GUILayout.Button("Browse..."))
                    {
                        string directory = EditorUtility.OpenFolderPanel("选择文件夹", AssetBundleBuildManager.OutputDirectory, string.Empty);
                        if (!string.IsNullOrEmpty(directory))
                        {
                            AssetBundleBuildManager.OutputDirectory = directory;
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
                AssetBundleBuildManager.OutputDirectory = EditorGUILayout.TextField(AssetBundleBuildManager.OutputDirectory, GUILayout.Width(900));
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(AssetBundleBuildManager.WorkingDirectory, GUILayout.Width(600));
                    EditorGUILayout.LabelField("用于增量打包的目录", GUILayout.Width(300));
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(AssetBundleBuildManager.UpdateFullDirectory, GUILayout.Width(600));
                    EditorGUILayout.LabelField("用于上传资源服务器的目录", GUILayout.Width(300));
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(AssetBundleBuildManager.PackageDirectory, GUILayout.Width(600));
                    EditorGUILayout.LabelField("用于整包发布拷贝到StreamingAsset的目录", GUILayout.Width(300));
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("清除AssetBundleName",GUILayout.Width(320)))
                {
                    AssetBundleBuildManager.Reset();
                }
                if (GUILayout.Button("设置AssetBundleName",GUILayout.Width(320)))
                {
                    AssetBundleBuildManager.SetAllAssetBundleName();
                } 
                if (GUILayout.Button("构建AssetBundle",GUILayout.Width(320)))
                {
                    AssetBundleBuildManager.BuildAssetBundle();
                }
                if (GUILayout.Button("分析AssetBundle",GUILayout.Width(320)))
                {
                }
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5);

            int height = 0;
            height += (AssetBundleBuildManager.AssetBundleRule.AssetBundleBuildDatas.Count + 1) * 20;
            scrollPosition = GUI.BeginScrollView(new Rect(0, 30, position.width, position.height - 30), scrollPosition,
                new Rect(0, 0, 1250, height));
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("AssetPath", EditorStyles.label, GUILayout.MinWidth(200));
                GUILayout.Label("AssetBundleName", EditorStyles.label, GUILayout.MinWidth(200));
                GUILayout.Label("AssetBundleVariant", EditorStyles.label, GUILayout.MinWidth(200));
                GUILayout.Label("FileFilter", EditorStyles.label, GUILayout.MinWidth(200));
                GUILayout.Label("RuleType", EditorStyles.label, GUILayout.MinWidth(200));
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            GUIStyle style = GUI.skin.textField;
            for (int i = 0; i < AssetBundleBuildManager.AssetBundleRule.AssetBundleBuildDatas.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                AssetBundleBuildManager.AssetBundleBuildRule.AssetBundleBuildData data =
                    AssetBundleBuildManager.AssetBundleRule.AssetBundleBuildDatas[i];

                GUI.color = GetDataEquale(data, currentSelectData) ? Color.green : new Color(0.8f, 0.8f, 0.8f, 1);

                if (GUILayout.Button(data.AssetPath, style, GUILayout.MinWidth(200)))
                {
                    CurrentIndex = i;
                    SetCurrentValue(currentSelectData, data);
                }
                if (GUILayout.Button(data.AssetBundleName, style, GUILayout.MinWidth(200)))
                {
                    CurrentIndex = i;
                    SetCurrentValue(currentSelectData, data);
                }
                if (GUILayout.Button(data.FileFilter, style, GUILayout.MinWidth(200)))
                {
                    CurrentIndex = i;
                    SetCurrentValue(currentSelectData, data);
                }
                if (GUILayout.Button(data.RuleType.ToString(), style, GUILayout.MinWidth(200)))
                {
                    CurrentIndex = i;
                    SetCurrentValue(currentSelectData, data);
                }

                EditorGUILayout.EndHorizontal();
            }

            GUI.EndScrollView();
            if (EditorUtility.IsDirty(AssetBundleBuildManager.AssetBundleRule))
            {
                EditorUtility.SetDirty(AssetBundleBuildManager.AssetBundleRule);
                AssetDatabase.Refresh();
            }
        }

        private void SetCurrentValue(AssetBundleBuildManager.AssetBundleBuildRule.AssetBundleBuildData data1,
            AssetBundleBuildManager.AssetBundleBuildRule.AssetBundleBuildData data2)
        {
            data1.AssetPath = data2.AssetPath;
            data1.AssetBundleName = data2.AssetBundleName;
            data1.FileFilter = data2.FileFilter;
            data1.RuleType = data2.RuleType;
        }

        private bool GetDataEquale(AssetBundleBuildManager.AssetBundleBuildRule.AssetBundleBuildData data1,
            AssetBundleBuildManager.AssetBundleBuildRule.AssetBundleBuildData data2)
        {
            if (data1.AssetPath == data2.AssetPath
                && data1.AssetBundleName == data2.AssetBundleName
                && data1.FileFilter == data2.FileFilter
                && data1.RuleType == data2.RuleType
            )
            {
                return true;
            }
            return false;
        }
        
        private static void GetVersion()
        {

            if (System.IO.File.Exists(Application.dataPath + "/" + AssetBundleBuildManager.VersionFile))
            {
                AssetBundleBuildManager.CurrentVersion = JsonUtility.FromJson<Version>(
                    System.IO.File.ReadAllText(Application.dataPath + "/" + AssetBundleBuildManager.VersionFile));
            }
            else
            {
                AssetBundleBuildManager.CurrentVersion = new Version("0.0.1");
            }
          
        }

        private static void SaveVersion()
        {
            System.IO.File.WriteAllText(Application.dataPath + "/" + AssetBundleBuildManager.VersionFile,JsonUtility.ToJson(AssetBundleBuildManager.CurrentVersion));
        }
    }
}