using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

namespace I2.TextAnimation
{
	public partial class SE_Animation_Inspector
    {
        void OnGUI_Sequence()
        {
            mAnimation._Sequences[mCurrentSequenceIndex].InspectorGUI();
            GUILayout.Space(5);
            OnGUI_Sequence_Playback();
            OnGUI_Sequence_Target();
        }

        void OnGUI_Sequence_Playback()
        {
            var changed = GUI.changed;
            var seq = mAnimation._Sequences[mCurrentSequenceIndex];
            var key = "TextAnimation AnimSeq Playback";
            bool state = EditorPrefs.GetBool(key, false);
            GUILayout.BeginHorizontal();
                bool newState = GUILayout.Toggle(state, "Playback", EditorStyles.foldout);
            GUI.changed = changed; // ignore changed if was on this toggle

                if (mCurrentSequenceIndex > 0)
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(new GUIContent("Copy Prev", "Copy values from previous sequence"), EditorStyles.miniButton, GUITools.DontExpandWidth))
                    {
                        var prevSeq = mAnimation._Sequences[mCurrentSequenceIndex - 1];
                        seq._StartType          = prevSeq._StartType;
                        seq._StartDelay         = prevSeq._StartDelay;
                        seq._Separation         = prevSeq._Separation;
                        seq._Playback           = prevSeq._Playback;
                        seq._PlaybackTimes      = prevSeq._PlaybackTimes;
                        seq._Backwards          = prevSeq._Backwards;
                        seq._Duration           = prevSeq._Duration;
                        seq._DurationType       = prevSeq._DurationType;
                        seq._EasingCurve.keys   = (Keyframe[])prevSeq._EasingCurve.keys;

                        GUI.changed = true;
                    }
                }
            GUILayout.EndHorizontal();



            if (state != newState) EditorPrefs.SetBool(key, newState);

            if (!state)
                return;

            GUILayout.BeginHorizontal();
            GUILayout.Space(12);
            GUILayout.BeginVertical("Box");
            GUILayout.Space(3);

            EditorGUIUtility.labelWidth = 100;
            GUILayout.BeginHorizontal();
            GUILayout.Label("When", GUITools.DontExpandWidth);
            seq._StartType = (SE_AnimSequence.eStartType) EditorGUILayout.EnumPopup(seq._StartType);

            GUILayout.Label("Delay", I2_InspectorTools.Style_LabelRightAligned);
            seq._StartDelay = EditorGUILayout.FloatField(seq._StartDelay);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
                seq._Separation = EditorGUILayout.Slider("Separation", seq._Separation, 0, 4);
            GUILayout.Space(-5);
                if (GUILayout.Button("X", EditorStyles.miniButtonRight, GUITools.DontExpandWidth))
                    seq._Separation = 1;
            GUILayout.EndHorizontal();

            seq._RandomStart = EditorGUILayout.Slider("Random", seq._RandomStart, 0, 1);


            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Playback", GUILayout.Width(100));
            seq._Playback = (SE_Animation.ePlayback) EditorGUILayout.EnumPopup(seq._Playback);
            if (seq._Playback != SE_Animation.ePlayback.Single)
            {
                GUI.contentColor = seq._PlaybackTimes > 0 ? GUITools.White : GUITools.LightGray;
                seq._PlaybackTimes = EditorGUILayout.IntField(seq._PlaybackTimes, GUILayout.Width(50));
                GUILayout.Label("times", GUITools.DontExpandWidth);
                GUI.contentColor = GUITools.White;
            }
            GUILayout.EndHorizontal();

            seq._Backwards = GUILayout.Toggle(seq._Backwards, new GUIContent("Backwards", "Plays the animation from Right to Left"));

            GUILayout.BeginHorizontal();
                GUILayout.Toggle(true, "Duration", EditorStyles.foldout, GUILayout.Width(100));
                //GUILayout.Label("Duration", GUILayout.Width(100));
                seq._Duration = Mathf.Max(0.0001f, EditorGUILayout.FloatField(seq._Duration));
                seq._DurationType = (SE_AnimSequence.eDurationType) EditorGUILayout.EnumPopup(seq._DurationType, GUILayout.Width(100));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.BeginVertical();
                    seq._DurationRandom_Slower = EditorGUILayout.Slider("Random Slower", seq._DurationRandom_Slower, 0, 1);
                    seq._DurationRandom_Faster = EditorGUILayout.Slider("Random Faster", seq._DurationRandom_Faster, 0, 1);
                GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            seq._InitAllElements = GUILayout.Toggle(seq._InitAllElements, new GUIContent("Set FROM value On Start", "On startup, set all elements to the value of FROM. If false, the elements will only be set only after start animating that element"));



