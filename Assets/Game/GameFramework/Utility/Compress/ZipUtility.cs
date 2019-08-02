using System;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression;

namespace GameFramework.Utility.Compress
{
    public class ZipUtility
    {
       
	    public static byte[] MBytesCache = new byte[1024 * 4];
        
	    public static void CompressFolder(string zipPath, string dirPath, string searchPattern = "*.*",string passworld = "", int compressLevel = 6)
	    {
		    try
		    {
				string outputDir = Path.GetDirectoryName(zipPath);
			    if (!dirPath.EndsWith("/"))
			    {
				    dirPath += "/";
			    }
				if (!string.IsNullOrEmpty(outputDir)) Directory.CreateDirectory(outputDir);
			    
			    using (ZipOutputStream stream = new ZipOutputStream(System.IO.File.Create(zipPath)))
			    {
				    stream.SetLevel(compressLevel);
				    stream.Password = passworld;
				    
				    foreach (string file in Directory.GetFiles(dirPath, searchPattern, SearchOption.AllDirectories))
				    {
					    var entry = new ZipEntry(file.Replace(dirPath, ""));
					    stream.PutNextEntry(entry);
					    using (FileStream fs = System.IO.File.OpenRead(file))
					    {
						    int sourceBytes;
						    do
						    {
							    sourceBytes = fs.Read(MBytesCache, 0, MBytesCache.Length);
							    stream.Write(MBytesCache, 0, sourceBytes);

						    } while (sourceBytes > 0);
					    }
				    }
				    stream.Finish();
				    stream.Close();
			    }
		    } catch (Exception e) {
				UnityEngine.Debug.Log ("压缩出错：" + e);
		    }

	    }
	    
		public static void CompressFile(string zipPath, string filePath, string entryName, string password = "", int compressLevel = 6)
		{
			try
			{
				string outputDir = Path.GetDirectoryName(zipPath);
				if (!string.IsNullOrEmpty(outputDir)) Directory.CreateDirectory(outputDir);
				using (ZipOutputStream stream = new ZipOutputStream(System.IO.File.Create(zipPath)))
				{
					stream.SetLevel(compressLevel);
					stream.Password = password;
					var entry = new ZipEntry(entryName);
					stream.PutNextEntry(entry);
					using (FileStream fs = System.IO.File.OpenRead(filePath))
					{
						int sourceBytes;
						do
						{
							sourceBytes = fs.Read(MBytesCache, 0, MBytesCache.Length);
							stream.Write(MBytesCache, 0, sourceBytes);

						} while (sourceBytes > 0);
					}
					stream.Finish();
					stream.Close();
				}
			} catch (Exception e) {
				UnityEngine.Debug.Log ("压缩出错：" + e);
			}
		}
	    
	    public static void DeCompressionFile(string zipPath, string outPath,string password)
	    {
		    if (System.IO.File.Exists(zipPath))
		    {
			    if (!outPath.EndsWith("/"))
			    {
				    outPath += "/";
			    }
			    using (ZipInputStream stream = new ZipInputStream(System.IO.File.OpenRead(zipPath)))
			    {
				    ZipEntry theEntry;
				    stream.Password = password;
				    while ((theEntry = stream.GetNextEntry()) != null)
				    {
					    string fileName = Path.GetFileName(theEntry.Name);
					    string filePath = PathUtility.PathUtility.GetCombinePath(outPath, theEntry.Name);
					    string directoryName = Path.GetDirectoryName(filePath);
					   
					    if (directoryName.Length > 0)
						    Directory.CreateDirectory(directoryName);
					    if (fileName != String.Empty)
					    {
						    using (FileStream streamWriter = System.IO.File.Create(filePath))
						    {
							    while (true)
							    {
								    var size = stream.Read(MBytesCache, 0, MBytesCache.Length);
								    if (size > 0)
									    streamWriter.Write(MBytesCache, 0, size);
								    else
									    break;
							    }
						    }
					    }
				    }
			    }
		    }
	    }
	    
	    public static void DeCompressionFileInSamePath(string zipPath,string password)
	    {
		    if (System.IO.File.Exists(zipPath))
		    {
			    using (ZipInputStream stream = new ZipInputStream(System.IO.File.OpenRead(zipPath)))
			    {
				    ZipEntry theEntry;
				    stream.Password = password;
				    while ((theEntry = stream.GetNextEntry()) != null)
				    {
					    string fileName = Path.GetFileName(theEntry.Name);
					    string filePath = zipPath.Replace(".zip", "");
					 	
					    if (fileName != String.Empty)
					    {
						    using (FileStream streamWriter = System.IO.File.Create(filePath))
						    {
							    while (true)
							    {
								    var size = stream.Read(MBytesCache, 0, MBytesCache.Length);
								    if (size > 0)
									    streamWriter.Write(MBytesCache, 0, size);
								    else
									    break;
							    }
						    }
					    }
				    }
			    }
		    }
	    }
    }
}