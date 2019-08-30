using System;
using System.Collections;
using UnityEngine;

namespace GameFramework.Animation.UIAnimation
{
    [Serializable]
    public class UIAnimationSection
    {
        public bool UseSection;
        public enMotionType HideType;
        public enMotionType ShowType;
        public bool ShowIsUseHideSameValue;
        public Vector3 WantedVectorValue;
        public float WantedFloatValue;
        public float ShowAfter;
        public float HideAfter;
        public float ShowingDuration;
        public float HidingDuration;
        [HideInInspector]
        public Vector3 startVectorValue;
        [HideInInspector]
        public float startFloatValue;
        [SerializeField]
        internal EquationsParameters hidingParameters;
        [SerializeField]
        internal EquationsParameters showingParameters;
        internal IEnumerator motionEnum;
        
        public UIAnimationSection(bool use)
        {
            UseSection = use;
            HideType = enMotionType.EaseOut;
            ShowType = enMotionType.EaseOut;
            ShowIsUseHideSameValue = true;
            ShowAfter = -1;
            HideAfter = -1;
            ShowingDuration = -1;
            HidingDuration = -1;
        }
    }
}