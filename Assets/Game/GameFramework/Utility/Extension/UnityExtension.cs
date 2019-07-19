using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework.Utility.Extension
{
    public static class UnityExtension
    {
        static List<Component> m_ComponentCache = new List<Component>();

        public static Component GetComponentNoAlloc(this GameObject @this, System.Type componentType)
        {
            @this.GetComponents(componentType, m_ComponentCache);
            var component = m_ComponentCache.Count > 0 ? m_ComponentCache[0] : null;
            m_ComponentCache.Clear();
            return component;
        }

        public static T GetComponentNoAlloc<T>(this GameObject @this) where T : Component
        {
            @this.GetComponents(typeof(T), m_ComponentCache);
            var component = m_ComponentCache.Count > 0 ? m_ComponentCache[0] : null;
            m_ComponentCache.Clear();
            return component as T;
        }

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

        public static GameObject FindChild(this GameObject gameObject, string name)
        {
            Transform child = gameObject.transform.Find(name);
            return child ? child.gameObject : null;
        }

        public static T FindChild<T>(this GameObject gameObject, string name) where T : Component
        {
            GameObject child = FindChild(gameObject, name);
            if (child != null)
                return child.GetComponent<T>();
            else
                return null;
        }

        public static void RequireChild(this GameObject gameObject, out GameObject child, string name)
        {
            child = FindChild(gameObject, name);
        }

        public static void RequireChild<T>(this GameObject obj, out T component, string name) where T : Component
        {
            component = FindChild<T>(obj, name);
        }

        public static GameObject FindChildRRecursively(this GameObject gameObject, string name)
        {
            if (gameObject.name == name)
            {
                return gameObject;
            }
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                var child = FindChildRRecursively(gameObject.transform.GetChild(i).gameObject, name);
                if (child != null)
                {
                    return child;
                }
            }
            return null;
        }

        public static T FindChildComponentRecursively<T>(this GameObject obj, string childName) where T : Component
        {
            GameObject child = FindChildRRecursively(obj, childName);
            if (child != null)
                return child.GetComponent<T>();
            else
                return null;
        }

        private static readonly Dictionary<string, List<MeshRenderer>> batchDictionary =
            new Dictionary<string, List<MeshRenderer>>();

        public static void StaticBatching(GameObject gameObject)
        {
            batchDictionary.Clear();
            MeshRenderer[] meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer meshRenderer in meshRenderers)
            {
                Material material = meshRenderer.material;
                if (material != null)
                {
                    List<MeshRenderer> renderers;
                    if (!batchDictionary.TryGetValue(material.name, out renderers))
                    {
                        renderers = new List<MeshRenderer>();
                        batchDictionary.Add(material.name, renderers);
                    }
                    renderers.Add(meshRenderer);
                }
            }
            int index = 0;
            var e = batchDictionary.GetEnumerator();
            while (e.MoveNext())
            {
                List<MeshRenderer> renderers = e.Current.Value;
                if (renderers.Count <= 1)
                    continue;

                GameObject root = new GameObject(string.Format("Batch{0}", index++));
                Material material = renderers[0].sharedMaterial;

                var goArray = new GameObject[renderers.Count];
                var rootTrans = root.transform;
                for (int i = 0; i < renderers.Count; i++)
                {
                    var cc = renderers[i];
                    var g = cc.gameObject;
                    g.transform.parent = rootTrans;

                    if (i != 0)
                        cc.sharedMaterial = material;
                    goArray[i] = g;
                }

                StaticBatchingUtility.Combine(goArray, root);
                rootTrans.parent = gameObject.transform;
            }
            e.Dispose();
        }

        public static void StopStaticBatching()
        {
            batchDictionary.Clear();
        }
    }
}