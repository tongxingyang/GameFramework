using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameFrame.Editor
{
    public class FBXEditor  {

	[MenuItem("FBXTools/保存Mash&Texture2")]
	public static void SaveMesh()
	{
		Object[] fbxs = Selection.GetFiltered<Object>(SelectionMode.DeepAssets);
		if (fbxs != null)
		{
			for (int i = 0; i < fbxs.Length; i++)
			{
				GameObject fbx = fbxs[i] as GameObject;
				string path = "";
				if (fbx != null)
				{
					path = AssetDatabase.GetAssetPath(fbx);
					ModelImporter modelImporter = AssetImporter.GetAtPath(path) as ModelImporter;
					if (modelImporter != null)
					{
						string saveRootPath = "Assets/Resources/Equipments/";
						if (!Directory.Exists(saveRootPath))
						{
							Directory.CreateDirectory(saveRootPath);
						}
						modelImporter.isReadable = true;
						GameObject go = Object.Instantiate(fbx);
						SkinnedMeshRenderer[] smrs = go.GetComponentsInChildren<SkinnedMeshRenderer>();
						for (int j = 0; j < smrs.Length; j++)
						{
							//创建Mesh
							Mesh mesh = Object.Instantiate(smrs[j].sharedMesh);
							mesh.name = smrs[j].sharedMesh.name;
							SaveMeshAsset(mesh,smrs[j].sharedMaterial.mainTexture as Texture2D,saveRootPath );
						}
						modelImporter.isReadable = false;
						Object.DestroyImmediate(go);
					}
				}
			}
		}
	}
	
	public static void CleanMesh(Mesh mesh)
	{
		mesh.uv2 = null;
		mesh.uv3 = null;
		mesh.uv4 = null;
		mesh.colors = null;
		mesh.colors32 = null;
		mesh.tangents = null;
	}
	
	public static void SaveMeshAsset(Mesh mesh, Texture2D tex, string path)
	{
		CleanMesh(mesh);
		string meshPath = path + mesh.name + ".asset";
		AssetDatabase.CreateAsset(mesh,meshPath);
		if (tex != null)
		{
			string srcPath = AssetDatabase.GetAssetPath(tex);
			string desPath =  "Assets/Resources/Equipments/" + tex.name + ".tga";
			AssetDatabase.CopyAsset(srcPath, desPath);
		}
		AssetDatabase.SaveAssets();
	}
	[MenuItem("FBXTools/优化FBX")]
	private static void OptmizeFbx()
	{
		Rect rect = new Rect(0,0,600,800);
		SelectFbxEditor window =
			(SelectFbxEditor) EditorWindow.GetWindowWithRect(typeof(SelectFbxEditor), rect, true, "Fbx面板");
		window.Init();
		window.Show();
	}
	
	public class SelectFbxEditor:EditorWindow
	{
		protected bool checkAll = false;
		protected GameObject model = null;
		protected GameObject newGo = null;
		protected List<string> exposedBones = new List<string>();
		protected List<string> ExposedBones = new List<string>();
		protected ModelBone root = new ModelBone();
		protected ModelImporter Importer;
		protected string Path = "";
		protected Vector2 scrollPos = Vector2.zero;
		protected string createFolderName = "";
		protected bool gameResource = true;
		protected string spriteName = "sprite";
		/// <summary>
		/// Bone类
		/// </summary>
		public class ModelBone
		{
			public bool Check = false;
			public string BoneName = "";
			public string Path = "";
			public ModelBone Parent = null;
			public List<ModelBone> Child = new List<ModelBone>();
		}
		public void Init()
		{
			Object[] fbxs = Selection.GetFiltered<GameObject>(SelectionMode.DeepAssets);
			if (fbxs != null && fbxs.Length > 0)
			{
				model = fbxs[0] as GameObject;
				Path = "";
				if (model != null)
				{
					Path = AssetDatabase.GetAssetPath(model);
					Importer = AssetImporter.GetAtPath(Path) as ModelImporter;
					Importer.optimizeGameObjects = false;// todo txy
					Importer.extraExposedTransformPaths = null;// todo txy
					AssetDatabase.ImportAsset(Path, ImportAssetOptions.ForceUpdate);
					FilterBone();
				}
			}
			AssetDatabase.Refresh();
		}

		public void OnGUI()
		{
			scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true));
			DrawTree(root, 0);
			GUILayout.EndScrollView();
			bool beforeCheck = checkAll;
			checkAll = EditorGUILayout.ToggleLeft("Check All", checkAll);
			if (beforeCheck != checkAll)
			{
				CheckAll(root);
			}
			gameResource = EditorGUILayout.ToggleLeft("Game Resource", gameResource);
			if (GUILayout.Button("OK", GUILayout.ExpandWidth(false)))
			{
				MakeGameObject();
				Close();
			}
			if (GUILayout.Button("Cancel", GUILayout.ExpandWidth(false)))
			{
				Close();
			}
		}
		/// <summary>
		/// 添加Bone
		/// </summary>
		/// <param name="bone"></param>
		public void AddBone(string bone)
		{
			if (!ExposedBones.Contains(bone))
			{
				ExposedBones.Add(bone);
			}
		}

		/// <summary>
		/// 过滤Bone
		/// </summary>
		/// <returns></returns>
		public void FilterBone()
		{
			if (model != null)
			{
				root.BoneName = model.name;
				FindModelBones(model.transform,"",root);
			}
		}
		/// <summary>
		/// 查找模型Bone
		/// </summary>
		/// <param name="t"></param>
		/// <param name="parentPath"></param>
		/// <param name="parent"></param>
		public ModelBone FindModelBones(Transform t, string parentPath, ModelBone parent)
		{
			ModelBone modelBone = null;
			if (t != model.transform)
			{
				modelBone = new ModelBone();
			}
			if (modelBone != null)
			{
				modelBone.BoneName = t.name;
				modelBone.Path = parentPath + t.name;
				modelBone.Check = exposedBones.Contains(modelBone.Path) || ExposedBones.Contains(t.name);
				modelBone.Parent = parent;
			}
			if (t != model.transform)
			{
				parentPath = parentPath + t.name + "/";
			}
			for (int i = 0; i < t.childCount; i++)
			{
				ModelBone childModelBone = FindModelBones(t.GetChild(i), parentPath, modelBone);
				if (modelBone != null)
					modelBone.Child.Add(childModelBone);
				else
					parent.Child.Add(childModelBone);
			}
			return modelBone;
		}
		/// <summary>
		/// 查找SkinnedMesh
		/// </summary>
		/// <param name="t"></param>
		/// <param name="list"></param>
		private void FindSkinnedMesh(Transform t,List<SkinnedMeshRenderer> list)
		{
			SkinnedMeshRenderer temp = t.GetComponent<SkinnedMeshRenderer>();
			if (temp != null)
			{
				list.Add(temp);
			}
			for (int i = 0; i < t.childCount; i++)
			{
				FindSkinnedMesh(t.GetChild(i),list);
			}
		}
		/// <summary>
		/// 查找Mesh
		/// </summary>
		/// <param name="t"></param>
		/// <param name="list"></param>
		private void FindMesh(Transform t, List<MeshRenderer> list)
		{
			MeshRenderer temp = t.GetComponent<MeshRenderer>();
			if (temp != null)
			{
				list.Add(temp);
			}
			for (int i = 0; i < t.childCount; i++)
			{
				FindMesh(t.GetChild(i),list);
			}
		}
		/// <summary>
		/// 绘制Tree
		/// </summary>
		/// <param name="modelBone"></param>
		/// <param name="depth"></param>
		private void DrawTree(ModelBone modelBone,int depth)
		{
			GUILayout.BeginHorizontal();
			for (int i = 0; i < depth; i++)
			{
				GUILayout.Label(" ",GUILayout.ExpandWidth(false));
			}
			modelBone.Check = EditorGUILayout.Toggle(modelBone.Check, GUILayout.Width(30));
			GUILayout.Label(modelBone.BoneName,GUILayout.ExpandWidth(false));
			GUILayout.EndHorizontal();
			for (int i = 0; i < modelBone.Child.Count; i++)
			{
				DrawTree(modelBone.Child[i],depth+1);
			}
		}
		/// <summary>
		/// 导出Bone
		/// </summary>
		/// <param name="modelBone"></param>
		private void ExposedBone(ModelBone modelBone)
		{
			if (modelBone.Check && !exposedBones.Contains(modelBone.Path))
			{
				exposedBones.Add(modelBone.Path);
			}
			for (int i = 0; i < modelBone.Child.Count; i++)
			{
				ExposedBone(modelBone.Child[i]);
			}
		}

		private void SetLayer(Transform t,int layer)
		{
			
		}

		private bool CopyValue<T>(GameObject srcGO, GameObject destGO) where T : Component
		{
			T src = srcGO.GetComponent<T>();
			if (src == null)
			{
				return false;
			}
			T dest = destGO.GetComponent<T>();
			if (dest == null)
			{
				dest = destGO.AddComponent<T>();
			}
			//复制粘贴Component的值
			ComponentUtility.CopyComponent(src);
			ComponentUtility.PasteComponentValues(dest);
			return true;
		}
		
		private void CheckAll(ModelBone modelBone)
		{
			modelBone.Check = checkAll;
			for (int i = 0; i < modelBone.Child.Count; i++)
			{
				CheckAll(modelBone.Child[i]);
			}
		}

		protected void MakeGameObject()
		{
			ExposedBone(root);
			Importer.optimizeGameObjects = true;
			Importer.isReadable = false;
			Importer.extraExposedTransformPaths = exposedBones.ToArray();
			AssetDatabase.ImportAsset(Path,ImportAssetOptions.ForceUpdate);
			newGo = Instantiate(model);
			newGo.name = model.name;
			if (newGo != null)
			{
				List<SkinnedMeshRenderer> skins = new List<SkinnedMeshRenderer>();
				FindSkinnedMesh(newGo.transform,skins);
				foreach (SkinnedMeshRenderer skinnedMeshRenderer in skins)
				{
					skinnedMeshRenderer.receiveShadows = false;
					skinnedMeshRenderer.lightProbeUsage = LightProbeUsage.BlendProbes;
					skinnedMeshRenderer.shadowCastingMode = ShadowCastingMode.Off;
				}
				List<MeshRenderer> meshs = new List<MeshRenderer>();
				FindMesh(newGo.transform,meshs);
				foreach (MeshRenderer meshRenderer in meshs)
				{
					meshRenderer.receiveShadows = false;
					meshRenderer.lightProbeUsage = LightProbeUsage.BlendProbes;
					meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
				}
				GameObject spriteGo = new GameObject(spriteName);
				spriteGo.transform.parent = newGo.transform;
				if (gameResource)
				{
					int count = newGo.transform.childCount;
					for (int i = count - 1; i >= 0; i--)
					{
						Transform child = newGo.transform.GetChild(i);
						if (child.GetComponent<SkinnedMeshRenderer>() != null || child.GetComponent<MeshRenderer>() != null)
						{
							DestroyImmediate(child.gameObject);
						}
					}
					SkinnedMeshRenderer smr = newGo.AddComponent<SkinnedMeshRenderer>();
					smr.receiveShadows = false;
					smr.lightProbeUsage = LightProbeUsage.BlendProbes;
					smr.shadowCastingMode = ShadowCastingMode.Off;
					smr.updateWhenOffscreen = false;
					smr.rootBone = newGo.transform;
					smr.localBounds = new Bounds(new Vector3(0, 0.5f, 0), new Vector3(0.5f, 1.0f, 0.5f));
				}
			}
		}
	}
}
}