            GUILayout.Space(3);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

        }

        void OnGUI_Sequence_Target()
        {
            var changed = GUI.changed;

            var seq = mAnimation._Sequences[mCurrentSequenceIndex];
            var key = "TextAnimation AnimSeq Target";
            bool state = EditorPrefs.GetBool(key, false);
            GUILayout.BeginHorizontal();
                bool newState = GUILayout.Toggle(state, "Target", EditorStyles.foldout);
            GUI.changed = changed; // ignore changed if was on this toggle

            if (mCurrentSequenceIndex > 0)
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(new GUIContent("Copy Prev", "Copy values from previous sequence"), EditorStyles.miniButton, GUITools.DontExpandWidth))
                    {
                        var prevSeq = mAnimation._Sequences[mCurrentSequenceIndex - 1];
                        seq._TargetType = prevSeq._TargetType;
                        seq._TargetRangeType = prevSeq._TargetRangeType;
                        seq._TargetRangeStart = prevSeq._TargetRangeStart;
                        seq._TargetRangeAmount = prevSeq._TargetRangeAmount;
                        seq._TargetRangeCustom = prevSeq._TargetRangeCustom==null ? null : (int[])prevSeq._TargetRangeCustom.Clone();
                        GUI.changed = true;
                    }
                }
            GUILayout.EndHorizontal();
            if (state != newState) EditorPrefs.SetBool(key, newState);

            if (!state)
                return;

            GUILayout.BeginHorizontal();
            GUILayout.Space(12);
            GUILayout.BeginVertical("Box");
            GUILayout.Space(3);


            EditorGUIUtility.labelWidth = 60;
            GUI.enabled = false;
            seq._TargetType = (SE_AnimSequence.eElements) EditorGUILayout.EnumPopup("Animate", seq._TargetType, GUILayout.Width(150));
            GUI.enabled = true;
            GUILayout.Space(5);

            string targetType = seq._TargetType.ToString().ToLowerInvariant();
            var disabledColor = GUITools.White;// new Color(1, 1, 1, 0.5f);

            EditorGUI.BeginChangeCheck();
            GUILayout.BeginHorizontal();
                GUI.color = (int)seq._TargetRangeType == 0 ? GUITools.White : disabledColor;
                GUILayout.Toggle((int)seq._TargetRangeType == 0, "", EditorStyles.radioButton, GUITools.DontExpandWidth);
                GUILayout.Label("All " + targetType);
            GUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck()) seq._TargetRangeType = (SE_AnimSequence.eTargetRange)0;


            EditorGUI.BeginChangeCheck();
            GUILayout.BeginHorizontal();
                GUI.color = (int)seq._TargetRangeType == 1 ? GUITools.White : disabledColor;
                GUILayout.Toggle((int)seq._TargetRangeType == 1, "", EditorStyles.radioButton, GUITools.DontExpandWidth);
                GUILayout.Label("First", GUILayout.Width(50));
                seq._TargetRangeAmount = EditorGUILayout.IntField(seq._TargetRangeAmount);
                GUILayout.Label(targetType, GUITools.DontExpandWidth);
                GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck()) seq._TargetRangeType = (SE_AnimSequence.eTargetRange)1;


            EditorGUI.BeginChangeCheck();
            GUILayout.BeginHorizontal();
                GUI.color = (int)seq._TargetRangeType == 2 ? GUITools.White : disabledColor;
                GUILayout.Toggle((int)seq._TargetRangeType == 2, "", EditorStyles.radioButton, GUITools.DontExpandWidth);
                GUILayout.Label("Last", GUILayout.Width(50));
                seq._TargetRangeAmount = EditorGUILayout.IntField(seq._TargetRangeAmount);
                GUILayout.Label(targetType, GUITools.DontExpandWidth);
                GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck()) seq._TargetRangeType = (SE_AnimSequence.eTargetRange)2;

            EditorGUI.BeginChangeCheck();
            GUILayout.BeginHorizontal();
                GUI.color = (int)seq._TargetRangeType == 3 ? GUITools.White : disabledColor;
                GUILayout.Toggle((int)seq._TargetRangeType == 3, "", EditorStyles.radioButton, GUITools.DontExpandWidth);
                GUILayout.Label("Skip", GUILayout.Width(50));
                seq._TargetRangeStart = EditorGUILayout.IntField(seq._TargetRangeStart);
                GUILayout.Label("Take", GUITools.DontExpandWidth);
                seq._TargetRangeAmount = EditorGUILayout.IntField(seq._TargetRangeAmount);
                GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck()) seq._TargetRangeType = (SE_AnimSequence.eTargetRange)3;

            //EditorGUI.BeginChangeCheck();
            GUILayout.BeginHorizontal();
                GUI.color = (int)seq._TargetRangeType == 4 ? GUITools.White : disabledColor;
                GUILayout.Toggle((int)seq._TargetRangeType == 4, "", EditorStyles.radioButton, GUITools.DontExpandWidth);
                GUILayout.Label("Custom", GUITools.DontExpandWidth);
                GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            //if (EditorGUI.EndChangeCheck()) seq._TargetRangeType = (SE_AnimSequence.eTargetRange)4;
            GUI.color = GUITools.White;


            GUILayout.Space(3);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
        }
    }
}