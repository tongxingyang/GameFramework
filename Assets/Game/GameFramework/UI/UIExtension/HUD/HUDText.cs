using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameFramework.UI.UIExtension.HUD
{
    public class HUDText : UIBehaviour
    {
        public Text Label;
        public Image Image;
        public CanvasGroup CanvasGrp;
        public static Action<HUDText> OnComplete = null;
        public GameObject FollowGameObject;
        public bool IsFollow;
        public bool IsPlayEndHide;
        private RectTransform rectTransform;
        public float currentTime;
        public float TotalTime;
        [HideInInspector]
        public Vector3 OffectVector3;
        [HideInInspector]
        public Vector3 StartPos;
        public MotionTextModel currentModel;
        public RectTransform RectTrans
        {
            get
            {
                if (rectTransform == null)
                    rectTransform = transform as RectTransform;
                return rectTransform ?? (rectTransform = gameObject.AddComponent<RectTransform>());
            }
        }

        public void Show(string text, Vector3 uipos, MotionTextModel config , Sprite image = null)
        {
            if (!IsActive())
                gameObject.SetActive(true);
            
            CanvasGrp.alpha = 1;
            currentModel = config;
            RectTrans.position = uipos;
            var fontScale = config.fountScale;
            RectTrans.localScale = Vector3.one * fontScale;
            Label.text = text;
    
            if (config.isUseBG && image != null)
            {
                Image.transform.localScale = Vector3.one;
                Image.sprite = image;
            }
            else
            {
                Image.rectTransform.localScale = Vector3.zero;
            }
    
            TotalTime = config.GetTotalTime();
            currentTime = 0;
        }
        
        public void OnMotionEnd()
        {
            RectTrans.localScale = Vector3.one;
            CanvasGrp.alpha = 0;
            OnComplete?.Invoke(this);
        }
    }
}