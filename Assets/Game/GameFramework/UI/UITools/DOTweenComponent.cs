using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework.UI.UITools
{
    public class DOTweenComponent : MonoBehaviour
    {
        void OnDisable()
        {
            KillAll();
        }
        
        public void KillAll()
        {
            DoKill();
            DoKillAlpha();
        }
        
        void DoKill()
        {
            gameObject.transform.DOKill();
        }
        
        void DoKillAlpha()
        {
            Image image = gameObject.GetComponent<Image>();
            Text text = gameObject.GetComponent<Text>();
            Material material = gameObject.GetComponent<Material>();
            if (image != null)
                image?.DOKill();
            if (text != null)
                text.DOKill();
            if (material != null)
                material.DOKill();    
        }
        
        public void DoKillSlider()
        {
            Slider sld = GetComponent<Slider>();
            sld.DOKill();
        }
        
        public void DoSlider(float toPos, float interval, Ease easeType, Action callback)
        {
            DoKillSlider();
            Slider sld = GetComponent<Slider>();
            Tweener tweener = DOTween.To(() => sld.value, delegate(float x)
            {
                sld.value = x;
            }, toPos, interval).SetTarget(sld);
            if (tweener != null)
            {
                tweener.OnComplete(
                    delegate
                    {
                       callback?.Invoke();
                    });
                tweener.SetEase(easeType);
            }
        }
        
        public void DoLocalRotateQuaternion(Quaternion toPos, float interval, Ease easeType, Action callback)
        {
            Tweener tweener = gameObject.transform.DOLocalRotateQuaternion(toPos, interval);
            if (tweener != null)
            {
                tweener.OnComplete(
                    delegate
                    {
                        callback?.Invoke();
                    });
                tweener.SetEase(easeType);
            }
        }
        
        public void DoLoopRotate(Vector3 endValue, float interval, Ease easeType, Action callback)
        {
            Tweener tweener = gameObject.transform.DORotate(endValue, interval);
            tweener.SetEase(easeType);
            tweener.OnComplete(
                delegate
                {
                    callback?.Invoke();
                });
            tweener.SetLoops(-1, LoopType.Incremental);
        }
        
        public void DoScaleFrom(Vector3 fromScale, float interval, Ease easeType, Action callback)
        {
            GameObject obj = this.gameObject;
            Tweener tweener = obj.transform.DOScale(fromScale, interval);
            if (tweener != null)
            {
                tweener.From();
                tweener.SetEase(easeType);
                tweener.OnComplete(
                    delegate
                    {
                        callback?.Invoke();
                    });
            }
        }
        
        public void DoAlpha(float endValue, float interval, Action callback)
        {
            DoKillAlpha();
            Image image = gameObject.GetComponent<Image>();
            Text text = gameObject.GetComponent<Text>();
            Material material = gameObject.GetComponent<Material>();
            Tweener tweener = null;
            if (image != null)
            {
                var c = image.color;
                c.a = 1;
                image.color = c;
                tweener = image.DOFade(endValue, interval);

            }
            if (text != null)
            {
                var c = text.color;
                c.a = 1;
                text.color = c;
                tweener = text.DOFade(endValue, interval);
            }
            if (material != null)
            {
                var c = material.color;
                c.a = 1;
                material.color = c;
                tweener = material.DOFade(endValue, interval);
            }

            if (tweener != null)
            {
                tweener.Restart();
                tweener.OnComplete(
                    delegate
                    {
                        callback?.Invoke();
                    });
                tweener.SetEase(Ease.Linear);
            }
        }
        
        public void DoScale(Vector3 endscale, float interval, Ease easeType, Action callback)
        {
            Tweener tweener = gameObject.transform.DOScale(endscale, interval);
            if (tweener != null)
            {
                tweener.OnComplete(
                    delegate
                    {
                        callback?.Invoke();
                    });
                tweener.SetEase(easeType);
            }
        }
        
        public void DoLocalMove(Vector3 toPos, float interval, Ease easeType, Action callback)
        {
            Tweener tweener = gameObject.transform.DOLocalMove(toPos, interval);
            if (tweener != null)
            {
                tweener.OnComplete(
                    delegate
                    {
                        callback?.Invoke();
                    });
                tweener.SetEase(easeType);
            }
        }
        
        public void DoMove(Vector3 toPos, float interval, Ease easeType,float fDelay, Action callback)
        {
            Tweener tweener = gameObject.transform.DOMove(toPos, interval);
            if (tweener != null)
            {
                tweener.OnComplete( 
                    delegate { 
                        callback?.Invoke();
                    });
                tweener.SetDelay(fDelay);
                tweener.SetEase(easeType);
            }
        }
    }
}