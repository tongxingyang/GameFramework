using System.Collections;
using GameFramework.Base;
using GameFramework.Res;
using GameFramework.UI.UIExtension;
using GameFramework.Utility.Extension;
using GameFramework.Utility.Singleton;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework.UI.Base
{
    public class UIWindow : MonoBehaviour, IUIWindow
    {
        public const int DepthStep = 100;
        public float OpenAnimationTime = 0.5f;
        public float CloseAnimationTime = 0.5f;
        public bool IsFullScreen = true;
        public bool IsCanInput = true;
        
        [Header("打开UI动画")]
        public bool IsPlayOpenFadeAnim = true;
        public AnimationCurve OpenFadeCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        public bool IsPlayOpenScaleXAnim = true;
        public AnimationCurve OpenScaleXCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        public bool IsPlayOpenScaleYAnim = true;
        public AnimationCurve OpenScaleYCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        
        [Header("关闭UI动画")]
        public bool IsPlayCloseFadeAnim = true;
        public AnimationCurve CloseFadeCurve = new AnimationCurve(new Keyframe(1, 1), new Keyframe(0, 0));
        public bool IsPlayCloseScaleXAnim = true;
        public AnimationCurve CloseScaleXCurve = new AnimationCurve(new Keyframe(1, 1), new Keyframe(0, 0));
        public bool IsPlayCloseScaleYAnim = true;
        public AnimationCurve CloseScaleYCurve = new AnimationCurve(new Keyframe(1, 1), new Keyframe(0, 0));
        
        
        private int serialId;
        private bool visible = false;
        private bool windowPaused = false;
        private bool windowCovered = false;
        private int originalLayer = 0;
        private int originalDepth = 0;
        private string assetName;
        private bool pauseCovered;
        private Camera uiCamera;
        private UIWindowInfo windowInfo;
        private GameObject cacheGameObject;
        private RectTransform cacheTransform;
        private IUIWindowGroup uiGroup;
        private int depthInUIGroup;
        private Canvas canvas;
        private CanvasGroup canvasGroup;
        private CanvasScaler canvasScaler;
        private GraphicRaycaster graphicRaycaster;
        
        public int SerialId => serialId;

        public bool WindowPaused
        {
            get => windowPaused;
            set => windowPaused = value;
        }

        public bool WindowCovered
        {
            get => windowCovered;
            set => windowPaused = value;
        }
        public int OriginalLayer => originalLayer;
        public int OriginalDepth => originalDepth;
        public int Depth => canvas.sortingOrder;
        public string AssetName => assetName;
        public bool PauseCovered => pauseCovered;
        public Camera UICamera => uiCamera;
        public UIWindowInfo WindowInfo => windowInfo;
        public GameObject CacheGameObject => cacheGameObject ?? (cacheGameObject = this.gameObject);
        public RectTransform CacheTransform => cacheTransform ?? (cacheTransform = GetComponent<RectTransform>());
        public IUIWindowGroup UIGroup => uiGroup;
        public int DepthInUIGroup => depthInUIGroup;


#if UNITY_EDITOR
        private int resolutionWidth = 0;
        private int resolutionHeight = 0;
#endif
        
        public string Name
        {
            get => CacheGameObject.name;
            set => CacheGameObject.name = value;
        }
        
        public virtual void CustomColliderFunc()
        {
            
        }

        public virtual void OnInit(int serialId, string assetName, Camera uiCamera, IUIWindowGroup uiGroup, bool pauseCovered, UIWindowContext uiWindowContext = null)
        {
          
            this.serialId = serialId;
            this.assetName = assetName;
            this.uiCamera = uiCamera;
            this.pauseCovered = pauseCovered;
            this.uiGroup = uiGroup;
            this.originalLayer = CacheGameObject.layer;
            InitCanvas();
            SetCanvasMode(uiCamera);
        }
        
        public virtual void OnOpen(UIWindowContext uiWindowContext = null)
        {
            visible = true;
            CacheGameObject.SetActive(true);
            StopAllCoroutines();
            if (IsPlayOpenFadeAnim || IsPlayOpenScaleXAnim || IsPlayOpenScaleYAnim)
            {
                StartCoroutine(OpenAnim(OpenAnimationTime, OpenFadeCurve, OpenScaleXCurve, OpenScaleYCurve));
            }
        }

        public virtual void OnClose(UIWindowContext uiWindowContext = null)
        {
            CacheGameObject.SetLayerRecursively(originalLayer);
            visible = false;
            CacheGameObject.SetActive(false);
        }

        public virtual void OnReFocus(UIWindowContext uiWindowContext = null)
        {
        }

        public virtual void OnCover()
        {
        }

        public virtual void OnReveal()
        {
        }

        public virtual void OnPause()
        {
            visible = false;
            CacheGameObject.SetActive(false);
        }

        public virtual void OnResume()
        {
            visible = true;
            CacheGameObject.SetActive(true);
            StopAllCoroutines();
            if (IsPlayOpenFadeAnim || IsPlayOpenScaleXAnim || IsPlayOpenScaleYAnim)
            {
                StartCoroutine(OpenAnim(OpenAnimationTime, OpenFadeCurve, OpenScaleXCurve, OpenScaleYCurve));
            }
        }

        public virtual void OnRecycle()
        {
            this.serialId = 0;
            this.depthInUIGroup = 0;
            pauseCovered = true;
        }

        public virtual void OnDepthChange(int groupDepth, int depthInGroup)
        {
            this.depthInUIGroup = depthInGroup;
        }
        
        public virtual void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
#if UNITY_EDITOR
            if (resolutionWidth != Screen.width || resolutionHeight != Screen.height)
            {
                resolutionWidth = Screen.width;
                resolutionHeight = Screen.height;
                MatchCanvas();
            }
#endif
        }
        
        public virtual void OnLateUpdate()
        {
            
        }

        public void Close(bool ignoreAnim, UIWindowContext uiWindowContext = null)
        {
            StopAllCoroutines();
            if (ignoreAnim)
            {
                //todo 关闭界面
            }
            else
            {
                StartCoroutine(CloseAnim(CloseAnimationTime, CloseFadeCurve, CloseScaleXCurve, CloseScaleYCurve));
            }
        }

        private void InitCanvas()
        {
            canvas = gameObject.GetOrAddComponent<Canvas>();
            canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
            canvasScaler = gameObject.GetOrAddComponent<CanvasScaler>();
            graphicRaycaster = gameObject.GetOrAddComponent<GraphicRaycaster>();
            canvas.overrideSorting = true;
            MatchCanvas();
            graphicRaycaster.enabled = IsCanInput;
        }

        private void MatchCanvas()
        {
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = AppConst.UIConfig.GameResolution;
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            if (Screen.width / canvasScaler.referenceResolution.x > Screen.height / canvasScaler.referenceResolution.y)
            {
                canvasScaler.matchWidthOrHeight = IsFullScreen ? 0 : 1;
            }
            else
            {
                canvasScaler.matchWidthOrHeight = IsFullScreen ? 1 : 0;
            }
        }

        private void SetCanvasMode(Camera camera)
        {
            canvas.renderMode = camera == null ? RenderMode.ScreenSpaceOverlay : RenderMode.ScreenSpaceCamera;
            canvas.pixelPerfect = false;
        }

        public Camera GetCamera()
        {
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay) return null;
            return canvas.worldCamera;
        }

        public void SetEnableInput(bool value)
        {
            if (!value)
            {
                graphicRaycaster.enabled = false;
            }
            else if(IsCanInput)
            {
                graphicRaycaster.enabled = true;
            }
        }
        
        public bool IsOverlay()
        {
            return canvas.renderMode == RenderMode.ScreenSpaceOverlay;
        }
        
        IEnumerator OpenAnim(float duration, AnimationCurve openFadeCurve, AnimationCurve openScaleXCurve, AnimationCurve openScaleYCurve)
        {
            float journey = 0f;
            while (journey < duration)
            {
                float percent = Mathf.Clamp01(journey / duration);
                if (IsPlayCloseFadeAnim)
                {
                    canvasGroup.alpha = openFadeCurve.Evaluate(percent);
                }
                if (IsPlayCloseScaleXAnim || IsPlayCloseScaleYAnim)
                {
                    CacheTransform.localScale = new Vector3(openScaleXCurve.Evaluate(percent),openScaleYCurve.Evaluate(percent),CacheTransform.localScale.z);
                }

                journey += Time.unscaledDeltaTime;
                yield return null;
            }
            yield return null;
        }
        
        IEnumerator CloseAnim(float duration, AnimationCurve closeFadeCurve, AnimationCurve closeScaleXCurve, AnimationCurve closeScaleYCurve)
        {
            float journey = 0f;
            while (journey < duration)
            {
                float percent = Mathf.Clamp01(journey / duration);
                if (IsPlayCloseFadeAnim)
                {
                    canvasGroup.alpha = closeFadeCurve.Evaluate(percent);
                }
                if (IsPlayCloseScaleXAnim || IsPlayCloseScaleYAnim)
                {
                    CacheTransform.localScale = new Vector3(closeScaleXCurve.Evaluate(percent),closeScaleYCurve.Evaluate(percent),CacheTransform.localScale.z);
                }

                journey += Time.unscaledDeltaTime;
                yield return null;
            }
            //todo 关闭界面
            yield return null;
        }
    }
}