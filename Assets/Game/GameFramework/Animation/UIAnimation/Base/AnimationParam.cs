using UnityEngine;
using UnityEngine.UI;

namespace GameFramework.Animation.Base
{
    [RequireComponent(typeof(AnimationCombo))]
    public class AnimationParam : MonoBehaviour
    {
        public string animationName;
        public float delay;
        public AnimationPlayType playType = AnimationPlayType.Once;
        public int loopCount = -1;
        public float durationTime = 1f;
        public bool isRealTime = true;
        
        #region Rotating


        [HideInInspector] public bool isRotating;
        [HideInInspector] public int angleInterval = 0;
        [HideInInspector] public Vector3 startAngle;
        [HideInInspector] public Vector3 targetAngle;
        [HideInInspector] public AnimationCurve rotateCurveX = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));
        [HideInInspector] public AnimationCurve rotateCurveY = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));
        [HideInInspector] public AnimationCurve rotateCurveZ = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));

        #endregion

        #region Fading

        [HideInInspector] public bool isFading;
        [HideInInspector] public CanvasGroup canvasGroup;
        [HideInInspector] public SpriteRenderer spriteRender;
        [HideInInspector] public Image fadeImage;
        [HideInInspector] public float startAlpha;
        [HideInInspector] public float targetAlpha;
        [HideInInspector] public AnimationCurve fadeCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));

        #endregion

        #region Scaling

        [HideInInspector] public bool isScaling;
        [HideInInspector] public Vector3 startScale;
        [HideInInspector] public Vector3 targetScale;
        [HideInInspector] public AnimationCurve scaleCurveX = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));
        [HideInInspector] public AnimationCurve scaleCurveY = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));
        [HideInInspector] public AnimationCurve scaleCurveZ = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));

        #endregion

        #region Resizing

        [HideInInspector] public bool isResizing;
        [HideInInspector] public Vector2 startSize;
        [HideInInspector] public Vector2 targetSize;

        [HideInInspector] public AnimationCurve sizeCurveX = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));
        [HideInInspector] public AnimationCurve sizeCurveY = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));

        #endregion

        #region Coloring

        [HideInInspector] public bool isColor;
        [HideInInspector] public SpriteRenderer spriteRenderer;
        [HideInInspector] public Image image;
        [HideInInspector] public Color startColor;
        [HideInInspector] public Color targetColor;
        [HideInInspector] public AnimationCurve colorCurveR = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));
        [HideInInspector] public AnimationCurve colorCurveG = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));
        [HideInInspector] public AnimationCurve colorCurveB = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));
        [HideInInspector] public AnimationCurve colorCurveA = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));

        #endregion

        #region Moving

        [HideInInspector] public bool isMoving;
        [HideInInspector] public enPositionSpace positionSpace;
        [HideInInspector] public Vector3 startPosition;
        [HideInInspector] public Vector3 targetPosition;
        [HideInInspector] public AnimationCurve moveCurveX = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));
        [HideInInspector] public AnimationCurve moveCurveY = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));
        [HideInInspector] public AnimationCurve moveCurveZ = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));

        #endregion
    }
}