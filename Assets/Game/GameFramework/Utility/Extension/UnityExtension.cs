using System;
using System.Collections.Generic;
using GameFramework.UI.UITools;
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

        public static void StaticBatching(this GameObject gameObject)
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
        
        static void SetAlignment(this RectTransform rect, Vector2 value, bool adjustPivot, Vector2 offset, Vector2 size)
        {
            rect.anchorMax = value;
            rect.anchorMin = value;
            if (adjustPivot) rect.pivot = value;
            rect.anchoredPosition = offset;
            rect.sizeDelta = size;
        }

        static void SetStretchHorizontalRect(this RectTransform rect, Vector4 anchor, bool adjustPivot,
            float paddingLeft, float offsetY, float paddingRight, float height)
        {
            rect.anchorMin = new Vector2(anchor.x, anchor.y);
            rect.anchorMax = new Vector2(anchor.z, anchor.w);
            if (adjustPivot)
            {
                rect.pivot = new Vector2(rect.pivot.x, anchor.y);
            }
            rect.offsetMin = new Vector2(paddingLeft, rect.offsetMin.y);
            rect.offsetMax = new Vector2(-paddingRight, rect.offsetMax.y);
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, offsetY);
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, height);
        }

        static void SetStretchVerticalRect(this RectTransform rect, Vector4 anchor, bool adjustPivot, float offsetX,
            float paddingTop, float width, float paddingBottom)
        {
            rect.anchorMin = new Vector2(anchor.x, anchor.y);
            rect.anchorMax = new Vector2(anchor.z, anchor.w);
            if (adjustPivot)
            {
                rect.pivot = new Vector2(anchor.x, rect.pivot.y);
            }
            rect.offsetMin = new Vector2(rect.offsetMin.x, paddingBottom);
            rect.offsetMax = new Vector2(rect.offsetMax.y, -paddingTop);
            rect.anchoredPosition = new Vector2(offsetX, rect.anchoredPosition.y);
            rect.sizeDelta = new Vector2(width, rect.sizeDelta.y);
        }

        static void SetStretchFull(this RectTransform rect, Vector4 anchor, float paddingLeft, float paddingTop,
            float paddingRight, float paddingBottom)
        {
            rect.anchorMin = new Vector2(anchor.x, anchor.y);
            rect.anchorMax = new Vector2(anchor.z, anchor.w);
            rect.offsetMin = new Vector2(paddingLeft, paddingBottom);
            rect.offsetMax = new Vector2(-paddingRight, -paddingTop);
        }

        public static void Align(this RectTransform rect, UGUITools.enLayoutAlign alignment, bool adjustPivot, Vector2 offset,
            Vector2 size)
        {
            rect.SetAlignment(UGUITools.GetAlignedPivot(alignment), adjustPivot, offset, size);
        }

        public static void Stretch(this RectTransform rect, UGUITools.enLayoutStretch stretch, float leftOrOffsetX,
            float topOrOffsetY, float rightOrWidth, float bottomOrHeight, bool adjustPivot)
        {
            if (stretch == UGUITools.enLayoutStretch.FullStretch)
            {
                rect.SetStretchFull(UGUITools.GetStretchPivot(stretch), leftOrOffsetX, topOrOffsetY, rightOrWidth,
                    bottomOrHeight);
            }
            else if (stretch == UGUITools.enLayoutStretch.HorizontalBottom || stretch == UGUITools.enLayoutStretch.HorizontalCenter ||
                     stretch == UGUITools.enLayoutStretch.HorizontalTop)
            {
                rect.SetStretchHorizontalRect(UGUITools.GetStretchPivot(stretch), adjustPivot, leftOrOffsetX,
                    topOrOffsetY, rightOrWidth, bottomOrHeight);
            }
            else if (stretch == UGUITools.enLayoutStretch.VerticalCenter || stretch == UGUITools.enLayoutStretch.VerticalLeft ||
                     stretch == UGUITools.enLayoutStretch.VerticalRight)
            {
                rect.SetStretchVerticalRect(UGUITools.GetStretchPivot(stretch), adjustPivot, leftOrOffsetX,
                    topOrOffsetY, rightOrWidth, bottomOrHeight);
            }
        }

        public static void SetRectTransformDeltaSizeOld(this RectTransform rect, Vector2 size)
        {
            RectTransform parent = rect.parent as RectTransform;
            if (parent)
            {
                Vector2 parentSize = parent.rect.size;
                Vector2 min = Vector2.Scale(parentSize, rect.anchorMax - rect.anchorMin);
                rect.sizeDelta = size - min;
            }
            else
            {
                rect.sizeDelta = size;
            }
        }

        public static void SetRectTransformDeltaSize(this RectTransform trans, Vector2 newSize)
        {
            Vector2 oldSize = trans.rect.size;
            Vector2 deltaSize = newSize - oldSize;
            trans.offsetMin = trans.offsetMin - new Vector2(deltaSize.x * trans.pivot.x, deltaSize.y * trans.pivot.y);
            trans.offsetMax = trans.offsetMax +
                              new Vector2(deltaSize.x * (1f - trans.pivot.x), deltaSize.y * (1f - trans.pivot.y));
        }
    }
}