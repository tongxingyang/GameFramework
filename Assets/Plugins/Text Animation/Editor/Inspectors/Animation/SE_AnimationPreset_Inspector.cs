using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

namespace I2.TextAnimation
{
    [CustomEditor(typeof(SE_AnimationPreset))]
	public partial class SE_AnimationPreset_Inspector : Editor
    {
        SE_Animation_Inspector mInspector;

        SerializedProperty mProp_SerializedData;
        public static bool mDirty;



        public void OnEnable()
        {
            mProp_SerializedData = serializedObject.FindProperty("mSerializedData");

            mInspector = new SE_Animation_Inspector((target as SE_AnimationPreset).CreateAnimation(), null);
         }

        public override void OnInspectorGUI()
        {
			#if UNITY_5_6_OR_NEWER
				serializedObject.UpdateIfRequiredOrScript();
			#else
				serializedObject.UpdateIfDirtyOrScript();
			#endif

			EditorGUIUtility.labelWidth = 50;

            GUI.backgroundColor = Color.Lerp(Color.black, Color.gray, 1);
            GUILayout.BeginVertical(I2_InspectorTools.GUIStyle_Background, GUILayout.Height(1));
            GUI.backgroundColor = Color.white;


            //--[ HEADER ]----------------------
                if (GUILayout.Button("SE Animation", I2_InspectorTools.GUIStyle_Header))
                {
                    //Application.OpenURL(SE_InspectorTools.HelpURL_Documentation);
                }

                GUILayout.Space(5);

            //--[ INSPECTOR ]---------------------

                EditorGUI.BeginChangeCheck();

                    mInspector.OnGUI_Animation();

                if (EditorGUI.EndChangeCheck() || mDirty)
                {
                    mDirty = false;
                    var data = SE_Animation.SaveSerializedData(mInspector.mAnimation);
                    if (mProp_SerializedData.stringValue != data)
                    {
                       mProp_SerializedData.stringValue = data;
                        //Debug.Log(data);
                    }
                }

            //--[ FOOTER ]-------------------------
                GUILayout.Space(10);
                GUILayout.FlexibleSpace();

                GUITools.OnGUI_Footer("I2 TextAnimation", I2_InspectorTools.GetVersion(), I2_InspectorTools.HelpURL_forum, I2_InspectorTools.HelpURL_Documentation, I2_InspectorTools.HelpURL_AssetStore);

                EditorGUIUtility.labelWidth = 0;

            GUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }
}