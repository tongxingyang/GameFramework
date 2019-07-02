using UnityEngine;

namespace GameFramework.Animation.Base
{
    public class AnimationRotate
    {
        public static bool Rotate(AnimationItem item)
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
            Vector3 angle = param.targetAngle - param.startAngle;
            int interval = item.parameter.angleInterval;
            Vector3 changeAngle = new Vector3(
                angle.x * param.rotateCurveX.Evaluate(time),
                angle.y * param.rotateCurveY.Evaluate(time),
                angle.z * param.rotateCurveZ.Evaluate(time)
                );
            if (interval != 0) {
                changeAngle = new Vector3(
                    ((int)changeAngle.x / interval) * interval,
                    ((int)changeAngle.y / interval) * interval,
                    ((int)changeAngle.z / interval) * interval
                );
            }
            
            item.obj.transform.rotation = Quaternion.Euler(
                item.parameter.startAngle + changeAngle
            );
           
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