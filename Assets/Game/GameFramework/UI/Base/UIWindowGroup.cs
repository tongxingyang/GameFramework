using System.Collections.Generic;

namespace GameFramework.UI.Base
{
    public sealed class UIWindowGroup : IUIWindowGroup
    {
        private string groupName;
        private int groupDepth;
        private bool groupPause;
        private LinkedList<UIWindow> uiWindows;

        public string Name => groupName;
        public int UIWindowCount => uiWindows?.Count ?? 0;
        public IUIWindow CurrentUIWindow => uiWindows.First?.Value;
        
        public int Depth
        {
            get => groupDepth;
            set
            {
                if (groupDepth == value)
                {
                    return;
                }
                groupDepth = value;
                Refresh();
            }
        }

        public bool Pause
        {
            get => groupPause;
            set
            {
                if (groupPause == value)
                {
                    return;
                }
                groupPause = value;
                Refresh();
            }
        }

        public UIWindowGroup(string name, int depth)
        {
            groupName = name;
            groupDepth = depth;
            groupPause = false;
            uiWindows = new LinkedList<UIWindow>();
        }
        
        public bool HasUIWindow(int serialId)
        {
            throw new System.NotImplementedException();
        }

        public bool HasUIWindow(string windowName)
        {
            throw new System.NotImplementedException();
        }

        public IUIWindow GetUIWindow(int serialId)
        {
            throw new System.NotImplementedException();
        }

        public IUIWindow GetUIWindow(string windowName)
        {
            throw new System.NotImplementedException();
        }

        public IUIWindow[] GetUIWindows(string windowName)
        {
            throw new System.NotImplementedException();
        }

        public void GetUIWindows(string windowName, List<IUIWindow> result)
        {
            throw new System.NotImplementedException();
        }

        public IUIWindow[] GetAllUIWindows()
        {
            throw new System.NotImplementedException();
        }

        public void GetAllUIWindows(List<IUIWindow> result)
        {
            throw new System.NotImplementedException();
        }
    }
}