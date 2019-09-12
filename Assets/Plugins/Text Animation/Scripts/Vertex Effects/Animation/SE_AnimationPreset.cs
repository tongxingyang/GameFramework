using UnityEngine;
using System.Collections.Generic;

namespace I2.TextAnimation
{
    #if !UNITY_5_0
    [CreateAssetMenu(fileName = "AnimationPreset", menuName = "TextAnimation/Animation Preset", order = 1)]
    #endif
    public class SE_AnimationPreset : ScriptableObject
    {
        public string mSerializedData;

        public SE_Animation CreateAnimation()
        {
            return SE_Animation.LoadFromSerializedData(mSerializedData) ?? new SE_Animation();
        }


        #region Dependencies 

        List<TextAnimation> mPresetDependants = new List<TextAnimation>();
        public void RegisterPresetDependency(TextAnimation child)
        {
            if (mPresetDependants!=null && !mPresetDependants.Contains(child) && child!=null)
                mPresetDependants.Add(child);
        }

        public void UnRegisterPresetDependency(TextAnimation child)
        {
            if (mPresetDependants == null)
                return;
            mPresetDependants.Remove(child);
            mPresetDependants.RemoveAll(x=>x == null);
        }

        public void NotifyPresetChanged(bool materialDirty, bool verticesDirty)
        {
            if (mPresetDependants == null)
                return;
            mPresetDependants.RemoveAll(x => x == null);
            for (int i = 0, imax = mPresetDependants.Count; i < imax; ++i)
                mPresetDependants[i].MarkWidgetAsChanged(verticesDirty, materialDirty);
        }
        #endregion
    }
}
