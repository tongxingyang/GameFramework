using System.Collections;
using System.Collections.Generic;
using GameFramework.Tool;

namespace GameFramework.Animation.UIAnimation
{
    public class UIAnimationElementsGroup : UIAnimationElementBase
    {
        public bool DeactivateWhileInvisible = true;
        public List<UIAnimationElement> AnimatedElements = new List<UIAnimationElement>();
        public List<UIAnimationElement> SharedAnimatedElements = new List<UIAnimationElement>();
        private float hidingTime;
        private float showingTime;
        private IEnumerator completeEventEnum;
        
        public override void ChangeVisibility(bool visible, bool ignoreEvent = false)
        {
            if (!Initialized)
                InitializeElements();
            
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
            
            if (!UseSimpleActivation)
            {
                foreach (UIAnimationElement e in AnimatedElements)
                {
                    if (e == null || !e.IsDependent) continue;
                    e.ChangeVisibility(visible,ignoreEvent);
                }
            }
            else
            {
                gameObject.SetActive(visible);
            }
            
            foreach (UIAnimationElement e in SharedAnimatedElements)
            {
                if (e == null || !e.IsDependent) continue;
                if (visible)
                {
                    if (!UseSimpleActivation)
                    {
                        if (!e.gameObject.activeSelf)
                            e.gameObject.SetActive(true);
                        e.ChangeVisibility(true, ignoreEvent);
                    }
                    else
                    {
                        e.gameObject.SetActive(true);
                        e.ChangeVisibilityImmediate(true, ignoreEvent);
                    }
                }
            }
            
            Visible = visible;
            
            if (!ignoreEvent)
            {
                if (completeEventEnum != null)
                {
                    StopCoroutine(completeEventEnum);
                    completeEventEnum = null;
                }

                if (visible)
                {
                    OnShow?.Invoke();
                    if (OnShowComplete != null)
                    {
                        completeEventEnum = Yielders.DelayCallEvent(OnShowComplete, showingTime);
                    }
                }
                else
                {
                    OnHide?.Invoke();
                    if (OnHideComplete != null)
                    {
                        completeEventEnum = Yielders.DelayCallEvent(OnHideComplete, hidingTime);
                    }
                }

                if (gameObject.activeInHierarchy)
                {
                    if(completeEventEnum!=null)
                        StartCoroutine(completeEventEnum);
                }
                
                if (UseSimpleActivation)
                {
                    if (completeEventEnum != null)
                    {
                        StopCoroutine(completeEventEnum);
                        completeEventEnum = null;
                    }
                    if (visible)
                    {
                        OnShowComplete?.Invoke();
                    }
                    else
                    {
                        OnHideComplete?.Invoke();
                    }
                }
            }
        }

        public override void ChangeVisibilityImmediate(bool visible, bool ignoreEvent = false)
        {
            if (!Initialized)
                InitializeElements();
            
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
            
            if (!UseSimpleActivation)
            {
                foreach (UIAnimationElement e in AnimatedElements)
                {
                    if (e == null || !e.IsDependent) continue;

                    e.ChangeVisibilityImmediate(visible, ignoreEvent);
                }
            }
            else
            {
                gameObject.SetActive(visible);
            }
            
            foreach (UIAnimationElement e in SharedAnimatedElements)
            {
                if (e == null || !e.IsDependent) continue;

                if (visible)
                {
                    if (!UseSimpleActivation)
                    {
                        if (!e.gameObject.activeSelf)
                            e.gameObject.SetActive(true);
                        e.ChangeVisibilityImmediate(true, ignoreEvent);
                    }
                    else
                        e.gameObject.SetActive(true);
                }
            }
            
            Visible = visible;

            if (!ignoreEvent)
            {
                if (visible)
                {
                    OnShow?.Invoke();
                    OnShowComplete?.Invoke();
                }
                else
                {
                    OnHide?.Invoke();
                    OnHideComplete?.Invoke();
                }
            }

            if (DeactivateWhileInvisible && !visible)
                gameObject.SetActive(false);
        }
        
        public float GetAllHidingTime()
        {
            if (hidingTime != 0)
                return hidingTime;

            for (int i = 0; i < AnimatedElements.Count; i++)
            {
                UIAnimationElement uiA = AnimatedElements[i];
                float uiAHidingTime = uiA.GetAllHidingTime();

                if (uiAHidingTime > hidingTime)
                    hidingTime = uiAHidingTime;
            }
            return hidingTime;
        }
        
        public float GetAllShowingTime()
        {
            if (showingTime != 0)
                return showingTime;

            for (int i = 0; i < AnimatedElements.Count; i++)
            {
                UIAnimationElement uiA = AnimatedElements[i];
                float uiAShowingTime = uiA.GetAllShowingTime();

                if (uiAShowingTime > showingTime)
                    showingTime = uiAShowingTime;
            }
            return showingTime;
        }

        public void InitializeElements()
        {
            if (Initialized) return;
            if (DeactivateWhileInvisible)
            {
                OnHideComplete.AddListener(() =>
                {
                    if (!Visible)
                    {
                        gameObject.SetActive(Visible);
                    }
                });
            }
            for (int i = 0; i < AnimatedElements.Count; i++)
            {
                if (AnimatedElements[i] != null)
                    AnimatedElements[i].Initialize();
                else
                {
                    AnimatedElements.RemoveAt(i);
                    i--;
                }
            }
            
            for (int i = 0; i < SharedAnimatedElements.Count; i++)
            {
                if (SharedAnimatedElements[i] != null)
                    SharedAnimatedElements[i].Initialize();
                else
                {
                    SharedAnimatedElements.RemoveAt(i);
                    i--;
                }
            }
            
            hidingTime = GetAllHidingTime();
            showingTime = GetAllShowingTime();
            Initialized = true;
        }
    }
}