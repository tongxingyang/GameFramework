using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

namespace I2.TextAnimation
{
	public partial class TextAnimation_Inspector
    {
        private SerializedProperty mSerialized_OnEnable_PlayAnim, mSerialized_AnimationSlots, mSerialized_TimeSource, mSerialized_OnAnimation_Finished;

        private ReorderableList mAnimationsReorderableList;

        private SerializedObject mSerializedObject_SelectedAnim;
        private SE_Animation_Inspector mEditor_SelectedAnim;
        private int iCurrentAnimationSlot = -1;



        void RegisterProperty_Animation()
		{
            mSerialized_OnEnable_PlayAnim    = serializedObject.FindProperty("_OnEnable_PlayAnim");
            mSerialized_AnimationSlots       = serializedObject.FindProperty("_AnimationSlots");
            mSerialized_TimeSource           = serializedObject.FindProperty("_TimeSource");
            mSerialized_OnAnimation_Finished = serializedObject.FindProperty("_OnAnimation_Finished");


            mAnimationsReorderableList                     = new ReorderableList(serializedObject, mSerialized_AnimationSlots, true, true, true, true);
            mAnimationsReorderableList.drawHeaderCallback  = (Rect rect) => { EditorGUI.LabelField(rect, "Animations"); };
            mAnimationsReorderableList.drawElementCallback = OnGUI_Animation_AnimListElement;
            mAnimationsReorderableList.onRemoveCallback    = OnRemoveAnimation;
            mAnimationsReorderableList.onAddCallback       = OnAddAnimation;

            var mAnimPath = TextAnimation_Inspector.GetI2TextAnimationPath() + "/Presets/";
            var len = mAnimPath.Length;
            mSavedAnimations = System.IO.Directory.GetFiles(mAnimPath, "*.asset", System.IO.SearchOption.AllDirectories).Select(f => f.Substring(len, f.Length - len - 6).Replace('\\', '/')).ToArray();
        }

        string[] mSavedAnimations = new string[0];

        public void OnGUI_Animations()
        {
            //GUI.color = showAnim ? new Color(1, 1, 1, 0.5f) : GUITools.White;
            GUI.color = GUITools.White;

            GUITools.BeginContents();
                GUILayout.Space(2);

            //--[ Popup: ON ENABLE PLAY ]---------------------

                EditorGUIUtility.labelWidth = 100;
                string[] mCurrentAnimations = new string[] { "None", "" }.Union(mTarget._AnimationSlots.Select(x => x == null ? "<empty>" : x.GetName())).ToArray();
                int index = (mSerialized_OnEnable_PlayAnim.intValue < 0) ? 0 : (mSerialized_OnEnable_PlayAnim.intValue + 2);
                int newIndex = EditorGUILayout.Popup("On Enable Play:", index, mCurrentAnimations);
                if (index!=newIndex)
                {
                    mSerialized_OnEnable_PlayAnim.intValue = (newIndex < 2 ? -1 : newIndex - 2);
                }

            GUILayout.Space(5);
            EditorGUILayout.PropertyField(mSerialized_TimeSource);

            EditorGUILayout.PropertyField(mSerialized_OnAnimation_Finished);

            mAnimationsReorderableList.DoLayoutList();

            GUITools.EndContents(false);
            GUI.color = GUITools.White;

            if (mAnimationsReorderableList.index>=0)
                OnGUI_AnimationSlot();
        }


        void OnGUI_Animation_AnimListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect.yMin += 1; rect.yMax -= 3;
            //var rectLocalBtn   = Rect.MinMaxRect(rect.xMax - 55, rect.yMin, rect.xMax-15, rect.yMax);
            //var rectCloseBtn = Rect.MinMaxRect(rect.xMax - 15, rect.yMin, rect.xMax, rect.yMax);
            var rectLocalBtn   = Rect.MinMaxRect(rect.xMax - 40, rect.yMin, rect.xMax, rect.yMax);

            rect.xMax = rect.xMax - 55;

            var prop_AnimSlot   = mSerialized_AnimationSlots.GetArrayElementAtIndex(index);
            var prop_SlotPreset = prop_AnimSlot.FindPropertyRelative("_Preset");
            var animPreset      = prop_SlotPreset.objectReferenceValue as SE_AnimationPreset;


            var isLocal = animPreset == null;

