using UnityEngine;

namespace GameFramework.Animation.Base
{
    public class AnimationColor
    {
        public static bool Color(AnimationItem item)
        {
            AnimationParam param = item.parameter;
            float time = 0;
            if (param.loop)
            {
                time = item.time % param.colorDuration;
            }
            else
            {
                time = Mathf.Min(item.time, param.colorDuration);
            }
            if (item.parameter.image != null)
            {
                item.parameter.image.color = new Color(
                    param.colorCurveR.Evaluate(time), param.colorCurveG.Evaluate(time),
                    param.colorCurveB.Evaluate(time), param.colorCurveA.Evaluate(time));
            }
            else if (item.parameter.renderer != null)
            {
                item.parameter.renderer.color = new Color(
                    param.colorCurveR.Evaluate(time), param.colorCurveG.Evaluate(time),
                    param.colorCurveB.Evaluate(time), param.colorCurveA.Evaluate(time));
            }
            else
            {
                return true;
            }

            if (item.time >= item.parameter.colorDuration && !item.parameter.loop)
            {
                return true;
            }
            return false;
        }
    }
}