using UnityEditor;
using UnityEngine;

namespace GameFramework.Editor.Core.AssetImportSetting
{
    public class AssetImportPostProcessor : AssetPostprocessor
    {
        private void OnPreprocessTexture()
        {
            if ( !IsAssetNew(assetImporter) )
                return;
            TextureImporter textureImporter = assetImporter as TextureImporter;
            TextureImportManager.TextureImport(textureImporter);
        } 
        private void OnPostprocessAudio(AudioClip audioClip)
        {
            if ( !IsAssetNew(assetImporter) )
                return;
            AudioImporter audioClipImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(audioClip)) as AudioImporter;
            AudioClipImportManager.AudioClipImport(audioClipImporter);
        }
        
        private static bool IsAssetNew( AssetImporter assetImporter )
        {
            string metaPath = AssetDatabase.GetTextMetaFilePathFromAssetPath( assetImporter.assetPath );
            return !System.IO.File.Exists(metaPath);
        }
    }
}