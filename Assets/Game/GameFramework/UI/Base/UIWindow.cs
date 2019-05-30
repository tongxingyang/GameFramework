using UnityEngine;
using UnityEngine.UI;

namespace GameFramework.UI.Base
{
    public class UIWindow : MonoBehaviour, IUIWindow
    {
        private int serialId;
        private UIWindowInfo windowInfo;
        private GameObject cacheGameObject;
        private Transform cacheTransform;
        private IUIGroup uiGroup;
        private int depthInUIGroup;
        private Canvas canvas;
        private CanvasScaler canvasScaler;
        private GraphicRaycaster graphicRaycaster;
        
        public int SerialId => serialId;
        public UIWindowInfo WindowInfo => windowInfo;
        public GameObject CacheGameObject => cacheGameObject ?? (cacheGameObject = this.gameObject);
        public Transform CacheTransform => cacheTransform ?? (cacheTransform = this.gameObject.transform);
        public IUIGroup Group => uiGroup;
        public int DepthInUIGroup => depthInUIGroup;

        public virtual void CustomColliderFunc()
        {
            
        }
        
        public virtual void OnInit(int serialId, Camera uiCamera, UIGroup uiGroup, UIWindowContext uiWindowContext = null)
        {
        }

        public virtual void OnOpen(UIWindowContext uiWindowContext = null)
        {
        }

        public virtual void OnClose(UIWindowContext uiWindowContext = null)
        {
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
        }

        public virtual void OnResume()
        {
        }

        public virtual void OnRecycle()
        {
        }

        public virtual void OnDepthChange(int groupDepth, int depthInGroup)
        {
        }
        
        public virtual void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            
        }
        
        public virtual void OnLateUpdate()
        {
            
        }
        
    }
}