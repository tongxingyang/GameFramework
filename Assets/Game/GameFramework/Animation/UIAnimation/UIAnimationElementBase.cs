using UnityEngine;
using UnityEngine.Events;

namespace GameFramework.Animation.UIAnimation
{
    public class UIAnimationElementBase : MonoBehaviour
    {
        public bool Visible;
        public bool UseSimpleActivation = false;
        public bool IgnoreEventsOnInitialization;
        public bool Initialized;
        
        public UnityEvent OnShow;
        public UnityEvent OnHide;
        public UnityEvent OnShowComplete;
        public UnityEvent OnHideComplete;

        public virtual void ChangeVisibility(bool visible, bool ignoreEvent = false)
        {
            
        }

        public virtual void ChangeVisibilityImmediate(bool visible, bool ignoreEvent = false)
        {
            
        }
        
        public virtual void SwitchVisibility()
        {
            ChangeVisibility(!Visible);
        }

        protected void Deactivate(bool forceInvisibility)
        {
            if (forceInvisibility)
            {
                ChangeVisibilityImmediate(false, true);
            }
            gameObject.SetActive(false);
        }
    }
}