using System;
using GameFramework.Localization;
using GameFramework.Localization.Base;
using UnityEngine;
using UnityEngine.UI;
namespace GameFramework.UI.UIExtension
{
    [AddComponentMenu("UI/LocalizeText",10)]
    public class LocalizationText : Text
    {
        public enLanguageKey LanguageKey;
        public UIFont Font;
        public bool IsOpenLocalize = true;
        protected override void Awake()
        {
            base.Awake();
            if (Font != null)
            {
                font = Font.UseFont;
            }
            RefreshLanguage();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (IsOpenLocalize)
            {
                LocalizationManager.RefreshLanguage = (LocalizationManager.LocalizeDelegate)Delegate.Combine(LocalizationManager.RefreshLanguage, (LocalizationManager.LocalizeDelegate)RefreshLanguage);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (IsOpenLocalize)
            {
                LocalizationManager.RefreshLanguage = (LocalizationManager.LocalizeDelegate)Delegate.Remove(LocalizationManager.RefreshLanguage, (LocalizationManager.LocalizeDelegate)RefreshLanguage);
            }
        }

        public void RefreshLanguage()
        {
            if (IsOpenLocalize)
            {
                text = LocalizationManager.GetTextValue(LanguageKey);
            }
        }
    }
}