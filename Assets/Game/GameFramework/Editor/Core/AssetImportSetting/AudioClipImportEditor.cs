using System;
using UnityEditor;
using UnityEngine;

namespace GameFramework.Editor.Core.AssetImportSetting
{
    public class AudioClipImportEditor : EditorWindow
    {
        [MenuItem("Tools/导入设置/AudioClip 导入设置")]
        public static void ShowAudioClipImportEditor()
        {
            AudioClipImportEditor AudioClipImportEditor = GetWindow<AudioClipImportEditor>();
            AudioClipImportEditor.Show();
            AudioClipImportEditor.position = new Rect(100, 100, 1300, 400);
            AudioClipImportEditor.minSize = new Vector2(1300, 600);
        }

        private AudioCompressionFormat androidClipType = AudioCompressionFormat.MP3;
        private AudioCompressionFormat iPhoneClipType = AudioCompressionFormat.MP3;
        private AudioClipLoadType androidLoadType = AudioClipLoadType.CompressedInMemory;
        private AudioClipLoadType iPhoneLoadType = AudioClipLoadType.CompressedInMemory;
        private Vector2 scrollPosition;
        private AudioClipImportManager.AudioClipImportRule.AudioClipImportData currenAudioClipImportData =
            new AudioClipImportManager.AudioClipImportRule.AudioClipImportData()
            {
                AssetPath = String.Empty,
                FileFilter = String.Empty
            };

        private int currentSelectIndex;

        private AudioClipImportManager.AudioClipImportRule.AudioClipImportData GetNextAudioClipImportData()
        {
            AudioClipImportManager.AudioClipImportRule.AudioClipImportData data =
                new AudioClipImportManager.AudioClipImportRule.AudioClipImportData
                {
                    Index = AudioClipImportManager.ImportRule.GetNextIndex()
                };
            return data;
        }

        private void GetSelectIndexDataInfo(AudioClipImportManager.AudioClipImportRule.AudioClipImportData data)
        {
            if (data == null) return;
            currenAudioClipImportData = data;
        }

        private void SetSelectIndexDataInfo()
        {
            AudioClipImportManager.AudioClipImportRule.AudioClipImportData data =
                AudioClipImportManager.ImportRule.GetRule(currentSelectIndex);
            if (data != null)
                data = currenAudioClipImportData;
        }

        private void SetAndroidFormatInfo()
        {
            currenAudioClipImportData.AndroidAudioClipLoadType = androidLoadType;
            currenAudioClipImportData.AndroidCompressionFormat = androidClipType;
        }

