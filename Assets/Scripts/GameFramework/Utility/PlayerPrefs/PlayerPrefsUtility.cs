using System;
using GameFramework.Utility.Json;
using UnityEditor;

namespace GameFramework.Utility.PlayerPrefs
{
    public class PlayerPrefsUtility
    {
      
        public static bool HasKey(string key)
        {
            return UnityEngine.PlayerPrefs.HasKey(key);
        }

        public static bool GetBool(string key)
        {
            return UnityEngine.PlayerPrefs.GetInt(key) != 0;
        }
        
        public static bool GetBool(string key,bool defaultValue)
        {
            return UnityEngine.PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) != 0;
        }

        public static void SetBool(string key, bool value)
        {
            UnityEngine.PlayerPrefs.DeleteKey(key);
            UnityEngine.PlayerPrefs.SetInt(key, value ? 1 : 0);
            UnityEngine.PlayerPrefs.Save();
        }

        public static int GetInt(string key,int defaultval = 0)
        {
            return UnityEngine.PlayerPrefs.GetInt(key,defaultval);
        }
        
       

        public static void SetInt(string key,int value)
        {
            if (HasKey(key))
            {
                UnityEngine.PlayerPrefs.DeleteKey(key);
            }
            UnityEngine.PlayerPrefs.SetInt(key,value);
            UnityEngine.PlayerPrefs.Save();
        }

       
        public static float GetFloat(string key,float defaultval = 0f)
        {
            return UnityEngine.PlayerPrefs.GetFloat(key,defaultval);
        }

        public static void SetFloat(string key,float value)
        {
            if (HasKey(key))
            {
                UnityEngine.PlayerPrefs.DeleteKey(key);
            }
            UnityEngine.PlayerPrefs.SetFloat(key,value);
            UnityEngine.PlayerPrefs.Save();
        }

        public static string GetString(string key,string defaultval = "")
        {
            return UnityEngine.PlayerPrefs.GetString(key,defaultval);
        }

        public static void SetString(string key,string value)
        {
            if (HasKey(key))
            {
                UnityEngine.PlayerPrefs.DeleteKey(key);
            }
            UnityEngine.PlayerPrefs.SetString(key,value);
            UnityEngine.PlayerPrefs.Save();
        }

        public static void DeleteAll()
        {
            UnityEngine.PlayerPrefs.DeleteAll();
        }

        public static void DeleteKey(string key )
        {
            UnityEngine.PlayerPrefs.DeleteKey(key);
        }
        
        public static void Save()
        {
            UnityEngine.PlayerPrefs.Save();
        }

        public static T GetObject<T>(string key)
        {
            return JsonUtility.ToObject<T>(UnityEngine.PlayerPrefs.GetString(key));
        }

        public static object GetObject(Type objectType,string key)
        {
            return JsonUtility.ToObject(objectType,UnityEngine.PlayerPrefs.GetString(key));
        }

        public static T GetObject<T>(string key, T defaultObj)
        {
            string json = UnityEngine.PlayerPrefs.GetString(key);
            if (json == null)
            {
                return defaultObj;
            }
            return JsonUtility.ToObject<T>(json);
        }

        public static object GetObject(Type objectType, string key, object defaultObj)
        {
            string json = UnityEngine.PlayerPrefs.GetString(key);
            if (json == null)
            {
                return defaultObj;
            }
            return JsonUtility.ToObject(objectType, json);
        }

        public static void SetObjet(string key, object obj)
        {
            string json = JsonUtility.ToJson(obj);
            if (json != null)
            {
                SetString(key,json);
            }
        }

        public static void SetObject<T>(string key, T obj)
        {
            string json = JsonUtility.ToJson(obj);
            if (json != null)
            {
                SetString(key,json);
            }
        }
    }
}