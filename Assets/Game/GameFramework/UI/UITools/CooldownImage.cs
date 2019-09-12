using System;
using GameFramework.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework.UI.UITools
{
    public class CooldownImage : MonoBehaviour
    {
        private Image coolDownImage;
        private float maxTime = 0;
        private float endTime = 0;
        private Action onFinish = null;
        private Text cdTimeText;
        private int lastSeceond = 0;
        private bool isReverse = false;
        private float currentTime = 0;
        private float fillAmount = 0;
        private float lastSec = 0;

        void Start()
        {
            if (coolDownImage == null)
            {
                coolDownImage = gameObject.GetComponent<Image>();
            }
            gameObject.SetActive(false);
        }
        
        public void SetParam(float max, float end, Text timeLabel, Action cb, bool isreverse = false)
        {
            gameObject.SetActive(true);
            isReverse = isreverse;
            maxTime = max;
            endTime = end + Time.time;
            lastSeceond = 0;
            cdTimeText = timeLabel.GetComponent<Text>();
            onFinish = cb;
        }

        void Update()
        {
            currentTime = Time.time;
            fillAmount = (endTime - currentTime) / maxTime;
            if (isReverse)
            {
                coolDownImage.fillAmount = 1 - fillAmount;
            }
            else
            {
                coolDownImage.fillAmount = fillAmount;
            }
            lastSec = Mathf.Abs(endTime - currentTime);
            if (lastSec <= 0.01f || currentTime > endTime)
            {
                if (cdTimeText != null && cdTimeText.gameObject.activeSelf)
                {
                    cdTimeText.text = "";
                    cdTimeText.gameObject.SetActive(false);
                }
                onFinish?.Invoke();
                onFinish = null;
                gameObject.SetActive(false);
            }
            else
            {
                if (cdTimeText != null)
                {
                    if (!cdTimeText.gameObject.activeSelf)
                    {
                        cdTimeText.gameObject.SetActive(true);
                    }
                    if (!Equals(lastSeceond, Mathf.CeilToInt(lastSec)))
                    {
                        lastSeceond = Mathf.CeilToInt(lastSec);
                        cdTimeText.text = StringUtility.Format("{0}", Mathf.CeilToInt(lastSec));
                    }
                }
            }
        }
    }
}