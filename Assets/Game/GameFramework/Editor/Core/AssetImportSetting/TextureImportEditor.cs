using UnityEditor;
using UnityEngine;
namespace GameFramework.Editor.Core.AssetImportSetting
{
    public class TextureImportEditor:EditorWindow
    {
        [MenuItem("Tools/导入设置/Texture 导入设置")]
        public static void ShowTextureImportWditor()
        {
            TextureImportEditor textureImportEditor = GetWindow<TextureImportEditor>();
            textureImportEditor.Show();
            textureImportEditor.position = new Rect(100,100,800,400);
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
        private TextureImportManager.TextureImportRule.TextureImportData  currenTextureImportData = new TextureImportManager.TextureImportRule.TextureImportData();
        private int currentSelectIndex;
        private bool isShowRes;

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
            if(data == null) return;
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
            if(data==null) return;
            data = currenTextureImportData;
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Path:");
                currenTextureImportData.AssetPath = EditorGUILayout.TextField(currenTextureImportData.AssetPath);
                currenTextureImportData.IsRecursive = EditorGUILayout.Toggle("Recursive", currenTextureImportData.IsRecursive);
                EditorGUILayout.LabelField("NameFilter:", GUILayout.MinWidth(80));
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
                    TextureImportManager.TextureImportRule.TextureImportData data = GetNextTextureImportData();
                    TextureImportManager.Instance.ImportRule.Add(data);
                    currenTextureImportData = data;
                    GetSelectIndexDataInfo(data);
                }
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.BeginHorizontal();
                textureTypeIndex = EditorGUILayout.Popup("TextureType", textureTypeIndex, textureType);
                GUILayout.Space(100);
                alphaSrcIndex = EditorGUILayout.Popup("AlphaSource", alphaSrcIndex, alphaSrc);
                GUILayout.Space(100);
                androidFormatsIndex = EditorGUILayout.Popup("Android", androidFormatsIndex, androidFormats);
                GUILayout.Space(50);
                iphoneFormatsIndex = EditorGUILayout.Popup("iPhone", iphoneFormatsIndex, iphoneFormats);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                currenTextureImportData.IsMinMap = EditorGUILayout.ToggleLeft("Mipmap", currenTextureImportData.IsMinMap);
                currenTextureImportData.IsReadWriteEnable = EditorGUILayout.ToggleLeft("ReadWriteEnable", currenTextureImportData.IsReadWriteEnable);
                currenTextureImportData.Index = EditorGUILayout.IntField("Priority", currenTextureImportData.Index);
                GUILayout.Space(50);
                currenTextureImportData.MaxTextureSize = EditorGUILayout.IntField("MaxSize", currenTextureImportData.MaxTextureSize);
                GUILayout.EndHorizontal();

                int height = (TextureImportDataManager.Instance.DataList.Count+1) * 20;
                TextureImportData rule = TextureImportDataManager.Instance.GetRule(m_selectedIndex);
                string[] guids = null;
                if (null != rule)
                {
                    guids = AssetDatabase.FindAssets("t:Texture", new string[] { rule.AssetPath });
                    height += (guids.Length + 1) * 20;
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}