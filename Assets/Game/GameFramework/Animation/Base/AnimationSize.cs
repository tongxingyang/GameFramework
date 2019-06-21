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
            if (param.loop)
            {
                time = item.time % param.resizeDuration;
            }
            else
            {
                time = Mathf.Min(item.time, param.resizeDuration);
            }

            transform.sizeDelta = new Vector2(param.sizeCurveX.Evaluate(time),
                param.sizeCurveY.Evaluate(time));
            if (item.time >= item.parameter.resizeDuration && !item.parameter.loop)
            {
                return true;
            }
            return false;
        }
    }
}