using GameFramework.DataTable.Base;
using GameFramework.Res.Base;

namespace GameFramework.Localization.Base
{
    public interface ILocalizationManager
    {
        Language Language { get; set; }
        Language SystemLanguage { get; }
        void SetResourceManager(IResourceManager resourceManager);
        void ParseLanguage(string text);
        string GetString(enLanguageKey key);
        string GetString(enLanguageKey key,params object[] args);
        bool HasString(enLanguageKey key);
        bool AddString(enLanguageKey key ,string value);
        bool RemoveString(enLanguageKey key);
        void ChangeLanguage(Language language,ResourceLoadInfo resourceLoadInfo);
    }
}