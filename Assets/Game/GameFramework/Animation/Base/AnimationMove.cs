using UnityEngine;

namespace GameFramework.Animation.Base
{
    public class AnimationMove
    {
        public static bool Move(AnimationItem item)
        {
            AnimationParam param = item.parameter;
            Vector3 startPosition = item.parameter.startPosition;
            Vector3 targetPosition = item.parameter.targetPosition;
            Vector3 section = targetPosition - startPosition;
            float time = 0;
            if (param.loop)
            {
                time = item.time % param.moveDuration;
            }
            else
            {
                time = Mathf.Min(item.time, param.moveDuration);
            }
            Vector3 currentPosition = new Vector3(
                startPosition.x + section.x * param.moveCurveX.Evaluate(time),
                startPosition.y + section.y * param.moveCurveY.Evaluate(time),
                startPosition.z + section.z * param.moveCurveZ.Evaluate(time)
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
            if (item.time >= item.parameter.colorDuration && !item.parameter.loop)
            {
                return true;
            }
            return false;
        }
    }
}