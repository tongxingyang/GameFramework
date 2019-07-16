using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

namespace GameFramework.Editor.Core.AssetImportSetting
{
    public class TextureCheckTool
    {
        public struct TextureData
        {
            public long Size;
            public string Message;
        }

        private static List<TextureData> GetCheckTextureDatas(string dir, out int totalCount, out long totalMemory)
        {
            List<TextureData> outPuts = new List<TextureData>();
            string[] guids = AssetDatabase.FindAssets("t:Texture", new string[] {dir});
            totalCount = guids.Length;
            totalMemory = 0;
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < guids.Length; i++)
            {
                stringBuilder.Clear();
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                if (string.IsNullOrEmpty(path)) continue;
                EditorUtility.DisplayProgressBar("Check Texture Format", path, (float) (i + 1) / guids.Length);
                Object obj = AssetDatabase.LoadAssetAtPath<Texture>(path);
                if (obj != null)
                {
                    Texture texture = (Texture) obj;
                    TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                    if (textureImporter != null)
                    {
                        long size = Profiler.GetRuntimeMemorySizeLong(obj);
                        totalMemory += size;
                        stringBuilder.Append("RuntimeMemorySize大小 : " + ConvertToString(size) + "\t");
                        stringBuilder.Append("长宽 : " + texture.width + "*" + texture.height + "\t");
                        stringBuilder.Append("是否开启mipmap : " + textureImporter.mipmapEnabled + "\t");
                        stringBuilder.Append("是否开启读写 : " + textureImporter.isReadable + "\t");
                        stringBuilder.Append("texture type:" + textureImporter.textureType + "\t");
                        stringBuilder.Append("alpha source:" + textureImporter.alphaSource + "\t");
                        TextureImporterPlatformSettings settingAndroid =
                            textureImporter.GetPlatformTextureSettings("Android");
                        stringBuilder.Append("android :" + settingAndroid.format + "\t");
                        TextureImporterPlatformSettings settingIos =
                            textureImporter.GetPlatformTextureSettings("iPhone");
                        stringBuilder.Append("ios :" + settingIos.format + "\t");
                        outPuts.Add(new TextureData() {Message = stringBuilder.ToString(), Size = size});
                    }
                }
            }
            outPuts.Sort(
                (a, b) => b.Size.CompareTo(a.Size)
            );
            EditorUtility.ClearProgressBar();
            return outPuts;
        }

        public static string ConvertToString(long size)
        {
            string output = "";
            float fSize = 0;
            if (size >= 1024 * 1024 * 1024)
            {
                fSize = size / (1024.0f * 1024.0f * 1024.0f);
                output = fSize.ToString("0.0");
                output += "GB";
            }
            else if (size >= 1024 * 1024)
            {
                fSize = size / (1024.0f * 1024.0f);
                output = fSize.ToString("0.00");
                output += "MB";
            }
            else
            {
                fSize = size / (1024.0f);
                output = fSize.ToString("0.00");
                output += "KB";
            }
            return output;
        }

        [MenuItem("Tools/AssetCheck/Texture检测信息", true)]
        private static bool CheckTextureFormatInfo()
        {
            if (Selection.assetGUIDs.Length == 1)
            {
                string path = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
                if (AssetDatabase.IsValidFolder(path))
                    return true;
            }
            return false;
        }

        [MenuItem("Tools/AssetCheck/Texture检测信息", false)]
        private static void CheckTextureFormatManual()
        {
            string path = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
            int totalCount;
            long totalMem;
            List<TextureData> outputs = GetCheckTextureDatas(path, out totalCount, out totalMem);

            for (int i = 0; i < outputs.Count; i++)
            {
                UnityEngine.Debug.Log(outputs[i].Message);
            }
            outputs.Clear();
        }
    }
}