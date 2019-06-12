using UnityEngine;

namespace GameFramework.UI.Base
{
    public interface IUIWindow
    {
        int SerialId { get; }
        UIWindowInfo WindowInfo { get; }
        GameObject CacheGameObject { get; }
        Transform CacheTransform { get; }
        IUIGroup UIGroup { get; }
        int DepthInUIGroup { get; }
        void OnInit(int serialId, string assetName, Camera uiCamera, IUIGroup uiGroup, bool pauseCovered, UIWindowContext uiWindowContext = null);
        void OnOpen(UIWindowContext uiWindowContext = null);
        void OnClose(UIWindowContext uiWindowContext = null);
        void OnReFocus(UIWindowContext uiWindowContext = null);
        void OnCover();
        void OnReveal();
        void OnPause();
        void OnResume();
        void OnRecycle();
        void OnDepthChange(int groupDepth, int depthInGroup);
        
    }
}