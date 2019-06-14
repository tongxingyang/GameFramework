using System.Collections.Generic;
using GameFramework.Utility.Extension;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework.UI.Base
{
    public class UIWindowGroup : MonoBehaviour ,IUIWindowGroup
    {
        public const int DepthStep = 10000;
        
        private string groupName;
        private int groupDepth = -1;
        private bool groupPause;
        private Canvas groupCanvas;
        private GraphicRaycaster groupGraphicRaycaster;
        private LinkedList<UIWindow> uiWindows;

        public string Name => groupName;
        public int UIWindowCount => uiWindows?.Count ?? 0;
        public IUIWindow CurrentUIWindow => uiWindows.First?.Value;
        public Canvas GroupCanvas => groupCanvas;
        public GraphicRaycaster GroupGraphicRaycaster => groupGraphicRaycaster;
        
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
                SetDepth(groupDepth);
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

        public void Awake()
        {
            groupCanvas = gameObject.GetOrAddComponent<Canvas>();
            groupGraphicRaycaster = gameObject.GetOrAddComponent<GraphicRaycaster>();
            groupCanvas.overrideSorting = true;
            groupName = name;
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

        private void SetDepth(int depth)
        {
            groupCanvas.overrideSorting = true;
            groupCanvas.sortingOrder = DepthStep * depth;
        }
    }
}