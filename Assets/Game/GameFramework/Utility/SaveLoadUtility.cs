using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace GameFramework.Utility
{
    public static class SaveLoadUtility
    {
        private const string _baseFolderName = "/Data/";

		static string DetermineSavePath()
		{
			var savePath = Application.platform == RuntimePlatform.IPhonePlayer ? PathUtility.PathUtility.GetCombinePath(AppConst.Path.PresistentDataPath, _baseFolderName) : PathUtility.PathUtility.GetCombinePath(AppConst.Path.PresistentDataPath, _baseFolderName);
			#if UNITY_EDITOR
			    savePath = PathUtility.PathUtility.GetCombinePath(Application.dataPath, _baseFolderName);
			#endif
			return savePath;
		}

		static string DetermineSaveFileName(string fileName)
		{
			return fileName+".binary";
		}

		public static void Save(object saveObject, string fileName)
		{
			string savePath = DetermineSavePath();
			string saveFileName = DetermineSaveFileName(fileName);
			if (!Directory.Exists(savePath))
			{
				Directory.CreateDirectory(savePath);
			}
	        BinaryFormatter formatter = new BinaryFormatter();
			FileStream saveFile = System.IO.File.Create(savePath+saveFileName);
			formatter.Serialize(saveFile, saveObject);
	        saveFile.Close();
		}

		public static object Load(string fileName)
		{
			string savePath = DetermineSavePath();
			string saveFileName = savePath + DetermineSaveFileName(fileName);
			if (!Directory.Exists(savePath) || !System.IO.File.Exists(saveFileName))
			{
				return null;
			}
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream saveFile = System.IO.File.Open(saveFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
			var returnObject = formatter.Deserialize(saveFile);
	        saveFile.Close();
			return returnObject;
		}

		public static void DeleteSave(string fileName)
		{
			string savePath = DetermineSavePath();
			string saveFileName = DetermineSaveFileName(fileName);
            if (System.IO.File.Exists(savePath + saveFileName))
            {
                System.IO.File.Delete(savePath + saveFileName);
            }			
		}

		public static void DeleteSaveFolder()
		{
            string savePath = DetermineSavePath();
            if (Directory.Exists(savePath))
            {
                DeleteDirectory(savePath);
            }
        }

        public static void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);
            foreach (string file in files)
            {
                System.IO.File.SetAttributes(file, FileAttributes.Normal);
                System.IO.File.Delete(file);
            }
            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }
            Directory.Delete(target_dir, false);
        }
    }
}