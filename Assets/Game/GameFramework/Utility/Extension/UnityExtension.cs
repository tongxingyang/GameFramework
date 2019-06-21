using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework.Utility.Extension
{
    public static  class UnityExtension
    {
        
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }

            return component;
        }

        public static Component GetOrAddComponent(this GameObject gameObject, Type type)
        {
            Component component = gameObject.GetComponent(type);
            if (component == null)
            {
                component = gameObject.AddComponent(type);
            }

            return component;
        }

        public static void SetLayerRecursively(this GameObject gameObject, int layer)
        {
            Transform[] transforms = gameObject.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < transforms.Length; i++)
            {
                transforms[i].gameObject.layer = layer;
            }
        }

        public static bool LayerMarskContains(this LayerMask mask, int layer)
        {
            return ((mask.value & (1 << layer)) > 0);
        }
        
        public static bool LayerMarskContains(this LayerMask mask, GameObject gameobject) 
        {
            return ((mask.value & (1 << gameobject.layer)) > 0);
        }

        public static void DestoryAllChildren(this Transform transform)
        {
            for (int t = transform.childCount - 1; t >= 0; t--)
            {
                if (Application.isPlaying)
                {
                    UnityEngine.Object.Destroy(transform.GetChild(t).gameObject);
                }
                else
                {
                    UnityEngine.Object.DestroyImmediate(transform.GetChild(t).gameObject);
                }
            }
        }
        
        public static void ScrollToTop(this ScrollRect scrollRect)
        {
            scrollRect.normalizedPosition = new Vector2(0, 1);
        }

        public static void ScrollToBottom(this ScrollRect scrollRect)
        {
            scrollRect.normalizedPosition = new Vector2(0, 0);
        }
    }
}