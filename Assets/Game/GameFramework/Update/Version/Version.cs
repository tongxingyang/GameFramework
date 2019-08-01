namespace GameFramework.Update.Version
{
    public enum VersionCompareResult:byte
    {
        Greater,
        Less,
        Equal,
    }
    public class Version
    {
        public int MasterVersion = 0;
        public int MinorVersion = 0;
        public int RevisedVersion = 0;

        public Version(string version)
        {
            string[] arr = version.Split('.');
            MasterVersion = int.Parse(arr[0]);
            MinorVersion = int.Parse(arr[1]);
            RevisedVersion = int.Parse(arr[2]);
        }

        public Version()
        {
            
        }

        public override string ToString()
        {
            return string.Format("{0}_{1}_{2}",MasterVersion,MinorVersion,RevisedVersion);
        }

        public static bool VersionEqual(Version a, Version b)
        {
            return a.MasterVersion == b.MasterVersion && a.MinorVersion == b.MinorVersion &&
                   a.RevisedVersion == b.RevisedVersion;
        }
        
        public static VersionCompareResult CompareResult(Version a, Version b,bool compareRevised)
        {
            if (a.MasterVersion > b.MasterVersion)
            {
                return VersionCompareResult.Greater;
            }
            if (a.MasterVersion < b.MasterVersion)
            {
                return VersionCompareResult.Less;
            }
            if (a.MinorVersion > b.MinorVersion)
            {
                return VersionCompareResult.Greater;
            }
            if (a.MinorVersion < b.MinorVersion)
            {
                return VersionCompareResult.Less;
            }
            if (compareRevised)
            {
                if (a.RevisedVersion > b.RevisedVersion)
                {
                    return VersionCompareResult.Greater;
                }
                if (a.RevisedVersion < b.RevisedVersion)
                {
                    return VersionCompareResult.Less;
                }
            }
            return VersionCompareResult.Equal;
        }
    }
}