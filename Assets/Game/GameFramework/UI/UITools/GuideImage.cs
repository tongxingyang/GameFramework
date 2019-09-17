using UnityEngine;
using UnityEngine.UI;

namespace GameFramework.UI.UITools
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(CanvasRenderer))]
    [DisallowMultipleComponent]
    [AddComponentMenu("UI/GuideImage")]
    public class GuideImage : Image
    {
        public class RectPoint
        {
            class Point
            {
                public float x, y;
                public void Init(float x, float y)
                {
                    this.x = x;
                    this.y = y;
                }
            }

            private Point _p1 = new Point();
            private Point _p2 = new Point();
            private Point _p3 = new Point();
            private Point _p4 = new Point();

            public void Init(float x, float y, float width, float height)
            {
                _p1.Init(x - width, y + height);
                _p2.Init(x - width, y - height);
                _p3.Init(x + width, y - height);
                _p4.Init(x + width, y + height);
            }

            Point p = new Point();
            public bool InRange(Vector2 pos)
            {
                p.Init(pos.x, pos.y);
                return GetCross(_p1, _p2, p) * GetCross(_p3, _p4, p) >= 0 && GetCross(_p2, _p3, p) * GetCross(_p4, _p1, p) >= 0;
            }

            float GetCross(Point p1, Point p2, Point p)
            {
                return (p2.x - p1.x) * (p.y - p1.y) - (p.x - p1.x) * (p2.y - p1.y);
            }
        }
        
        public enum MaskType
        {
            Rect,
            Cricle,
            Global,
        }

        private Vector2 uiPos;
        private MaskType CurrentType = MaskType.Global;
        private float radius;
        public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            if (base.IsRaycastLocationValid(screenPoint, eventCamera))
            {
                if (CurrentType == MaskType.Global)
                {
                    return true;
                }
                if (CurrentType == MaskType.Cricle)
                {
                    Vector2 local;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out local);
                    float tempDis = Vector2.Distance(local, uiPos);
                    if (tempDis <= radius)
                    {
                        return false;
                    }
                }
                if (CurrentType == MaskType.Rect)
                {
                    Vector2 local;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out local);
                    if (bounds.InRange(local))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public void ShowGlobalMask(Color color)
        {
            CurrentType = MaskType.Global;
            raycastTarget = true;
            Vector4 center = Vector4.zero;
            this.color = color;
            material.SetVector("_Center", center);
            material.SetFloat("_Width", 0f);
            material.SetFloat("_Height", 0f);
            material.EnableKeyword("_IsRect");
        }

        public void ShowCricleMask(Color color, RectTransform rectTransform,int radiusOffect = 0, int transition = 10)
        {
            CurrentType = MaskType.Cricle;
            this.color = color;
            Vector4 center;
            raycastTarget = true;
            Vector2 uiScreenPos = AppConst.GlobalCahce.UICamera.WorldToScreenPoint(rectTransform.position);
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                AppConst.GlobalCahce.UIRootCanvas.transform as RectTransform, uiScreenPos,
                AppConst.GlobalCahce.UICamera, out pos);
            uiPos = pos;
            radius = rectTransform.rect.width > rectTransform.rect.height ? rectTransform.rect.width : rectTransform.rect.height;
            radius = radius / 2;
            radius += radiusOffect;
            center = new Vector4(uiPos.x, uiPos.y, 0f, 0f);
            material.SetVector("_Center", center);
            material.SetFloat("_Radius", radius);
            material.SetInt("_TransitionRange", transition);
            material.DisableKeyword("_IsRect");
        }
        
        private  RectPoint bounds = new RectPoint(); 
        
        public void ShowRectMask(Color color, RectTransform rectTransform,int offect = 0, int ellipse = 7)
        {
            CurrentType = MaskType.Rect;
            Vector4 center;
            this.color = color;
            raycastTarget = true;
            float width = 0f, height = 0f;
            Vector2 uiScreenPos = AppConst.GlobalCahce.UICamera.WorldToScreenPoint(rectTransform.position);
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                AppConst.GlobalCahce.UIRootCanvas as RectTransform, uiScreenPos,
                AppConst.GlobalCahce.UICamera, out pos);
            uiPos = pos;
            center = new Vector4(uiPos.x, uiPos.y, 0f, 0f);
            width = rectTransform.sizeDelta.x * rectTransform.localScale.x;
            height = rectTransform.sizeDelta.y * rectTransform.localScale.y;
            width += offect;
            height += offect;
            bounds.Init(uiPos.x, uiPos.y, width, height);
            
            material.SetVector("_Center", center);
            material.SetFloat("_Width", width);
            material.SetFloat("_Height", height);
            material.SetInt("_Ellipse", ellipse);
            material.EnableKeyword("_IsRect");
        }
    }
}