using System;
using System.Collections.Generic;
using System.IO;
using GameFramework.Utility.PathUtility;
using UnityEditor;
using UnityEngine;

namespace Game.GameFramework.Editor.UIEditor
{
    public class UIEmojiTextEditor : EditorWindow
    {
        struct UIEmojiInfo
        {
            public float x;
            public float y;
            public float size;
        }
        
        [MenuItem("Tools/UITools/Emoji Maker")]
        static void CreateWizard()
        {
            UIEmojiTextEditor window = GetWindow<UIEmojiTextEditor>();
            window.ShowUtility();
            window.maxSize = new Vector2(700, 300);
            
        }
        private Vector2 texSize = Vector2.zero;
        private string inDirectory = String.Empty;
        private string outDirectory = String.Empty;
        private int uiEmojiSize = 64;
        
        private readonly Vector2[] AtlasSize = new Vector2[]{
            new Vector2(32,32),
            new Vector2(64,64),
            new Vector2(128,128),
            new Vector2(256,256),
            new Vector2(512,512),
            new Vector2(1024,1024),
            new Vector2(2048,2048)
        };
        
        private void OnGUI()
        {
            EditorGUILayout.LabelField("生成 UIEmoji 资源",EditorStyles.boldLabel);
            GUILayout.Space(10);
            uiEmojiSize = EditorGUILayout.IntField(
                "UIEmoji 图片的尺寸", uiEmojiSize,
                GUILayout.Width(600));
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("原始Emoji图片的目录:", GUILayout.Width(300));
                if (GUILayout.Button("Browse...",GUILayout.Width(300)))
                {
                    string directory = EditorUtility.OpenFolderPanel("选择文件夹", inDirectory, string.Empty);
                    if (!string.IsNullOrEmpty(directory))
                    {
                        inDirectory = directory;
                        string[] files = Directory.GetFiles(inDirectory,"*.png");
                        texSize = ComputeAtlasSize(files.Length);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            if (texSize != Vector2.zero)
            {
                EditorGUILayout.LabelField("生成UIEmoji图集的尺寸  x : "+texSize.x+"  y : "+texSize.y);
                EditorGUILayout.Space();
            }
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("输出Emoji资源的目录:", GUILayout.Width(300));
                if (GUILayout.Button("Browse...",GUILayout.Width(300)))
                {
                    string directory = EditorUtility.OpenFolderPanel("选择文件夹", outDirectory, string.Empty);
                    if (!string.IsNullOrEmpty(directory))
                    {
                        outDirectory = directory;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            if (GUILayout.Button("开始生成Emoji资源",GUILayout.Width(690)))
            {
                if (inDirectory == String.Empty || outDirectory == String.Empty || inDirectory == outDirectory)
                {
                    EditorUtility.DisplayDialog ("错误", "路径选择出错 请检查!", "OK");
                    return;
                }
                
                Dictionary<string,int> sourceDic = new Dictionary<string,int> ();
                string[] files = Directory.GetFiles(inDirectory,"*.png");
                foreach (string file in files)
                {
                    string[] paths = file.Split ('/');
                    string[] fileNames = paths [paths.Length - 1].Split ('.');
                    string fileName = fileNames [0];
                    string[] strs = fileName.Split('_');
                    string emojiID = strs[0];
                    if (sourceDic.ContainsKey(emojiID)) {
                        sourceDic[emojiID]++;
                    } else {
                        sourceDic.Add(emojiID, 1);
                    }
                }

                if (Directory.Exists(outDirectory))
                {
                    Directory.Delete(outDirectory,true);
                }
                Directory.CreateDirectory (outDirectory);
                Dictionary<string,UIEmojiInfo> emojiDic = new Dictionary<string, UIEmojiInfo> ();
                int totalFrames = 0;
                foreach (int value in sourceDic.Values) {
                    totalFrames += value;
                }
                texSize = ComputeAtlasSize (totalFrames);
                Texture2D newTex = new Texture2D ((int)texSize.x, (int)texSize.y, TextureFormat.ARGB32, false);
                Texture2D dataTex = new Texture2D ((int)texSize.x / uiEmojiSize, (int)texSize.y / uiEmojiSize, TextureFormat.ARGB32, false);
                int x = 0;
                int y = 0;
                foreach (string key in sourceDic.Keys) 
                {
                    for (int index = 0; index < sourceDic[key]; index++)
                    {
                        string path;
                        if (sourceDic[key] == 1) {
                            path = PathUtility.GetCombinePath(inDirectory , key);
                            path += ".png";
                        } else
                        {
                            path = PathUtility.GetCombinePath(inDirectory, key);
                            path += "_" + (index + 1).ToString() + ".png";
                        }
                        path = path.Substring(path.IndexOf("Assets", StringComparison.Ordinal));
                        Texture2D asset = AssetDatabase.LoadAssetAtPath<Texture2D> (path);
                        Color[] colors = asset.GetPixels (0); 
        
                        for (int i = 0; i < uiEmojiSize; i++) {
                            for (int j = 0; j < uiEmojiSize; j++) {
                                newTex.SetPixel (x + i, y + j, colors [i + j * uiEmojiSize]);
                            }
                        }
        
                        string t = Convert.ToString (sourceDic [key] - 1, 2);
                        float r = 0, g = 0, b = 0;
                        if (t.Length >= 3) {
                            r = t [2] == '1' ? 0.5f : 0;
                            g = t [1] == '1' ? 0.5f : 0;
                            b = t [0] == '1' ? 0.5f : 0;
                        } else if (t.Length >= 2) {
                            r = t [1] == '1' ? 0.5f : 0;
                            g = t [0] == '1' ? 0.5f : 0;
                        } else {
                            r = t [0] == '1' ? 0.5f : 0;
                        }
                        dataTex.SetPixel (x / uiEmojiSize, y / uiEmojiSize, new Color (r, g, b, 1));
                        if (!emojiDic.ContainsKey (key)) {
                            UIEmojiInfo info;
                            info.x = (x * 1.0f / texSize.x);
                            info.y = (y * 1.0f / texSize.y);
                            info.size = (uiEmojiSize * 1.0f / texSize.x);
                            emojiDic.Add (key, info);
                        }
                        x += uiEmojiSize;
                        if (x >= texSize.x) {
                            x = 0;
                            y += uiEmojiSize;
                        }
                    }
                }
                
                byte[] newTexBytes = newTex.EncodeToPNG ();
                string outputTex = PathUtility.GetCombinePath(outDirectory, "emoji_tex.png");
                File.WriteAllBytes (outputTex, newTexBytes);
                byte[] newDataBytes = dataTex.EncodeToPNG ();
                string outputData = PathUtility.GetCombinePath(outDirectory, "emoji_data.png");
                File.WriteAllBytes (outputData, newDataBytes);
                
                using (StreamWriter sw = new StreamWriter (PathUtility.GetCombinePath(outDirectory, "emoji.txt"),false)) {
                    sw.WriteLine ("Name\tX\tY\tSize");
                    foreach (string key in emojiDic.Keys) {
                        sw.WriteLine (key + "\t" + emojiDic[key].x + "\t" + emojiDic[key].y + "\t" + emojiDic[key].size);
                    }
                    sw.Close ();
                }

                AssetDatabase.Refresh ();
                FormatTexture ();
                EditorUtility.DisplayDialog ("Success", "生成 Emoji 资源成功!", "OK");
            }
        }
        
        private Vector2 ComputeAtlasSize(int count)
        {
            long total = count * uiEmojiSize * uiEmojiSize;
            for (int i = 0; i < AtlasSize.Length; i++) {
                if (total <= AtlasSize [i].x * AtlasSize [i].y) {
                    return AtlasSize [i];
                }
            }
            return Vector2.zero;
        }
        
        private void FormatTexture() {
            TextureImporter emojiTex = AssetImporter.GetAtPath (PathUtility.GetCombinePath(outDirectory,"emoji_tex.png")) as TextureImporter;
            if (emojiTex != null)
            {
                emojiTex.filterMode = FilterMode.Point;
                emojiTex.mipmapEnabled = false;
                emojiTex.sRGBTexture = true;
                emojiTex.alphaSource = TextureImporterAlphaSource.FromInput;
                emojiTex.textureCompression = TextureImporterCompression.Uncompressed;
                emojiTex.SaveAndReimport();
            }

            TextureImporter emojiData = AssetImporter.GetAtPath (PathUtility.GetCombinePath(outDirectory,"emoji_data.png")) as TextureImporter;
            if (emojiData != null)
            {
                emojiData.filterMode = FilterMode.Point;
                emojiData.mipmapEnabled = false;
                emojiData.sRGBTexture = false;
                emojiData.alphaSource = TextureImporterAlphaSource.None;
                emojiData.textureCompression = TextureImporterCompression.Uncompressed;
                emojiData.SaveAndReimport ();
            }
        }
    }
}