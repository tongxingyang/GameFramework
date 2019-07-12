using UnityEngine;

namespace GameFramework.Animation.Base
{
    public class AnimationScale
    {
        public static bool Scale(AnimationItem item)
        {
            AnimationParam param = item.parameter;
            float time = 0;
            if (param.playType == AnimationPlayType.Loop)
            {
                float factor = item.time / param.durationTime;
                time = factor - Mathf.Floor(factor);
            }
            else if(param.playType == AnimationPlayType.PingPong)
            {
                float factor = item.time / param.durationTime;
                bool isOdd = Mathf.FloorToInt(factor)%2 == 1;
                factor =factor - Mathf.Floor(factor); 
                time = isOdd ? 1f-factor:factor;
            }
            else if (param.playType == AnimationPlayType.Once)
            {
                time = Mathf.Clamp01(item.time / param.durationTime);
            }
            Vector3 scale = param.targetScale - param.startScale;
            item.obj.transform.localScale = new Vector3(param.startScale.x + scale.x * param.scaleCurveX.Evaluate(time),
                param.startScale.y + scale.y * param.scaleCurveY.Evaluate(time), param.startScale.z + scale.z * param.scaleCurveZ.Evaluate(time));
            
            if (item.time >= item.parameter.durationTime && item.parameter.playType == AnimationPlayType.Once)
            {
                return true;
            }
            if (item.parameter.playType == AnimationPlayType.Loop ||
                item.parameter.playType == AnimationPlayType.PingPong)
            {
                if (param.loopCount == -1 || param.loopCount == 0)
                {
                    return false;
                }
                if (item.time >= item.parameter.durationTime * item.parameter.loopCount)
                {
                    return true;
                }
            }
            return false;
        }
    }
}