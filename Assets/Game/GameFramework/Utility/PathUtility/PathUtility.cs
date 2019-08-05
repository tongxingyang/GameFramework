using System;
using System.IO;
using System.Text;
using GameFramework.Debug;
using GameFramework.Utility.File;
using UnityEditor;

namespace GameFramework.Utility.PathUtility
{
   public class PathUtility
   {
        public static string GetRegularPath(string path)
        {
            return path?.Replace('\\', '/');
        }

        public static string GetCombinePath(params string[] path)
        {
            if (path == null || path.Length < 1)
            {
                return null;
            }

            string combinePath = path[0];
            for (int i = 1; i < path.Length; i++)
            {
                combinePath = System.IO.Path.Combine(combinePath, path[i]);
            }

            return GetRegularPath(combinePath);
        }

        public static string GetRemotePath(params string[] path)
        {
            string combinePath = GetCombinePath(path);
            if (combinePath == null)
            {
                return null;
            }

            return combinePath.Contains("://") ? combinePath : ("file:///" + combinePath).Replace("file:////", "file:///");
        }
       
        public static bool RemoveEmptyDirectory(string directoryName)
        {
            try
            {
                if (!FileUtility.IsDirectoryExist(directoryName))
                {
                    return false;
                }

                string[] subDirectoryNames = Directory.GetDirectories(directoryName, "*");
                int subDirectoryCount = subDirectoryNames.Length;
                foreach (string subDirectoryName in subDirectoryNames)
                {
                    if (RemoveEmptyDirectory(subDirectoryName))
                    {
                        subDirectoryCount--;
                    }
                }

                if (subDirectoryCount > 0)
                {
                    return false;
                }

                if (Directory.GetFiles(directoryName, "*").Length > 0)
                {
                    return false;
                }

                Directory.Delete(directoryName);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}