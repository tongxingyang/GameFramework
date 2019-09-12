using UnityEngine;
using System.Collections;
using System;
using System.Xml.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace I2.TextAnimation
{
    [Serializable]
    public class SE_AnimSequence_Scale : SE_AnimSequence_Position
    {
        public Vector3 _Pivot = new Vector3(0.5f, 0.5f, 0.0f);

        public enum ePivotType { Relative_Letter, Relative_Word, Relative_Line, Relative_All, Relative_Rect, Explict };
        public ePivotType _PivotType = ePivotType.Relative_Letter;

        public override string GetTypeName() { return "Scale"; }

        // Copy from SE_AnimSequence_Position, except where noted
        public override void Apply_Characters(TextAnimation se, SE_Animation anim, int sequenceIndex)
        {
            if (anim.mTime < mDelay && !_InitAllElements)
                return;

            bool applyRandomFrom = HasRandom(_FromRandom);
            bool applyRandomTo   = HasRandom(_ToRandom);
            DRandom.mCurrentSeed = GetRandomSeed(anim, sequenceIndex);


            Vector3 from       = _From;// * se.mCharacterSize;                                                              // REMOVED *se.mCharacterSize
            Vector3 to         = _To;// * se.mCharacterSize;                                                                // REMOVED *se.mCharacterSize
            Vector3 newValue   = MathUtils.v3zero;

            // Iterate through all the valid range
            for (int iElement = mElementRangeStart; iElement < mElementRangeEnd; ++iElement)
            {
                float progress = GetProgress(anim.mTime, anim, iElement);
                if (!_InitAllElements && progress < 0)
                    continue;
                progress = progress < 0 ? 0 : progress;

                float tx = _EasingCurve.Evaluate(progress);
                float ty = _UseAxisEasingCurves ? _EasingCurveY.Evaluate(progress) : tx;
                float tz = _UseAxisEasingCurves ? _EasingCurveZ.Evaluate(progress) : tx;


                var currentValue = MathUtils.v3one;                                                                         // MODIFIED

                var vFrom = (_AnimBlend_From == ePositionAnimBlendMode.Replace) ? from : (currentValue + from);
                var vTo = (_AnimBlend_To == ePositionAnimBlendMode.Replace) ? to : (currentValue + to);

                if (applyRandomFrom)    vFrom += GetRandom(_FromRandom /* se.mCharacterSize*/, iElement);                   // REMOVED *se.mCharacterSize
                if (applyRandomTo)      vTo   += GetRandom(_ToRandom /* se.mCharacterSize*/,   iElement * 2+90);               // REMOVED *se.mCharacterSize

                if (_ApplyX) newValue.x = vFrom.x + (vTo.x - vFrom.x) * tx;
                if (_ApplyY) newValue.y = vFrom.y + (vTo.y - vFrom.y) * ty;
                if (_ApplyZ) newValue.z = vFrom.z + (vTo.z - vFrom.z) * tz;


                // NEW CODE-----------------------------------------------------------------------------------------------------------------------------------------
                Vector3 vPivot;
                if (_PivotType == ePivotType.Relative_Letter || _PivotType == ePivotType.Relative_Word || _PivotType == ePivotType.Relative_Line)
                {
                    vPivot.x = MathUtils.LerpUnclamped(TextAnimation.mCharacters.Buffer[iElement].Min.x, TextAnimation.mCharacters.Buffer[iElement].Max.x, _Pivot.x);
                    vPivot.y = MathUtils.LerpUnclamped(TextAnimation.mCharacters.Buffer[iElement].Min.y, TextAnimation.mCharacters.Buffer[iElement].Max.y, _Pivot.y);
                }
                else
                if (_PivotType == ePivotType.Relative_All)
                {
                    vPivot.x = MathUtils.LerpUnclamped(se.mAllCharactersMin.x, se.mAllCharactersMax.x, _Pivot.x);
                    vPivot.y = MathUtils.LerpUnclamped(se.mAllCharactersMin.y, se.mAllCharactersMax.y, _Pivot.y);
                }
                else
                if (_PivotType == ePivotType.Relative_Rect)
                {
                    vPivot.x = MathUtils.LerpUnclamped(se.mWidgetRectMin.x, se.mWidgetRectMax.x, _Pivot.x);
                    vPivot.y = MathUtils.LerpUnclamped(se.mWidgetRectMin.y, se.mWidgetRectMax.y, _Pivot.y);
                }
                else
                    vPivot = _Pivot * se.mCharacterSize;
                // END NEW CODE


                // Apply to all Vertices
                for (int v = iElement * 4; v < iElement * 4 + 4; ++v)
                {
                    if (_ApplyX) TextAnimation.mOriginalVertices.Buffer[v].position.x = (TextAnimation.mOriginalVertices.Buffer[v].position.x - vPivot.x) * newValue.x + vPivot.x;
                    if (_ApplyY) TextAnimation.mOriginalVertices.Buffer[v].position.y = (TextAnimation.mOriginalVertices.Buffer[v].position.y - vPivot.y) * newValue.y + vPivot.y;
                    //if (_ApplyZ) TextAnimation.mOriginalVertices.Buffer[v].position.z = (TextAnimation.mOriginalVertices.Buffer[v].position.z - vPivot.z) * newValue.z + vPivot.z;
                }
            }
        }

#if UNITY_EDITOR
        public override void InspectorGUI()
        {
            GUILayout.BeginHorizontal();
                _Pivot = EditorGUILayout.Vector3Field("Pivot", _Pivot);
                _PivotType = (ePivotType)EditorGUILayout.EnumPopup(_PivotType, GUILayout.Width(100));
            if (_PivotType == ePivotType.Relative_Word || _PivotType == ePivotType.Relative_Line)
                _PivotType = ePivotType.Relative_Letter;
                GUILayout.EndHorizontal();

            GUILayout.Space(15);

            base.InspectorGUI();
        }
#endif
    }
}
