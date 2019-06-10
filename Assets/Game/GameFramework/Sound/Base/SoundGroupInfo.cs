using System;
using UnityEngine;

namespace GameFramework.Sound
{
    [Serializable]
    public sealed class SoundGroupInfo
    {
        [SerializeField]
        private string name = null;
        [SerializeField]
        private bool addWhenDontHaveEnoughSound = true;
        [SerializeField]
        private bool mute = false;
        [SerializeField]
        private int soundCount = 1;
        [SerializeField, Range(0f, 1f)]
        private float volue = 1f;
        public string Name => name;
        public bool Mute => mute;
        public float Volume => volue;
        public int SoundCount => soundCount;
        public bool AddWhenDontHaveEnoughSound => addWhenDontHaveEnoughSound;
    }
}