using UnityEngine;

namespace GameFramework.Animation.Base
{
    public class AnimationMove
    {
        public static bool Move(AnimationItem item)
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
            Vector3 position = param.targetPosition - param.startPosition;
            Vector3 currentPosition = new Vector3(
                param.targetPosition.x + position.x * param.moveCurveX.Evaluate(time),
                param.targetPosition.y + position.y * param.moveCurveY.Evaluate(time),
                param.targetPosition.z + position.z * param.moveCurveZ.Evaluate(time)
            );
            if (item.parameter.positionSpace == enPositionSpace.Self)
            {
                item.obj.transform.localPosition = currentPosition;
            }
            else if (item.parameter.positionSpace == enPositionSpace.World)
            {
                item.obj.transform.position = currentPosition;
            }
            else
            {
                var rectTransform = item.obj.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = currentPosition;
            }
//            item.obj.transform.Translate(currentPosition, Space.Self);

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