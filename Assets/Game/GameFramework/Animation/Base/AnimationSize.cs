using UnityEngine;

namespace GameFramework.Animation.Base
{
    public class AnimationSize
    {
        public static bool Resize(AnimationItem item)
        {
            AnimationParam param = item.parameter;
            RectTransform transform = item.obj.GetComponent<RectTransform>();
            if (transform == null)
            {
                return true;
            }
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
            Vector2 size = param.targetSize - param.startSize;
            transform.sizeDelta = new Vector2(param.startSize.x + size.x * param.sizeCurveX.Evaluate(time),
                param.startSize.y + size.y * param.sizeCurveY.Evaluate(time));
            
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