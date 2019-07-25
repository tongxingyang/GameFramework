using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GameFramework.Debug;
using UnityEngine;
using UnityEngine.Networking;

namespace GameFramework.Utility.File
{
    public class FileUtility
    {
        #region Delegate

        public delegate void OnFileOperateFail(string fullPath, enFileOperation fileOperation, Exception exception);

        #endregion

        private static OnFileOperateFail FileOperateFail = null;

        public static void SetFileOperateFailDelgate(OnFileOperateFail del)
        {
            FileOperateFail = del;
        }

        private static void InvokeDelegate(string fullPath, enFileOperation fileOperation, Exception exception)
        {
            if (FileOperateFail != null)
            {
                FileOperateFail(fullPath, fileOperation, exception);
            }
        }

        public static byte[] ReadAllBytes(string filePath)
        {
            return System.IO.File.ReadAllBytes(filePath);
        } 
        
        public static bool IsFileExist(string filePath)
        {
            return System.IO.File.Exists(filePath);
        }

        public static bool IsDirectoryExist(string directory)
        {
            return Directory.Exists(directory);
        }

        public static string ReadAllText(string filePath)
        {
            return System.IO.File.ReadAllText(filePath);
        }

        public static byte[] ReadFile(string filePath)
        {
            byte[] result = null;
            if (IsFileExist(filePath))
            {
                try
                {
                    result = System.IO.File.ReadAllBytes(filePath);
                }
                catch (Exception e)
                {
                    InvokeDelegate(filePath, enFileOperation.ReadFile, e);
                    Debuger.LogError("Error ReadFile " + e.Message);
                }
            }
            else
            {
                InvokeDelegate(filePath, enFileOperation.ReadFile, null);
                Debuger.LogError("Error ReadFile  file is not exist  ,file path = " + filePath );
            }
            return result;
        }

        public static FileStream OpenWrite(string fileName)
        {
            return System.IO.File.OpenWrite(fileName);
        }
        
        public static void CreateFile(string filePath)
        {
            System.IO.File.Create(filePath);
        }

        public static void Move(string name1, string name2)
        {
            System.IO.File.Move(name1,name2);
        }
        
        public static void DeleteFile(string filePath)
        {
            if (IsFileExist(filePath))
            {
                try
                {
                    System.IO.File.Delete(filePath);
                }
                catch (Exception e)
                {
                    InvokeDelegate(filePath,enFileOperation.DeleteFile, e);
                    Debuger.LogError("Error DeleteFile " + e.Message);
                }
            }
            else
            {
                Debuger.LogError("Error DeleteFile  file path "+filePath);
            }
        }

        public static bool CreateDirectory(string directory)
        {
            bool result = false;
            if (IsDirectoryExist(directory))
            {
                result = true;
            }
            else
            {
                try
                {
                    Directory.CreateDirectory(directory);
                    result = true;
                }
                catch (Exception e)
                {
                    result = false;
                    InvokeDelegate(directory,enFileOperation.CreateDirectory, e);
                }
            }
            return result;
        }

        public static bool DeleteDirectory(string directory,bool recursive = true)
        {
            bool result = false;
            if (IsDirectoryExist(directory))
            {
                try
                {
                    Directory.Delete(directory,recursive);
                    result = true;
                }
                catch (Exception e)
                {
                    result = false;
                    InvokeDelegate(directory,enFileOperation.DeleteDirectory, e);
                }
            }
            else
            {
                result = true;
            }
            return result;
        }

        public static int GetFileLength(string filePath)
        {
            int result = 0;
            if (IsFileExist(filePath))
            {
                FileInfo fileInfo = new FileInfo(filePath);
                result = (int) fileInfo.Length;
            }
            else
            {
                result = 0;
            }
            return result;
        }

        public static bool WriteFile(string filePath, byte[] data)
        {
            bool result = false;
            try
            {
                System.IO.File.WriteAllBytes(filePath,data);
                result = true;
            }
            catch (Exception e)
            {
                result = false;
                InvokeDelegate(filePath,enFileOperation.WriteFile, e);
                Debuger.LogError("wirte file error file path: "+filePath);
            }
            return result;
        }

