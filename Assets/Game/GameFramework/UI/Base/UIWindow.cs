using System.Collections;
using GameFramework.Utility.Extension;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework.UI.Base
{
    public class UIWindow : MonoBehaviour, IUIWindow
    {
        public const int DepthStep = 100;
        public float AnimationTime = 0.5f;
        
        [Header("打开UI动画")]
        public AnimationCurve OpenFadeCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        public AnimationCurve OpenScaleXCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        public AnimationCurve OpenScaleYCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        
        [Header("关闭UI动画")]
        public AnimationCurve CloseFadeCurve = new AnimationCurve(new Keyframe(1, 1), new Keyframe(0, 0));
        public AnimationCurve CloseScaleXCurve = new AnimationCurve(new Keyframe(1, 1), new Keyframe(0, 0));
        public AnimationCurve CloseScaleYCurve = new AnimationCurve(new Keyframe(1, 1), new Keyframe(0, 0));
        
        
        private int serialId;
        private bool visible = false;
        private int originalLayer = 0;
        private int originalDepth = 0;
        private string assetName;
        private bool pauseCovered;
        private Camera uiCamera;
        private UIWindowInfo windowInfo;
        private GameObject cacheGameObject;
        private Transform cacheTransform;
        private IUIGroup uiGroup;
        private int depthInUIGroup;
        private Canvas canvas;
        private CanvasGroup canvasGroup;
        private CanvasScaler canvasScaler;
        private GraphicRaycaster graphicRaycaster;
        
        public int SerialId => serialId;
        public int OriginalLayer => originalLayer;
        public int OriginalDepth => originalDepth;
        public int Depth => canvas.sortingOrder;
        public string AssetName => assetName;
        public bool PauseCovered => pauseCovered;
        public Camera UICamera => uiCamera;
        public UIWindowInfo WindowInfo => windowInfo;
        public GameObject CacheGameObject => cacheGameObject ?? (cacheGameObject = this.gameObject);
        public Transform CacheTransform => cacheTransform ?? (cacheTransform = this.gameObject.transform);
        public IUIGroup UIGroup => uiGroup;
        public int DepthInUIGroup => depthInUIGroup;

        public string Name
        {
            get => CacheGameObject.name;
            set => CacheGameObject.name = value;
        }

        public virtual void CustomColliderFunc()
        {
            
        }

        public virtual void OnInit(int serialId, string assetName, Camera uiCamera, IUIGroup uiGroup, bool pauseCovered, UIWindowContext uiWindowContext = null)
        {
            this.serialId = serialId;
            this.assetName = assetName;
            this.uiCamera = uiCamera;
            this.pauseCovered = pauseCovered;
            this.uiGroup = uiGroup;
            this.originalLayer = CacheGameObject.layer;
        }

        public virtual void OnOpen(UIWindowContext uiWindowContext = null)
        {
            visible = true;
            CacheGameObject.SetActive(true);
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
            visible = false;
            CacheGameObject.SetActive(false);
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
                StartCoroutine(CloseAnim(AnimationTime, CloseFadeCurve, CloseScaleXCurve, CloseScaleYCurve));
            }
        }

        IEnumerator CloseAnim(float duration, AnimationCurve closeFadeCurve, AnimationCurve closeScaleXCurve, AnimationCurve closeScaleYCurve)
        {
            float journey = 0f;
            while (journey < duration)
            {
                float percent = Mathf.Clamp01(journey / duration);
                canvasGroup.alpha = closeFadeCurve.Evaluate(percent)
                vector.x = curveX.Evaluate(percent) * multiplier;
                vector.y = curveY.Evaluate(percent) * multiplier;
                vector.z = curveZ.Evaluate(percent) * multiplier;
                targetTransform.localEulerAngles = vector;

                journey += Time.deltaTime;
                yield return null;
            }
            yield return null;
        }
    }
}