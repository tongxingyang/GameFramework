using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class BMFontTool : EditorWindow
{
    [SerializeField]
    private TextAsset fontData;
    [SerializeField]
    private Texture2D fontTexture;

    private Font targetFont;
    private string fontSavePath;
    private Material fontMaterial;
    private readonly BMFont m_bmFont = new BMFont();
    public BMFontTool()    {  }
    void OnGUI()
    {
        fontData = EditorGUILayout.ObjectField("Font Data", fontData, typeof(TextAsset), false) as TextAsset;
        fontTexture = EditorGUILayout.ObjectField("Font Texture", fontTexture, typeof(Texture2D), false) as Texture2D;

        if (GUILayout.Button("Create BMFont"))
        {

            if (null == fontData || null == fontTexture)
            {
                EditorUtility.DisplayDialog("Error", "两个数据均不能为空", "OK");
                return;
            }
            string localPath = AssetDatabase.GetAssetPath(fontData);
            string directoryName = Path.GetDirectoryName(localPath);
            string fileName = Path.GetFileNameWithoutExtension(localPath);

            string  filePath = directoryName + "/" + fileName + ".mat";
            fontMaterial = new Material(Shader.Find("UI/Default")) {mainTexture = fontTexture};
            fontSavePath = filePath;
            AssetDatabase.CreateAsset(fontMaterial, fontSavePath);

            fontMaterial = AssetDatabase.LoadAssetAtPath<Material>(fontSavePath);



            BMFontReader.Load(m_bmFont, fontData.name, fontData.bytes);
            CharacterInfo[] characterInfo = new CharacterInfo[m_bmFont.glyphs.Count];
            filePath = directoryName + "/" + fileName + ".fontsettings";
            targetFont = new Font(fileName) {material = fontMaterial};

            for (int i = 0; i < m_bmFont.glyphs.Count; i++)
            {
                BMGlyph bmInfo = m_bmFont.glyphs[i];
                CharacterInfo info = new CharacterInfo();
                info.index = bmInfo.index;

                float uvx = (float)(bmInfo.x) / m_bmFont.texWidth;
                float uvy = 1.0f * (m_bmFont.texHeight - bmInfo.y - bmInfo.height) / m_bmFont.texHeight;
                float uvw = (float)bmInfo.width / m_bmFont.texWidth;
                float uvh = 1.0f * bmInfo.height / m_bmFont.texHeight;

                info.uvBottomLeft = new Vector2(uvx, uvy);
                info.uvBottomRight = new Vector2(uvx + uvw, uvy);
                info.uvTopLeft = new Vector2(uvx, uvy + uvh);
                info.uvTopRight = new Vector2(uvx + uvw, uvy + uvh);

                info.minX = bmInfo.offsetX;
                info.minY = bmInfo.offsetY - bmInfo.height;
                info.glyphWidth = bmInfo.width;
                info.glyphHeight = bmInfo.height;

                info.advance = bmInfo.advance;

                characterInfo[i] = info;
            }
            targetFont.characterInfo = characterInfo;

            AssetDatabase.CreateAsset(targetFont, filePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    [MenuItem("Tools/UITools/BMFont Maker")]
    public static void OpenBMFontMaker()
    {
        var window = EditorWindow.GetWindow<BMFontTool>(false, "BMFont Maker", true);
        window.minSize = new Vector2(800, 600);
        window.Show();
    }
}
