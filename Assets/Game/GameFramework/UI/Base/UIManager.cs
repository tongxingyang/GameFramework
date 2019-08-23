using System.Collections.Generic;
using Game.GameFramework.UI.Base;
using GameFramework.ObjectPool.Base;
using GameFramework.Res.Base;
using GameFramework.Utility.Extension;
using UnityEngine;

namespace GameFramework.UI.Base
{
    public class UIManager : IUIManager
    {
        public static int Serial = 0;
        
        private readonly Dictionary<string, UIWindowGroup> uiWindowGroups;
        private readonly List<int> uiWindowBeingLoaded;
        private readonly List<string> uiWindowAssetNamesBeingLoaded;
        private readonly HashSet<int> uiWindowsToReleaseOnLoad;
        private readonly LinkedList<UIWindow> recycleQueue;
        private IResourceManager resourceManager;
        private IObjectPoolManager objectPoolManager;
        private IObjectPool uiObjectPool;
        private float instanceAutoReleaseInterval;
        private int instanceCapacity;
        private float instanceExpireTime;
        private int instancePriority;
        private Camera renderCamera;
        public int UIWindowGroupCount => uiWindowGroups.Count;
        public float InstanceAutoReleaseInterval
        {
            get => instanceAutoReleaseInterval;
            set => instanceAutoReleaseInterval = value;
        }
        public int InstanceCapacity
        {
            get => instanceCapacity;
            set => instanceCapacity = value;
        }
        public float InstanceExpireTime
        {
            get => instanceExpireTime;
            set => instanceExpireTime = value;
        }
        public int InstancePriority
        {
            get => instancePriority;
            set => instancePriority = value;
        }

        public UIManager(Camera camera)
        {
            renderCamera = camera;
            uiWindowGroups = new Dictionary<string, UIWindowGroup>();
            uiWindowBeingLoaded = new List<int>();
            uiWindowAssetNamesBeingLoaded = new List<string>();
            uiWindowsToReleaseOnLoad = new HashSet<int>();
            recycleQueue = new LinkedList<UIWindow>();
        }
        
        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            while (recycleQueue.Count > 0)
            {
                UIWindow uiWindow = recycleQueue.First.Value;
                recycleQueue.RemoveFirst();
                uiWindow.OnRecycle();
                //m_InstancePool.Unspawn(uiForm.Handle); todo
            }

            foreach (KeyValuePair<string, UIWindowGroup> uiGroup in uiWindowGroups)
            {
                uiGroup.Value.OnUpdate(elapseSeconds, realElapseSeconds);
            }
        }

        public void OnLateUpdate()
        {
            foreach (KeyValuePair<string, UIWindowGroup> uiGroup in uiWindowGroups)
            {
                uiGroup.Value.OnLateUpdate();
            }
        }

        public void Shutdown()
        {
            CloseAllLoadedUIWindows();
            uiWindowGroups.Clear();
            uiWindowBeingLoaded.Clear();
            uiWindowAssetNamesBeingLoaded.Clear();
            uiWindowsToReleaseOnLoad.Clear();
            recycleQueue.Clear();
        }
        
        public void SetResourceManager(IResourceManager resourceManager)
        {
            this.resourceManager = resourceManager;
        }

        public void SetObjectPoolManager(IObjectPoolManager objectPoolManager)
        {
            this.objectPoolManager = objectPoolManager;
//            uiObjectPool = objectPoolManager.CreateSingleSpawnObjectPool<UIFormInstanceObject>("UI Instance Pool"); // todo
        }
        
        public bool HasUIWindowGroup(string uiGroupName)
        {
            return uiWindowGroups.ContainsKey(uiGroupName);
        }

        public UIWindowGroup GetUIWindowGroup(string uiGroupName)
        {
            UIWindowGroup uiGroup = null;
            if (uiWindowGroups.TryGetValue(uiGroupName, out uiGroup))
            {
                return uiGroup;
            }
            return null;
        }

        public IUIWindowGroup[] GetAllUIWindowGroups()
        {
            int index = 0;
            UIWindowGroup[] results = new UIWindowGroup[uiWindowGroups.Count];
            foreach (KeyValuePair<string, UIWindowGroup> uiGroup in uiWindowGroups)
            {
                results[index++] = uiGroup.Value;
            }
            return results;
        }