            var rectPreset = Rect.MinMaxRect(isLocal ? (rect.xMin+rect.width*0.6f) : rect.xMin, rect.yMin, rect.xMax - 15, rect.yMax);

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(rectPreset, prop_SlotPreset, GUITools.EmptyContent);
            if (EditorGUI.EndChangeCheck())
            {
                DeselectAnimation();
                mAnimationsReorderableList.index = index;
                prop_SlotPreset.serializedObject.ApplyModifiedProperties();
                mTarget._AnimationSlots[index].CreateAnimation();
            }

            var rectPresetList = Rect.MinMaxRect(rect.xMax - 15, rect.yMin, rect.xMax, rect.yMax);
            int newAnimIdx = EditorGUI.Popup(rectPresetList, -1, mSavedAnimations);
            if (newAnimIdx != -1)
            {
                var path = TextAnimation_Inspector.GetI2TextAnimationPath() + "/Presets/" + mSavedAnimations[newAnimIdx] + ".asset";
                animPreset = AssetDatabase.LoadAssetAtPath(path, typeof(SE_AnimationPreset)) as SE_AnimationPreset;
                DeselectAnimation();

                if (animPreset != null)
                {
                    prop_SlotPreset.objectReferenceValue = animPreset;
                    mAnimationsReorderableList.index = index;
                }
                prop_SlotPreset.serializedObject.ApplyModifiedProperties();
                mTarget._AnimationSlots[index].CreateAnimation();
            }


            if (isLocal)
            {
                var rectName = Rect.MinMaxRect(rect.xMin, rect.yMin, rect.xMin + rect.width * 0.6f, rect.yMax);
                GUI.Label(rectName, mTarget._AnimationSlots[index]._Animation.Name, EditorStyles.label);

                if (GUI.Button(rectLocalBtn, "Save", EditorStyles.miniButton))
                    SaveAnimation(index);
            }
            else
            {
                if (GUI.Button(rectLocalBtn, prop_SlotPreset.objectReferenceValue != null ? "Clone" : "New", EditorStyles.miniButton))
                {
                    CloneAnimation(index);
                }
            }
         }

        private void DeselectAnimation()
        {
            DestroyAnimationInspector();

            iCurrentAnimationSlot = -1;
            mAnimationsReorderableList.index = -1;
        }

        public void OnGUI_AnimationSlot()
        {
            var prop_Slot          = mSerialized_AnimationSlots.GetArrayElementAtIndex(mAnimationsReorderableList.index);
            var prop_SlotPreset    = prop_Slot.FindPropertyRelative("_Preset");
            var prop_SlotLocalData = prop_Slot.FindPropertyRelative("_LocalSerializedData");

            if (mEditor_SelectedAnim == null || iCurrentAnimationSlot!=mAnimationsReorderableList.index)
            {
                iCurrentAnimationSlot = mAnimationsReorderableList.index;

                if (prop_SlotPreset.objectReferenceValue != null)
                    mSerializedObject_SelectedAnim = new SerializedObject(prop_SlotPreset.objectReferenceValue);
                else
                    mSerializedObject_SelectedAnim = null;

                DestroyAnimationInspector();

                mEditor_SelectedAnim = new SE_Animation_Inspector(mTarget._AnimationSlots[iCurrentAnimationSlot]._Animation/*CreateAnimation()*/, mTarget);
            }

			if (prop_SlotPreset.objectReferenceValue != null && mSerializedObject_SelectedAnim != null)
			{
				#if UNITY_5_6_OR_NEWER
					mSerializedObject_SelectedAnim.UpdateIfRequiredOrScript();
				#else
					mSerializedObject_SelectedAnim.UpdateIfDirtyOrScript();
				#endif
			}

			GUILayout.Space(10);

   
            //GUITools.DrawHeader("Animation", true);
            var boxArea = new GUIStyle("Box");
            boxArea.overflow = new RectOffset(2,2,0,0);
            bool deselectAnimation = false;
            GUILayout.BeginHorizontal(boxArea);
                GUILayout.Toggle(true, "Animation", EditorStyles.foldout);
                GUILayout.FlexibleSpace();
                deselectAnimation = GUILayout.Button("X", EditorStyles.miniButton);
            GUILayout.EndHorizontal();

            GUILayout.Space(-4);


            GUI.backgroundColor = GUITools.DarkGray;
            GUILayout.BeginVertical(/*EditorStyles.textArea, */GUILayout.Height(1));
            GUI.backgroundColor = Color.white;

            EditorGUI.BeginChangeCheck();
                mEditor_SelectedAnim.OnGUI_Animation();

            if (EditorGUI.EndChangeCheck() || SE_AnimationPreset_Inspector.mDirty)
            {
                SE_AnimationPreset_Inspector.mDirty = false;

                var data = SE_Animation.SaveSerializedData(mEditor_SelectedAnim.mAnimation);
                if (prop_SlotLocalData.stringValue != data)
                {
                    prop_SlotLocalData.stringValue = data;
                    //Debug.Log(data);
                }

                if (prop_SlotPreset.objectReferenceValue != null)
                {
                    var prop_PresetData = mSerializedObject_SelectedAnim.FindProperty("mSerializedData");
                    if (prop_PresetData.stringValue != data)
                    {
                        prop_PresetData.stringValue = data;
                        mSerializedObject_SelectedAnim.ApplyModifiedProperties();
                        //mTarget._AnimationSlots[iCurrentAnimationSlot].CreateAnimation();
                    }
                }
            }
            if (mEditor_SelectedAnim.mAnimation.IsPlaying)
            {
                mMakeMaterialDirty = mMakeVerticesDirty = true;
            }


            GUITools.CloseHeader();

            if (deselectAnimation)
                DeselectAnimation();
        }

