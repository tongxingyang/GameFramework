using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UI;

namespace I2.TextAnimation
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(TextAnimation),true)]
	public partial class TextAnimation_Inspector : Editor 
	{
		TextAnimation mTarget;


		//Vector2 mRectPivot;

		bool mMakeMaterialDirty, mMakeVerticesDirty;

		public void OnEnable()
		{
			mTarget = target as TextAnimation;

			RegisterProperty_Animation();

			//EditorApplication.update += UpdateAnims;
		}

		public void OnDisable()
		{
			// EditorApplication.update -= UpdateAnims;
			//DestroyImmediate( mBevelTexture );
			if (!Application.isPlaying && mTarget!=null)
			{
				if (mEditor_SelectedAnim != null)
					mEditor_SelectedAnim.OnDestroy();
				mTarget.StopAllAnimations(false);
				foreach (var se in targets)
					(se as TextAnimation).MarkWidgetAsChanged(mMakeVerticesDirty, mMakeMaterialDirty);
			}
		}

 

		/*public void UpdateAnims()
		{
			mTarget.Update();
		}*/

		public override  void OnInspectorGUI()
		{
			mMakeMaterialDirty = mMakeVerticesDirty = false;

			#if UNITY_5_6_OR_NEWER
				serializedObject.UpdateIfRequiredOrScript();
			#else
				serializedObject.UpdateIfDirtyOrScript();
			#endif
			//mRectPivot = mTarget.mRectPivot;

			EditorGUIUtility.labelWidth = 50;

			GUILayout.BeginHorizontal();
			GUILayout.Space(-15);


			GUI.backgroundColor = Color.Lerp (Color.black, Color.gray, 1);
			GUILayout.BeginVertical(I2_InspectorTools.GUIStyle_Background, GUILayout.Height(1));
			GUI.backgroundColor = Color.white;

			OnGUI_Animations();

			GUITools.OnGUI_Footer("I2 TextAnimation", I2_InspectorTools.GetVersion(), I2_InspectorTools.HelpURL_forum, I2_InspectorTools.HelpURL_Documentation, I2_InspectorTools.HelpURL_AssetStore);

			EditorGUIUtility.labelWidth = 0;

			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			if (serializedObject.ApplyModifiedProperties() || mMakeMaterialDirty || mMakeVerticesDirty) 
			{
				SceneView.RepaintAll();
			}
		}

		public static string GetI2TextAnimationPath()
		{
			string[] assets = AssetDatabase.FindAssets("TextAnimationsManager");
			if (assets.Length==0)
				return string.Empty;

			string PluginPath = AssetDatabase.GUIDToAssetPath(assets[0]);
			PluginPath = PluginPath.Substring(0, PluginPath.Length - "/Scripts/TextAnimationsManager.cs".Length);

			return PluginPath;
		}
	}
}