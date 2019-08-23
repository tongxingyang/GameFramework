using System;
using UnityEngine;
using UnityEngine.UI;

public class ImageProgressBar : MonoBehaviour {

    public Image ProgressBar;
    public Image ProgressBarSlider;
    public Image ProgressBarHandle;
    public Text LoadingPercent;
    public Text ProgressComment;
    float _fillSpeed;
    private bool isUpdateProgress = false;
    
    public float CurrentProgressValue = 0;
    public string CurrentProgressComment = string.Empty;
    public Action OnDoneAction = null;

    public void ShowProgressBar(Action action,string comment ,float startValue = 0.0f)
    {
        ProgressBar.gameObject.SetActive(true);
        ProgressBarSlider.fillAmount = startValue;
        CurrentProgressValue = startValue;
        ProgressBarHandle.transform.localEulerAngles  = Vector3.zero;
        OnDoneAction = action;
        CurrentProgressComment = comment;
        isUpdateProgress = true;
    }

    
    void Update () {
        
        if (isUpdateProgress)
        {
            if (ProgressBarSlider.fillAmount >= 1)
            {
                OnDoneAction.Invoke();
                OnDoneAction = null;
                isUpdateProgress = false;
                return;
            }

            if (ProgressComment.text != CurrentProgressComment)
            {
                ProgressComment.text = CurrentProgressComment;
            }
        
            _fillSpeed = ProgressBarSlider.fillAmount < CurrentProgressValue ? 0.5f : 0.03f * (1f - CurrentProgressValue + 0.01f);
            ProgressBarSlider.fillAmount += Time.deltaTime * _fillSpeed;
            ProgressBarSlider.transform.localEulerAngles = new Vector3(0,0,ProgressBarSlider.fillAmount*-360);
            float v = ProgressBarSlider.fillAmount * 100;
            LoadingPercent.text = "" + v.ToString("F0") + " %";
        }
    }
}
