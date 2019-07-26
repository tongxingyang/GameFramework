using System;
using GameFramework.Utility.File;
using UnityEditor;
using UnityEngine;

namespace GameFramework.Editor.Core.AssetBundle
{
    public class AssetBundleEditor : EditorWindow
    {
        [MenuItem("Tools/AssetBunle/Build AssetBundle设置")]
        public static void BuildAssetBundleEditor()
        {
            AssetBundleEditor assetBundleEditor = GetWindow<AssetBundleEditor>();
            assetBundleEditor.Show();
            assetBundleEditor.position = new Rect(100, 100, 1300, 400);
            assetBundleEditor.minSize = new Vector2(1300, 600);
        }

        private Vector2 scrollPosition;
        private int CurrentIndex = -1;
        private string scenceName = String.Empty;

        private AssetBundleBuildManager.AssetBundleBuildRule.AssetBundleBuildData currentSelectData =
            new AssetBundleBuildManager.AssetBundleBuildRule.AssetBundleBuildData();

        private void OnGUI()
        {
            EditorGUILayout.LabelField("设置场景随包发布,不需要打成ab的名字:",EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
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
    }
}