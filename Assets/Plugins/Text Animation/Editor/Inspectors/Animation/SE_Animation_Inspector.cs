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
        public TextAnimation mTextAnimation;
        public SE_Animation mAnimation;


        int mCurrentSequenceIndex = -1;

        public SE_Animation_Inspector(SE_Animation anim, TextAnimation se)
        {
            mTextAnimation = se;
            mAnimation = anim;

            if (mTextAnimation != null && !Application.isPlaying)
                mTextAnimation.StopAllAnimations(false);

            mAnimation.InitTimes(mTextAnimation, true);
            if (!Application.isPlaying)
                mAnimation.Play(mTextAnimation);
            AnimationControls_StopPlaying(true);
         }

        public void OnDestroy()
        {
            if (mTextAnimation != null && !Application.isPlaying)
                mTextAnimation.StopAllAnimations(false);

            AnimationControls_StopPlaying(false);
            SceneView.RepaintAll();
        }

        public void OnGUI_Animation()
        {
            InitGUIStyles();

            EditorGUI.BeginChangeCheck();

            //--[ Show Animation ]------------------------------------------
            GUI.color = GUITools.White;
            GUITools.BeginContents();
            GUILayout.Space(2);

            OnGUI_AnimationContent();

            GUITools.EndContents(false);
            GUI.color = GUITools.White;

            //--[ Show Sequence (if anyone is selected) ]------------------------------------------
            if (mCurrentSequenceIndex >= 0)
            {
                OnGUI_SequenceContent();
            }

            if (EditorGUI.EndChangeCheck())
            {
                mAnimation.InitTimes(mTextAnimation, true);
            }

            if (mSequence_IsDragging || mTimeLine_PlayingStartTime > 0)
                /*Repaint();*/HandleUtility.Repaint();
        }

        private void OnGUI_AnimationContent()
        {
            var origColor = GUI.color;
            var transpColor = new Color(origColor.r, origColor.g, origColor.b, origColor.a * 0.5f);

            EditorGUIUtility.labelWidth = 80;
            mAnimation.Name = EditorGUILayout.TextField("Name", mAnimation.Name);
            GUILayout.Space(3);

            GUILayout.BeginHorizontal();
                GUILayout.Label("Playback", GUILayout.Width(80));

                mAnimation._Playback = (SE_Animation.ePlayback)EditorGUILayout.EnumPopup(mAnimation._Playback, GUILayout.Width(100));
                if (mAnimation._Playback != SE_Animation.ePlayback.Single)
                {
                    bool infinite = mAnimation._PlaybackTimes <= 0;
                    GUI.color = infinite ? transpColor : origColor;

                    bool newInfinite = GUILayout.Toggle(infinite, "", GUITools.DontExpandWidth);
                    if (newInfinite!=infinite)
                    {
                        if (newInfinite) mAnimation._PlaybackTimes = -1;
                                    else mAnimation._PlaybackTimes = 1;
                    }
                    if (infinite)
                        GUILayout.Label("infinite");
                    else
                    {
                        mAnimation._PlaybackTimes = EditorGUILayout.IntField(mAnimation._PlaybackTimes, GUILayout.Width(50));
                        GUILayout.Label("times", GUITools.DontExpandWidth);
                    }
                    GUI.color = origColor;
                }
                GUILayout.FlexibleSpace();

                GUILayout.Label("Backwards", GUITools.DontExpandWidth);
                mAnimation._Backwards = EditorGUILayout.Toggle(mAnimation._Backwards, GUILayout.Width(15));//GUITools.DontExpandWidth);

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
                GUILayout.Label("Extra Time:", GUITools.DontExpandWidth);
                GUILayout.FlexibleSpace();
                mAnimation._ExtraTimePerLoop = EditorGUILayout.FloatField(new GUIContent("Every Loop"), mAnimation._ExtraTimePerLoop, GUILayout.Width(150));
                mAnimation._ExtraTimeFinal   = EditorGUILayout.FloatField(new GUIContent("At the End"), mAnimation._ExtraTimeFinal, GUILayout.Width(150));
            GUILayout.EndHorizontal();
            EditorGUIUtility.labelWidth = 50;


            GUILayout.Space(5);
            OnGUI_Sequences();
        }

        void OnGUI_SequenceContent()
        {
            GUILayout.Space(10);
            var seq = mAnimation._Sequences[mCurrentSequenceIndex];
            var changed = GUI.changed;

            GUI.backgroundColor = Color.blue;
            GUILayout.BeginVertical(EditorStyles.textArea);
            GUI.backgroundColor = Color.white;


            var boxArea = new GUIStyle("Box");
            boxArea.overflow = new RectOffset(4, 4, 0, 0);
            GUILayout.BeginHorizontal(boxArea);
                GUILayout.Toggle(true, "Sequence:", EditorStyles.foldout, GUITools.DontExpandWidth);

                seq._Name = EditorGUILayout.TextField(seq._Name, GUILayout.ExpandWidth(true) );
                
                if (GUILayout.Button("X", EditorStyles.miniButton, GUITools.DontExpandWidth))
                    mCurrentSequenceIndex = -1;
            GUILayout.EndHorizontal();

            GUILayout.Space(-4);


            GUI.backgroundColor = GUITools.DarkGray;
            GUILayout.BeginVertical(GUILayout.Height(1));
            GUI.backgroundColor = Color.white;

            GUI.changed = changed; // ignore changed in the header

            GUITools.BeginContents();
                if (mCurrentSequenceIndex>=0)
                    OnGUI_Sequence();
            GUITools.EndContents(true);

            GUILayout.EndVertical();
        }
    }
}