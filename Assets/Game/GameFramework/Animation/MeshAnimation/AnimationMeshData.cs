using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace GameFramework.Animation.MeshAnimation
{
    public partial class MeshAnimationDataItem
    {
        public class AnimationMeshData
        {
            public string Name;
            public Vector3[][] Frames;
            public bool GenerateNormal;
            public int FrameCount;
            public float SpeedScale = 1.0f;
            private Mesh[] frameMeshs;
            public Vector2[] Uv;
            public int[] Triangles;
            
            private bool isInitFrame = false;
            private UnityEngine.Vector3[] GetFrameVertices(int frame)
            {
                if (frame < Frames.Length && frame >= 0)
                {
                    return Frames[frame];
                }
                return null;
            }
            
            public void GenerateMeshAnimationFrames()
            {
                frameMeshs = new Mesh[FrameCount];
                for (int i = 0; i < FrameCount; i++)
                {
                    Mesh mesh = new Mesh
                    {
                        name = "SkinMesh",
                        vertices = GetFrameVertices(i),
                        uv = Uv,
                        triangles = Triangles
                    };
                    if (GenerateNormal)
                    {
                        mesh.RecalculateNormals();
                    }
                    frameMeshs[i] = mesh;
                }
                Uv = null;
                Triangles = null;
                Frames = null;
            }

            public Mesh GetFrame(int frame)
            {
                if (!isInitFrame)
                {
                    GenerateMeshAnimationFrames();
                    isInitFrame = true;
                }
                if (frame >= 0 && frame < frameMeshs.Length)
                {
                    return frameMeshs[frame];
                }
                return null;
            }

            public void Destroy()
            {
                if (frameMeshs != null)
                {
                    for (int i = 0; i < frameMeshs.Length; i++)
                    {
                        Object.DestroyImmediate(frameMeshs[i], true);
                    }

                    frameMeshs = null;
                }
                Uv = null;
                Triangles = null;
                Frames = null;
            }
        }
    }
}