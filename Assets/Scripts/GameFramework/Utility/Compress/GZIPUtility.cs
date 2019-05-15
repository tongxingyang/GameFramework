using System.IO;
using ICSharpCode.SharpZipLib.GZip;
namespace GameFramework.Utility.Compress
{
    public class GZIPUtility
    {
        public static byte[] MBytesCache = new byte[1024 * 4];
        public static int CompressLevel = 9;

        static void Compress(Stream inputFs, Stream outputFs)
        {
            using (GZipOutputStream stream = new GZipOutputStream(outputFs))
            {
                stream.SetLevel(CompressLevel);
                int size;
                while ((size = inputFs.Read(MBytesCache,0,MBytesCache.Length))>0)
                {
                    stream.Write(MBytesCache,0,size);
                }
                stream.Flush();
            }
        }

        static void DeCompress(Stream inputFs, Stream outputFs)
        {
            using (GZipInputStream s = new GZipInputStream(inputFs))
            {
                int size;
                while ((size = s.Read(MBytesCache, 0, MBytesCache.Length)) > 0)  
                    outputFs.Write(MBytesCache, 0, size);
                outputFs.Flush();
            }
        }
        
        public static byte[] Compress(byte[] inData)
        {
            byte[] outData = null;
            using (MemoryStream inputFs = new MemoryStream(inData))
            {
                using (MemoryStream outputFs = new MemoryStream())
                {
                    Compress(inputFs, outputFs);
                    outData = outputFs.GetBuffer();
                }
            }
            return outData;
        }
        public static byte[] DeCompress(byte[] inData,int sourcesSize)
        {
            byte[] outData = new byte[sourcesSize];
            using (MemoryStream inputFs = new MemoryStream(inData))
            {
                using (MemoryStream outputFs = new MemoryStream(outData))
                {
                    DeCompress(inputFs, outputFs);
                }
            }
            return outData;
        }
        
        public static byte[] Compress(string inpath)
        {
            byte[] outData = null;
            using (FileStream inputFs = new FileStream(inpath, FileMode.Open))
            {
                using (MemoryStream outputFs = new MemoryStream(1024))
                {
                    Compress(inputFs, outputFs);
                    outData = outputFs.GetBuffer(); 
                }
            }
            return outData;
        }
        
        public static void DeCompress(byte[] inData, string outpath)
        {
            using (MemoryStream inputFs = new MemoryStream(inData))
            {
                using (FileStream outputFs = new FileStream(outpath, FileMode.OpenOrCreate))
                {
                    DeCompress(inputFs, outputFs);
                }
            }
        }    
        
        public static long Compress(string inPath, string outPath)
        {
            long size = 0;
            using (FileStream inputFs = new FileStream(inPath,FileMode.Open))
            {
                size = inputFs.Length;
                using (FileStream outputFs = new FileStream(outPath,FileMode.OpenOrCreate))
                {
                    Compress(inputFs, outputFs);
                }
            }
            return size;
        }

        public static void DeCompress(string inPath, string outPath)
        {
            using (FileStream inputFs = new FileStream(inPath,FileMode.Open))   
            {
                using (FileStream outputFs = new FileStream(outPath,FileMode.OpenOrCreate))
                {
                    DeCompress(inputFs,outputFs);
                }
            }
        }
    }
}