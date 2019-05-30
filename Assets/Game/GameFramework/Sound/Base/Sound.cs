using System;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using GameFramework.Debug;
using GameFramework.Utility.Extension;
using UnityEngine;

namespace GameFramework.Sound.Base
{
    public sealed class Sound : MonoBehaviour,ISound
    {
        private Transform cacheTransform = null;
        private AudioSource audioSource = null;
        [SerializeField]
        private Transform followTransform = null;
        [SerializeField]
        private float pauseVolume = 0f;
        [SerializeField]
        private SoundGroup soundGroup;
        [SerializeField]
        private int serialId;
        private AudioClip audioClip = null;
        private DateTime setSoundAsseTime;
        [SerializeField]
        private bool muteInGroup;
        [SerializeField]
        private float volumeInGroup;

        private bool isPause = false;
        
        public bool IsPlaying => audioSource.isPlaying || isPause;

        public DateTime SetSoundAssetTime
        {
            get { return setSoundAsseTime; }
            set { setSoundAsseTime = value; }
        }
        public ISoundGroup SoundGroup => soundGroup;

        public int SeriaiId
        {
            get { return serialId; }
            set { serialId = value; }
        }
        public float Length => audioSource.clip != null ? audioSource.clip.length : 0f;

        public float Time
        {
            get
            {
                return audioSource.time;
            }
            set
            {
                if (value < 0) 
                    value = 0;
                audioSource.time = value;
            }
        }

        public float VolumeInGroup
        {
            get { return volumeInGroup; }
            set
            {
                if (value < 0) 
                    value = 0;
                volumeInGroup = value;
                RefreshVolume();
            }
        }

        public bool MuteInGroup
        {
            get { return muteInGroup; }
            set { muteInGroup = value; RefreshMute(); }
        }

        public bool Loop
        {
            get { return audioSource.loop; }
            set { audioSource.loop = value; }
        }

        public int Priority
        {
            get { return 128 - audioSource.priority; }
            set { audioSource.priority = 128 - value; }
        }

        public float Pitch
        {
            get { return audioSource.pitch; }
            set { audioSource.pitch = value; }
        }

        public float PanStereo
        {
            get { return audioSource.panStereo; }
            set { audioSource.panStereo = value; }
        }

        public float SpatialBlend
        {
            get { return audioSource.spatialBlend; }
            set { audioSource.spatialBlend = value; }
        }

        public float MaxDistance
        {
            get { return audioSource.maxDistance; }
            set { audioSource.maxDistance = value; }
        }
        public float DopplerLevel { get; set; }


        void Awake()
        {
            cacheTransform = transform;
            audioSource = gameObject.GetOrAddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.rolloffMode = AudioRolloffMode.Custom;
        }

        void Update()
        {
            if (!IsPlaying && audioSource.clip != null)
            {
                Reset();
                return;
            }
            if (followTransform != null)
            {
                UpdateSoundPosition();
            }
        }
        
        public void RefreshVolume()
        {
            audioSource.volume = soundGroup.Volume * volumeInGroup;
        }

        public void RefreshMute()
        {
            audioSource.mute = soundGroup.Mute || muteInGroup;
        }
        
        public void SetSoundGroup(SoundGroup soundGroup)
        {
            this.soundGroup = soundGroup;
            this.serialId = 0;
            audioClip = null;
            this.isPause = false;
            Reset();
        }
        
        public void Play()
        {
            Play(Constant.DefaultFadeInSeconds);
        }

        public void Play(float fadeInTime)
        {
            StopAllCoroutines();
            audioSource.Play();
            if (fadeInTime > 0f)
            {
                float volume = audioSource.volume;
                audioSource.volume = 0f;
                StartCoroutine(FadeToVolume(volume, fadeInTime));
            }
        }

        public void Stop()
        {
            Stop(Constant.DefaultFadeOutSeconds);
        }

        public void Stop(float fadeOutTime)
        {
            StopAllCoroutines();
            if (fadeOutTime > 0f)
            {
                StartCoroutine(StopToVolume(fadeOutTime));
            }
            else
            {
                audioSource.Stop();
            }
        }

        public void Pause()
        {
            Pause(Constant.DefaultFadeOutSeconds);
        }

        public void Pause(float fadeOutTime)
        {
            this.isPause = true;
            StopAllCoroutines();
            pauseVolume = audioSource.volume;
            if (fadeOutTime > 0f)
            {
                StartCoroutine(PauseToVolume(fadeOutTime));
            }
            else
            {
                audioSource.Pause();
            }
        }

        public void Resume()
        {
            Resume(Constant.DefaultFadeInSeconds);
        }

        public void Resume(float fadeInTime)
        {
            this.isPause = false;
            StopAllCoroutines();
            audioSource.UnPause();
            if (fadeInTime > 0f)
            {
                StartCoroutine(FadeToVolume(pauseVolume, fadeInTime));
            }
            else
            {
                audioSource.volume = pauseVolume;
            }
        }

        public void Reset()
        {
            if (audioClip != null)
            {
                //释放音效文件 todo txy
                audioClip = null;
            }
            isPause = false;
            SetSoundAssetTime = DateTime.MinValue;
            Time = Constant.DefaultTime;
            MuteInGroup = Constant.DefaultMute;
            Loop = Constant.DefaultLoop;
            Priority = Constant.DefaultPriority;
            VolumeInGroup = Constant.DefaultVolume;
            Pitch = Constant.DefaultPitch;
            PanStereo = Constant.DefaultPanStereo;
            SpatialBlend = Constant.DefaultSpatialBlend;
            MaxDistance = Constant.DefaultMaxDistance;
            DopplerLevel = Constant.DefaultDopplerLevel;
            cacheTransform.localPosition = Vector3.zero;
            pauseVolume = 0f;
            audioSource.clip = null;
            followTransform = null;
        }

        public bool SetSoundAsset(AudioClip soundAsset)
        {
            if (soundAsset == null) return false;
            Reset();
            audioClip = soundAsset;
            audioSource.clip = soundAsset;
            return true;
        }

        public void SetFollowObject(Transform followTransform)
        {
            this.followTransform = followTransform;
            UpdateSoundPosition();
        }

        public void SetWorldPosition(Vector3 worldVector3)
        {
            cacheTransform.position = worldVector3;
        }

        private void UpdateSoundPosition()
        {
            if (followTransform != null)
            {
                cacheTransform.position = followTransform.transform.position;
                return;
            }
            Reset();
        }
        
        private IEnumerator FadeToVolume(float volume, float duration)
        {
            float time = 0f;
            float originalVolume = audioSource.volume;
            while (time < duration)
            {
                time += UnityEngine.Time.deltaTime;
                audioSource.volume = Mathf.Lerp(originalVolume, volume, time / duration);
                yield return new WaitForEndOfFrame();
            }

            audioSource.volume = volume;
        }

        private IEnumerator StopToVolume(float fadeTime)
        {
            yield return FadeToVolume(0f, fadeTime);
            audioSource.Stop();
        }

        private IEnumerator PauseToVolume(float fadeTime)
        {
            yield return FadeToVolume(0f, fadeTime);
            audioSource.Pause();
        }
    }
}