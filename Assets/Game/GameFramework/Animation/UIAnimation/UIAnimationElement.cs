using System.Collections;
using GameFramework.Tool;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameFramework.Animation.UIAnimation
{
    public class UIAnimationElement : UIAnimationElementBase
    {
        public bool IsDependent = true;
        public bool Prewarm = true;
        public bool DeactivateWhileInvisible;
        public float ShowAfter = 0.0f;
        public float HideAfter = 0.0f;
        public float HidingDuration = 0.5f;
        public float ShowingDuration = 0.5f;
        public bool UseUnscaledTime = false;
        public UIAnimationElementBase ControlledBy;
        [SerializeField]
        private bool ShowIsUseHideDuration = true;
        private RectTransform selfRectTransform;
        private CanvasScaler parentCanvasScaler;
        private RectTransform parentCanvasRectTransform;
        private RectTransform directParentRectTransform;
        private float canvasHalfWidth;
        private float canvasHalfHeight;
        private float directParentHalfWidth;
        private float directParentHalfHeight;
        private float selfRectTransformWidth;
        private float selfRectTransformHeight;
        private bool forceVisibilityCall;
        private float hidingTime;
        private float showingTime;
        private IEnumerator startEventEnum;
        private IEnumerator completeEventEnum;
        
        #region Movement

        public UIAnimationSection MovementSection;
        public enScreenSides HidingPosition;
        public float EdgeGap = 0.25f;
        public bool LocalCustomPosition = true;
        private Vector3 outOfScreenPos;

        #endregion

        #region Rotation

        public UIAnimationSection RotationSection;
        public enRotationDirection ShowingDirection;
        public enRotationDirection HidingDirection;

        #endregion

        #region Scale

        public UIAnimationSection ScaleSection;

        #endregion

        #region Opacity

        public UIAnimationSection OpacitySection;
        public Component TargetFader;

        #endregion

        #region Slice

        public UIAnimationSection SliceSection;
        [HideInInspector]
        public Image SliceImage;

        #endregion

        private IEnumerator activationEnum;

        void Start()
        {
            if (!Initialized)
            {
                Initialize();
            }
            if (IsDependent)
            {
                return;
            }
            if (!forceVisibilityCall)
            {
                if (Prewarm)
                {
                    ChangeVisibilityImmediate(Visible);
                }
                else
                {
                    ChangeVisibility(Visible);
                }
            }
        }

        public void Initialize()
        {
            if (Initialized) return;
            if (DeactivateWhileInvisible)
            {
                OnHideComplete.AddListener(() =>
                {
                    if (!Visible)
                    {
                        gameObject.SetActive(Visible);
                    }
                });
            }
            hidingTime = GetAllHidingTime();
            showingTime = GetAllShowingTime();
            selfRectTransform = GetComponent<RectTransform>();
            parentCanvasScaler = GetComponentInParent<CanvasScaler>();
            parentCanvasRectTransform = parentCanvasScaler.GetComponent<RectTransform>();
            if (transform.parent)
                directParentRectTransform = transform.parent.GetComponent<RectTransform>();

            Vector2 canvasLossyScale = parentCanvasRectTransform.lossyScale;
            canvasHalfWidth = canvasLossyScale.x * parentCanvasRectTransform.rect.width / 2;
            canvasHalfHeight = canvasLossyScale.y * parentCanvasRectTransform.rect.height / 2;
            if (directParentRectTransform)
            {
                directParentHalfWidth = canvasLossyScale.x * directParentRectTransform.rect.width / 2;
                directParentHalfHeight = canvasLossyScale.y * directParentRectTransform.rect.height / 2;
            }
            selfRectTransformWidth = canvasLossyScale.x * selfRectTransform.rect.width;
            selfRectTransformHeight = canvasLossyScale.y * selfRectTransform.rect.height;
            MovementSection.startVectorValue = selfRectTransform.position;
            outOfScreenPos = GetHidingPosition(HidingPosition, EdgeGap, MovementSection.WantedVectorValue, LocalCustomPosition);
            RotationSection.startVectorValue = selfRectTransform.eulerAngles;
            ScaleSection.startVectorValue = selfRectTransform.localScale;
            FindTargetFader();
            FindSliceImage();
            Initialized = true;
        }
        
        public float GetAllHidingTime()
        {
            if (hidingTime != 0)
                return hidingTime;

            if (HideAfter + HidingDuration > hidingTime)
                hidingTime = HideAfter + HidingDuration;

            float movementDuration = (MovementSection.HidingDuration > 0 ? MovementSection.HidingDuration : HidingDuration);
            if (MovementSection.HideAfter + movementDuration > hidingTime)
                hidingTime = MovementSection.HideAfter + movementDuration;

            float rotationDuration = (RotationSection.HidingDuration > 0 ? RotationSection.HidingDuration : HidingDuration);
            if (RotationSection.HideAfter + rotationDuration > hidingTime)
                hidingTime = RotationSection.HideAfter + rotationDuration;

            float scaleDuration = (ScaleSection.HidingDuration > 0 ? ScaleSection.HidingDuration : HidingDuration);
            if (ScaleSection.HideAfter + scaleDuration > hidingTime)
                hidingTime = ScaleSection.HideAfter + scaleDuration;

            float opacityDuration = (OpacitySection.HidingDuration > 0 ? OpacitySection.HidingDuration : HidingDuration);
            if (OpacitySection.HideAfter + opacityDuration > hidingTime)
                hidingTime = OpacitySection.HideAfter + opacityDuration;
            return hidingTime;
        }
        
        public float GetAllShowingTime()
        {
            if (showingTime != 0)
                return showingTime;

            if (ShowAfter + ShowingDuration > showingTime)
                showingTime = ShowAfter + ShowingDuration;

            float movementDuration = (MovementSection.ShowingDuration > 0 ? MovementSection.ShowingDuration : ShowingDuration);
            if (MovementSection.ShowAfter + movementDuration > showingTime)
                showingTime = MovementSection.ShowAfter + movementDuration;

            float rotationDuration = (RotationSection.ShowingDuration > 0 ? RotationSection.ShowingDuration : ShowingDuration);
            if (RotationSection.ShowAfter + rotationDuration > showingTime)
                showingTime = RotationSection.ShowAfter + rotationDuration;

            float scaleDuration = (ScaleSection.ShowingDuration > 0 ? ScaleSection.ShowingDuration : ShowingDuration);
            if (ScaleSection.ShowAfter + scaleDuration > showingTime)
                showingTime = ScaleSection.ShowAfter + scaleDuration;

            float opacityDuration = (OpacitySection.ShowingDuration > 0 ? OpacitySection.ShowingDuration : ShowingDuration);
            if (OpacitySection.ShowAfter + opacityDuration > showingTime)
                showingTime = OpacitySection.ShowAfter + opacityDuration;
            return showingTime;
        }
        
        Vector3 GetHidingPosition(enScreenSides hidingPos, float edgeGap, Vector2 customPosition, bool customLocal)
        {
            Vector3 pos = new Vector3();
            float y = 0;
            float x = 0;
            Vector2 distanceToEdge = new Vector2(selfRectTransform.pivot.x, selfRectTransform.pivot.y);
            Vector3 originalPosition = MovementSection.startVectorValue;
    
            switch (hidingPos)
            {
                case enScreenSides.Top:
                    y = parentCanvasRectTransform.position.y + canvasHalfHeight + selfRectTransformHeight * (distanceToEdge.y + edgeGap);
                    pos = new Vector3(originalPosition.x, y, originalPosition.z);
                    break;
                case enScreenSides.Bottom:
                    y = parentCanvasRectTransform.position.y - canvasHalfHeight - selfRectTransformHeight * (1 - distanceToEdge.y + edgeGap);
                    pos = new Vector3(originalPosition.x, y, originalPosition.z);
                    break;
                case enScreenSides.Left:
                    x = parentCanvasRectTransform.position.x - canvasHalfWidth - selfRectTransformWidth * (1 - distanceToEdge.x + edgeGap);
                    pos = new Vector3(x, originalPosition.y, originalPosition.z);
                    break;
                case enScreenSides.Right:
                    x = parentCanvasRectTransform.position.x + canvasHalfWidth + selfRectTransformWidth * (distanceToEdge.x + edgeGap);
                    pos = new Vector3(x, originalPosition.y, originalPosition.z);
                    break;
                case enScreenSides.TopLeftCorner:
                    y = parentCanvasRectTransform.position.y + canvasHalfHeight + selfRectTransformHeight * (distanceToEdge.y + edgeGap);
                    x = parentCanvasRectTransform.position.x - canvasHalfWidth - selfRectTransformWidth * (1 - distanceToEdge.x + edgeGap);
                    pos = new Vector3(x, y, originalPosition.z);
                    break;
                case enScreenSides.TopRightCorner:
                    y = parentCanvasRectTransform.position.y + canvasHalfHeight + selfRectTransformHeight * (distanceToEdge.y + edgeGap);
                    x = parentCanvasRectTransform.position.x + canvasHalfWidth + selfRectTransformWidth * (distanceToEdge.x + edgeGap);
                    pos = new Vector3(x, y, originalPosition.z);
                    break;
                case enScreenSides.BotLeftCorner:
                    y = parentCanvasRectTransform.position.y - canvasHalfHeight - selfRectTransformHeight * (1 - distanceToEdge.y + edgeGap);
                    x = parentCanvasRectTransform.position.x - canvasHalfWidth - selfRectTransformWidth * (1 - distanceToEdge.x + edgeGap);
                    pos = new Vector3(x, y, originalPosition.z);
                    break;
                case enScreenSides.BotRightCorner:
                    y = parentCanvasRectTransform.position.y - canvasHalfHeight - selfRectTransformHeight * (1 - distanceToEdge.y + edgeGap);
                    x = parentCanvasRectTransform.position.x + canvasHalfWidth + selfRectTransformWidth * (distanceToEdge.x + edgeGap);
                    pos = new Vector3(x, y, originalPosition.z);
                    break;
                case enScreenSides.Custom:
                    Vector3 holderPos;
                    float holderHalfWidth = 0;
                    float holderHalfHeight = 0;
    
                    if (customLocal && directParentRectTransform)
                    {
                        holderPos = directParentRectTransform.position;
                        holderHalfWidth = directParentHalfWidth;
                        holderHalfHeight = directParentHalfHeight;
                    }
                    else
                    {
                        holderPos = parentCanvasRectTransform.position;
                        holderHalfWidth = canvasHalfWidth;
                        holderHalfHeight = canvasHalfHeight;
                    }
                    pos = new Vector3(
                        holderPos.x + (customPosition.x - 0.5f) * holderHalfWidth * 2,
                        holderPos.y + (customPosition.y - 0.5f) * holderHalfHeight * 2, originalPosition.z);
                    break;
            }
            return pos;
        }
        
        public void FindTargetFader()
        {
            if (TargetFader)
            {
                if (TargetFader is Graphic)
                {
                    Graphic tf = (Graphic) TargetFader;
                    OpacitySection.startFloatValue = tf.color.a;
                }
                else if (TargetFader is CanvasGroup)
                {
                    CanvasGroup tf = (CanvasGroup) TargetFader;
                    OpacitySection.startFloatValue = tf.alpha;
                }
            }
            else
            {
                TargetFader = GetComponent<Graphic>();
                if (TargetFader)
                    OpacitySection.startFloatValue = ((Graphic)TargetFader).color.a;
                else
                {
                    TargetFader = GetComponent<CanvasGroup>();
                    if (TargetFader)
                        OpacitySection.startFloatValue = ((CanvasGroup)TargetFader).alpha;
                    else if (OpacitySection.UseSection)
                        UnityEngine.Debug.LogError("没有 Image, Text, RawImage ,CanvasGroup component " + gameObject.name);
                }
            }
        }
        
        public void FindSliceImage()
        {
            if (SliceImage)
            {
                SliceSection.startFloatValue = SliceImage.fillAmount;
            }
            else
            {
                SliceImage = GetComponent<Image>();
                if (SliceImage)
                    SliceSection.startFloatValue = SliceImage.fillAmount;
                else if (SliceSection.UseSection)
                    UnityEngine.Debug.LogError("没有 Image component on " + gameObject.name);
            }
        }

        public override void ChangeVisibility(bool visible, bool ignoreEvent = false)
        {
            forceVisibilityCall = true;
    
            if (!Initialized)
                Initialize();
    
            if (!gameObject.activeSelf)
            {
                if (!UseSimpleActivation)
                {
                    if (MovementSection.UseSection)
                    {
                        Vector3 ePos = visible ? MovementSection.startVectorValue : outOfScreenPos;
                        selfRectTransform.position = ePos;
                    }
                    if (RotationSection.UseSection)
                    {
                        Vector3 eEuler = visible ? RotationSection.startVectorValue : RotationSection.WantedVectorValue;
                        selfRectTransform.eulerAngles = eEuler;
                    }
                    if (ScaleSection.UseSection)
                    {
                        Vector3 eScale = visible ? ScaleSection.startVectorValue : ScaleSection.WantedVectorValue;
                        selfRectTransform.localScale = eScale;
                    }
                    if (OpacitySection.UseSection)
                    {
                        float eOpacity = visible ? OpacitySection.startFloatValue : OpacitySection.WantedFloatValue;
                        if (TargetFader)
                        {
                            if (TargetFader is Graphic)
                            {
                                Graphic tf = (Graphic) TargetFader;
                                Color col = tf.color;
                                col.a = eOpacity;
                                tf.color = col;
                            }
                            else if (TargetFader is CanvasGroup)
                            {
                                CanvasGroup tf = (CanvasGroup) TargetFader;
                                tf.alpha = eOpacity;
                            }
                        }
                    }
                    if (SliceSection.UseSection)
                    {
                        float eFill = visible ? SliceSection.startFloatValue : SliceSection.WantedFloatValue;
                        if (SliceImage)
                            SliceImage.fillAmount = eFill;
                    }
                }
                gameObject.SetActive(true);
            }
    
            Visible = visible;
            if (!ignoreEvent)
            {
                if (startEventEnum != null)
                {
                    StopCoroutine(startEventEnum);
                    startEventEnum = null;
                }

                if (completeEventEnum != null)
                {
                    StopCoroutine(completeEventEnum);
                    completeEventEnum = null;
                }
    
                if (visible)
                {
                    if (OnShow != null)
                        startEventEnum = Yielders.DelayCallEvent(OnShow, ShowAfter);
                    if (OnShowComplete != null)
                        completeEventEnum = Yielders.DelayCallEvent(OnShowComplete, showingTime);
                }
                else
                {
                    if (OnHide != null)
                        startEventEnum = Yielders.DelayCallEvent(OnHide, HideAfter);
                    if (OnHideComplete != null)
                        completeEventEnum = Yielders.DelayCallEvent(OnHideComplete, hidingTime);
                }

                if (gameObject.activeInHierarchy)
                {
                    if (startEventEnum != null)
                        StartCoroutine(startEventEnum);
                    if (completeEventEnum != null)
                        StartCoroutine(completeEventEnum);
                }
            }
    
            if (UseSimpleActivation)
            {
                gameObject.SetActive(visible);
                if (startEventEnum != null)
                {
                    StopCoroutine(startEventEnum);
                    startEventEnum = null;
                }

                if (completeEventEnum != null)
                {
                    StopCoroutine(completeEventEnum);
                    completeEventEnum = null;
                }
                if (!ignoreEvent)
                {
                    if (visible)
                    {
                        OnShow?.Invoke();
                        OnShowComplete?.Invoke();
                    }
                    else
                    {
                        OnHide?.Invoke();
                        OnHideComplete?.Invoke();
                    }
                }
                return;
            }
    
            if (MovementSection.UseSection)
            {
                enMotionType type = GetSectionType(MovementSection);
                float duration = GetSectionDuration(MovementSection);
                EquationsParameters easingParams = GetEasingParams(MovementSection);
                ControlMovement(visible, type, HidingPosition, duration, easingParams, EdgeGap, MovementSection.WantedVectorValue, LocalCustomPosition);
            }
            if (RotationSection.UseSection)
            {
                enMotionType type = GetSectionType(RotationSection);
                float duration = GetSectionDuration(RotationSection);
                EquationsParameters easingParams = GetEasingParams(RotationSection);
                ControlRotation(visible, type, visible ? ShowingDirection : HidingDirection, RotationSection.WantedVectorValue, duration, easingParams);
            }
            if (ScaleSection.UseSection)
            {
                enMotionType type = GetSectionType(ScaleSection);
                float duration = GetSectionDuration(ScaleSection);
                EquationsParameters easingParams = GetEasingParams(ScaleSection);
                ControlScale(visible, type, ScaleSection.WantedVectorValue, duration, easingParams);
            }
            if (OpacitySection.UseSection)
            {
                enMotionType type = GetSectionType(OpacitySection);
                float duration = GetSectionDuration(OpacitySection);
                EquationsParameters easingParams = GetEasingParams(OpacitySection);
                ControlOpacity(visible, type, OpacitySection.WantedFloatValue, duration, easingParams);
            }
            if (SliceSection.UseSection)
            {
                enMotionType type = GetSectionType(SliceSection);
                float duration = GetSectionDuration(SliceSection);
                EquationsParameters easingParams = GetEasingParams(SliceSection);
                ControlSlice(visible, type, SliceSection.WantedFloatValue, duration, easingParams);
            }
        }

        public override void ChangeVisibilityImmediate(bool visible, bool ignoreEvent = false)
        {
            forceVisibilityCall = true;

            if (!Initialized)
                Initialize();
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);
    
            Visible = visible;
            if (!ignoreEvent)
            {
                if (visible)
                {
                    OnShow?.Invoke();
                    OnShowComplete?.Invoke();
                }
                else
                {
                    OnHide?.Invoke();
                    OnHideComplete?.Invoke();
                }
            }
    
            if (UseSimpleActivation)
            {
                gameObject.SetActive(visible);
                return;
            }
            if (MovementSection.UseSection)
            {
                Vector3 ePos = visible ? MovementSection.startVectorValue : outOfScreenPos;
                selfRectTransform.position = ePos;
            }
            if (RotationSection.UseSection)
            {
                Vector3 eEuler = visible ? RotationSection.startVectorValue : RotationSection.WantedVectorValue;
                selfRectTransform.eulerAngles = eEuler;
            }
            if (ScaleSection.UseSection)
            {
                Vector3 eScale = visible ? ScaleSection.startVectorValue : ScaleSection.WantedVectorValue;
                selfRectTransform.localScale = eScale;
            }
            if (OpacitySection.UseSection)
            {
                float eOpacity = visible ? OpacitySection.startFloatValue : OpacitySection.WantedFloatValue;
                if (TargetFader)
                {
                    if (TargetFader is Graphic)
                    {
                        Graphic tf = (Graphic) TargetFader;
                        Color col = tf.color;
                        col.a = eOpacity;
                        tf.color = col;
                    }
                    else if (TargetFader is CanvasGroup)
                    {
                        CanvasGroup tf = (CanvasGroup) TargetFader;
                        tf.alpha = eOpacity;
                    }
                }
            }
            if (SliceSection.UseSection)
            {
                float eFill = visible ? SliceSection.startFloatValue : SliceSection.WantedFloatValue;
                if (SliceImage)
                    SliceImage.fillAmount = eFill;
            }
    
            if (DeactivateWhileInvisible && !visible)
                gameObject.SetActive(false);
        }
        
        enMotionType GetSectionType(UIAnimationSection s)
        {
            return !Visible ? s.HideType : (s.ShowIsUseHideSameValue ? s.HideType : s.ShowType);
        }
        
        float GetSectionDuration(UIAnimationSection s)
        {
            float hidingDuration = s.HidingDuration < 0 ? HidingDuration : s.HidingDuration;
            float showingDuration = s.ShowingDuration < 0 ? (ShowIsUseHideDuration ? HidingDuration : ShowingDuration) : s.ShowingDuration;
            return !Visible ? hidingDuration : showingDuration;
        }
        
        EquationsParameters GetEasingParams(UIAnimationSection s)
        {
            return !Visible ? s.hidingParameters : (s.ShowIsUseHideSameValue ? s.hidingParameters : s.showingParameters);
        }
        
        public Vector3 UnClampedLerp(Vector3 a, Vector3 b, float t)
        {
            return new Vector3(UnClampedLerp(a.x, b.x, t), UnClampedLerp(a.y, b.y, t), UnClampedLerp(a.z, b.z, t));
        }
        
        public float UnClampedLerp(float a, float b, float t)
        {
            return t * b + (1 - t) * a;
        }
        
        public void ControlMovement(bool visible, enMotionType motionType, enScreenSides side, float duration, EquationsParameters easingParams, float edgeGap = 0.25f,  Vector3 customPosition = new Vector3(), bool customLocal = false)
        {
            Vector3 outPos = outOfScreenPos;

            if (side != HidingPosition || edgeGap != EdgeGap || (side == enScreenSides.Custom && customLocal != LocalCustomPosition))
            {
                outPos = GetHidingPosition(side, edgeGap, customPosition, customLocal);
            }

            Vector3 ePos = visible ? MovementSection.startVectorValue : outPos;
            Vector3 sPos = selfRectTransform.position;

            if (!gameObject.activeInHierarchy)
            {
                selfRectTransform.position = ePos;
                return;
            }

            if (MovementSection.motionEnum != null)
            {
                StopCoroutine(MovementSection.motionEnum);
                MovementSection.motionEnum = null;
            }

            MovementSection.motionEnum = VectorMotion((v)=> {
                    selfRectTransform.position = v;
                }, sPos, ePos, MovementSection.HideAfter, MovementSection.ShowAfter,
                duration, easingParams, motionType);
            StartCoroutine(MovementSection.motionEnum);
        }
        
        public void ControlRotation(bool visible, enMotionType motionType, enRotationDirection direction, Vector3 euler, float duration, EquationsParameters easingParams)
        {
            euler = ClampAngleVector(euler);
            Vector3 eEuler = visible ? RotationSection.startVectorValue : euler;
            Vector3 sEuler = selfRectTransform.eulerAngles;

            if (eEuler.z > sEuler.z)
            {
                if (direction == enRotationDirection.ClockWise)
                {
                    eEuler -= new Vector3(eEuler.x > 0 ? 360 : 0, eEuler.y > 0 ? 360 : 0, eEuler.z > 0 ? 360 : 0);
                }
            }
            else
            {
                if (direction == enRotationDirection.AntiClockWise)
                {
                    sEuler -= new Vector3(sEuler.x > 0 ? 360 : 0, sEuler.y > 0 ? 360 : 0, sEuler.z > 0 ? 360 : 0);
                }
            }

            if (!gameObject.activeInHierarchy)
            {
                selfRectTransform.eulerAngles = eEuler;
                return;
            }

            if (RotationSection.motionEnum != null)
            {
                StopCoroutine(RotationSection.motionEnum);
                RotationSection.motionEnum = null; 
            }

            RotationSection.motionEnum = VectorMotion((v)=> { selfRectTransform.eulerAngles = v; }, sEuler, eEuler, RotationSection.HideAfter, RotationSection.ShowAfter, duration, easingParams, motionType);
            StartCoroutine(RotationSection.motionEnum);

        }

        Vector3 ClampAngleVector(Vector3 vec)
        {
            if (vec.x < 0) vec.x += 360;
            if (vec.y < 0) vec.y += 360;
            if (vec.z < 0) vec.z += 360;
            
            if (vec.x > 360) vec.x -= 360;
            if (vec.y > 360) vec.y -= 360;
            if (vec.z > 360) vec.z -= 360;

            if (vec.x < 0 || vec.x > 360 || vec.y < 0 || vec.y > 360 || vec.z < 0 || vec.z > 360)
                vec = ClampAngleVector(vec);
            return vec;
        }
        
        public void ControlScale(bool visible, enMotionType motionType, Vector3 scale, float duration, EquationsParameters easingParams)
        {
            Vector3 eScale = visible ? ScaleSection.startVectorValue : scale;
            Vector3 sScale = selfRectTransform.localScale;

            if (!gameObject.activeInHierarchy)
            {
                selfRectTransform.localScale = eScale;
                return;
            }

            if (ScaleSection.motionEnum != null)
            {
                StopCoroutine(ScaleSection.motionEnum);
                ScaleSection.motionEnum = null;
            }

            ScaleSection.motionEnum = VectorMotion((v)=> { selfRectTransform.localScale = v; }, sScale, eScale, ScaleSection.HideAfter, ScaleSection.ShowAfter, duration, easingParams, motionType);
            StartCoroutine(ScaleSection.motionEnum);
        }
        
        public void ControlOpacity(bool visible, enMotionType motionType, float opac, float duration, EquationsParameters easingParams)
        {
            if (!TargetFader)
                FindTargetFader();

            if (!TargetFader)
                return;

            float eOpacity = visible ? OpacitySection.startFloatValue : opac;
            float sOpacity = 0;
            if (!gameObject.activeInHierarchy)
            {
                if (TargetFader is Graphic)
                {
                    Graphic tf = TargetFader as Graphic;
                    Color col = tf.color;
                    col.a = eOpacity;
                    tf.color = col;
                }
                else if (TargetFader is CanvasGroup)
                {
                    CanvasGroup tf = TargetFader as CanvasGroup;
                    tf.alpha = eOpacity;
                }
                return;
            }

            if (TargetFader is Graphic)
            {
                Graphic tf = (Graphic) TargetFader;
                sOpacity = tf.color.a;
            }
            else if (TargetFader is CanvasGroup)
            {
                CanvasGroup tf = (CanvasGroup) TargetFader;
                sOpacity = tf.alpha;
            }

            if (OpacitySection.motionEnum != null)
            {
                StopCoroutine(OpacitySection.motionEnum);
                OpacitySection.motionEnum = null;
            }

            OpacitySection.motionEnum = FloatMotion((f) => {
                if (TargetFader is Graphic)
                {
                    Graphic tf = (Graphic) TargetFader;
                    Color col = tf.color;
                    col.a = f;
                    tf.color = col;
                }
                else if (TargetFader is CanvasGroup)
                {
                    CanvasGroup tf = (CanvasGroup) TargetFader;
                    tf.alpha = f;
                }
            }, sOpacity, eOpacity, OpacitySection.HideAfter, OpacitySection.ShowAfter, duration, easingParams, motionType);

            StartCoroutine(OpacitySection.motionEnum);
        }
        
        public void ControlSlice(bool visible, enMotionType motionType, float fill, float duration, EquationsParameters easingParams)
        {
            if (!SliceImage)
                FindSliceImage();

            if (!SliceImage)
                return;

            float eFill = visible ? SliceSection.startFloatValue : fill;
            float sFill = 0;

            if (!gameObject.activeInHierarchy)
            {
                SliceImage.fillAmount = eFill;
                return;
            }
            sFill = SliceImage.fillAmount;

            if (SliceSection.motionEnum != null)
            {
                StopCoroutine(SliceSection.motionEnum);
                SliceSection.motionEnum = null;
            }

            SliceSection.motionEnum = FloatMotion((f) => { SliceImage.fillAmount = f; }, sFill, eFill, SliceSection.HideAfter, 
                SliceSection.ShowAfter, duration, easingParams, motionType);

            StartCoroutine(SliceSection.motionEnum);
        }
        
        IEnumerator VectorMotion(UnityAction<Vector3> output, Vector3 start, Vector3 end, float sectionHideAfter, float sectionShowAfter, float duration, EquationsParameters easingParams, enMotionType motionType)
        {
            float startAfter = Visible ? (sectionShowAfter < 0 ? ShowAfter : sectionShowAfter) : (sectionHideAfter < 0 ? HideAfter : sectionHideAfter);
            yield return Yielders.GetWaitForSeconds(startAfter);
            float curTime = UseUnscaledTime ? Time.unscaledTime : Time.time;
            float startTime = curTime;
            while (curTime < startTime + duration)
            {
                float t = (curTime - startTime) / duration;
                float ease = UIAnimationEquations.GetEaseFloat(t, motionType, easingParams);
                output(UnClampedLerp(start, end, ease));
                yield return null;
                curTime = UseUnscaledTime ? Time.unscaledTime : Time.time;
            }
            output(end);
        }
        
        IEnumerator FloatMotion(UnityAction<float> output, float start, float end, float sectionHideAfter, float sectionShowAfter, float duration, EquationsParameters easingParams, enMotionType motionType)
        {
            float startAfter = Visible ? (sectionShowAfter < 0 ? ShowAfter : sectionShowAfter) : (sectionHideAfter < 0 ? HideAfter : sectionHideAfter);
            yield return Yielders.GetWaitForSeconds(startAfter);
            float curTime = UseUnscaledTime ? Time.unscaledTime : Time.time;
            float startTime = curTime;
            while (curTime < startTime + duration)
            {
                float t = (curTime - startTime) / duration;
                float ease = UIAnimationEquations.GetEaseFloat(t, motionType, easingParams);
                output(UnClampedLerp(start, end, ease));
                yield return null;
                curTime = UseUnscaledTime ? Time.unscaledTime : Time.time;
            }
            output(end);
        }
    }
}