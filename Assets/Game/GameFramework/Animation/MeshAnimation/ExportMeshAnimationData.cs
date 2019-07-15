using System;
using UnityEngine;

namespace GameFramework.Animation.MeshAnimation
{
    public class ExportMeshAnimationData : ScriptableObject
    {
        public int SubMeshLength;
        public float Fps;
        public bool GenerateNormal;
        public AnimationSubMeshData[] SubMeshData;
    
        [Serializable]
        public struct AnimationFrameData
        {
            public Vector3[] Vertexs;
        }
    
        [Serializable]
        public struct AnimationClipData
        {
            public string ClipName;
            public AnimationFrameData[] FrameDatas;
        }
    
        [Serializable]
        public struct AnimationSubMeshData
        {
            public float FrameRate;
            public AnimationClipData[] ClipDatas;
            public int[] Triangles;
            public Vector2[] UVs;
        }
    }
}