using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Animation.Base
{
    public class AnimationCombo : MonoBehaviour
    {
        public Dictionary<string, AnimationParam> animationDict = new Dictionary<string, AnimationParam>();

        public AnimationParam GetAnimation(string name)
        {
            AnimationParam animationParam = null;
            if (this.animationDict.TryGetValue(name, out animationParam))
            {
                return animationParam;
            }
            else
            {
                return null;
            }
        }

        void Awake()
        {
            foreach (AnimationParam item in this.GetComponents<AnimationParam>())
            {
                this.animationDict[item.animationName] = item;
            }
        }
    }
}