using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Animation.MeshAnimation
{
    public  partial class MeshAnimationDataItem
    {
        public float Fps;
        public float FrameLength;
        public ExportMeshAnimationData ExportMeshAnimationData;
        public Dictionary<int,Dictionary<string,AnimationMeshData>> animationSubMeshDic = new Dictionary<int, Dictionary<string, AnimationMeshData>>();
        public MeshAnimationDataItem(ExportMeshAnimationData pData)
        {
            Fps = pData.Fps;
            FrameLength = 1f / Fps;
            ExportMeshAnimationData = pData;
            PauseMeshAnimationData();
        }

        private void PauseMeshAnimationData()
        {
            for (int i = 0; i < ExportMeshAnimationData.SubMeshLength; i++)
            {
                GenerateMeshAnimationDataBySubMeshId(i);
            }
        }

        private void GenerateMeshAnimationDataBySubMeshId(int subMeshIndex)
        {
            for (int i = 0; i < ExportMeshAnimationData.SubMeshData[subMeshIndex].ClipDatas.Length; i++)
            {
                ExportMeshAnimationData.AnimationClipData clipData = ExportMeshAnimationData.SubMeshData[subMeshIndex].ClipDatas[i];
                int frameCount = clipData.FrameDatas.Length;
                Vector3[][] animData = new Vector3[frameCount][];
                for (int j = 0; j < clipData.FrameDatas.Length; j++)
                {
                    animData[j] = clipData.FrameDatas[j].Vertexs;
                }
                Dictionary<string, AnimationMeshData> animationDataDict;
                bool isHas = animationSubMeshDic.TryGetValue(subMeshIndex, out animationDataDict);
                if (!isHas)
                {
                    animationDataDict = new Dictionary<string, AnimationMeshData>();
                    animationSubMeshDic.Add(subMeshIndex,animationDataDict);
                }
                AnimationMeshData animationMeshData = new AnimationMeshData
                {
                    Name = ExportMeshAnimationData.SubMeshData[subMeshIndex].ClipDatas[i].ClipName,
                    Frames = animData,
                    GenerateNormal = ExportMeshAnimationData.GenerateNormal,
                    FrameCount = animData.Length,
                    Uv = ExportMeshAnimationData.SubMeshData[subMeshIndex].UVs,
                    Triangles = ExportMeshAnimationData.SubMeshData[subMeshIndex].Triangles,
                };

                animationDataDict.Add(ExportMeshAnimationData.SubMeshData[subMeshIndex].ClipDatas[i].ClipName,animationMeshData);
            }
        }

        public void UpdateMeshWithSubMesh(int frame, string animName, List<MeshFilter> meshFilterList)
        {
            for (int i = 0; i < meshFilterList.Count; i++)
            {
                MeshFilter meshFilter = meshFilterList[i];
                Dictionary<string, AnimationMeshData> animationDataDict = animationSubMeshDic[i];
                AnimationMeshData anim;
                bool ret = animationDataDict.TryGetValue(animName, out anim);
                if (ret)
                {
                    meshFilter.mesh = anim.GetFrame(frame);
                }
            }
        }
        
        public void Destroy()
        {
            if (null != animationSubMeshDic)
            {
                var itor = animationSubMeshDic.GetEnumerator();
                while (itor.MoveNext())
                {
                    Dictionary<string, AnimationMeshData> animDict = itor.Current.Value;
                    if (null != animDict)
                    {
                        var animItor = animDict.GetEnumerator();
                        while (animItor.MoveNext())
                        {
                            animItor.Current.Value.Destroy();
                        }

                        animItor.Dispose();
                        animDict.Clear();
                    }
                }

                itor.Dispose();
                animationSubMeshDic.Clear();
                animationSubMeshDic = null;
            }
            ExportMeshAnimationData = null;
        }
    }
}