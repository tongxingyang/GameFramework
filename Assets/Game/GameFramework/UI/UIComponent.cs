using GameFramework.Base;
using GameFramework.UI.UIExtension;
using GameFramework.Utility.Extension;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework.UI
{
    [DisallowMultipleComponent]
    public class UIComponent : GameFrameworkComponent
    {
        public UIFont MainFont = null;
        private CanvasScaler canvasScaler;
    
#if UNITY_EDITOR
        private int resolutionWidth = 0;
        private int resolutionHeight = 0;
#endif
        
        public override void OnAwake()
        {
            base.OnAwake();
            canvasScaler = gameObject.GetOrAddComponent<CanvasScaler>();
            MatchCanvas();
        }
        
        public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
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
        
        private void MatchCanvas()
        {
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = AppConst.UIConfig.GameResolution;
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            if (Screen.width / canvasScaler.referenceResolution.x > Screen.height / canvasScaler.referenceResolution.y)
            {
                canvasScaler.matchWidthOrHeight = 1;
            }
            else
            {
                canvasScaler.matchWidthOrHeight = 0;
            }
        }
    }
}