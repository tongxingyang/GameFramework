using UnityEngine;

namespace GameFramework.Animation.UIAnimation
{
    public static class UIAnimationEquations
    {
        public static float Custom(float t, AnimationCurve curve)
        {
            return curve.Evaluate(t);
        }
    
        public static float Linear(float t)
        {
            return t;
        }
    
        public static float EaseIn(float t, int power)
        {
            return Mathf.Pow(t, power);
        }
        
        public static float EaseOut(float t, int power)
        {
            return 1 - Mathf.Abs(Mathf.Pow(t - 1, power));
        }
        
        public static float EaseInOut(float t, int power)
        {
            return t < 0.5f ? EaseIn(t * 2, power) / 2 : EaseOut(t * 2 - 1, power) / 2 + 0.5f;
        }
    
        public static float EaseInElastic(float t, float magnitude = 0.7f)
        {
            if (t == 0 || t == 1)
            {
                return t;
            }
    
            float scaledTime = t / 1;
            float scaledTime1 = scaledTime - 1;
    
            float p = 1 - magnitude;
            float s = p / (2 * Mathf.PI) * Mathf.Asin(1);
    
            return -(
                Mathf.Pow(2, 10 * scaledTime1) *
                Mathf.Sin((scaledTime1 - s) * (2 * Mathf.PI) / p)
            );
        }
        
        public static float EaseOutElastic(float t, float magnitude = 0.7f)
        {
            float p = 1 - magnitude;
            float scaledTime = t * 1f;
    
            if (t == 0 || t == 1)
            {
                return t;
            }
    
            float s = p / (2 * Mathf.PI) * Mathf.Asin(1);
            return (
                Mathf.Pow(2, -10 * scaledTime) *
                Mathf.Sin((scaledTime - s) * (2 * Mathf.PI) / p)
            ) + 1;
        }
        
        public static float EaseInOutElastic(float t, float magnitude = 0.7f)
        {
            float p = 1 - magnitude;
    
            if (t == 0 || t == 1)
            {
                return t;
            }
    
            float scaledTime = t * 2;
            float scaledTime1 = scaledTime - 1;
    
            float s = p / (2 * Mathf.PI) * Mathf.Asin(1);
    
            if (scaledTime < 1)
            {
                return -0.5f * (
                    Mathf.Pow(2, 10 * scaledTime1) *
                    Mathf.Sin((scaledTime1 - s) * (2 * Mathf.PI) / p)
                );
            }
    
            return (
                Mathf.Pow(2, -10 * scaledTime1) *
                Mathf.Sin((scaledTime1 - s) * (2 * Mathf.PI) / p) * 0.5f
            ) + 1;
        }
    
        public static float EaseInBounce(float t)
        {
            return 1 - EaseOutBounce(1 - t);
        }
        
        public static float EaseOutBounce(float t)
        {
            float scaledTime = t / 1;
    
            if (scaledTime < (1 / 2.75f))
            {
                return 7.5625f * scaledTime * scaledTime;
            }
            if (scaledTime < (2 / 2.75f))
            {
                float scaledTime2 = scaledTime - (1.5f / 2.75f);
                return (7.5625f * scaledTime2 * scaledTime2) + 0.75f;
            }
            if (scaledTime < (2.5f / 2.75f))
            {
                float scaledTime2 = scaledTime - (2.25f / 2.75f);
                return (7.5625f * scaledTime2 * scaledTime2) + 0.937f;
            }
            else {
                float scaledTime2 = scaledTime - (2.625f / 2.75f);
                return (7.5625f * scaledTime2 * scaledTime2) + 0.984375f;
            }
        }
        public static float EaseInOutBounce(float t)
        {
            if (t < 0.5)
                return EaseInBounce(t * 2) * 0.5f;
            else
                return (EaseOutBounce((t * 2) - 1) * 0.5f) + 0.5f;
        }
    
        public static float GetEaseFloat(float t, enMotionType type, EquationsParameters p)
        {
            float ease = 0;
            switch (type)
            {
                case enMotionType.Custom:
                    ease = Custom(t, p.Custom);
                    break;
                case enMotionType.Linear:
                    ease = Linear(t);
                    break;
                case enMotionType.EaseIn:
                    ease = EaseIn(t, p.EaseIn);
                    break;
                case enMotionType.EaseOut:
                    ease = EaseOut(t, p.EaseOut);
                    break;
                case enMotionType.EaseInOut:
                    ease = EaseInOut(t, p.EaseInOut);
                    break;
                case enMotionType.EaseInElastic:
                    ease = EaseInElastic(t, p.EaseInElastic);
                    break;
                case enMotionType.EaseOutElastic:
                    ease = EaseOutElastic(t, p.EaseOutElastic);
                    break;
                case enMotionType.EaseInOutElastic:
                    ease = EaseInOutElastic(t, p.EaseInOutElastic);
                    break;
                case enMotionType.EaseInBounce:
                    ease = EaseInBounce(t);
                    break;
                case enMotionType.EaseOutBounce:
                    ease = EaseOutBounce(t);
                    break;
                case enMotionType.EaseInOutBounce:
                    ease = EaseInOutBounce(t);
                    break;
            }
            return ease;
        }
    }
}
