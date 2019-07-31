using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression;

namespace GameFramework.Utility.Compress
{
    public class ZipUtility
    {
        static ZipUtility()
        {
            ZipConstants.DefaultCodePage = Encoding.UTF8.CodePage;
        }
	    
	    public static byte[] MBytesCache = new byte[1024 * 4];
        
        public static byte[] Compress(byte[] input, int level = Deflater.BEST_COMPRESSION)
		{
			if (input == null || input.Length == 0)
			{
				UnityEngine.Debug.LogError("Compress error inputBytes Len = 0");
				return input;
			}

			Deflater compressor = new Deflater(level);
			compressor.SetInput(input);
			compressor.Finish();
			MemoryStream result = new MemoryStream(input.Length);
			while (!compressor.IsFinished)
			{
				int count = compressor.Deflate(MBytesCache);
				result.Write(MBytesCache, 0, count);
			}

			return result.ToArray();
		}

		public static byte[] Decompress(byte[] input)
		{
			if (input == null || input.Length == 0)
			{
				UnityEngine.Debug.LogError("Uncompress error inputBytes Len = 0");
				return input;
			}

			Inflater decompressor = new Inflater();
			decompressor.SetInput(input);
			MemoryStream result = new MemoryStream(input.Length);
			while (!decompressor.IsFinished)
			{
				int count = decompressor.Inflate(MBytesCache);
				result.Write(MBytesCache, 0, count);
			}

			return result.ToArray();
		}

		public static void DecompressFile(string zipFile, string targetDir, string password = null)
		{
			ZipFile zf = null;
			try
			{
				FileStream fs = System.IO.File.OpenRead(zipFile);
				zf = new ZipFile(fs);
				if (!string.IsNullOrEmpty(password))
				{
					zf.Password = password;
				}
				foreach (ZipEntry zipEntry in zf)
				{
					if (!zipEntry.IsFile)
					{
						continue;
					}
					string entryFileName = zipEntry.Name;
					Stream zipStream = zf.GetInputStream(zipEntry);

					string fullZipToPath = Path.Combine(targetDir, entryFileName);
					string outputDir = Path.GetDirectoryName(fullZipToPath);
					if (!string.IsNullOrEmpty(outputDir))
						Directory.CreateDirectory(outputDir);

					using (FileStream streamWriter = System.IO.File.Create(fullZipToPath))
					{
						StreamUtils.Copy(zipStream, streamWriter, MBytesCache);
					}
				}
			}
			finally
			{
				if (zf != null)
				{
					zf.IsStreamOwner = true; 
					zf.Close(); 
				}
			}
		}

		public static void CompressFile(string zipPath, string filePath, string entryName, string password = "")
		{
			string outputDir = Path.GetDirectoryName(zipPath);
			if (!string.IsNullOrEmpty(outputDir)) Directory.CreateDirectory(outputDir);

			using (var zipFile = ZipFile.Create(zipPath))
			{
				zipFile.Password = password;
				zipFile.BeginUpdate();
				zipFile.Add(filePath,entryName);
				zipFile.CommitUpdate();
			}
		}

		public static void CompressFiles(string zipPath, string[] files, string password = "")
		{
			string outputDir = Path.GetDirectoryName(zipPath);
			if (!string.IsNullOrEmpty(outputDir)) Directory.CreateDirectory(outputDir);

			using (var zipFile = ZipFile.Create(zipPath))
			{
				zipFile.Password = password;
				zipFile.BeginUpdate();
				foreach (string file in files)
				{
					zipFile.Add(file);
				}
				zipFile.CommitUpdate();
			}
		}

		public static void CompressFolder(string zipPath, string dirPath, string searchPattern = "*.*", string password = "")
		{
			string outputDir = Path.GetDirectoryName(zipPath);
			if (!string.IsNullOrEmpty(outputDir)) Directory.CreateDirectory(outputDir);

			using (var zipFile = ZipFile.Create(zipPath))
			{
				zipFile.Password = password;
				zipFile.BeginUpdate();
				foreach (string file in Directory.GetFiles(dirPath, searchPattern, SearchOption.AllDirectories))
				{
					zipFile.Add(file, file.Replace(dirPath, ""));
				}
				zipFile.CommitUpdate();
			}
        }
    }
}