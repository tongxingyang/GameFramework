using UnityEditor;

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
        
        private static bool IsAssetNew( AssetImporter assetImporter )
        {
            string metaPath = AssetDatabase.GetTextMetaFilePathFromAssetPath( assetImporter.assetPath );
            return !System.IO.File.Exists(metaPath);
        }
    }
}