using System;
using UnityEngine;
using UnityEngine.Video;

namespace GameFramework.Sound.Base
{
    sealed class Constant
    {
        public static readonly float DefaultTime = 0f;
        public static readonly bool DefaultMute = false;
        public static readonly bool DefaultLoop = false;
        public static readonly int DefaultPriority = 0;
        public static readonly float DefaultVolume = 1f;
        public static readonly float DefaultFadeInSeconds = 0f;
        public static readonly float DefaultFadeOutSeconds = 0f;
        public static readonly float DefaultPitch = 1f;
        public static readonly float DefaultPanStereo = 0f;
        public static readonly float DefaultSpatialBlend = 0f;
        public static readonly float DefaultMaxDistance = 100f;
        public static readonly float DefaultDopplerLevel = 1f;
        public static readonly Transform DefaultFollowTransform = null;
        public static readonly Vector3 DefaultWorldVector3 = default(Vector3);
    }

    public sealed class PlaySoundParams
    {
        private float time;
        private bool muteInSoundGroup;
        private bool loop;
        private int priority;
        private float volumeInSoundGroup;
        private float fadeInSeconds;
        private float pitch;
        private float panStereo;
        private float spatialBlend;
        private float maxDistance;
        private float dopplerLevel;
        private Transform followTransform;
        private Vector3 worldVector3;
        private object userData;
        private string groupName;
        private int serialId;
        
        public PlaySoundParams()
        {
            serialId = -1;
            userData = null;
            groupName = String.Empty;
            time = Constant.DefaultTime;
            muteInSoundGroup = Constant.DefaultMute;
            loop = Constant.DefaultLoop;
            priority = Constant.DefaultPriority;
            volumeInSoundGroup = Constant.DefaultVolume;
            fadeInSeconds = Constant.DefaultFadeInSeconds;
            pitch = Constant.DefaultPitch;
            panStereo = Constant.DefaultPanStereo;
            spatialBlend = Constant.DefaultSpatialBlend;
            maxDistance = Constant.DefaultMaxDistance;
            dopplerLevel = Constant.DefaultDopplerLevel;
            followTransform = Constant.DefaultFollowTransform;
            worldVector3 = Constant.DefaultWorldVector3;
        }

        public string SoundGroupName
        {
            get => groupName;
            set => groupName = value;
        }
        
        public object UserData
        {
            get => userData;
            set => userData = value;
        }
        
        public int SerialId
        {
            get => serialId;
            set => serialId = value;
        }
        
        public float Time
        {
            get => time;
            set => time = value;
        }

        public bool MuteInSoundGroup
        {
            get => muteInSoundGroup;
            set => muteInSoundGroup = value;
        }

        public bool Loop
        {
            get => loop;
            set => loop = value;
        }

        public int Priority
        {
            get => priority;
            set => priority = value;
        }

        public float VolumeInSoundGroup
        {
            get => volumeInSoundGroup;
            set => volumeInSoundGroup = value;
        }

        public float FadeInSeconds
        {
            get => fadeInSeconds;
            set => fadeInSeconds = value;
        }

        public float Pitch
        {
            get => pitch;
            set => pitch = value;
        }

        public float PanStereo
        {
            get => panStereo;
            set => panStereo = value;
        }

        public float SpatialBlend
        {
            get => spatialBlend;
            set => spatialBlend = value;
        }

        public float MaxDistance
        {
            get => maxDistance;
            set => maxDistance = value;
        }

        public float DopplerLevel
        {
            get => dopplerLevel;
            set => dopplerLevel = value;
        }

        public Transform FollowTransform
        {
            get => followTransform;
            set => followTransform = value;
        }

        public Vector3 WorldVector3
        {
            get => worldVector3;
            set => worldVector3 = value;
        }
    }
}