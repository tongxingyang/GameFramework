using System;
using System.Collections.Generic;
using GameFramework.Pool.ReferencePool;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameFramework.Res.Base
{
    public abstract class AbstractAssetInfo : IReference
    {
        public delegate void OnBundleUnload(AbstractAssetInfo info);

        public OnBundleUnload OnBundleUnloadAction;
        
        public string AssetBundleName { get; set; }
        
        public float LastUsedTime { get; set; }
        
        public bool IsReady { get; set; }

        public bool IsUnused => IsReady && ReferenceCount <= 0 && UpdateReference() == 0;
        
        public int ReferenceCount { get; set; }

        public List<WeakReference> ReferenceList { get; set; }

        protected Dictionary<string, Object> LoadedAssetMap;

        public abstract void LoadAllAsset<T>() where T : Object;

        public abstract T LoadAsset<T>(string assetName) where T : Object;

        public abstract GameObject Instantiate(string assetName);

        public abstract T GetAsset<T>(Object owner, string assetName) where T : Object;

        public abstract void Dispose();
        
        public abstract void Reset();
        
        public void Retain()
        {
            ReferenceCount++;
        }

        public void Release()
        {
            ReferenceCount = Mathf.Max(0, ReferenceCount - 1);
        }

        public void UpdateLastUsedTime()
        {
            LastUsedTime = Time.time;
        }

        public void RetainInfoOwner(Object owner)
        {
            if (owner == null)
            {
                return;
            }
            if (ReferenceList != null)
            {
                foreach (WeakReference weakReference in ReferenceList)
                {
                    if (owner.Equals(weakReference.Target))
                    {
                        return;
                    }
                }
                WeakReference wr = new WeakReference(owner);
                ReferenceList.Add(wr);
            }
        }

        public int UpdateReference()
        {
            if (ReferenceList != null)
            {
                for (int i = 0; i < ReferenceList.Count; i++)
                {
                    Object obj = (Object) ReferenceList[i].Target;
                    if (!obj)
                    {
                        ReferenceList.RemoveAt(i);
                        i--;
                    }
                }
                return ReferenceList.Count;
            }
            return 0;

        }

    }
}