        public static bool WriteFile(string filePath, byte[] data, int offect, int length)
        {
            bool result = false;
            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
                fileStream.Write(data, offect, length);
                fileStream.Close();
                fileStream = null;
                result = true;
            }
            catch (Exception e)
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
                DeleteFile(filePath);
                InvokeDelegate(filePath, enFileOperation.WriteFile, e);
            }
            return result;
        }
        
        public static string GetFullName(string fullPath)
        {
            string result = string.Empty;
            int length = fullPath.LastIndexOf("/", StringComparison.Ordinal);
            if (length > 0)
            {
                result = fullPath.Substring(length + 1, fullPath.Length - length - 1);
            }
            return result;
        }

        public static string ChangeExtension(string path, string ext)
        {
            string e = Path.GetExtension(path);
            if (string.IsNullOrEmpty(e))
            {
                return path + ext;
            }

            bool backDSC = path.IndexOf('\\') != -1;
            path = path.Replace('\\', '/');
            if (path.IndexOf('/') == -1)
            {
                return path.Substring(0, path.LastIndexOf('.')) + ext;
            }

            string dir = path.Substring(0, path.LastIndexOf('/'));
            string name = path.Substring(path.LastIndexOf('/'), path.Length - path.LastIndexOf('/'));
            name = name.Substring(0, name.LastIndexOf('.')) + ext;
            path = dir + name;

            if (backDSC)
            {
                path = path.Replace('/', '\\');
            }
            return path;
        }
        
        public static string EraseExtension(string fullname)
        {
            string result = string.Empty;
            int length = fullname.LastIndexOf(".", StringComparison.Ordinal);
            if (length > 0)
            {
                result = fullname.Substring(0,length);
            }
            return result;
        }

        public static string GetExtension(string fullname)
        {
            string result = string.Empty;
            int length = fullname.LastIndexOf(".", StringComparison.Ordinal);
            if (length > 0 && length+1>fullname.Length)
            {
                result = fullname.Substring(length+1);
            }
            return result;
        }
        
        public static void RecursiveFile(List<string> paths, List<string> fileList, List<string> exts = null)
        {
            RecursiveFile(paths.ToArray(), fileList, exts);
        }

        public static void RecursiveFile(string[] paths, List<string> fileList, List<string> exts = null)
        {
            for (int i = 0; i < paths.Length; i++)
            {
                RecursiveFile(paths[i], fileList, exts);
            }
        }
        
        public static void RecursiveFile(string path, List<string> fileList, List<string> exts = null)
        {
            
            string[] names = Directory.GetFiles(path);
            string[] dirs = Directory.GetDirectories(path);
            bool isCheckExt = exts != null && exts.Count > 0;
            foreach (string filename in names)
            {
                if (isCheckExt)
                {
                    var extension = Path.GetExtension(filename);
                    if (extension != null)
                    {
                        string ext = extension.ToLower();
                        if (!exts.Contains(ext))
                            continue;
                    }
                }

                string fn = Path.GetFileName(filename);
                if (fn.Equals(".DS_Store")) continue;

                string file = filename.Replace('\\', '/');
                fileList.Add(file);
            }
            
            foreach (string dir in dirs)
            {
                RecursiveFile(dir, fileList, exts);
            }
        }
        
        public static string GetDirectoryName(string fullName)
        {
            return Path.GetDirectoryName(fullName);
        }
        
        public static bool ClearDirectory(string directoryPath)
        {
            bool result = false;
            try
            {
                string[] files = Directory.GetFiles(directoryPath);
                foreach (var file in files)
                {
                    DeleteFile(file);
                }
                string[] directorys = Directory.GetDirectories(directoryPath);
                foreach (var directory in directorys)
                {
                    DeleteDirectory(directory);
                }
                result = true;
            }
            catch (Exception e)
            {
                result = false;
                InvokeDelegate(directoryPath, enFileOperation.DeleteDirectory, e);
            }
            return result;
        }
        
        public static string GetFileFullName(string path,string filename)
        {
            return path +  filename; 
        }
        public static bool CopyDirectory(string srcDir,string desDir)
        {
            bool result = false;
            if (!Directory.Exists(srcDir))
            {
                result = false;
            }
            else
            {
                if (!Directory.Exists(desDir))
                {
                    Directory.CreateDirectory(desDir);
                }
                string[] files = Directory.GetFiles(srcDir);
                foreach (var file in files)
                {
                    string filename = Path.GetFileName(file);
                    string desDirfilename = Path.Combine(desDir, filename);
                    System.IO.File.Copy(file,desDirfilename);
                }
                string[] dirs = Directory.GetDirectories(srcDir);
                foreach (var dir in dirs)
                {
                    string dirname = Path.GetDirectoryName(dir);
                    string name = Path.Combine(desDir, dirname);
                    CopyDirectory(dir, name);
                }
                result = true;
            }
            return result;
        }

        public static void CopyFile(string srcFile, string desFile)
        {
            if (!IsDirectoryExist(desFile))
            {
                CreateDirectory(desFile);
            }
            System.IO.File.Copy(srcFile, desFile);
        }

        public static IEnumerator StartCopyInitialFile(string sourcePath,string desPath,string localname)
        {
            yield return CopyStreamingAssetsToFile(GetInitialFileName(sourcePath,localname), GetFileFullName(desPath,localname));
        }

        public static IEnumerator CopyStreamingAssetsToFile(string src,string des)
        {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_IPHONE
            src = "file:///" + src;
#endif
            using (UnityWebRequest w = new UnityWebRequest(src))
            {
                yield return w.SendWebRequest();
                if (!w.isNetworkError)
                {
                    byte[] bytes = w.downloadHandler.data;
                    WriteBytesToFile(des,bytes,bytes.Length);
                }
                else
                {
                    Debuger.LogError("文件拷贝出错 源文件 :"+src+"  目标文件:   "+des+"  "+w.error);
                }
            }
        }

        public static void WriteBytesToFile(string path, byte[] bytes, int length = -1)
        {
            string directory = GetDirectoryName(path);
            if (!IsDirectoryExist(directory))
            {
                CreateDirectory(directory);
            }
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(path);
            if (length == -1)
            {
                length = bytes.Length;
            }
            using (Stream sw = fileInfo.Open(FileMode.Create,FileAccess.ReadWrite))
            {
                if (bytes != null && length > 0)
                {
                    sw.Write(bytes,0,length);
                }
            }
        }

        public static string GetInitialFileName(string path,string flie)
        {
            return path + flie;
        }
    }
}