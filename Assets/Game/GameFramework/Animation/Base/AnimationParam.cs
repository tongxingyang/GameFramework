using UnityEngine;
using UnityEngine.UI;

namespace GameFramework.Animation.Base
{
    [RequireComponent(typeof(AnimationCombo))]
    public class AnimationParam : MonoBehaviour
    {
        public string animationName;
        public float delay;
        public bool loop;
        [HideInInspector] public float startTime;

        #region Rotating

        [HideInInspector] public bool isRotating;
        [HideInInspector] public int angleInterval = 0;

        [HideInInspector]
        public float rotateDuration => Mathf.Max(
            rotateCurveX.length == 0 ? 0 : rotateCurveX.keys[rotateCurveX.length - 1].time,
            rotateCurveY.length == 0 ? 0 : rotateCurveY.keys[rotateCurveY.length - 1].time,
            rotateCurveZ.length == 0 ? 0 : rotateCurveZ.keys[rotateCurveZ.length - 1].time
        );

        [HideInInspector] public AnimationCurve rotateCurveX = new AnimationCurve();
        [HideInInspector] public AnimationCurve rotateCurveY = new AnimationCurve();
        [HideInInspector] public AnimationCurve rotateCurveZ = new AnimationCurve();

        #endregion

        #region Fading

        [HideInInspector] public bool isFading;
        [HideInInspector] public CanvasGroup canvasGroup;
        [HideInInspector] public SpriteRenderer spriteRender;
        [HideInInspector] public Image fadeImage;

        [HideInInspector]
        public float fadeDuration
        {
            get
            {
                if (fadeCurve.length == 0)
                {
                    return 0;
                }
                return fadeCurve.keys[fadeCurve.length - 1].time;
            }
        }

        [HideInInspector] public AnimationCurve fadeCurve = new AnimationCurve();

        #endregion

        #region Scaling

        [HideInInspector] public bool isScaling;

        [HideInInspector]
        public float scaleDuration => Mathf.Max(
            scaleCurveX.length == 0 ? 0 : scaleCurveX.keys[scaleCurveX.length - 1].time,
            scaleCurveY.length == 0 ? 0 : scaleCurveY.keys[scaleCurveY.length - 1].time,
            scaleCurveZ.length == 0 ? 0 : scaleCurveZ.keys[scaleCurveZ.length - 1].time
        );
        [HideInInspector] public AnimationCurve scaleCurveX = new AnimationCurve();
        [HideInInspector] public AnimationCurve scaleCurveY = new AnimationCurve();
        [HideInInspector] public AnimationCurve scaleCurveZ = new AnimationCurve();

        #endregion

        #region Resizing

        [HideInInspector] public bool isResizing;

        [HideInInspector]
        public float resizeDuration => Mathf.Max(
            sizeCurveX.length == 0 ? 0 : sizeCurveX.keys[sizeCurveX.length - 1].time,
            sizeCurveY.length == 0 ? 0 : sizeCurveY.keys[sizeCurveY.length - 1].time
        );

        [HideInInspector] public AnimationCurve sizeCurveX = new AnimationCurve();
        [HideInInspector] public AnimationCurve sizeCurveY = new AnimationCurve();

        #endregion

        #region Coloring

        [HideInInspector] public bool isColor;
        [HideInInspector] public SpriteRenderer renderer;
        [HideInInspector] public Image image;

        [HideInInspector]
        public float colorDuration => Mathf.Max(
            colorCurveR.length == 0 ? 0 : colorCurveR.keys[colorCurveR.length - 1].time,
            colorCurveG.length == 0 ? 0 : colorCurveG.keys[colorCurveG.length - 1].time,
            colorCurveB.length == 0 ? 0 : colorCurveB.keys[colorCurveB.length - 1].time,
            colorCurveA.length == 0 ? 0 : colorCurveA.keys[colorCurveA.length - 1].time
        );

        [HideInInspector] public AnimationCurve colorCurveR = new AnimationCurve();
        [HideInInspector] public AnimationCurve colorCurveG = new AnimationCurve();
        [HideInInspector] public AnimationCurve colorCurveB = new AnimationCurve();
        [HideInInspector] public AnimationCurve colorCurveA = new AnimationCurve();

        #endregion

        #region Moving

        [HideInInspector] public bool isMoving;
        [HideInInspector] public enPositionSpace positionSpace;
        [HideInInspector] public Vector3 startPosition;
        [HideInInspector] public Vector3 targetPosition;
        [HideInInspector] public AnimationCurve moveCurveX = new AnimationCurve();
        [HideInInspector] public AnimationCurve moveCurveY = new AnimationCurve();
        [HideInInspector] public AnimationCurve moveCurveZ = new AnimationCurve();
        [HideInInspector]
        public float moveDuration => Mathf.Max(
            moveCurveX.length == 0 ? 0 : moveCurveX.keys[moveCurveX.length - 1].time,
            moveCurveY.length == 0 ? 0 : moveCurveY.keys[moveCurveY.length - 1].time,
            moveCurveZ.length == 0 ? 0 : moveCurveZ.keys[moveCurveZ.length - 1].time
        );

        #endregion
    }
}