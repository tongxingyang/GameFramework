using System;
using System.Collections.Generic;
using System.IO;
using GameFramework.Animation.MeshAnimation;
using GameFramework.Editor.Core.AssetImportSetting;
using UnityEditor;
using UnityEngine;

namespace GameFramework.Editor.Core.Animation
{
public class ExportMeshAnimationWindow : EditorWindow {

    public class ExportMeshAnimationParams
    {
        public float FrameRate = 30f;
        public AnimationClip[] AnimationClips;
        public string[] AnimationNames;
        public string OutputFolderPath = "";
        public string OutputFilePath = "";
        public bool GenerateNormal = false;
    }

    private ExportMeshAnimationParams exportParams = new ExportMeshAnimationParams();
    private GameObject fbx;
    private int numberOfClips;
    private string outputFolderPath = "";
    private string outputFilePath = "";
    
    [MenuItem("Tools/Animation/Export Mesh Animation")]
    static void CreateWizard()
    {
        ExportMeshAnimationWindow window = GetWindow<ExportMeshAnimationWindow>();
        window.ShowUtility();
    }
    
    private  string GetProjectPath()
    {
        return Application.dataPath.Substring (0, Application.dataPath.LastIndexOf ("/", StringComparison.Ordinal));
    }
    
    public  string GetAssetPath (string pAbsolutePath)
    {
        string projectPath = GetProjectPath();
        if(pAbsolutePath.StartsWith(projectPath))
        {
            string relativePath = pAbsolutePath.Substring(projectPath.Length, pAbsolutePath.Length - projectPath.Length);

            if(relativePath.StartsWith("/") || relativePath.StartsWith("\\"))
            {
                relativePath = relativePath.Substring(1, relativePath.Length - 1);
            }

            return relativePath+"/";
        }

        return null;
    }

    private void BrowseFile()
    {
        string output = EditorUtility.SaveFolderPanel(
            "保存输出文件",
            outputFolderPath,
            ""
        );

        if (!string.IsNullOrEmpty(output.Trim()))
        {
            exportParams.OutputFilePath = outputFilePath = output;
            exportParams.OutputFolderPath = outputFolderPath = Path.GetDirectoryName(output);
        }

        GUI.FocusControl("");
    }

    private void GenerateSetting(GameObject gameObject)
    {
        GameObject instanceObj = Instantiate(gameObject);
        UnityEngine.Animation animation = instanceObj.GetComponentInChildren<UnityEngine.Animation>();
        if (!animation)
        {
            return;
        }
        int index = 0;
        int animclipCount = animation.GetClipCount();
        AnimationClip[] animationClips = new AnimationClip[animclipCount];
        string[] animationClipNames = new string[animclipCount];
        foreach (AnimationState animationState in animation)
        {
            animationClips[index] = animationState.clip;
            animationClipNames[index] = animationState.clip.name;
            index++;
        }
        DestroyImmediate(instanceObj);
        exportParams.AnimationClips = animationClips;
        exportParams.AnimationNames = animationClipNames;

    }

