using System;
using GameFramework.Base;
using GameFramework.Localization;
using GameFramework.Localization.Base;
using GameFramework.Utility.Singleton;
using UnityEngine;
using UnityEngine.UI;
namespace GameFramework.UI.UIExtension
{
    [AddComponentMenu("UI/LocalizeText",10)]
    public class LocalizationText : Text
    {
        public enLanguageKey LanguageKey;
        public bool IsOpenLocalize = true;
        public UIFont Font
        {
            get
            {
                if (Singleton<GameEntry>.GetInstance().GetComponent<UIComponent>().MainFont != null)
                {
                    return Singleton<GameEntry>.GetInstance().GetComponent<UIComponent>().MainFont;
                }
                return null;
            }
        }
        
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
                Singleton<GameEntry>.GetInstance().GetComponent<LocalizationComponent>().GetString(LanguageKey);
            }
        }

        public void RefreshLanguage(params object[] parms)
        {
            Singleton<GameEntry>.GetInstance().GetComponent<LocalizationComponent>().GetString(LanguageKey, parms);
        }
        
        public override string text
        {
            get
            {
                if(IsOpenLocalize)
                {
                    RefreshLanguage();
                    return m_Text;
                }
                else
                {
                    return m_Text;
                }
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    if (string.IsNullOrEmpty(m_Text))
                        return;
                    m_Text = "";
                    SetVerticesDirty();
                }
                else if (m_Text != value)
                {
                    m_Text = value;
                    SetVerticesDirty();
                    SetLayoutDirty();
                }
            }
        }

    }
}