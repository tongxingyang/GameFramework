using UnityEngine;
using UnityEngine.UI;

namespace GameFramework.UI.UITools
{
    public class UGUITools
    {
        public enum enLayoutAlign
        {
            TopLeft,
            Top,
            TopRight,
            Left,
            Center,
            Right,
            BottomLeft,
            Bottom,
            BottomRight,
        }
    
        public enum enLayoutStretch
        {
            HorizontalTop,
            HorizontalCenter,
            HorizontalBottom,
            VerticalLeft,
            VerticalCenter,
            VerticalRight,
            FullStretch
        }
    
        public enum enLayoutDirection
        {
            Horizontal = 1,
            Vertical
        }
        
        public static T SecureComponetInParent<T>(Transform t) where T : MonoBehaviour
        {
            T tObj = null;

            if (t != null)
                t = t.parent;

            while (t != null)
            {
                tObj = t.GetComponent<T>();
                if (tObj != null)
                    break;
                t = t.parent;
            }

            return tObj;
        }

        public static void SetVisible(Transform t, bool flag)
        {
            t.localScale = flag ? Vector3.one : Vector3.zero;
        }

        public static Object Instantiate(Object original)
        {
            Object obj = null;
            if (original != null)
                obj = Object.Instantiate(original);
            return obj;
        }

        public static Vector2 GetAlignedPivot(enLayoutAlign align)
        {
            Vector2 pivot = Vector2.zero;
            switch (align)
            {
                case enLayoutAlign.Left:
                    pivot = new Vector2(0f, 0.5f);
                    break;
                case enLayoutAlign.Right:
                    pivot = new Vector2(1f, 0.5f);
                    break;
                case enLayoutAlign.Top:
                    pivot = new Vector2(0.5f, 1f);
                    break;
                case enLayoutAlign.Bottom:
                    pivot = new Vector2(0.5f, 0f);
                    break;
                case enLayoutAlign.Center:
                    pivot = new Vector2(0.5f, 0.5f);
                    break;
                case enLayoutAlign.TopLeft:
                    pivot = new Vector2(0f, 1f);
                    break;
                case enLayoutAlign.TopRight:
                    pivot = new Vector2(1f, 1f);
                    break;
                case enLayoutAlign.BottomLeft:
                    pivot = new Vector2(0f, 0f);
                    break;
                case enLayoutAlign.BottomRight:
                    pivot = new Vector2(1f, 0f);
                    break;
            }
            return pivot;
        }

        public static Vector4 GetStretchPivot(enLayoutStretch stretch)
        {
            Vector4 pivot = Vector4.zero;
            switch (stretch)
            {
                case enLayoutStretch.HorizontalTop:
                    pivot = new Vector4(0, 1f, 1f, 1f);
                    break;
                case enLayoutStretch.HorizontalCenter:
                    pivot = new Vector4(0, 0.5f, 1f, 0.5f);
                    break;
                case enLayoutStretch.HorizontalBottom:
                    pivot = new Vector4(0, 0, 1f, 0);
                    break;
                case enLayoutStretch.VerticalLeft:
                    pivot = new Vector4(0, 0, 0, 1f);
                    break;
                case enLayoutStretch.VerticalCenter:
                    pivot = new Vector4(0.5f, 0, 0.5f, 1f);
                    break;
                case enLayoutStretch.VerticalRight:
                    pivot = new Vector4(1f, 0, 1f, 1f);
                    break;
                case enLayoutStretch.FullStretch:
                    pivot = new Vector4(0, 0, 1f, 1f);
                    break;
            }
            return pivot;
        }

        public static float ClampScrollPos(float flatPos, RectTransform rt, ScrollRect scroll)
        {
            if (scroll != null && rt != null)
            {
                RectTransform scrollTransform = scroll.viewport ?? scroll.GetComponent<RectTransform>();
                if (scrollTransform != null)
                {
                    float max = scroll.vertical
                        ? rt.rect.height - scrollTransform.rect.height
                        : rt.rect.width - scrollTransform.rect.width;
                    if (flatPos > max) flatPos = max;
                    if (flatPos < 0) flatPos = 0;
                }
            }
            return flatPos;
        }

        public static Rect GetRelativeRect(RectTransform rt_root, RectTransform rt_item)
        {
            if (rt_item == null) return new Rect(0, 0, 0, 0);
            if (rt_root == null || rt_item == rt_root) return rt_item.rect;
            Vector3 l_pos = rt_root.InverseTransformPoint(rt_item.position);
            Rect rect_item = rt_item.rect;
            rect_item.x += l_pos.x;
            rect_item.y += l_pos.y;
            return rect_item;
        }
    }
}