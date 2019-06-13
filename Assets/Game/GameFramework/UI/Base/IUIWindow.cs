using UnityEngine;

namespace GameFramework.UI.Base
{
    public interface IUIWindow
    {
        int SerialId { get; }
        UIWindowInfo WindowInfo { get; }
        GameObject CacheGameObject { get; }
        RectTransform CacheTransform { get; }
        IUIWindowGroup UIGroup { get; }
        int DepthInUIGroup { get; }
        bool WindowPaused { get; set; }
        bool WindowCovered { get; set; }
        void OnInit(int serialId, string assetName, Camera uiCamera, IUIWindowGroup uiGroup, bool pauseCovered, UIWindowContext uiWindowContext = null);
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