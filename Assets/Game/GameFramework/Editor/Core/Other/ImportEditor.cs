using UnityEditor;
using UnityEngine;

namespace GameFrame.Editor
{
    /// <summary>
    /// 自定义导入监听
    /// </summary>
    public class ImportEditor:AssetPostprocessor
    {
        public void OnPreprocessModel()
        {
            
        }
        public void OnPostprocessModel(GameObject go)
        {
            
        }
        public void OnPreprocessTexture()
        {
            Debug.Log ("OnPreProcessTexture="+this.assetPath);
            TextureImporter impor = this.assetImporter as TextureImporter;
 
        }
        public void OnPostprocessTexture(Texture2D tex)
        {
            Debug.Log ("OnPostProcessTexture="+this.assetPath);
        }
 
 
        public void OnPostprocessAudio(AudioClip clip)
        {
	
        }
        public void OnPreprocessAudio()
        {
            
        }
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            
        }
    }
}