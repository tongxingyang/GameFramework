namespace GameFramework.Res
{
    public static class AssetUtility
    {
        public static string GetMusicAsset(string assetName)
        {
            return Utility.StringUtility.Format("Assets/GameRes/Music/{0}.mp3", assetName);
        }

        public static string GetSoundAsset(string assetName)
        {
            return Utility.StringUtility.Format("Assets/GameRes/Sounds/{0}.wav", assetName);
        }
        
        public static string GetUISoundAsset(string assetName)
        {
            return Utility.StringUtility.Format("Assets/GameMain/UI/UISounds/{0}.wav", assetName);
        }
    }
}