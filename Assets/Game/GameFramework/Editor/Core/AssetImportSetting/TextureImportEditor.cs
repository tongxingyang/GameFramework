using System;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace GameFramework.Editor.Core.AssetImportSetting
{
    public class TextureImportEditor : EditorWindow
    {
        [MenuItem("Tools/导入设置/Texture 导入设置")]
        public static void ShowTextureImportWditor()
        {
            TextureImportEditor textureImportEditor = GetWindow<TextureImportEditor>();
            textureImportEditor.Show();
            textureImportEditor.position = new Rect(100, 100, 1300, 400);
            textureImportEditor.minSize = new Vector2(1300, 600);
        }
        private TextureImporterType textureType = TextureImporterType.Default;
        private TextureImporterAlphaSource alphaSourceType = TextureImporterAlphaSource.FromInput;
        private TextureImporterFormat androidFormatType = TextureImporterFormat.ETC2_RGB4;
        private TextureImporterFormat iPhoneFormatType = TextureImporterFormat.PVRTC_RGB2;
        private Vector2 scrollPosition;
        private TextureImportManager.TextureImportRule.TextureImportData currenTextureImportData =
            new TextureImportManager.TextureImportRule.TextureImportData()
            {
                AssetPath = String.Empty,
                FileFilter = String.Empty
            };

        private int currentSelectIndex;
        private TextureImportManager.TextureImportRule.TextureImportData GetNextTextureImportData()
        {
            TextureImportManager.TextureImportRule.TextureImportData data =
                new TextureImportManager.TextureImportRule.TextureImportData
                {
                    Index = TextureImportManager.Instance.ImportRule.GetNextIndex()
                };
            return data;
        }

        private void GetSelectIndexDataInfo(TextureImportManager.TextureImportRule.TextureImportData data)
        {
            if (data == null) return;
            currenTextureImportData = data;
        }

        private void SetSelectIndexDataInfo()
        {
            TextureImportManager.TextureImportRule.TextureImportData data =
                TextureImportManager.Instance.ImportRule.GetRule(currentSelectIndex);
            if (data != null) 
                data = currenTextureImportData;
        }

        private void SetTextureTypeInfo()
        {
            currenTextureImportData.TextureImporterType = textureType;
        }

        private void SetAlphaSourceInfo()
        {
            currenTextureImportData.AlphaSource = alphaSourceType;
        }

        private void SetAndroidFormatInfo()
        {
            currenTextureImportData.AndroidImporterFormat = androidFormatType;
        }

        private void SetiPhoneFormatInfo()
        {
            currenTextureImportData.IphoneImporterFormat = iPhoneFormatType;
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Path:");
                currenTextureImportData.AssetPath = EditorGUILayout.TextField(currenTextureImportData.AssetPath);
                EditorGUILayout.LabelField("NameFilter:", GUILayout.MinWidth(200));
                currenTextureImportData.FileFilter = EditorGUILayout.TextField(currenTextureImportData.FileFilter);
                if (GUILayout.Button("Save", GUILayout.MinWidth(100)))
                {
                    SetSelectIndexDataInfo();
                }
                if (GUILayout.Button("Delete", GUILayout.MinWidth(100)))
                {
                    TextureImportManager.Instance.ImportRule.Delete(currentSelectIndex);
                }
                if (GUILayout.Button("New Data", GUILayout.MinWidth(100)))
                {
                    if (string.IsNullOrEmpty(currenTextureImportData.AssetPath) ||
                        string.IsNullOrEmpty(currenTextureImportData.FileFilter))
                    {
                        return;
                    }
                    TextureImportManager.TextureImportRule.TextureImportData data = GetNextTextureImportData();
                    data.AssetPath = currenTextureImportData.AssetPath;
                    data.FileFilter = currenTextureImportData.FileFilter;
                    TextureImportManager.Instance.ImportRule.Add(data);
                    currenTextureImportData = data;
                    currentSelectIndex = data.Index;
                    GetSelectIndexDataInfo(data);
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            {
                textureType = (TextureImporterType)EditorGUILayout.EnumPopup("Texture类型", textureType);
                SetTextureTypeInfo();
                alphaSourceType = (TextureImporterAlphaSource)EditorGUILayout.EnumPopup("AlphaSource", alphaSourceType);
                SetAlphaSourceInfo();
                androidFormatType = (TextureImporterFormat)EditorGUILayout.EnumPopup("Android", androidFormatType);
                SetAndroidFormatInfo();
                iPhoneFormatType = (TextureImporterFormat)EditorGUILayout.EnumPopup("iPhone", iPhoneFormatType);
                SetiPhoneFormatInfo();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            {
                currenTextureImportData.IsMinMap =
                    EditorGUILayout.ToggleLeft("是否开启Mipmap", currenTextureImportData.IsMinMap);
                currenTextureImportData.IsReadWriteEnable =
                    EditorGUILayout.ToggleLeft("是否开启ReadWrite", currenTextureImportData.IsReadWriteEnable);
                currenTextureImportData.Index = EditorGUILayout.IntField("Rule Index", currenTextureImportData.Index);
                currenTextureImportData.MaxTextureSize =
                    EditorGUILayout.IntField("纹理MaxSize", currenTextureImportData.MaxTextureSize);
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(25);
            int height = (TextureImportManager.Instance.ImportRule.TextureImportDatas.Count + 1) * 20;
            TextureImportManager.TextureImportRule.TextureImportData rule =
                TextureImportManager.Instance.ImportRule.GetRule(currentSelectIndex);
            string[] guids = null;
            if (null != rule)
            {
                guids = AssetDatabase.FindAssets("t:Texture", new string[] {rule.AssetPath});
                height += (guids.Length + 1) * 20;
            }

            scrollPosition = GUI.BeginScrollView(new Rect(0, 30, position.width, position.height - 30), scrollPosition,
                new Rect(0, 0, 1250, height));
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("AssetPath", EditorStyles.label, GUILayout.MinWidth(100));
                GUILayout.Label("FileFilter", EditorStyles.label, GUILayout.MinWidth(100));
                GUILayout.Label("Index", EditorStyles.label, GUILayout.MinWidth(100));
                GUILayout.Label("AlphaSource", EditorStyles.label, GUILayout.MinWidth(100));
                GUILayout.Label("TextureType", EditorStyles.label, GUILayout.MinWidth(100));
                GUILayout.Label("Mipmap", EditorStyles.label, GUILayout.MinWidth(100));
                GUILayout.Label("R/W", EditorStyles.label, GUILayout.MinWidth(100));
                GUILayout.Label("MaxSize", EditorStyles.label, GUILayout.MinWidth(100));
                GUILayout.Label("Android", EditorStyles.label, GUILayout.MinWidth(100));
                GUILayout.Label("iPhone", EditorStyles.label, GUILayout.MinWidth(100));
                GUILayout.Label("Apply", EditorStyles.label, GUILayout.MinWidth(100));
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            GUIStyle style = GUI.skin.textField;
            for (int i = 0; i < TextureImportManager.Instance.ImportRule.TextureImportDatas.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                TextureImportManager.TextureImportRule.TextureImportData data =
                    TextureImportManager.Instance.ImportRule.TextureImportDatas[i];

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
                if (GUILayout.Button(data.AlphaSource.ToString(), style, GUILayout.MinWidth(100)))
                {
                    currentSelectIndex = data.Index;
                    GetSelectIndexDataInfo(data);
                }
                if (GUILayout.Button(data.TextureImporterType.ToString(), style, GUILayout.MinWidth(100)))
                {
                    currentSelectIndex = data.Index;
                    GetSelectIndexDataInfo(data);
                }
                if (GUILayout.Button(data.IsMinMap.ToString(), style, GUILayout.MinWidth(100)))
                {
                    currentSelectIndex = data.Index;
                    GetSelectIndexDataInfo(data);
                }
                if (GUILayout.Button(data.IsReadWriteEnable.ToString(), style, GUILayout.MinWidth(100)))
                {
                    currentSelectIndex = data.Index;
                    GetSelectIndexDataInfo(data);
                }
                if (GUILayout.Button(data.MaxTextureSize.ToString(), style, GUILayout.MinWidth(100)))
                {
                    currentSelectIndex = data.Index;
                    GetSelectIndexDataInfo(data);
                }
                if (GUILayout.Button(data.AndroidImporterFormat.ToString(), style, GUILayout.MinWidth(100)))
                {
                    currentSelectIndex = data.Index;
                    GetSelectIndexDataInfo(data);
                }
                if (GUILayout.Button(data.IphoneImporterFormat.ToString(), style, GUILayout.MinWidth(100)))
                {
                    currentSelectIndex = data.Index;
                    GetSelectIndexDataInfo(data);
                }
                if (GUILayout.Button("Apply", GUILayout.MinWidth(100)))
                {
                    TextureImportManager.ReImportTextures(data);
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
                    if (!dir.Equals(currenTextureImportData.AssetPath))
                    {
                        continue;
                    }
                    string fileName = path.Substring(path.LastIndexOf('/') + 1);
                    if (!currenTextureImportData.IsMatch(fileName))
                    {
                        continue;
                    }
                    TextureImporter ai = AssetImporter.GetAtPath(path) as TextureImporter;
                    if (null != ai)
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            GUILayout.Label(fileName, EditorStyles.label, GUILayout.MinWidth(120));
                            GUILayout.Label("", EditorStyles.label, GUILayout.MinWidth(100));
                            GUILayout.Label((++count).ToString(), EditorStyles.label, GUILayout.MinWidth(100));
                            GUILayout.Label(ai.alphaSource.ToString(), EditorStyles.label, GUILayout.MinWidth(120));
                            GUILayout.Label(ai.textureType.ToString(), EditorStyles.label, GUILayout.MinWidth(120));
                            GUILayout.Label(ai.mipmapEnabled.ToString(), EditorStyles.label, GUILayout.MinWidth(100));
                            GUILayout.Label(ai.isReadable.ToString(), EditorStyles.label, GUILayout.MinWidth(100));
                            TextureImporterPlatformSettings settingAndroid = ai.GetPlatformTextureSettings("Android");
                            TextureImporterPlatformSettings settingiPhone = ai.GetPlatformTextureSettings("iPhone");
                            GUILayout.Label(settingAndroid.maxTextureSize.ToString(), EditorStyles.label,
                                GUILayout.MinWidth(100));
                            GUILayout.Label(settingAndroid.format.ToString(), EditorStyles.label,
                                GUILayout.MinWidth(100));
                            GUILayout.Label(settingiPhone.format.ToString(), EditorStyles.label,
                                GUILayout.MinWidth(100));
                            GUILayout.Label("", EditorStyles.label, GUILayout.MinWidth(100));
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
            GUI.EndScrollView();
            if (EditorUtility.IsDirty(TextureImportManager.Instance.ImportRule))
            {
                EditorUtility.SetDirty(TextureImportManager.Instance.ImportRule);
                AssetDatabase.Refresh();
            }
        }
    }
}