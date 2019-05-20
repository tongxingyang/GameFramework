using System;
using GameFramework.Base;
using GameFramework.Debug;
using GameFramework.Utility.PlayerPrefs;
using GameFramework.Utility.Singleton;
using UnityEngine;

namespace GameFramework.Setting
{
    [DisallowMultipleComponent]
    public class SettingComponent:GameFrameworkComponent
    {
        public override int Priority => SingletonMono<GameFramework>.GetInstance().SettingPriority;
        
        public bool HasSetting(string settingName)
        {
            return PlayerPrefsUtility.HasKey(settingName);
        }

        public void RemoveSetting(string settingName)
        {
            PlayerPrefsUtility.DeleteKey(settingName);
        }

        public void RemoveAllSettings()
        {
            PlayerPrefsUtility.DeleteAll();
        }

        public bool GetBool(string settingName)
        {
            return PlayerPrefsUtility.GetBool(settingName);
        }

        public bool GetBool(string settingName, bool defaultValue)
        {
            return PlayerPrefsUtility.GetBool(settingName,defaultValue);
        }

        public void SetBool(string settingName, bool value)
        {
            PlayerPrefsUtility.SetBool(settingName,value);
        }
        
        public int GetInt(string settingName)
        {
            return PlayerPrefsUtility.GetInt(settingName);
        }

        public int GetInt(string settingName, int defaultValue)
        {
            return PlayerPrefsUtility.GetInt(settingName, defaultValue);
        }

        public void SetInt(string settingName, int value)
        {
            PlayerPrefsUtility.SetInt(settingName, value);
        }

        public float GetFloat(string settingName)
        {
            return PlayerPrefsUtility.GetFloat(settingName);
        }

        public float GetFloat(string settingName, float defaultValue)
        {
            return PlayerPrefsUtility.GetFloat(settingName, defaultValue);
        }

        public void SetFloat(string settingName, float value)
        {
            PlayerPrefsUtility.SetFloat(settingName, value);
        }

        public string GetString(string settingName)
        {
            return PlayerPrefsUtility.GetString(settingName);
        }

        public string GetString(string settingName, string defaultValue)
        {
            return PlayerPrefsUtility.GetString(settingName, defaultValue);
        }
        public void SetString(string settingName, string value)
        {
            PlayerPrefsUtility.SetString(settingName, value);
        }
        
        public T GetObject<T>(string settingName)
        {
            return PlayerPrefsUtility.GetObject<T>(settingName);
        }

        public object GetObject(Type objectType, string settingName)
        {
            return PlayerPrefsUtility.GetObject(objectType, settingName);
        }

        public T GetObject<T>(string settingName, T defaultObj)
        {
            return PlayerPrefsUtility.GetObject(settingName, defaultObj);
        }

        public object GetObject(Type objectType, string settingName, object defaultObj)
        {
            return PlayerPrefsUtility.GetObject(objectType, settingName, defaultObj);
        }

        public void SetObject<T>(string settingName, T obj)
        {
            PlayerPrefsUtility.SetObject(settingName, obj);
        }

        public void SetObject(string settingName, object obj)
        {
            PlayerPrefsUtility.SetObject(settingName, obj);
        }
        
    }
}