    private void DrawAnimationClipsInfo()
    {
        float interval = 1.0f / exportParams.FrameRate;
        if (exportParams.AnimationClips != null && exportParams.AnimationNames != null &&
            exportParams.AnimationClips.Length > 0)
        {
            for (int i = 0; i < exportParams.AnimationNames.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    exportParams.AnimationNames[i] = EditorGUILayout.TextField(exportParams.AnimationNames[i]);
                    AnimationClip newclip = EditorGUILayout.ObjectField(exportParams.AnimationClips[i],typeof(AnimationClip),false) as AnimationClip;
                    if (newclip != null && newclip != exportParams.AnimationClips[i])
                    {
                        exportParams.AnimationClips[i] = newclip;
                        exportParams.AnimationNames[i] = newclip.name;
                    }
                    EditorGUILayout.LabelField((exportParams.AnimationClips[i].length/interval).ToString());
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    private void ExportMeshAnimation()
    {
        AnimationClip[] animationClips = exportParams.AnimationClips;
        string[] animationClipNames = exportParams.AnimationNames;
        GameObject instanceObj = Instantiate(fbx) as GameObject;
        UnityEngine.Animation animation = instanceObj.GetComponentInChildren<UnityEngine.Animation>();
        SkinnedMeshRenderer[] skinnedMeshRenders = instanceObj.GetComponentsInChildren<SkinnedMeshRenderer>();
        int subMeshLength = skinnedMeshRenders.Length;
        Mesh[] subMeshArr = new Mesh[subMeshLength];
        for (int i = 0; i < subMeshLength; i++)
        {
            subMeshArr[i] = skinnedMeshRenders[i].sharedMesh;
        }
        float interval = 1.0f / exportParams.FrameRate;
        if (File.Exists(exportParams.OutputFilePath))
        {
            File.Delete(exportParams.OutputFilePath);
        }
        ExportMeshAnimationData meshAnimationData = ScriptableObject.CreateInstance<ExportMeshAnimationData>();
        meshAnimationData.GenerateNormal = exportParams.GenerateNormal;
        meshAnimationData.SubMeshLength = subMeshLength;
        meshAnimationData.Fps = exportParams.FrameRate;
        meshAnimationData.SubMeshData = new ExportMeshAnimationData.AnimationSubMeshData[subMeshLength];
        for (int i = 0; i < subMeshLength; i++)
        {
            meshAnimationData.SubMeshData[i].ClipDatas = new ExportMeshAnimationData.AnimationClipData[animationClips.Length];
            meshAnimationData.SubMeshData[i].FrameRate = exportParams.FrameRate;
            for (int j = 0; j < animationClips.Length; j++)
            {
                AnimationClip clip = animationClips[j];
                if(clip==null)return;
                animation.AddClip(clip,animationClipNames[j]);
                animation.clip = clip;
                AnimationState state = animation[animationClipNames[j]];
                state.enabled = true;
                state.weight = 1;
                List<float> frameTimes = GetFrameTimes(clip.length, interval);
                meshAnimationData.SubMeshData[i].ClipDatas[j].FrameDatas = new ExportMeshAnimationData.AnimationFrameData[frameTimes.Count];
                meshAnimationData.SubMeshData[i].ClipDatas[j].ClipName = animationClipNames[j];
                for (int k = 0; k < frameTimes.Count; k++)
                {
                    state.time = frameTimes[k];
                    animation.Play();
                    animation.Sample();
                    Matrix4x4 matrix4X4 = skinnedMeshRenders[i].transform.localToWorldMatrix;
                    Mesh backMesh = BakeFrameAfterMatrixTransform(skinnedMeshRenders[i], matrix4X4);
                    meshAnimationData.SubMeshData[i].ClipDatas[j].FrameDatas[k].Vertexs = backMesh.vertices;
                    backMesh.Clear();
                    DestroyImmediate(backMesh);
                    animation.Stop();
                }
            }
            meshAnimationData.SubMeshData[i].UVs = subMeshArr[i].uv;
            meshAnimationData.SubMeshData[i].Triangles = subMeshArr[i].triangles;
        }
        AssetDatabase.CreateAsset(meshAnimationData,GetAssetPath(exportParams.OutputFilePath)+fbx.name+".asset");
        AssetDatabase.Refresh();
    }
    
    private Mesh BakeFrameAfterMatrixTransform(SkinnedMeshRenderer pRenderer, Matrix4x4 matrix) 
    {
        Mesh result = new Mesh();
        pRenderer.BakeMesh(result);
        Vector3[] resVector3 = new Vector3[result.vertices.Length];
        for (int i = 0; i < result.vertices.Length; i++)
        {
            resVector3[i] = matrix.MultiplyPoint3x4(result.vertices[i]);
        }
        result.vertices = resVector3;
        return result;
    }
    
    private List<float> GetFrameTimes(float pLength, float pInterval)
    {
        List<float> times = new List<float>();
        float time = 0;
        do
        {
            times.Add(time);
            time += pInterval;
        } while (time < pLength - pInterval);
        times.Add(pLength);
        return times;
    }
    
    void OnGUI()
    {
        if (string.IsNullOrEmpty(outputFolderPath))
        {
            outputFolderPath = Application.streamingAssetsPath;
        }
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("默认文件夹 : "+exportParams.OutputFolderPath);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("输出文件 : ");
        EditorGUILayout.BeginHorizontal();
        {
            string relativeFilePath = GetAssetPath(outputFilePath);
            EditorGUILayout.SelectableLabel(relativeFilePath, EditorStyles.textField,
                GUILayout.Height(EditorGUIUtility.singleLineHeight));
            if (GUILayout.Button("浏览文件夹"))
            {
                BrowseFile();
            }
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("模型文件: ");
            GameObject gameObject = EditorGUILayout.ObjectField(fbx,typeof(GameObject),false) as GameObject;
            if (gameObject != null && fbx != gameObject)
            {
                fbx = gameObject;
                GenerateSetting(gameObject);
                if (exportParams.AnimationClips != null)
                {
                    numberOfClips = exportParams.AnimationClips.Length;
                }
            }
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        exportParams.FrameRate =
            EditorGUILayout.FloatField(new GUIContent("Capture Framerate"), exportParams.FrameRate);
        exportParams.FrameRate = Mathf.Max(exportParams.FrameRate, 1.0f);
        EditorGUILayout.Space();
        
        exportParams.GenerateNormal =
            EditorGUILayout.Toggle(new GUIContent("GenerateNormal"), exportParams.GenerateNormal);
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("Animation Count: "+numberOfClips);
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("Animation Name:");
            EditorGUILayout.LabelField("Animation File:");
            EditorGUILayout.LabelField("Frames:");
        }
        EditorGUILayout.EndHorizontal();
        DrawAnimationClipsInfo();
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("清除"))
            {
                exportParams = new ExportMeshAnimationParams();
            }

            if (GUILayout.Button("导出"))
            {
                if(exportParams == null || fbx == null) return;
                ExportMeshAnimation();
            }
        }
    }
}
}