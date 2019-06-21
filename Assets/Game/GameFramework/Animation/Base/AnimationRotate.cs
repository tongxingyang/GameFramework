using UnityEngine;

namespace GameFramework.Animation.Base
{
    public class AnimationRotate
    {
        public static float Multiplier = 360;
        public static bool Rotate(AnimationItem item)
        {
            AnimationParam param = item.parameter;
            float time = 0;
            if (param.loop)
            {
                time = item.time % param.rotateDuration;
            }
            else
            {
                time = Mathf.Min(item.time, param.rotateDuration);
            }
            int interval = item.parameter.angleInterval;
            Vector3 changeAngle = new Vector3(
                param.rotateCurveX.Evaluate(time) * Multiplier,
                param.rotateCurveY.Evaluate(time) * Multiplier,
                param.rotateCurveZ.Evaluate(time) * Multiplier
                );
            if (interval != 0) {
                changeAngle = new Vector3(
                    ((int)changeAngle.x / interval) * interval,
                    ((int)changeAngle.y / interval) * interval,
                    ((int)changeAngle.z / interval) * interval
                );
            }
            item.obj.transform.localEulerAngles = changeAngle;
            if (item.time >= item.parameter.rotateDuration && !item.parameter.loop)
            {
                return true;
            }
            return false;
        }
    }
}