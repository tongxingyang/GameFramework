using UnityEngine;
using System.Collections;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace I2.TextAnimation
{
    [Serializable]
    public class SE_AnimSequence_Position : SE_AnimSequence
    {
        public Vector3  _From, _To = new Vector3(0, 1, 0);
        public Vector3  _FromRandom,   
                        _ToRandom;

        public enum ePositionAnimBlendMode { Offset, Replace };
        public ePositionAnimBlendMode _AnimBlend_From = ePositionAnimBlendMode.Offset;
        public ePositionAnimBlendMode _AnimBlend_To = ePositionAnimBlendMode.Offset;

        public bool _ApplyX = true;
        public bool _ApplyY = true;
        public bool _ApplyZ = true;

        public bool _UseAxisEasingCurves = false;
        public AnimationCurve _EasingCurveY = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        public AnimationCurve _EasingCurveZ = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

        public override string GetTypeName() { return "Position"; }

        public override void UpdateSequence(float dt, TextAnimation se, SE_Animation anim, int sequenceIndex, ref bool makeMaterialDirty, ref bool makeVerticesDirty)
        {
            base.UpdateSequence(dt, se, anim, sequenceIndex, ref makeMaterialDirty, ref makeVerticesDirty);
            makeVerticesDirty = true;
        }


        public bool HasRandom(Vector3 maxRandom) { return maxRandom.sqrMagnitude > 0.001f; }

        public Vector3 GetRandom(Vector3 maxRandom, int iElement)
        {
            MathUtils.tempV3.x = maxRandom.x * (DRandom.GetUnit(iElement));
            MathUtils.tempV3.y = maxRandom.y * (DRandom.GetUnit(iElement+13*iElement));
            MathUtils.tempV3.z = maxRandom.z * (DRandom.GetUnit(iElement+29*iElement));
            return MathUtils.tempV3;
        }

        public override void Apply_Characters(TextAnimation se, SE_Animation anim, int sequenceIndex)
        {
            if (anim.mTime < mDelay && !_InitAllElements)
                return;

            bool applyRandomFrom = HasRandom(_FromRandom);
            bool applyRandomTo   = HasRandom(_ToRandom);
            DRandom.mCurrentSeed = GetRandomSeed(anim, sequenceIndex);


            Vector3 from       = _From * se.mCharacterSize;
            Vector3 to         = _To * se.mCharacterSize;
            Vector3 newValue   = MathUtils.v3zero;

            if (_AnimBlend_From == ePositionAnimBlendMode.Replace)
            {
                from.x = MathUtils.LerpUnclamped(se.mAllCharactersMin.x, se.mAllCharactersMax.x, _From.x);
                from.y = MathUtils.LerpUnclamped(se.mAllCharactersMin.y, se.mAllCharactersMax.y, _From.y);
            }

            if (_AnimBlend_To == ePositionAnimBlendMode.Replace)
            {
                to.x = MathUtils.LerpUnclamped(se.mAllCharactersMin.x, se.mAllCharactersMax.x, _To.x);
                to.y = MathUtils.LerpUnclamped(se.mAllCharactersMin.y, se.mAllCharactersMax.y, _To.y);
            }




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


                var currentValue = TextAnimation.mOriginalVertices.Buffer[iElement*4].position;

                var vFrom = (_AnimBlend_From == ePositionAnimBlendMode.Offset) ? (currentValue + from) : from;
                var vTo = (_AnimBlend_To == ePositionAnimBlendMode.Offset) ? (currentValue + to) : to;

                if (applyRandomFrom)    vFrom += GetRandom(_FromRandom * se.mCharacterSize, iElement);
                if (applyRandomTo)      vTo   += GetRandom(_ToRandom * se.mCharacterSize,   iElement * 2+90);

                if (_ApplyX) newValue.x = vFrom.x + (vTo.x - vFrom.x) * tx;
                if (_ApplyY) newValue.y = vFrom.y + (vTo.y - vFrom.y) * ty;
                if (_ApplyZ) newValue.z = vFrom.z + (vTo.z - vFrom.z) * tz;

                // Apply to all Vertices
                for (int v = iElement * 4; v < iElement * 4 + 4; ++v)
                {
                    // Apply Lerp(vFrom, vTo, t) + offset_from_currentValue   (the offset is applied to avoid changing the character's size)
                    if (_ApplyX) TextAnimation.mOriginalVertices.Buffer[v].position.x = newValue.x    + (TextAnimation.mOriginalVertices.Buffer[v].position.x- currentValue.x);
                    if (_ApplyY) TextAnimation.mOriginalVertices.Buffer[v].position.y = newValue.y    + (TextAnimation.mOriginalVertices.Buffer[v].position.y - currentValue.y);
                    if (_ApplyZ) TextAnimation.mOriginalVertices.Buffer[v].position.z = newValue.z    + (TextAnimation.mOriginalVertices.Buffer[v].position.z - currentValue.z);
                }
            }
        }


#if UNITY_EDITOR
        public override void InspectorGUI()
        {
            var color       = GUI.color;
            var transpColor = new Color(color.r, color.g, color.b, color.a * 0.5f);

            GUILayout.BeginHorizontal();
                GUILayout.Space(110);
                _ApplyX = GUILayout.Toggle(_ApplyX, "", GUILayout.ExpandWidth(true));
                _ApplyY = GUILayout.Toggle(_ApplyY, "", GUILayout.ExpandWidth(true));
                _ApplyZ = GUILayout.Toggle(_ApplyZ, "", GUILayout.ExpandWidth(true));
                GUILayout.Space(40);
            GUILayout.EndHorizontal();


            //--[ FROM ]------------------------------------------

            EditorGUIUtility.labelWidth = 70;
            GUILayout.BeginHorizontal();
                _From       = EditorGUILayout.Vector3Field("From", _From);
                _AnimBlend_From = (ePositionAnimBlendMode)EditorGUILayout.EnumPopup(_AnimBlend_From, GUILayout.Width(70));
            GUILayout.EndHorizontal();

            GUI.color = HasRandom(_FromRandom) ? color : transpColor;
            GUILayout.BeginHorizontal();
                _FromRandom = EditorGUILayout.Vector3Field("Random", _FromRandom);
                if (GUILayout.Button("X", EditorStyles.miniButton, GUILayout.ExpandWidth(false)))  {  _FromRandom = Vector3.zero;  GUIUtility.keyboardControl = -1;  }
                GUILayout.Space(51);
            GUILayout.EndHorizontal();
            GUI.color = color;



            GUILayout.Space(15);



            //--[ TO ]------------------------------------------

            GUILayout.BeginHorizontal();
                  _To         = EditorGUILayout.Vector3Field("To", _To);
                _AnimBlend_To = (ePositionAnimBlendMode)EditorGUILayout.EnumPopup(_AnimBlend_To, GUILayout.Width(70));
            GUILayout.EndHorizontal();

            GUI.color = HasRandom(_ToRandom) ? color : transpColor;
            GUILayout.BeginHorizontal();
                _ToRandom = EditorGUILayout.Vector3Field("Random", _ToRandom);
                if (GUILayout.Button("X", EditorStyles.miniButton, GUILayout.ExpandWidth(false)))  {  _ToRandom = Vector3.zero;  GUIUtility.keyboardControl = -1;  }
                GUILayout.Space(51);
            GUILayout.EndHorizontal();
            GUI.color = color;


            //--[ EASING ]------------------------------------------

            GUILayout.BeginHorizontal();
                GUILayout.Label("Easing", GUILayout.Width(78));
                _EasingCurve = EditorGUILayout.CurveField(_EasingCurve);
                if (_UseAxisEasingCurves)
                {
                    _EasingCurveY = EditorGUILayout.CurveField(_EasingCurveY);
                    _EasingCurveZ = EditorGUILayout.CurveField(_EasingCurveZ);
                }
                bool newUseAC = GUILayout.Toggle(_UseAxisEasingCurves, "Per Axis", GUILayout.Width(70));
                if (newUseAC!=_UseAxisEasingCurves && newUseAC)
                {
                    _EasingCurveY.keys = (Keyframe[])_EasingCurve.keys;
                    _EasingCurveZ.keys = (Keyframe[])_EasingCurve.keys;
                }
                _UseAxisEasingCurves = newUseAC;
            GUILayout.EndHorizontal();
        }
#endif
    
    }
}
