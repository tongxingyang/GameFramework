using UnityEngine;
using System.Collections;
using System;
using System.Xml.Serialization;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace I2.TextAnimation
{
    [Serializable]
    public class SE_AnimSequence_Color : SE_AnimSequence
    {
        public Gradient _Gradient = new Gradient();
        public enum eColorBlendMode { Replace, AlphaBlend, Multiply, Additive };
        public eColorBlendMode _ColorBlend = eColorBlendMode.AlphaBlend;

        public bool _ApplyR = true, _ApplyG = true, _ApplyB = true, _ApplyA = true;

        public bool _OnFinish_SetColorToFinalValue;  // When the animation finishes, sets the TextAnimation.color to the final value


        public override string GetTypeName() { return "Color";  }


        public override void UpdateSequence(float dt, TextAnimation se, SE_Animation anim, int sequenceIndex, ref bool makeMaterialDirty, ref bool makeVerticesDirty) 
        { 
            base.UpdateSequence(dt, se, anim, sequenceIndex, ref makeMaterialDirty, ref makeVerticesDirty);
            makeVerticesDirty = true;
        }

        public override void Apply_Characters(TextAnimation se, SE_Animation anim, int sequenceIndex)
        {
            if (anim.mTime < mDelay && !_InitAllElements)
                return;

            // Iterate through all the valid range
            for (int iElement = mElementRangeStart; iElement < mElementRangeEnd; ++iElement)
            {
                float progress = GetProgress(anim.mTime, anim, iElement);
                if (!_InitAllElements && progress < 0)
                    continue;
                progress = progress < 0 ? 0 : progress;

                progress = _EasingCurve.Evaluate(progress);
                var currentColor = TextAnimation.mOriginalVertices.Buffer[iElement * 4].color;

                // Find Color for this Character
                var newColor = _Gradient.Evaluate(progress);//Color32.Lerp(cFrom, cTo, progress);

                //--[ Blend ]----------------------------
                switch (_ColorBlend)
                {
                    case eColorBlendMode.Replace: break;
                    case eColorBlendMode.Multiply: newColor = newColor * currentColor; break;
                    case eColorBlendMode.Additive: newColor = newColor + currentColor; break;
                    case eColorBlendMode.AlphaBlend: newColor = Color.Lerp(currentColor, newColor, newColor.a); break;
                }

                if (_ApplyR) currentColor.r = (byte)(newColor.r * 255);
                if (_ApplyG) currentColor.g = (byte)(newColor.g * 255);
                if (_ApplyB) currentColor.b = (byte)(newColor.b * 255);
                if (_ApplyA) currentColor.a = (byte)(newColor.a * 255);

                // Apply to all Vertices
                for (int v=iElement*4; v<iElement*4+4; ++v)
                    TextAnimation.mOriginalVertices.Buffer[v].color = currentColor;
            }
        }

        public bool HasRandom(float maxRandom) { return maxRandom < -0.001f || maxRandom > 0.001f; }


        public override void OnStop(TextAnimation se, SE_Animation anim, int sequenceIndex)
        {
            if (!Application.isPlaying || !_OnFinish_SetColorToFinalValue)
                return;

            float t = anim._Backwards ? 0 : 1;
            var progress = _EasingCurve.Evaluate(t);
            var currentColor = se.mWidgetColor;

            // Find Color for this Character
            var newColor = _Gradient.Evaluate(progress);

            //--[ Blend ]----------------------------
            switch (_ColorBlend)
            {
                case eColorBlendMode.Replace: break;
                case eColorBlendMode.Multiply: newColor = newColor * currentColor; break;
                case eColorBlendMode.Additive: newColor = newColor + currentColor; break;
                case eColorBlendMode.AlphaBlend: newColor = Color.Lerp(currentColor, newColor, newColor.a); break;
            }

            if (_ApplyR) currentColor.r = (byte)(newColor.r * 255);
            if (_ApplyG) currentColor.g = (byte)(newColor.g * 255);
            if (_ApplyB) currentColor.b = (byte)(newColor.b * 255);
            if (_ApplyA) currentColor.a = (byte)(newColor.a * 255);

            se.SetWidgetColor(currentColor);
        }
#if UNITY_EDITOR

        public override void InspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            _OnFinish_SetColorToFinalValue = GUILayout.Toggle(_OnFinish_SetColorToFinalValue, new GUIContent("On Finish: SetColorToFinalValue", "When the animation finishes, sets the TextAnimation.color to the final value"));
            GUILayout.Space(5);


            GUILayout.BeginHorizontal();
                GUILayout.Label("Colors", GUILayout.Width(EditorGUIUtility.labelWidth));

                GUILayout.Label("", GUILayout.ExpandWidth(true));
                Rect rect = GUILayoutUtility.GetLastRect();
                _Gradient = InvokeGradientField(rect, _Gradient);

                _ColorBlend = (eColorBlendMode)EditorGUILayout.EnumPopup(_ColorBlend, GUILayout.Width(100));
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
                _EasingCurve = EditorGUILayout.CurveField("Easing", _EasingCurve);
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
                GUILayout.Label("Apply:");
                _ApplyR = GUILayout.Toggle(_ApplyR, "R");
                _ApplyG = GUILayout.Toggle(_ApplyG, "G");
                _ApplyB = GUILayout.Toggle(_ApplyB, "B");
                _ApplyA = GUILayout.Toggle(_ApplyA, "A");
                GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }


        //static private BindingFlags _flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
        static private MethodInfo mMethod_GradientField = null;

        static public Gradient InvokeGradientField(params object[] p_args)
        {
            if (mMethod_GradientField == null)
            {
                MethodInfo[] mi = typeof(UnityEditor.EditorGUI).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                foreach (var m in mi)
                    if (m.Name == "GradientField")
                    {
                        mMethod_GradientField = m;
                        break;
                    }
            }
            if (mMethod_GradientField!=null)
                return (Gradient)mMethod_GradientField.Invoke(null, p_args);
            return null;
        }

#endif
    }
}
