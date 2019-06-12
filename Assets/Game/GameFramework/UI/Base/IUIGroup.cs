using System.Collections.Generic;

namespace GameFramework.UI.Base
{
    public interface IUIGroup
    {
        string Name { get; }
        int Depth { get; set; }
        bool Pause { get; set; }
        int UIWindowCount { get; }
        IUIWindow CurrentUIWindow { get; }
        bool HasUIWindow(int serialId);
        bool HasUIWindow(string windowName);
        IUIWindow GetUIWindow(int serialId);
        IUIWindow GetUIWindow(string windowName);
        IUIWindow[] GetUIWindows(string windowName);
        void GetUIWindows(string windowName, List<IUIWindow> result);
        IUIWindow[] GetAllUIWindows();
        void GetAllUIWindows(List<IUIWindow> result);
    }
}