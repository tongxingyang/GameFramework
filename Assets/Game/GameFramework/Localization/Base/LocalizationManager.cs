using System.Collections.Generic;
using GameFramework.DataTable.Base;
using GameFramework.Res.Base;
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
                    default: return Language.Unspecified;
                }
            }
        }
        
        public LocalizationManager()
        {
           languageDictionary = new Dictionary<enLanguageKey, string>();
           resourceManager = null;
           loadAssetCallBacks = new LoadAssetCallbacks(LoadLanguageSuccessCallback, LoadLanguageFailureCallback, LoadLanguageUpdateCallback,LoadLanguageDependencyAssetCallback);
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
        

        public void LoadLanguage(ResourceLoadInfo resourceLoadInfo)
        {
            resourceManager.LoadAsset<TextAsset>(resourceLoadInfo,loadAssetCallBacks);
        }

        public void ParseLanguage(string text)
        {
            throw new System.NotImplementedException();
        }


        public string GetString(enLanguageKey key)
        {
            throw new System.NotImplementedException();
        }

        public string GetString(enLanguageKey key, params object[] args)
        {
            throw new System.NotImplementedException();
        }

        public bool HasString(enLanguageKey key)
        {
            throw new System.NotImplementedException();
        }

        public bool AddString(enLanguageKey key, string value)
        {
            throw new System.NotImplementedException();
        }

        public bool RemoveString(enLanguageKey key)
        {
            throw new System.NotImplementedException();
        }

        public void ChangeLanguage(Language language)
        {
            throw new System.NotImplementedException();
        }
        
        private void LoadLanguageSuccessCallback(string soundAssetName, object soundAsset, float duration, object userData)
        {
           
        }

        private void LoadLanguageFailureCallback(string soundAssetName, string errorMessage,
            object userData)
        {
           
        }

        private void LoadLanguageUpdateCallback(string soundAssetName, float progress, object userData)
        {
           
        }

        private void LoadLanguageDependencyAssetCallback(string soundAssetName, string dependencyAssetName,
            int loadedCount, int totalCount, object userData)
        {
           
        }

        public static string GetTextValue(enLanguageKey languageKey)
        {
            throw new System.NotImplementedException();
        }
    }
}