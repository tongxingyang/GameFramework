using System.Collections.Generic;
using GameFramework.Animation.UIAnimation;
using UnityEngine;

namespace GameFramework.UI.UIExtension.HUD
{
    public abstract class MotionModel : ScriptableObject
    {
        public float duration;
        public float delay;
        public abstract enHUDMotionType GetMotionType();
        public float GetTotalTime()
        {
            return duration + delay;
        }
    }

    public class MotionTextModel : ScriptableObject
    {
        public Vector3 offect;
        public float randomNum;
        public float fountScale = 1;
        public bool isOnTopLayer;
        public bool isUseBG;
        public List<MotionModel> Models = new List<MotionModel>();
        public float GetTotalTime()
        {
            float time = 0;
            foreach (MotionModel model in Models)
            {
                if (time < model.GetTotalTime())
                {
                    time = model.GetTotalTime();
                }
            }
            return time;
        }
    } 
    
    public class MotionScaleModel : MotionModel
    {
        public Vector3 startScale; 
        public Vector3 endScale; 

        public enMotionType easeType = enMotionType.Linear;
        public EquationsParameters Parameters;
        
        public override enHUDMotionType GetMotionType()
        {
            return enHUDMotionType.Scale;
        }
    }
    
    public class MotionLinearModel : MotionModel
    {
        public Vector3 startOffset = Vector3.zero;
        public Vector3 endOffset = Vector3.zero;
        public enMotionType easeType = enMotionType.Linear;
        public EquationsParameters Parameters;

        public override enHUDMotionType GetMotionType()
        {
            return enHUDMotionType.Linear;
        }
    }
    
    public class MotionAlphaModel : MotionModel
    {
        public float startAlpha;
        public float endAlpha;
        public enMotionType easeType = enMotionType.Linear;
        public EquationsParameters Parameters;
        
        public override enHUDMotionType GetMotionType()
        {
            return enHUDMotionType.Alpha;
        }
    }
}