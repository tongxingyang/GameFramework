using UnityEngine;
using UnityEngine.Events;

namespace GameFramework.Animation.Base
{
    public enum enPositionSpace:byte
    {
        Self = 0,
        UI = 1,
        World = 2,
    }
    public class AnimationItem
    {
        public GameObject obj;
        public float time;
        public AnimationParam parameter;
        public UnityAction callback;
        public UnityAction frameCallback;
    }
}