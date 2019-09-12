using UnityEngine;
using System.Collections;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace I2.TextAnimation
{
    [Serializable]
    public class SE_AnimSequence_Alpha : SE_AnimSequence_Float
    {
        public bool _OnFinish_SetAlphaToFinalValue;  // When the animation finishes, sets the TextAnimation.color to the final value

        public override string GetTypeName() { return "Alpha";  }


        public override void Apply_Characters(TextAnimation se, SE_Animation anim, int sequenceIndex)
        {
            base.Apply_Characters(se, anim, sequenceIndex);
        }

        public override void OnStop(TextAnimation se, SE_Animation anim, int sequenceIndex)
        {
            if (!Application.isPlaying || !_OnFinish_SetAlphaToFinalValue)
                return;

            var currentAlpha = se.mWidgetColor.a;

            float t = anim._Backwards ? 0 : 1;

            var progress = _EasingCurve.Evaluate(t);

            //--[ From ]----------------------------
            float aFrom = _From * 255;
            if (_AnimBlend_From == eFloatAnimBlendMode.Offset) aFrom = aFrom + currentAlpha;
            if (_AnimBlend_From == eFloatAnimBlendMode.Blend) aFrom = _From * currentAlpha;

            if (HasRandom(_FromRandom))
                aFrom += 255 * _FromRandom * DRandom.GetUnit(0);

            //--[ To ]----------------------------
            float aTo = 255 * _To;
            if (_AnimBlend_To == eFloatAnimBlendMode.Offset) aTo = (currentAlpha + _To);
            if (_AnimBlend_To == eFloatAnimBlendMode.Blend) aTo = (currentAlpha * _To);


            if (HasRandom(_ToRandom))
                aTo += 255 * _ToRandom * DRandom.GetUnit(0* 2+90);

            // Find Alpha for this Character
            float falpha = (aFrom + (aTo - aFrom) * progress);
            byte alpha = (byte)(falpha < 0 ? 0 : falpha > 255 ? 255 : falpha);

            var color = se.mWidgetColor;
            color.a = alpha;
            se.SetWidgetColor(color);
        }


#if UNITY_EDITOR
        public override void InspectorGUI()
        {
            _OnFinish_SetAlphaToFinalValue = GUILayout.Toggle(_OnFinish_SetAlphaToFinalValue, new GUIContent("On Finish: SetAlphaToFinalValue", "When the animation finishes, sets the TextAnimation.color to the final value"));
            GUILayout.Space(5);
            base.InspectorGUI();
        }
#endif

    }
}
