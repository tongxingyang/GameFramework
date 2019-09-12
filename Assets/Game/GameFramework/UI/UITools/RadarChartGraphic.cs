using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework.UI.UITools
{
    public class RadarChartGraphic : MaskableGraphic , ICanvasRaycastFilter
    {
        [SerializeField] private Texture texture;
        public bool fill = true;
        public float thickness = 5;
        [Range(3, 360)] public int sides = 3;
        [Range(0, 360)] public float rotation = 3;
        [Range(0, 1)] public float[] VerticesDistances = new float[3];
        public float size = 0;
        public override Texture mainTexture => texture == null ? s_WhiteTexture : texture;
        public Texture Texture
        {
            get => texture;
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

        protected UIVertex[] SetVertexBufferObject(Vector2[] vertices, Vector2[] uvs)
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
                for (int i = 0; i < vertices-1; i++)
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

                    Vector2 pos = new Vector2(rectTransform.position.x,rectTransform.position.y);
                    DrawLine(pos+pos0, pos+pos1, pos+pos2, pos+pos3, local);
//                    if (!vectors.Contains(pos0))
//                    {
//                        vectors.Add(pos0);
//                    }
//                    if (!vectors.Contains(pos1))
//                    {
//                        vectors.Add(pos1);
//                    }
//                    if (!vectors.Contains(pos2))
//                    {
//                        vectors.Add(pos2);
//                    }
//                    if (!vectors.Contains(pos3))
//                    {
//                        vectors.Add(pos3);
//                    }
                    vectors.Add(pos0);
                    vectors.Add(pos1);
                    vectors.Add(pos2);
                    vectors.Add(pos3);
                } 
                if (IsInPolygon(local, vectors))
                {
                    return true;
                }
                
            }
            return false;
        }
        
        private bool IsInPolygon(Vector2 vertex0, Vector2 vertex1, Vector2 vertex2, Vector2 vertex3, Vector2 point)
        {  
            Bounds bounds = new Bounds(new Vector3(0.5f,0.5f,0f),Vector3.zero);
            bounds.Encapsulate(vertex0);
            bounds.Encapsulate(vertex1);
            bounds.Encapsulate(vertex2);
            bounds.Encapsulate(vertex3);
            if (bounds.Contains(point))
            {
                return true;
            }
            return false;
        }
        
        private bool DrawLine(Vector2 vertex0, Vector2 vertex1, Vector2 vertex2, Vector2 vertex3, Vector2 point)
        {  
            UnityEngine.Debug.DrawLine(vertex0,vertex1,Color.red,3);
            UnityEngine.Debug.DrawLine(vertex1,vertex2,Color.red,3);
            UnityEngine.Debug.DrawLine(vertex2,vertex3,Color.red,3);
            UnityEngine.Debug.DrawLine(vertex3,vertex0,Color.red,3);
//            Bounds bounds = new Bounds(new Vector3(0.5f,0.5f,0f),Vector3.zero);
//            bounds.Encapsulate(vertex0);
//            bounds.Encapsulate(vertex1);
//            bounds.Encapsulate(vertex2);
//            bounds.Encapsulate(vertex3);
//            if (bounds.Contains(point))
//            {
//                return true;
//            }
            return false;
        }
        
        public bool IsInPolygon(Vector2 checkPoint, List<Vector2> polygonPoints)
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
        
        public bool IsInPolygon2(Vector2 checkPoint, List<Vector2> polygonPoints)
        {
            int counter = 0;
            int i;
            double xinters;
            Vector2 p1, p2;
            
            int pointCount = polygonPoints.Count;
            p1 = polygonPoints[0];
            for (i = 1; i <= pointCount; i++)
            {
                p2 = polygonPoints[i % pointCount];
                if (checkPoint.y > Math.Min(p1.y, p2.y)//校验点的Y大于线段端点的最小Y 
                    && checkPoint.y <= Math.Max(p1.y, p2.y))//校验点的Y小于线段端点的最大Y 
                {
                    if (checkPoint.x <= Math.Max(p1.x, p2.x))//校验点的X小于等线段端点的最大X(使用校验点的左射线判断). 
                    {
                        if (p1.y != p2.y)//线段不平行于X轴 
                        {
                            xinters = (checkPoint.y - p1.y) * (p2.x - p1.x) / (p2.y - p1.y) + p1.x;
                            if (p1.x == p2.x || checkPoint.x <= xinters)
                            {
                                counter++;
                            }
                        }
                    }
 
                }
                p1 = p2;
            }
 
            if (counter % 2 == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}