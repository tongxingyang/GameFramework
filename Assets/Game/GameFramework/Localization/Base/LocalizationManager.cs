using System;
using System.Collections.Generic;
using GameFramework.Base;
using GameFramework.Res.Base;
using GameFramework.Setting;
using GameFramework.Utility.Singleton;
using UnityEngine;

namespace GameFramework.Localization.Base
{
    public sealed class LocalizationManager:ILocalizationManager
    {
        private IResourceManager resourceManager;
        private Language language;
        private readonly Dictionary<enLanguageKey, string> languageDictionary;
        private LoadAssetCallbacks loadAssetCallBacks;

        public delegate void LocalizeDelegate();
        public static LocalizeDelegate RefreshLanguage;
        
        public Language Language
        {
            get => language;
            set
            {
                if (value == Language.Unspecified)
                {
                    language = Language.English;
                    return;
                }
                language = value;
            }
        }

        public Language SystemLanguage
        {
            get
            {
                switch (Application.systemLanguage)
                {
                    case UnityEngine.SystemLanguage.Chinese: return Language.ChineseSimplified;
                    case UnityEngine.SystemLanguage.ChineseSimplified: return Language.ChineseSimplified;
                    case UnityEngine.SystemLanguage.ChineseTraditional: return Language.ChineseTraditional;
                    case UnityEngine.SystemLanguage.English: return Language.English;
                    default: return Language.English;
                }
            }
        }
        
        public LocalizationManager()
        {
           languageDictionary = new Dictionary<enLanguageKey, string>();
           resourceManager = null;
           loadAssetCallBacks = new LoadAssetCallbacks(LoadLanguageSuccessCallback, LoadLanguageFailureCallback, LoadLanguageUpdateCallback,LoadLanguageDependencyAssetCallback);
        }

        public Language GetCurrentLanguage()
        {
            Language language = Singleton<GameEntry>.GetInstance().GetComponent<SettingComponent>().GetLanguage();
            if (language == Language.Unspecified)
            {
                Singleton<GameEntry>.GetInstance().GetComponent<SettingComponent>().SetLanguage(SystemLanguage);
            }
            Language = language;
            return language;
        }
        
        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            
        }

        public void Shutdown()
        {
            languageDictionary.Clear();
        }
        
        public void SetResourceManager(IResourceManager resource)
        {
            this.resourceManager = resource;
        }
        

        private void LoadLanguage(ResourceLoadInfo resourceLoadInfo)
        {
            resourceManager.LoadAsset<TextAsset>(resourceLoadInfo,loadAssetCallBacks);
        }

        public void ParseLanguage(string text)
        {
            if (text == String.Empty)
            {
                return;
            }
            string content = text.Replace("\r", "");
            string[] lines = content.Split('\n');
            foreach (string t in lines)
            {
                if (t.StartsWith("#"))
                {
                    continue;
                }
                var data = t.Split(',');
                languageDictionary.Add((enLanguageKey)int.Parse(data[0]),data[1]);
            }
            RefreshLanguage?.Invoke();
        }


        public string GetString(enLanguageKey key)
        {
            if (languageDictionary.TryGetValue(key,out var data))
            {
                return data;
            }
            return string.Empty;
        }

        public string GetString(enLanguageKey key, params object[] args)
        {
            if (languageDictionary.TryGetValue(key,out var data))
            {
                return Utility.StringUtility.Format(data, args);
            }
            return string.Empty;
        }

        public bool HasString(enLanguageKey key)
        {
            if (languageDictionary.ContainsKey(key))
            {
                return true;
            }
            return false;
        }

        public bool AddString(enLanguageKey key, string value)
        {
            if (!HasString(key))
            {
                languageDictionary.Add(key, value);
                return true;
            }
            return false;
        }

        public bool RemoveString(enLanguageKey key)
        {
            if (HasString(key))
            {
                languageDictionary.Remove(key);
                return true;
            }
            return false;
        }


        public void ChangeLanguage(Language language,ResourceLoadInfo resourceLoadInfo)
        {
            Language = language;
            LoadLanguage(resourceLoadInfo);
        }
        
        private void LoadLanguageSuccessCallback(string languageAssetName, object languageAsset, float duration, object userData)
        {
            var textAsset = languageAsset as TextAsset;
            if (textAsset != null) ParseLanguage(textAsset.text);
        }

        private void LoadLanguageFailureCallback(string languageAssetName, string errorMessage,
            object userData)
        {
           
        }

        private void LoadLanguageUpdateCallback(string languageAssetName, float progress, object userData)
        {
           
        }

        private void LoadLanguageDependencyAssetCallback(string languageAssetName, string dependencyAssetName,
            int loadedCount, int totalCount, object userData)
        {
           
        }

    }
}