        void SaveAnimation(int index, string FileName = null)
        {

            if (string.IsNullOrEmpty(FileName))
            {
                var mAnimPath = GetI2TextAnimationPath() + "/Presets/";
                var anim = mTarget._AnimationSlots[index]._Animation;
                var animName = anim==null || string.IsNullOrEmpty(anim.Name) ? "SE Animation" : anim.Name;

                FileName = EditorUtility.SaveFilePanelInProject("Save As", animName + ".asset", "asset", "Save Animation", mAnimPath);
                if (string.IsNullOrEmpty(FileName))
                    return;
            }

            var animPreset = ScriptableObject.CreateInstance<SE_AnimationPreset>();
            animPreset.mSerializedData = mTarget._AnimationSlots[iCurrentAnimationSlot]._LocalSerializedData;

            AssetDatabase.CreateAsset(animPreset, FileName);
            AssetDatabase.SaveAssets();

            var prop_Slot = mSerialized_AnimationSlots.GetArrayElementAtIndex(index);
            var prop_SlotPreset = prop_Slot.FindPropertyRelative("_Preset");
            prop_SlotPreset.objectReferenceValue = animPreset;
            mSerializedObject_SelectedAnim = new SerializedObject(animPreset);
            mSerialized_AnimationSlots.serializedObject.ApplyModifiedProperties();
            mTarget._AnimationSlots[index].CreateAnimation();
        }

        void CloneAnimation( int index )
        {
            var prop_Slot = mSerialized_AnimationSlots.GetArrayElementAtIndex(index);
            var prop_SlotPreset = prop_Slot.FindPropertyRelative("_Preset");
            if (prop_SlotPreset.objectReferenceValue == null)
                return;

            prop_Slot.FindPropertyRelative("_LocalSerializedData").stringValue = (prop_SlotPreset.objectReferenceValue as SE_AnimationPreset).mSerializedData;

            prop_SlotPreset.objectReferenceValue = null;
            prop_SlotPreset = null;


            mSerialized_AnimationSlots.serializedObject.ApplyModifiedProperties();
            DestroyAnimationInspector();
            mTarget._AnimationSlots[index].CreateAnimation();
        }

        void OnAddAnimation(ReorderableList list)
        {
            list.serializedProperty.InsertArrayElementAtIndex(Mathf.Max(0, list.index));
            //list.serializedProperty.GetArrayElementAtIndex(list.index).objectReferenceValue = null;
            if (list.index >= list.serializedProperty.arraySize)
                list.index = list.serializedProperty.arraySize - 1;
            list.serializedProperty.serializedObject.ApplyModifiedProperties();
            DestroyAnimationInspector();

        }

        void OnRemoveAnimation( ReorderableList list )
        {
            list.serializedProperty.DeleteArrayElementAtIndex(list.index);
            if (list.index >= list.serializedProperty.arraySize)
                list.index = list.serializedProperty.arraySize - 1;
            list.serializedProperty.serializedObject.ApplyModifiedProperties();

            DestroyAnimationInspector();
        }

        void DestroyAnimationInspector()
        {
            if (mEditor_SelectedAnim != null)
                mEditor_SelectedAnim.OnDestroy();
            mEditor_SelectedAnim = null;
        }
    }
}