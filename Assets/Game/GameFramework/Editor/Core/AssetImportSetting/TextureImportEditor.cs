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

        private string[] textureType = new[] {"Default", "NormalMap", "Sprite", "Lightmap", "Cookie"};
        private string[] alphaSrc = new string[] {"FromInput", "None", "FromGrayScale"};

        private string[] androidFormats = new string[]
        {
            "RGB ETC 4Bits", "RGB ETC2 4Bits", "RGB1A ETC2 4Bits", "RGBA ETC2 8Bits", "RGB 16", "RGB 24", "RGBA 16",
            "RGBA 32"
        };

        private string[] iphoneFormats = new string[]
        {
            "RGB PVRTC 2Bits", "RGB PVRTC 4Bits", "RGBA PVRTC 2Bits", "RGBA PVRTC 4Bits",
            "RGB 16", "RGB 24", "RGBA 16", "RGBA 32",
            "RGB ASTC 4x4", "RGB ASTC 5x5", "RGB ASTC 6x6", "RGB ASTC 8x8", "RGB ASTC 10x10", "RGB ASTC 12x12",
            "RGBA ASTC 4x4", "RGBA ASTC 5x5", "RGBA ASTC 6x6", "RGBA ASTC 8x8", "RGBA ASTC 10x10", "RGBA ASTC 12x12",
        };

        private int textureTypeIndex = 0;
        private int alphaSrcIndex = 0;
        private int androidFormatsIndex = 0;
        private int iphoneFormatsIndex = 0;

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
            switch (data.AlphaSource)
            {
                case TextureImporterAlphaSource.FromInput:
                    alphaSrcIndex = 0;
                    break;
                case TextureImporterAlphaSource.None:
                    alphaSrcIndex = 1;
                    break;
                case TextureImporterAlphaSource.FromGrayScale:
                    alphaSrcIndex = 2;
                    break;
            }
            switch (data.TextureImporterType)
            {
                case TextureImporterType.Default:
                    textureTypeIndex = 0;
                    break;
                case TextureImporterType.NormalMap:
                    textureTypeIndex = 1;
                    break;
                case TextureImporterType.Sprite:
                    textureTypeIndex = 2;
                    break;
                case TextureImporterType.Lightmap:
                    textureTypeIndex = 3;
                    break;
                case TextureImporterType.Cookie:
                    textureTypeIndex = 4;
                    break;
            }
            switch (data.AndroidImporterFormat)
            {
                case TextureImporterFormat.ETC_RGB4:
                    androidFormatsIndex = 0;
                    break;
                case TextureImporterFormat.ETC2_RGB4:
                    androidFormatsIndex = 1;
                    break;
                case TextureImporterFormat.ETC2_RGB4_PUNCHTHROUGH_ALPHA:
                    androidFormatsIndex = 2;
                    break;
                case TextureImporterFormat.ETC2_RGBA8:
                    androidFormatsIndex = 3;
                    break;
                case TextureImporterFormat.RGB16:
                    androidFormatsIndex = 4;
                    break;
                case TextureImporterFormat.RGB24:
                    androidFormatsIndex = 5;
                    break;
                case TextureImporterFormat.RGBA16:
                    androidFormatsIndex = 6;
                    break;
                case TextureImporterFormat.RGBA32:
                    androidFormatsIndex = 7;
                    break;
            }
            switch (data.IphoneImporterFormat)
            {
                case TextureImporterFormat.PVRTC_RGB2:
                    iphoneFormatsIndex = 0;
                    break;
                case TextureImporterFormat.PVRTC_RGB4:
                    iphoneFormatsIndex = 1;
                    break;
                case TextureImporterFormat.PVRTC_RGBA2:
                    iphoneFormatsIndex = 2;
                    break;
                case TextureImporterFormat.PVRTC_RGBA4:
                    iphoneFormatsIndex = 3;
                    break;
                case TextureImporterFormat.RGB16:
                    iphoneFormatsIndex = 4;
                    break;
                case TextureImporterFormat.RGB24:
                    iphoneFormatsIndex = 5;
                    break;
                case TextureImporterFormat.RGBA16:
                    iphoneFormatsIndex = 6;
                    break;
                case TextureImporterFormat.RGBA32:
                    iphoneFormatsIndex = 7;
                    break;
                case TextureImporterFormat.ASTC_RGB_4x4:
                    iphoneFormatsIndex = 8;
                    break;
                case TextureImporterFormat.ASTC_RGB_5x5:
                    iphoneFormatsIndex = 9;
                    break;
                case TextureImporterFormat.ASTC_RGB_6x6:
                    iphoneFormatsIndex = 10;
                    break;
                case TextureImporterFormat.ASTC_RGB_8x8:
                    iphoneFormatsIndex = 11;
                    break;
                case TextureImporterFormat.ASTC_RGB_10x10:
                    iphoneFormatsIndex = 12;
                    break;
                case TextureImporterFormat.ASTC_RGB_12x12:
                    iphoneFormatsIndex = 13;
                    break;
                case TextureImporterFormat.ASTC_RGBA_4x4:
                    iphoneFormatsIndex = 14;
                    break;
                case TextureImporterFormat.ASTC_RGBA_5x5:
                    iphoneFormatsIndex = 15;
                    break;
                case TextureImporterFormat.ASTC_RGBA_6x6:
                    iphoneFormatsIndex = 16;
                    break;
                case TextureImporterFormat.ASTC_RGBA_8x8:
                    iphoneFormatsIndex = 17;
                    break;
                case TextureImporterFormat.ASTC_RGBA_10x10:
                    iphoneFormatsIndex = 18;
                    break;
                case TextureImporterFormat.ASTC_RGBA_12x12:
                    iphoneFormatsIndex = 19;
                    break;
            }
        }

        private void SetSelectIndexDataInfo()
        {
            TextureImportManager.TextureImportRule.TextureImportData data =
                TextureImportManager.Instance.ImportRule.GetRule(currentSelectIndex);
            if (data == null) return;
            data = currenTextureImportData;
        }

        private void SetTextureTypeInfo()
        {
            switch (textureTypeIndex)
            {
                case 0:
                    currenTextureImportData.TextureImporterType = TextureImporterType.Default;
                    break;
                case 1:
                    currenTextureImportData.TextureImporterType = TextureImporterType.NormalMap;
                    break;
                case 2:
                    currenTextureImportData.TextureImporterType = TextureImporterType.Sprite;
                    break;
                case 3:
                    currenTextureImportData.TextureImporterType = TextureImporterType.Lightmap;
                    break;
                case 4:
                    currenTextureImportData.TextureImporterType = TextureImporterType.Cookie;
                    break;
            }
        }

        private void SetAlphaSourceInfo()
        {
            switch (alphaSrcIndex)
            {
                case 0:
                    currenTextureImportData.AlphaSource = TextureImporterAlphaSource.FromInput;
                    break;
                case 1:
                    currenTextureImportData.AlphaSource = TextureImporterAlphaSource.None;
                    break;
                case 2:
                    currenTextureImportData.AlphaSource = TextureImporterAlphaSource.FromGrayScale;
                    break;
            }
        }

        private void SetAndroidFormatInfo()
        {
            switch (androidFormatsIndex)
            {
                case 0:
                    currenTextureImportData.AndroidImporterFormat = TextureImporterFormat.ETC_RGB4;
                    break;
                case 1:
                    currenTextureImportData.AndroidImporterFormat = TextureImporterFormat.ETC2_RGB4;
                    break;
                case 2:
                    currenTextureImportData.AndroidImporterFormat = TextureImporterFormat.ETC2_RGB4_PUNCHTHROUGH_ALPHA;
                    break;
                case 3:
                    currenTextureImportData.AndroidImporterFormat = TextureImporterFormat.ETC2_RGBA8;
                    break;
                case 4:
                    currenTextureImportData.AndroidImporterFormat = TextureImporterFormat.RGB16;
                    break;
                case 5:
                    currenTextureImportData.AndroidImporterFormat = TextureImporterFormat.RGB24;
                    break;
                case 6:
                    currenTextureImportData.AndroidImporterFormat = TextureImporterFormat.RGBA16;
                    break;
                case 7:
                    currenTextureImportData.AndroidImporterFormat = TextureImporterFormat.RGBA32;
                    break;
            }
        }

        private void SetiPhoneFormatInfo()
        {
            switch (iphoneFormatsIndex)
            {
                case 0:
                    currenTextureImportData.IphoneImporterFormat = TextureImporterFormat.PVRTC_RGB2;
                    break;
                case 1:
                    currenTextureImportData.IphoneImporterFormat = TextureImporterFormat.PVRTC_RGB4;
                    break;
                case 2:
                    currenTextureImportData.IphoneImporterFormat = TextureImporterFormat.PVRTC_RGBA2;
                    break;
                case 3:
                    currenTextureImportData.IphoneImporterFormat = TextureImporterFormat.PVRTC_RGBA4;
                    break;
                case 4:
                    currenTextureImportData.IphoneImporterFormat = TextureImporterFormat.RGB16;
                    break;
                case 5:
                    currenTextureImportData.IphoneImporterFormat = TextureImporterFormat.RGB24;
                    break;
                case 6:
                    currenTextureImportData.IphoneImporterFormat = TextureImporterFormat.RGBA16;
                    break;
                case 7:
                    currenTextureImportData.IphoneImporterFormat = TextureImporterFormat.RGBA32;
                    break;
                case 8:
                    currenTextureImportData.IphoneImporterFormat = TextureImporterFormat.ASTC_RGB_4x4;
                    break;
                case 9:
                    currenTextureImportData.IphoneImporterFormat = TextureImporterFormat.ASTC_RGB_5x5;
                    break;
                case 10:
                    currenTextureImportData.IphoneImporterFormat = TextureImporterFormat.ASTC_RGB_6x6;
                    break;
                case 11:
                    currenTextureImportData.IphoneImporterFormat = TextureImporterFormat.ASTC_RGB_8x8;
                    break;
                case 12:
                    currenTextureImportData.IphoneImporterFormat = TextureImporterFormat.ASTC_RGB_10x10;
                    break;
                case 13:
                    currenTextureImportData.IphoneImporterFormat = TextureImporterFormat.ASTC_RGB_12x12;
                    break;
                case 14:
                    currenTextureImportData.IphoneImporterFormat = TextureImporterFormat.ASTC_RGBA_4x4;
                    break;
                case 15:
                    currenTextureImportData.IphoneImporterFormat = TextureImporterFormat.ASTC_RGBA_5x5;
                    break;
                case 16:
                    currenTextureImportData.IphoneImporterFormat = TextureImporterFormat.ASTC_RGBA_6x6;
                    break;
                case 17:
                    currenTextureImportData.IphoneImporterFormat = TextureImporterFormat.ASTC_RGBA_8x8;
                    break;
                case 18:
                    currenTextureImportData.IphoneImporterFormat = TextureImporterFormat.ASTC_RGBA_10x10;
                    break;
                case 19:
                    currenTextureImportData.IphoneImporterFormat = TextureImporterFormat.ASTC_RGBA_12x12;
                    break;
            }
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
                textureTypeIndex = EditorGUILayout.Popup("Texture类型", textureTypeIndex, textureType);
                SetTextureTypeInfo();
                alphaSrcIndex = EditorGUILayout.Popup("AlphaSource", alphaSrcIndex, alphaSrc);
                SetAlphaSourceInfo();
                androidFormatsIndex = EditorGUILayout.Popup("Android", androidFormatsIndex, androidFormats);
                SetAndroidFormatInfo();
                iphoneFormatsIndex = EditorGUILayout.Popup("iPhone", iphoneFormatsIndex, iphoneFormats);
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
                currenTextureImportData.Index = EditorGUILayout.IntField("Priority", currenTextureImportData.Index);
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