using System;
using UnityEngine;

namespace GameFramework.Animation.UIAnimation
{
    [Serializable]
    public class EquationsParameters
    {
        public AnimationCurve Custom = AnimationCurve.Linear(0,0,1,1);
        [Range(2, 25)]
        public int EaseIn = 2;
        [Range(2, 25)]
        public int EaseOut;
        [Range(2, 25)]
        public int EaseInOut;
        [Range(0.1f, 0.9f)]
        public float EaseInElastic;
        [Range(0.1f, 0.9f)]
        public float EaseOutElastic;
        [Range(0.1f, 0.9f)]
        public float EaseInOutElastic;
    }
}