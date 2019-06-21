using UnityEngine;

namespace GameFramework.Animation.Base
{
    public class AnimationScale
    {
        public static bool Scale(AnimationItem item)
        {
            AnimationParam param = item.parameter;
            float time = 0;
            if (param.loop)
            {
                time = item.time % param.scaleDuration;
            }
            else
            {
                time = Mathf.Min(item.time, param.scaleDuration);
            }

            item.obj.transform.localScale = new Vector3(param.scaleCurveX.Evaluate(time),
                param.scaleCurveY.Evaluate(time), param.scaleCurveZ.Evaluate(time));
            if (item.time >= item.parameter.scaleDuration && !item.parameter.loop)
            {
                return true;
            }
            return false;
        }
    }
}