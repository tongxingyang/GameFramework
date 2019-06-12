using GameFramework.Base;
using GameFramework.Localization.Base;
using GameFramework.Res;
using GameFramework.Res.Base;
using GameFramework.Utility.Singleton;
using UnityEngine;

namespace GameFramework.Localization
{
    [DisallowMultipleComponent]
    public class LocalizationComponent :GameFrameworkComponent
    {
        public override int Priority => SingletonMono<GameFramework>.GetInstance().LocalizationPriority;
        private LocalizationManager localizationManager;

        private Language Language
        {
            get => localizationManager.Language;
            set => localizationManager.Language = value;
        }
        private Language SystemLanguage => localizationManager.SystemLanguage;
        
        public override void OnAwake()
        {
            base.OnAwake();
            localizationManager = new LocalizationManager();
        }

        public override void OnStart()
        {
            base.OnStart();
            localizationManager.SetResourceManager(Singleton<GameEntry>.GetInstance().GetComponent<ResourceComponent>().GetResourceManager());
        }
        
        public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            localizationManager.OnUpdate(elapseSeconds,realElapseSeconds);
        }
        
        public override void Shutdown()
        {
            base.Shutdown();
            localizationManager.Shutdown();
        }

        public string GetString(enLanguageKey key)
        {
            return localizationManager.GetString(key);
        }

        public string GetString(enLanguageKey key, params object[] args)
        {
            return localizationManager.GetString(key, args);
        }

        public bool HasString(enLanguageKey key)
        {
            return localizationManager.HasString(key);
        }

        public bool AddString(enLanguageKey key, string value)
        {
            return localizationManager.AddString(key, value);
        }

        public bool RemoveString(enLanguageKey key)
        {
            return localizationManager.RemoveString(key);
        }

        public void ChangeLanguage(Language language, ResourceLoadInfo resourceLoadInfo)
        {
            localizationManager.ChangeLanguage(language,resourceLoadInfo);
        }
        
    }
}