using UnityEngine;

namespace GameFramework.Animation.Base
{
    public class AnimationFade
    {
        public static bool Fade(AnimationItem item)
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
            float alpha = param.targetAlpha - param.startAlpha;
            if (item.parameter.spriteRender != null)
            {
                item.parameter.spriteRender.color = new Color(
                    item.parameter.spriteRender.color.r, item.parameter.spriteRender.color.g,
                    item.parameter.spriteRender.color.b, param.startAlpha + alpha * param.fadeCurve.Evaluate(time));
            }
            else if (item.parameter.fadeImage != null)
            {
                item.parameter.fadeImage.color = new Color(
                    item.parameter.fadeImage.color.r, item.parameter.fadeImage.color.g,
                    item.parameter.fadeImage.color.b, param.startAlpha + alpha * param.fadeCurve.Evaluate(time));
            }
            else if (item.parameter.canvasGroup != null)
            {
                item.parameter.canvasGroup.alpha = param.startAlpha + alpha * param.fadeCurve.Evaluate(time);
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