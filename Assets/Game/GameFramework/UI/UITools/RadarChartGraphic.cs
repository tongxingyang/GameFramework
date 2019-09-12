using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework.UI.UITools
{
    //点击检测只实现外框的范围 中间镂空的没有处理
    public class RadarChartGraphic : MaskableGraphic , ICanvasRaycastFilter
    {
        [SerializeField] private Texture texture;
        public bool fill = true;
        public float thickness = 5;
        [Range(3, 360)] public int sides = 3;
        [Range(0, 360)] public float rotation = 3;
        [Range(0, 1)] public float[] VerticesDistances = new float[3];
        public float size = 0;
        public override Texture mainTexture 
        {
            get
            {
                return   s_WhiteTexture;
            }
        }
        public Texture Texture
        {
            get
            {
                
                return texture;
            }
            set
            {
                if(texture != null && texture == value)return;
                texture = value;
                SetVerticesDirty();
                SetMaterialDirty();
            }
        }

        public void DrawPolygon(int count)
        {
            sides = count;
            VerticesDistances = new float[sides + 1];
            for (int i = 0; i < sides; i++)
            {
                VerticesDistances[i] = 1;
            }
            rotation = 0;
        }

        public void DrawPolygon(int count, float[] vertices)
        {
            sides = count;
            VerticesDistances = vertices;
            rotation = 0;
        }

        void Update()
        {
            size = rectTransform.rect.width > rectTransform.rect.height ? rectTransform.rect.height : rectTransform.rect.width;
            thickness = Mathf.Clamp(thickness, 0, size / 2);
        }

        private UIVertex[] SetVertexBufferObject(Vector2[] vertices, Vector2[] uvs)
        {
            UIVertex[] VBO = new UIVertex[4];
            for (int i = 0; i < vertices.Length; i++)
            {
                var vert = UIVertex.simpleVert;
                vert.color = color;
                vert.position = vertices[i];
                vert.uv0 = uvs[i];
                VBO[i] = vert;
            }
            return VBO;
        }
        
        Vector2 pos0 = Vector2.zero;
        Vector2 pos1 = Vector2.zero;
        Vector2 pos2 = Vector2.zero;
        Vector2 pos3 = Vector2.zero;
        private ICanvasRaycastFilter _canvasRaycastFilterImplementation;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            Vector2 prevX = Vector2.zero; 
            Vector2 prevY = Vector2.zero;
            Vector2 uv0 = new Vector2(1, 0);
            Vector2 uv1 = new Vector2(0, 0);
            Vector2 uv2 = new Vector2(0, 1);
            Vector2 uv3 = new Vector2(1, 1);
            float degrees = 360f / sides;
            int vertices = sides + 1;
            if (VerticesDistances.Length != vertices)
            {
                VerticesDistances = new float[vertices];
                for (int i = 0; i < vertices-1; i++)
                {
                    VerticesDistances[i] = 1;
                }
            }
            VerticesDistances[vertices - 1] = VerticesDistances[0];
            for (int i = 0; i < vertices; i++)
            {
                float outer = 0.5f * size * VerticesDistances[i];
                float inner = 0.5f * size * VerticesDistances[i] - thickness;
                float rad = Mathf.Deg2Rad * (i * degrees + rotation);
                float c = Mathf.Cos(rad);
                float s = -Mathf.Sin(rad);
                pos0 = prevX;
                pos1 = new Vector2(outer * c, outer * s);
                if (fill)
                {
                    pos2 = Vector2.zero;
                    pos3 = Vector2.zero;
                }
                else
                {
                    pos2 = new Vector2(inner * c, inner * s);
                    pos3 = prevY;
                }
                prevX = pos1;
                prevY = pos2;
                vh.AddUIVertexQuad(SetVertexBufferObject(new[] { pos0, pos1, pos2, pos3 }, new[] { uv0, uv1, uv2, uv3 }));
            }
        }
        private List<Vector2> vectors = new List<Vector2>();
        public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            if (raycastTarget)
            {
                Vector2 local;
                Vector2 prevX = Vector2.zero; 
                Vector2 prevY = Vector2.zero;
                vectors.Clear();
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, sp, eventCamera, out local);
                float degrees = 360f / sides;
                int vertices = sides + 1;
                for (int i = 0; i < vertices; i++)
                {
                    float outer = 0.5f * size * VerticesDistances[i];
                    float inner = 0.5f * size * VerticesDistances[i] - thickness;
                    float rad = Mathf.Deg2Rad * (i * degrees + rotation);
                    float c = Mathf.Cos(rad);
                    float s = -Mathf.Sin(rad);
                    pos0 = prevX;
                    pos1 = new Vector2(outer * c, outer * s);
                    if (fill)
                    {
                        pos2 = Vector2.zero;
                        pos3 = Vector2.zero;
                    }
                    else
                    {
                        pos2 = new Vector2(inner * c, inner * s);
                        pos3 = prevY;
                    }
                    prevX = pos1;
                    prevY = pos2;
//                    if (fill)
                    {
                        if (prevX != Vector2.zero)
                        {
                            vectors.Add(prevX);
                        }
                    }
//                    else
                    {
//                        if (prevY != Vector2.zero)
//                        {
//                            vectors.Add(prevY);
//                        }
                    }
                }

                if (IsInPolygon(local, vectors))
                {
                    return true;
                }
//                if (fill)
//                {
//                    if (IsInPolygon(local, vectors))
//                    {
//                        return true;
//                    }
//                }
//                else
//                {
//                    if (!IsInPolygon(local, vectors))
//                    {
//                        return true;
//                    }
//                }
            }
            return false;
        }
        
        private bool IsInPolygon(Vector2 checkPoint, List<Vector2> polygonPoints)
        {
            bool inside = false;
            int pointCount = polygonPoints.Count;
            for (int i = 0, j = pointCount - 1; i < pointCount; j = i, i++) 
            {
                var p1 = polygonPoints[i];
                var p2 = polygonPoints[j];
                if (checkPoint.y < p2.y)
                {
                    if (p1.y <= checkPoint.y)
                    {
                        if ((checkPoint.y - p1.y) * (p2.x - p1.x) > (checkPoint.x - p1.x) * (p2.y - p1.y))
                        {
                            inside = (!inside);
                        }
                    }
                }
                else if (checkPoint.y < p1.y)
                {
                    if ((checkPoint.y - p1.y) * (p2.x - p1.x) < (checkPoint.x - p1.x) * (p2.y - p1.y))
                    {
                        inside = (!inside);
                    }
                }
            }
            return inside;
        }
    }
}