        private void SetiPhoneFormatInfo()
        {
            currenAudioClipImportData.iPhoneClipLoadType = iPhoneLoadType;
            currenAudioClipImportData.iPhoneCompressionFormat = iPhoneClipType;
        }
        private GUIStyle style = new GUIStyle();
        private void OnGUI()
        {
            style.normal.textColor = Color.red;
            GUILayout.Label("建议 小于200k : "+AudioClipLoadType.DecompressOnLoad+" 大于200k小于1M : "+AudioClipLoadType.CompressedInMemory+" 大于1M : "+AudioClipLoadType.Streaming,style);
            style.normal.textColor = Color.black;
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Path:");
                currenAudioClipImportData.AssetPath = EditorGUILayout.TextField(currenAudioClipImportData.AssetPath);
                EditorGUILayout.LabelField("NameFilter:", GUILayout.MinWidth(200));
                currenAudioClipImportData.FileFilter = EditorGUILayout.TextField(currenAudioClipImportData.FileFilter);
                if (GUILayout.Button("Save", GUILayout.MinWidth(100)))
                {
                    SetSelectIndexDataInfo();
                }
                if (GUILayout.Button("Delete", GUILayout.MinWidth(100)))
                {
                    AudioClipImportManager.ImportRule.Delete(currentSelectIndex);
                }
                if (GUILayout.Button("New Data", GUILayout.MinWidth(100)))
                {
                    if (string.IsNullOrEmpty(currenAudioClipImportData.AssetPath) ||
                        string.IsNullOrEmpty(currenAudioClipImportData.FileFilter))
                    {
                        return;
                    }
                    AudioClipImportManager.AudioClipImportRule.AudioClipImportData data = GetNextAudioClipImportData();
                    data.AssetPath = currenAudioClipImportData.AssetPath;
                    data.FileFilter = currenAudioClipImportData.FileFilter;
                    AudioClipImportManager.ImportRule.Add(data);
                    currenAudioClipImportData = data;
                    currentSelectIndex = data.Index;
                    GetSelectIndexDataInfo(data);
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            {
                androidClipType =
                    (AudioCompressionFormat) EditorGUILayout.EnumPopup("Android Compression类型", androidClipType);
                androidLoadType = (AudioClipLoadType) EditorGUILayout.EnumPopup("Android Load类型", androidLoadType);
                SetAndroidFormatInfo();
                iPhoneClipType =
                    (AudioCompressionFormat) EditorGUILayout.EnumPopup("iPhone Compression类型", iPhoneClipType);
                iPhoneLoadType = (AudioClipLoadType) EditorGUILayout.EnumPopup("iPhone Load类型", iPhoneLoadType);
                SetiPhoneFormatInfo();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            {
                currenAudioClipImportData.IsForceToMono =
                    EditorGUILayout.ToggleLeft("是否开启ForceToMono", currenAudioClipImportData.IsForceToMono);
                currenAudioClipImportData.IsLoadInBackground =
                    EditorGUILayout.ToggleLeft("是否开启LoadInBackground", currenAudioClipImportData.IsLoadInBackground);
                currenAudioClipImportData.IsAmbisonic =
                    EditorGUILayout.ToggleLeft("是否开启Ambisonic", currenAudioClipImportData.IsAmbisonic);
                currenAudioClipImportData.IsPreloadAudioClip =
                    EditorGUILayout.ToggleLeft("是否开启PreloadAudioClip", currenAudioClipImportData.IsPreloadAudioClip);
                currenAudioClipImportData.Index =
                    EditorGUILayout.IntField("Rule Index", currenAudioClipImportData.Index);
                currenAudioClipImportData.Quality =
                    EditorGUILayout.IntField("Quality", currenAudioClipImportData.Quality);
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(25);
            int height = (AudioClipImportManager.ImportRule.AudioClipImportDatas.Count + 1) * 20;
            AudioClipImportManager.AudioClipImportRule.AudioClipImportData rule =
                AudioClipImportManager.ImportRule.GetRule(currentSelectIndex);
            string[] guids = null;
            if (null != rule)
            {
                guids = AssetDatabase.FindAssets("t:AudioClip", new string[] {rule.AssetPath});
                height += (guids.Length + 1) * 20;
            }

            scrollPosition = GUI.BeginScrollView(new Rect(0, 30, position.width, position.height - 30), scrollPosition,
                new Rect(0, 0, 1250, height));
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("AssetPath", EditorStyles.label, GUILayout.MinWidth(100));
                GUILayout.Label("FileFilter", EditorStyles.label, GUILayout.MinWidth(100));
                GUILayout.Label("Index", EditorStyles.label, GUILayout.MinWidth(100));
                GUILayout.Label("ForceToMono", EditorStyles.label, GUILayout.MinWidth(100));
                GUILayout.Label("LoadInBackground", EditorStyles.label, GUILayout.MinWidth(100));
                GUILayout.Label("Ambisonic", EditorStyles.label, GUILayout.MinWidth(100));
                GUILayout.Label("PreloadAudioClip", EditorStyles.label, GUILayout.MinWidth(100));
                GUILayout.Label("Quality", EditorStyles.label, GUILayout.MinWidth(100));
                GUILayout.Label("AndroidFormat", EditorStyles.label, GUILayout.MinWidth(100));
                GUILayout.Label("AndroidLoadType", EditorStyles.label, GUILayout.MinWidth(100));
                GUILayout.Label("iPhoneFormat", EditorStyles.label, GUILayout.MinWidth(100));
                GUILayout.Label("iPhoneLoadType", EditorStyles.label, GUILayout.MinWidth(100));
                GUILayout.Label("Apply", EditorStyles.label, GUILayout.MinWidth(100));
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            style = GUI.skin.textField;
            for (int i = 0; i < AudioClipImportManager.ImportRule.AudioClipImportDatas.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                AudioClipImportManager.AudioClipImportRule.AudioClipImportData data =
                    AudioClipImportManager.ImportRule.AudioClipImportDatas[i];

                GUI.color = data.Index == currentSelectIndex ? Color.green : new Color(0.8f, 0.8f, 0.8f, 1);

                if (GUILayout.Button(data.AssetPath, style, GUILayout.MinWidth(100)))
                {
                    currentSelectIndex = data.Index;
                    GetSelectIndexDataInfo(data);
                }
                if (GUILayout.Button(data.FileFilter, style, GUILayout.MinWidth(100)))
                {
                    currentSelectIndex = data.Index;
                    GetSelectIndexDataInfo(data);
                }
                if (GUILayout.Button(data.Index.ToString(), style, GUILayout.MinWidth(100)))
                {
                    currentSelectIndex = data.Index;
                    GetSelectIndexDataInfo(data);
                }
                if (GUILayout.Button(data.IsForceToMono.ToString(), style, GUILayout.MinWidth(100)))
                {
                    currentSelectIndex = data.Index;
                    GetSelectIndexDataInfo(data);
                }
                if (GUILayout.Button(data.IsLoadInBackground.ToString(), style, GUILayout.MinWidth(100)))
                {
                    currentSelectIndex = data.Index;
                    GetSelectIndexDataInfo(data);
                }
                if (GUILayout.Button(data.IsAmbisonic.ToString(), style, GUILayout.MinWidth(100)))
                {
                    currentSelectIndex = data.Index;
                    GetSelectIndexDataInfo(data);
                }
                if (GUILayout.Button(data.IsPreloadAudioClip.ToString(), style, GUILayout.MinWidth(100)))
                {
                    currentSelectIndex = data.Index;
                    GetSelectIndexDataInfo(data);
                }
                if (GUILayout.Button(data.Quality.ToString(), style, GUILayout.MinWidth(100)))
                {
                    currentSelectIndex = data.Index;
                    GetSelectIndexDataInfo(data);
                }
                if (GUILayout.Button(data.AndroidCompressionFormat.ToString(), style, GUILayout.MinWidth(100)))
                {
                    currentSelectIndex = data.Index;
                    GetSelectIndexDataInfo(data);
                }
                if (GUILayout.Button(data.AndroidAudioClipLoadType.ToString(), style, GUILayout.MinWidth(100)))
                {
                    currentSelectIndex = data.Index;
                    GetSelectIndexDataInfo(data);
                }
                if (GUILayout.Button(data.iPhoneCompressionFormat.ToString(), style, GUILayout.MinWidth(100)))
                {
                    currentSelectIndex = data.Index;
                    GetSelectIndexDataInfo(data);
                }
                if (GUILayout.Button(data.iPhoneClipLoadType.ToString(), style, GUILayout.MinWidth(100)))
                {
                    currentSelectIndex = data.Index;
                    GetSelectIndexDataInfo(data);
                }
                if (GUILayout.Button("Apply", GUILayout.MinWidth(100)))
                {
                    AudioClipImportManager.ReImportAudioClips(data);
                }
                EditorGUILayout.EndHorizontal();
            }
            if (null != guids)
            {
                int count = 0;
                for (int i = 0; i < guids.Length; i++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                    if (string.IsNullOrEmpty(path))
                    {
                        continue;
                    }
                    string dir = path.Remove(path.LastIndexOf('/'));
                    if (!dir.Equals(currenAudioClipImportData.AssetPath))
                    {
                        continue;
                    }
                    string fileName = path.Substring(path.LastIndexOf('/') + 1);
                    if (!currenAudioClipImportData.IsMatch(fileName))
                    {
                        continue;
                    }
                    AudioImporter ai = AssetImporter.GetAtPath(path) as AudioImporter;
                    if (null != ai)
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            GUILayout.Label(fileName, EditorStyles.label, GUILayout.MinWidth(120));
                            GUILayout.Label("", EditorStyles.label, GUILayout.MinWidth(100));
                            GUILayout.Label((++count).ToString(), EditorStyles.label, GUILayout.MinWidth(100));
                            GUILayout.Label(ai.forceToMono.ToString(), EditorStyles.label, GUILayout.MinWidth(120));
                            GUILayout.Label(ai.loadInBackground.ToString(), EditorStyles.label,
                                GUILayout.MinWidth(120));
                            GUILayout.Label(ai.ambisonic.ToString(), EditorStyles.label, GUILayout.MinWidth(100));
                            GUILayout.Label(ai.preloadAudioData.ToString(), EditorStyles.label,
                                GUILayout.MinWidth(100));
                            AudioImporterSampleSettings settingAndroid = ai.GetOverrideSampleSettings("Android");
                            AudioImporterSampleSettings settingiPhone = ai.GetOverrideSampleSettings("iOS");
                            GUILayout.Label(settingAndroid.quality.ToString(), EditorStyles.label,
                                GUILayout.MinWidth(100));
                            GUILayout.Label(settingAndroid.compressionFormat.ToString(), EditorStyles.label,
                                GUILayout.MinWidth(100));
                            GUILayout.Label(settingAndroid.loadType.ToString(), EditorStyles.label,
                                GUILayout.MinWidth(100));
                            GUILayout.Label(settingiPhone.compressionFormat.ToString(), EditorStyles.label,
                                GUILayout.MinWidth(100));
                            GUILayout.Label(settingiPhone.loadType.ToString(), EditorStyles.label,
                                GUILayout.MinWidth(100));
                            GUILayout.Label("", EditorStyles.label, GUILayout.MinWidth(100));
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
            GUI.EndScrollView();
            if (EditorUtility.IsDirty(AudioClipImportManager.ImportRule))
            {
                EditorUtility.SetDirty(AudioClipImportManager.ImportRule);
                AssetDatabase.Refresh();
            }
        }
    }
}