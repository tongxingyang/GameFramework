using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace GameFramework.Editor.Core.AssetImportSetting
{
    public class TextureImportManager
    {
        public string TextureImportRulePath = "Assets/TextureImportRules.asset";
        public class TextureImportRule : ScriptableObject
        {
            [Serializable]
            public class TextureImportData
            {
                public string AssetPath = String.Empty;
                public string FileFilter = String.Empty;
                public int Index = 0;
                public bool IsMinMap = false;
                public bool IsReadWriteEnable = false;
                public int MaxTextureSize = -1;
                public TextureImporterAlphaSource AlphaSource = TextureImporterAlphaSource.FromInput;
                public TextureImporterType TextureImporterType = TextureImporterType.Default;
                public TextureImporterFormat AndroidImporterFormat = TextureImporterFormat.ETC2_RGB4;
                public TextureImporterFormat IphoneImporterFormat = TextureImporterFormat.PVRTC_RGB4;
                public bool IsMatch(string name)
                {
                    return Regex.IsMatch(name, FileFilter);
                }
            }

            public List<TextureImportData> TextureImportDatas = new List<TextureImportData>();

            public void Sort()
            {
                TextureImportDatas.Sort((a, b) => a.Index.CompareTo(b.Index));
            }

            public int GetNextIndex()
            {
                int next = 0;
                for (int i = 0; i < TextureImportDatas.Count; i++)
                {
                    if (TextureImportDatas[i].Index >= next)
                    {
                        next = TextureImportDatas[i].Index + 1;
                    }
                }
                return next;
            }

            public TextureImportData GetRule(int index)
            {
                return TextureImportDatas.FirstOrDefault(t => t.Index == index);
            }

            public TextureImportData GetRule(string path, string fileName)
            {
                TextureImportData rule = null;
                for (int i = 0; i < TextureImportDatas.Count; i++)
                {
                    if (path.Equals(TextureImportDatas[i].AssetPath))
                    {
                        if (TextureImportDatas[i].IsMatch(fileName))
                        {
                            rule = TextureImportDatas[i];
                        }
                    }
                }
                return rule;
            }

            public void Delete(int index)
            {
                for (int i = TextureImportDatas.Count - 1; i >= 0; i--)
                {
                    if (TextureImportDatas[i].Index == index)
                    {
                        TextureImportDatas.RemoveAt(i);
                    }
                }
            }

            public void Add(TextureImportData data, bool isSort = false)
            {
                TextureImportDatas.Add(data);
                if (isSort)
                {
                    Sort();
                }
            }
        }

        private TextureImportRule importRule;

        public TextureImportRule ImportRule
        {
            get
            {
                if (importRule == null)
                {
                    importRule = AssetDatabase.LoadAssetAtPath<TextureImportRule>(TextureImportRulePath);
                    if (importRule == null)
                    {
                        importRule = ScriptableObject.CreateInstance<TextureImportRule>();
                        AssetDatabase.CreateAsset(importRule, TextureImportRulePath);
                        AssetDatabase.SaveAssets();
                    }
                }
                return importRule;
            }
        }

        private static TextureImportManager instance;

        public static TextureImportManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TextureImportManager();
                }
                return instance;
            }
        }

        public static void ReImportTextures(TextureImportRule.TextureImportData data)
        {
            if(data == null) return;
            string[] guids = AssetDatabase.FindAssets("t:Texture", new string[] { data.AssetPath });
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                if (string.IsNullOrEmpty(path))
                {
                    continue;
                }
                string dir = path.Remove(path.LastIndexOf('/'));
                if (!dir.Equals(data.AssetPath))
                {
                    continue;
                }
                string fileName = path.Substring(path.LastIndexOf('/') + 1);
                if (!data.IsMatch(fileName))
                {
                    continue;
                }
                TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                if (null != textureImporter)
                {
                    ApplyRulesToTexture(textureImporter, data);
                }
            }
        }

        public static void TextureImport(TextureImporter textureImporter)
        {
            if(null == textureImporter) return;
            string dir = textureImporter.assetPath.Remove(textureImporter.assetPath.LastIndexOf('/'));
            string name = textureImporter.assetPath.Substring(textureImporter.assetPath.LastIndexOf('/') + 1);
            TextureImportRule.TextureImportData data = Instance.ImportRule.GetRule(dir, name);
            if (data != null)
            {
                ApplyRulesToTexture(textureImporter, data);
            }
        }
        
        public static void ApplyRulesToTexture(TextureImporter textureImporter, TextureImportRule.TextureImportData data)
        {
            if (null == textureImporter) return;
            if (textureImporter.textureType != data.TextureImporterType)
            {
                textureImporter.textureType = data.TextureImporterType;
            }
            textureImporter.isReadable = data.IsReadWriteEnable;
            textureImporter.mipmapEnabled = data.IsMinMap;
            if (data.MaxTextureSize > 0)
            {
                textureImporter.maxTextureSize = data.MaxTextureSize;
            }
            TextureImporterPlatformSettings settingAndroid = textureImporter.GetPlatformTextureSettings("Android");
            settingAndroid.overridden = true;
            settingAndroid.format = data.AndroidImporterFormat;
            settingAndroid.maxTextureSize = textureImporter.maxTextureSize;
            textureImporter.SetPlatformTextureSettings(settingAndroid);

            TextureImporterPlatformSettings settingIos = textureImporter.GetPlatformTextureSettings("iPhone");
            settingIos.overridden = true;
            settingIos.format = data.IphoneImporterFormat;
            settingIos.maxTextureSize = textureImporter.maxTextureSize;
            textureImporter.SetPlatformTextureSettings(settingIos);
            textureImporter.SaveAndReimport();
        }
    }
}