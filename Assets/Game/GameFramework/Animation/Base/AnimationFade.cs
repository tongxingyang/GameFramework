using UnityEngine;

namespace GameFramework.Animation.Base
{
    public class AnimationFade
    {
        public static bool Fade(AnimationItem item)
        {
            AnimationParam param = item.parameter;
            float time = 0;
            if (param.loop)
            {
                time = item.time % param.fadeDuration;
            }
            else
            {
                time = Mathf.Min(item.time, param.fadeDuration);
            }
            if (item.parameter.spriteRender != null)
            {
                item.parameter.spriteRender.color = new Color(
                    item.parameter.spriteRender.color.r, item.parameter.spriteRender.color.g,
                    item.parameter.spriteRender.color.b, param.fadeCurve.Evaluate(time));
            }
            else if (item.parameter.fadeImage != null)
            {
                item.parameter.fadeImage.color = new Color(
                    item.parameter.fadeImage.color.r, item.parameter.fadeImage.color.g,
                    item.parameter.fadeImage.color.b, param.fadeCurve.Evaluate(time));
            }
            else if (item.parameter.canvasGroup != null)
            {
                item.parameter.canvasGroup.alpha = param.fadeCurve.Evaluate(time);
            }
            else
            {
                return true;
            }

            if (item.time >= item.parameter.fadeDuration && !item.parameter.loop)
            {
                return true;
            }
            return false;
        }
    }
}