using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace GameFramework.Utility.Compress
{
    public class ZIPUtility
    {
        public static byte[] MBytesCache = new byte[1024 * 4];
        
        #region Zip 加密、压缩文件  解密、解压缩文件
        
        public static void ZipCompress(string dir, string toFileName, string searchPattern, int compressionLevel)
        {
            DirectoryInfo di = new DirectoryInfo(dir);
            FileInfo[] files = di.GetFiles(searchPattern, SearchOption.AllDirectories);
            ZipCompress(dir, files, toFileName, compressionLevel);
        }
        public static void ZipCompress(string dir, FileInfo[] files, string toFileName, int compressionLevel)
        {
            dir = dir.Replace('/', Path.DirectorySeparatorChar);
            using (ZipOutputStream s = new ZipOutputStream(System.IO.File.Create(toFileName)))
            {
                s.SetLevel(compressionLevel);
                foreach (FileInfo file in files)
                {
                    using (FileStream fs = file.Open(FileMode.Open, FileAccess.Read))
                    {
                        var entry = new ZipEntry(file.FullName.Replace(dir, ""));
                        entry.DateTime = (file.CreationTime > file.LastWriteTime ? file.LastWriteTime : file.CreationTime);
                        s.PutNextEntry(entry);
                        var size = MBytesCache.Length;
                        while (true)
                        {
                            size = fs.Read(MBytesCache, 0, size);
                            if (size <= 0) break;
                            s.Write(MBytesCache, 0, size);
                        }
                    }
                }
                s.Finish();
            }
        }
        
        public static void ZipDecompress(string zipFile, string targetPath)
        {
            if (!Directory.Exists(targetPath)) Directory.CreateDirectory(targetPath);
            using (ZipInputStream s = new ZipInputStream(System.IO.File.OpenRead(zipFile)))
            {
                ZipEntry theEntry = null;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    var currentPath = targetPath + theEntry.Name;
                    var currentDirectory = Path.GetDirectoryName(currentPath);
                    if (!Directory.Exists(currentDirectory)) Directory.CreateDirectory(currentDirectory);
                    using (FileStream streamWriter = new FileStream(currentPath, FileMode.Create))
                    {
                        while (true)
                        {
                            var size = s.Read(MBytesCache, 0, MBytesCache.Length);
                            if (size <= 0) break;
    
                            streamWriter.Write(MBytesCache, 0, size);
                        }
                        streamWriter.Flush();
                    }
                }
            }
        }
        #endregion
        
    }
}