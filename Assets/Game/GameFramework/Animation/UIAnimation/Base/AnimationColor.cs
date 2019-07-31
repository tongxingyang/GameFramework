using UnityEngine;

namespace GameFramework.Animation.Base
{
    public class AnimationColor
    {
        public static bool Color(AnimationItem item)
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
            Color diffColor = param.targetColor - param.startColor;
            if (item.parameter.image != null)
            {
                item.parameter.image.color = new Color(
                    param.startColor.r + diffColor.r * param.colorCurveR.Evaluate(time), param.startColor.g + diffColor.g * param.colorCurveG.Evaluate(time),
                    param.startColor.b + diffColor.b * param.colorCurveB.Evaluate(time), param.startColor.a + diffColor.a * param.colorCurveA.Evaluate(time));
            }
            else if (item.parameter.spriteRender != null)
            {
                item.parameter.spriteRender.color = new Color(
                    param.startColor.r + diffColor.r * param.colorCurveR.Evaluate(time), param.startColor.g + diffColor.g * param.colorCurveG.Evaluate(time),
                    param.startColor.b + diffColor.b * param.colorCurveB.Evaluate(time), param.startColor.a + diffColor.a * param.colorCurveA.Evaluate(time));
            }
            else
            {
                return true;
            }

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