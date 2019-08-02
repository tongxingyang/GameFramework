using GameFramework.Utility;

namespace GameFramework.Update
{
    public class UpdateFileInfo
    {
        public string AssetBundleName;
        public int Length;
        public int Md5Code;
        public int ZipLength;
        public int ZipMd5Code;
        public override string ToString()
        {
            return StringUtility.Format("{0}:{1}:{2}:{3}:{4}", AssetBundleName, Length, Md5Code, ZipLength, ZipMd5Code);
        }
    }
}