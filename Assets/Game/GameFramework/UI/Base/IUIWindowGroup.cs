using System.Collections.Generic;

namespace GameFramework.UI.Base
{
    public interface IUIWindowGroup
    {
        string Name { get; }
        int Depth { get; set; }
        bool Pause { get; set; }
        int UIWindowCount { get; }
        UIWindow CurrentUIWindow { get; }
        bool HasUIWindow(int serialId);
        bool HasUIWindow(string windowName);
        UIWindow GetUIWindow(int serialId);
        UIWindow GetUIWindow(string windowName);
        UIWindow[] GetUIWindows(string windowName);
        void GetUIWindows(string windowName, List<UIWindow> result, bool isClearList = true);
        UIWindow[] GetAllUIWindows();
        void GetAllUIWindows(List<UIWindow> result);
        void OnUpdate(float elapseSeconds, float realElapseSeconds);
        void OnLateUpdate();
    }
}