        public void GetAllUIWindowGroups(List<IUIWindowGroup> results)
        {
            results.Clear();
            foreach (KeyValuePair<string, UIWindowGroup> uiGroup in uiWindowGroups)
            {
                results.Add(uiGroup.Value);
            }
        }

        public bool AddUIWindowGroup(string uiGroupName, UIWindowGroup uiWindowGroup ,int uiGroupDepth)
        {
            if (HasUIWindowGroup(uiGroupName))
            {
                return false;
            }
            uiWindowGroups.Add(uiGroupName, uiWindowGroup);
            uiWindowGroup.Depth = uiGroupDepth;
            return true;
        }

        public bool HasUIWindow(int serialId)
        {
            foreach (KeyValuePair<string, UIWindowGroup> uiGroup in uiWindowGroups)
            {
                if (uiGroup.Value.HasUIWindow(serialId))
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasUIWindow(string uiAssetName)
        {
            foreach (KeyValuePair<string, UIWindowGroup> uiGroup in uiWindowGroups)
            {
                if (uiGroup.Value.HasUIWindow(uiAssetName))
                {
                    return true;
                }
            }
            return false;
        }

        public UIWindow GetUIWindow(int serialId)
        {
            foreach (KeyValuePair<string, UIWindowGroup> uiGroup in uiWindowGroups)
            {
                UIWindow uiWindow = uiGroup.Value.GetUIWindow(serialId);
                if (uiWindow != null)
                {
                    return uiWindow;
                }
            }
            return null;
        }

        public UIWindow GetUIWindow(string uiAssetName)
        {
            foreach (KeyValuePair<string, UIWindowGroup> uiGroup in uiWindowGroups)
            {
                UIWindow uiWindow = uiGroup.Value.GetUIWindow(uiAssetName);
                if (uiWindow != null)
                {
                    return uiWindow;
                }
            }
            return null;
        }

        public UIWindow[] GetUIWindows(string uiAssetName)
        {
            List<UIWindow> results = new List<UIWindow>();
            foreach (KeyValuePair<string, UIWindowGroup> uiGroup in uiWindowGroups)
            {
                results.AddRange(uiGroup.Value.GetUIWindows(uiAssetName));
            }
            return results.ToArray();
        }

        public void GetUIWindows(string uiFormAssetName, List<UIWindow> results)
        {
            results.Clear();
            foreach (KeyValuePair<string, UIWindowGroup> uiGroup in uiWindowGroups)
            {
                uiGroup.Value.GetUIWindows(uiFormAssetName,results,false);
            }
        }

        public UIWindow[] GetAllLoadedUIWindows()
        {
            List<UIWindow> results = new List<UIWindow>();
            foreach (KeyValuePair<string, UIWindowGroup> uiGroup in uiWindowGroups)
            {
                results.AddRange(uiGroup.Value.GetAllUIWindows());
            }
            return results.ToArray();
        }

        public void GetAllLoadedUIWindows(List<UIWindow> results)
        {
            results.Clear();
            foreach (KeyValuePair<string, UIWindowGroup> uiGroup in uiWindowGroups)
            {
                results.AddRange(uiGroup.Value.GetAllUIWindows());
            }
        }

        public int[] GetAllLoadingUIWindowSerialIds()
        {
            return uiWindowBeingLoaded.ToArray();
        }

        public void GetAllLoadingUIWindowsSerialIds(List<int> results)
        {
            results.Clear();
            results.AddRange(uiWindowBeingLoaded);
        }

        public bool IsLoadingUIWindow(int serialId)
        {
            return uiWindowBeingLoaded.Contains(serialId);
        }

        public bool IsLoadingUIWindow(string uiAssetName)
        {
            return uiWindowAssetNamesBeingLoaded.Contains(uiAssetName);
        }

        public bool IsValidUIWindow(UIWindow uiWindow)
        {
            if (uiWindow == null)
            {
                return false;
            }
            return HasUIWindow(uiWindow.SerialId);
        }

        public int OpenUIWindow(ResourceLoadInfo resourceLoadInfo, string uiGroupName, bool pauseCovered, UIWindowContext uiWindowContext)
        {
            UIWindowGroup windowGroup = GetUIWindowGroup(uiGroupName);
            if (windowGroup == null)
            {
                return -1;
            }
            int serialID = Serial++;
//            var objRes = uiObjectPool;// todo
            var objRes = new object();
            if (objRes == null)
            {
                uiWindowBeingLoaded.Add(serialID);
                uiWindowAssetNamesBeingLoaded.Add(resourceLoadInfo.AssetName);
                OpenUIWindowInfo openUiWindowInfo = new OpenUIWindowInfo(serialID,windowGroup,pauseCovered,uiWindowContext);
                resourceManager.RequestResource(resourceLoadInfo.AssetBundleName, LoadUIWindowCallback,
                    resourceLoadInfo.ResourceLoadMode, resourceLoadInfo.ResourceLoadCache,
                    resourceLoadInfo.ResourceLoadMethod,openUiWindowInfo);
            }
            else
            {
                OpenUIWindow(serialID, resourceLoadInfo.AssetName, windowGroup, objRes, pauseCovered, false,
                    uiWindowContext);
            }
            return serialID;
        }
        
        public void CloseUIWindow(int serialId)
        {
            CloseUIWindow(serialId, null);
        }

        public void CloseUIWindow(int serialId, UIWindowContext uiWindowContext)
        {
            if (IsLoadingUIWindow(serialId))
            {
                uiWindowsToReleaseOnLoad.Add(serialId);
                return;
            }
            UIWindow uiWindow = GetUIWindow(serialId);
            if (uiWindow == null)
            {
                return;
            }
            CloseUIWindow(uiWindow, uiWindowContext);
        }

        public void CloseUIWindow(UIWindow uiWindow)
        {
            CloseUIWindow(uiWindow, null);
        }

        public void CloseUIWindow(UIWindow uiWindow, UIWindowContext uiWindowContext)
        {
            if (uiWindow == null)
            {
                return;
            }
            UIWindowGroup uiGroup = uiWindow.UIGroup;
            if (uiGroup == null)
            {
                return;
            }
            uiGroup.RemoveUIForm(uiWindow);
            uiWindow.OnClose(uiWindowContext);
            uiGroup.Refresh();
            recycleQueue.AddLast(uiWindow);
        }

        public void CloseAllLoadedUIWindows()
        {
            CloseAllLoadedUIWindows(null);
        }

        public void CloseAllLoadedUIWindows(UIWindowContext uiWindowContext)
        {
            UIWindow[] uiWindows = GetAllLoadedUIWindows();
            foreach (UIWindow uiWindow in uiWindows)
            {
                CloseUIWindow(uiWindow, uiWindowContext);
            }
        }

        public void CloseAllLoadingUIWindows()
        {
            foreach (int serialId in uiWindowBeingLoaded)
            {
                uiWindowsToReleaseOnLoad.Add(serialId);
            }
        }

        public void RefocusUIWindow(UIWindow uiWindow)
        {
            RefocusUIWindow(uiWindow, null);
        }

        public void RefocusUIWindow(UIWindow uiWindow, UIWindowContext uiWindowContext)
        {
            if (uiWindow == null)
            {
                return;
            }
            UIWindowGroup uiGroup = uiWindow.UIGroup;
            if (uiGroup == null)
            {
                return;
            }
            uiGroup.RefocusUIWindow(uiWindow);
            uiGroup.Refresh();
            uiWindow.OnRefocus(uiWindowContext);
        }

        public void SetUIWindowInstanceLocked(object uiFormInstance, bool locked)
        {
            throw new System.NotImplementedException();
        }

        public void SetUIWindowInstancePriority(object uiFormInstance, int priority)
        {
            throw new System.NotImplementedException();
        }
        private void LoadUIWindowCallback(AbstractAssetInfo info, object userdata)
        {
           
        }

        private void OpenUIWindow(int serialId, string uiAssetName, UIWindowGroup uiGroup, object uiWindowInstance, bool pauseCovered, bool isInit, UIWindowContext uiWindowContext)
        {
            GameObject obj = uiWindowInstance as GameObject;
            Transform transform = obj.transform;
            transform.SetParent(uiGroup.transform);
            transform.localScale = Vector3.one;
            UIWindow uiWindow = obj.GetOrAddComponent<UIWindow>();
            if (isInit)
            {
                uiWindow.OnInit(serialId, uiAssetName, uiWindow.IsCameraRender?renderCamera:null, uiGroup, pauseCovered, uiWindowContext);
            }
            uiGroup.AddUIForm(uiWindow);
            uiWindow.OnOpen(uiWindowContext);
            uiGroup.Refresh();
        }
    }
}