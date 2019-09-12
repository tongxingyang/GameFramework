using UnityEngine;
using System.Collections;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace I2.TextAnimation
{
    [Serializable]
    public class SE_AnimSequence_Float : SE_AnimSequence
    {
        public float _From, _FromRandom;
        public float _To=1,   _ToRandom;
        public enum eFloatAnimBlendMode { Offset, Replace, Blend };
        public eFloatAnimBlendMode _AnimBlend_From = eFloatAnimBlendMode.Replace;
        public eFloatAnimBlendMode _AnimBlend_To = eFloatAnimBlendMode.Replace;

        public override void UpdateSequence(float dt, TextAnimation se, SE_Animation anim, int sequenceIndex, ref bool makeMaterialDirty, ref bool makeVerticesDirty) 
        { 
            base.UpdateSequence(dt, se, anim, sequenceIndex, ref makeMaterialDirty, ref makeVerticesDirty);
            makeVerticesDirty = true;
        }

        public override void Apply_Characters(TextAnimation se, SE_Animation anim, int sequenceIndex)
        {
            if (anim.mTime < mDelay && !_InitAllElements)
                return;

            bool applyRandomFrom = HasRandom(_FromRandom);
            bool applyRandomTo = HasRandom(_ToRandom);
            DRandom.mCurrentSeed = GetRandomSeed(anim, sequenceIndex);


            // Iterate through all the valid range
            for (int iElement = mElementRangeStart; iElement < mElementRangeEnd; ++iElement)
            {
                float progress = GetProgress(anim.mTime, anim, iElement);
                if (!_InitAllElements && progress < 0)
                    continue;
                progress = progress < 0 ? 0 : progress;

                progress = _EasingCurve.Evaluate(progress);
                var currentAlpha = TextAnimation.mOriginalVertices.Buffer[iElement * 4].color.a;

                //--[ From ]----------------------------
                float aFrom = _From*255;
                if (_AnimBlend_From == eFloatAnimBlendMode.Offset) aFrom = aFrom + currentAlpha;
                if (_AnimBlend_From == eFloatAnimBlendMode.Blend) aFrom = _From * currentAlpha;

                if (applyRandomFrom)
                    aFrom += 255*_FromRandom * DRandom.GetUnit(iElement);

                //--[ To ]----------------------------
                float aTo = 255*_To;
                if (_AnimBlend_To == eFloatAnimBlendMode.Offset) aTo = (currentAlpha + _To);
                if (_AnimBlend_To == eFloatAnimBlendMode.Blend) aTo = (currentAlpha * _To);


                if (applyRandomTo)
                    aTo += 255*_ToRandom * DRandom.GetUnit(iElement*2+90);


                // Find Alpha for this Character
                float falpha = (aFrom + (aTo - aFrom) * progress);
                byte alpha = (byte)(falpha<0?0 : falpha>255?255: falpha);


                // Apply to all Vertices
                for (int v=iElement*4; v<iElement*4+4; ++v)
                    TextAnimation.mOriginalVertices.Buffer[v].color.a = alpha;
            }
        }

        public bool HasRandom(float maxRandom) { return maxRandom < -0.001f || maxRandom > 0.001f; }


#if UNITY_EDITOR
        public override void InspectorGUI()
        {
            var color = GUI.color;
            GUILayout.BeginHorizontal();
                _From = EditorGUILayout.Slider("From", _From, 0, 1);
                _AnimBlend_From = (eFloatAnimBlendMode)EditorGUILayout.EnumPopup(_AnimBlend_From, GUILayout.Width(100));
            GUILayout.EndHorizontal();

            GUI.color = HasRandom(_FromRandom) ? color : new Color(color.r, color.g, color.b, color.a * 0.5f);
            GUILayout.BeginHorizontal();
                _FromRandom = EditorGUILayout.Slider("Random", _FromRandom, 0, 1);
                if (GUILayout.Button("X", EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
                {
                    _FromRandom = 0;
                    GUIUtility.keyboardControl = -1;
                }
                GUILayout.Space(81);
            GUILayout.EndHorizontal();
            GUI.color = color;



            GUILayout.Space(15);


            GUILayout.BeginHorizontal();
                _To = EditorGUILayout.Slider("To", _To, 0, 1);
                _AnimBlend_To = (eFloatAnimBlendMode)EditorGUILayout.EnumPopup(_AnimBlend_To, GUILayout.Width(100));
            GUILayout.EndHorizontal();

            GUI.color = HasRandom(_ToRandom) ? color : new Color(color.r, color.g, color.b, color.a * 0.5f);
            GUILayout.BeginHorizontal();
                _ToRandom = EditorGUILayout.Slider("Random", _ToRandom, 0, 1);
                if (GUILayout.Button("X", EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
                {
                    _ToRandom = 0;
                    GUIUtility.keyboardControl = -1;
                }
            GUILayout.Space(81);
            GUILayout.EndHorizontal();
            GUI.color = color;

            GUILayout.Space(15);

            GUILayout.BeginHorizontal();
                _EasingCurve = EditorGUILayout.CurveField("Easing", _EasingCurve);
            GUILayout.EndHorizontal();
        }
#endif
    }
}
