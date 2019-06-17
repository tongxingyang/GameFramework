using System.Collections.Generic;
using GameFramework.Debug;
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
        public UIWindow CurrentUIWindow => uiWindows.First?.Value;
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

        public void OnInit()
        {
            groupCanvas = gameObject.GetOrAddComponent<Canvas>();
            groupGraphicRaycaster = gameObject.GetOrAddComponent<GraphicRaycaster>();
            groupCanvas.overrideSorting = true;
            groupName = name;
            groupPause = false;
            uiWindows = new LinkedList<UIWindow>();
            RectTransform transform = GetComponent<RectTransform>();
            transform.anchorMin = Vector2.zero;
            transform.anchorMax = Vector2.one;
            transform.anchoredPosition = Vector2.zero;
            transform.sizeDelta = Vector2.zero;
        }

        public void Refresh()
        {
            LinkedListNode<UIWindow> current = uiWindows.First;
            bool pause = groupPause;
            bool cover = false;
            int depth = UIWindowCount;
            while (current != null)
            {
                LinkedListNode<UIWindow> next = current.Next;
                current.Value.OnDepthChange(Depth, depth--);
                if (pause)
                {
                    if (!current.Value.WindowCovered)
                    {
                        current.Value.WindowCovered = true;
                        current.Value.OnCover();
                    }
                    if (!current.Value.WindowPaused)
                    {
                        current.Value.WindowPaused = true;
                        current.Value.OnPause();
                    }
                }
                else
                {
                    if (current.Value.WindowPaused)
                    {
                        current.Value.WindowPaused = false;
                        current.Value.OnResume();
                    }
                    if (current.Value.PauseCovered)
                    {
                        pause = true;
                    }
                    if (cover)
                    {
                        if (!current.Value.WindowPaused)
                        {
                            current.Value.WindowPaused = true;
                            current.Value.OnCover();
                        }
                    }
                    else
                    {
                        if (current.Value.WindowCovered)
                        {
                            current.Value.WindowCovered = false;
                            current.Value.OnReveal();
                        }
                        cover = true;
                    }
                }
                current = next;
            }
        }
        
        public bool HasUIWindow(int serialId)
        {
            foreach (UIWindow uiWindow in uiWindows)
            {
                if (uiWindow.SerialId == serialId)
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasUIWindow(string windowName)
        {
            foreach (UIWindow uiWindow in uiWindows)
            {
                if (uiWindow.AssetName == windowName)
                {
                    return true;
                }
            }
            return false;
        }

        public UIWindow GetUIWindow(int serialId)
        {
            foreach (UIWindow uiWindow in uiWindows)
            {
                if (uiWindow.SerialId == serialId)
                {
                    return uiWindow;
                }
            }
            return null;
        }

        public UIWindow GetUIWindow(string windowName)
        {
            foreach (UIWindow uiWindow in uiWindows)
            {
                if (uiWindow.AssetName == windowName)
                {
                    return uiWindow;
                }
            }
            return null;
        }

        public UIWindow[] GetUIWindows(string windowName)
        {
            List<UIWindow> results = new List<UIWindow>();
            foreach (UIWindow uiWindow in uiWindows)
            {
                if (uiWindow.AssetName == windowName)
                {
                    results.Add(uiWindow);
                }
            }
            return results.ToArray();
        }

        public void GetUIWindows(string windowName, List<UIWindow> result, bool isClearList = true )
        {
            if(isClearList)result.Clear();
            foreach (UIWindow uiWindow in uiWindows)
            {
                if (uiWindow.AssetName == windowName)
                {
                    result.Add(uiWindow);
                }
            }
        }

        public UIWindow[] GetAllUIWindows()
        {
            List<UIWindow> results = new List<UIWindow>();
            foreach (UIWindow uiWindow in uiWindows)
            {
                results.Add(uiWindow);
            }
            return results.ToArray();
        }

        public void GetAllUIWindows(List<UIWindow> result)
        {
            result.Clear();
            foreach (UIWindow uiWindow in uiWindows)
            {
                result.Add(uiWindow);
            }
        }

        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            LinkedListNode<UIWindow> current = uiWindows.First;
            while (current != null)
            {
                if (current.Value.WindowPaused)
                {
                    break;
                }
                LinkedListNode<UIWindow> next = current.Next;
                current.Value.OnUpdate(elapseSeconds, realElapseSeconds);
                current = next;
            }
        }

        public void OnLateUpdate()
        {
            LinkedListNode<UIWindow> current = uiWindows.First;
            while (current != null)
            {
                if (current.Value.WindowPaused)
                {
                    break;
                }
                LinkedListNode<UIWindow> next = current.Next;
                current.Value.OnLateUpdate();
                current = next;
            }
        }

        private void SetDepth(int depth)
        {
            groupCanvas.overrideSorting = true;
            groupCanvas.sortingOrder = DepthStep * depth;
        }
        
        public void AddUIForm(UIWindow uiWindow)
        {
            if (uiWindow != null)
            {
                uiWindows.AddFirst(uiWindow);
            }
        }
        
        public void RemoveUIForm(UIWindow uiWindow)
        {
            if (uiWindow == null)
            {
                Debuger.LogError(Utility.StringUtility.Format("can not find the uiwindow '{0}'", uiWindow.AssetName));
                return;
            }
            if (!uiWindow.WindowCovered)
            {
                uiWindow.WindowCovered = true;
                uiWindow.OnCover();
            }
            if (!uiWindow.WindowPaused)
            {
                uiWindow.WindowPaused = true;
                uiWindow.OnPause();
            }
            uiWindows.Remove(uiWindow);
        }

        public void RefocusUIWindow(UIWindow uiWindow)
        {
            if (uiWindow == null)
            {
                Debuger.LogError(Utility.StringUtility.Format("can not find the uiwindow '{0}'", uiWindow.AssetName));
                return;
            }
            uiWindows.Remove(uiWindow);
            uiWindows.AddFirst(uiWindow);
        }
    }
}