using System.Collections.Generic;
using GameFramework.ObjectPool.Base;
using GameFramework.Res.Base;

namespace GameFramework.UI.Base
{
    public interface IUIManager
    {
        int UIWindowGroupCount { get; }
        float InstanceAutoReleaseInterval { get; set; }
        int InstanceCapacity { get; set; }
        float InstanceExpireTime { get; set; }
        int InstancePriority { get; set;}
        void SetObjectPoolManager(IObjectPoolManager objectPoolManager);
        void SetResourceManager(IResourceManager resourceManager);
        bool HasUIWindowGroup(string uiGroupName);
        UIWindowGroup GetUIWindowGroup(string uiGroupName);
        IUIWindowGroup[] GetAllUIWindowGroups();
        void GetAllUIWindowGroups(List<IUIWindowGroup> results);
        bool AddUIWindowGroup(string uiGroupName, UIWindowGroup uiWindowGroup, int uiGroupDepth);
        bool HasUIWindow(int serialId);
        bool HasUIWindow(string uiAssetName);
        UIWindow GetUIWindow(int serialId);
        UIWindow GetUIWindow(string uiAssetName);
        UIWindow[] GetUIWindows(string uiAssetName);
        void GetUIWindows(string uiFormAssetName, List<UIWindow> results);
        UIWindow[] GetAllLoadedUIWindows();
        void GetAllLoadedUIWindows(List<UIWindow> results);
        int[] GetAllLoadingUIWindowSerialIds();
        void GetAllLoadingUIWindowsSerialIds(List<int> results);
        bool IsLoadingUIWindow(int serialId);
        bool IsLoadingUIWindow(string uiAssetName);
        bool IsValidUIWindow(UIWindow uiWindow);
        int OpenUIWindow(ResourceLoadInfo resourceLoadInfo, string uiGroupName, bool pauseCovered,UIWindowContext uiWindowContext);
        void CloseUIWindow(int serialId);
        void CloseUIWindow(int serialId, UIWindowContext uiWindowContext);
        void CloseUIWindow(UIWindow uiWindow);
        void CloseUIWindow(UIWindow uiWindow, UIWindowContext uiWindowContext);
        void CloseAllLoadedUIWindows();
        void CloseAllLoadedUIWindows(UIWindowContext uiWindowContext);
        void CloseAllLoadingUIWindows();
        void RefocusUIWindow(UIWindow uiWindow);
        void RefocusUIWindow(UIWindow uiWindow, UIWindowContext uiWindowContext);
        void SetUIWindowInstanceLocked(object uiFormInstance, bool locked);
        void SetUIWindowInstancePriority(object uiFormInstance, int priority);
    }
}