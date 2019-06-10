using System;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using GameFramework.Debug;
using GameFramework.Utility.Extension;
using UnityEngine;

namespace GameFramework.Sound.Base
{
    public sealed class Sound : MonoBehaviour, ISound
    {
        private Transform cacheTransform = null;
        private AudioSource audioSource = null;
        private bool isPause = false;
        private string soundName;
        
        [SerializeField]
        private Transform followTransform = null;
        [SerializeField]
        private float pauseVolume = 0f;
        [SerializeField]
        private SoundGroup soundGroup;
        [SerializeField]
        private int serialId;
        [SerializeField]
        private bool muteInGroup;
        [SerializeField]
        private float volumeInGroup;

     
        public bool IsPlaying => audioSource.isPlaying || isPause;
        public ISoundGroup SoundGroup => soundGroup;
        public float Length => audioSource.clip != null ? audioSource.clip.length : 0f;
        
        public string SoundName
        {
            get => soundName;
            set => soundName = value;
        }
        public int SerialId
        {
            get => serialId;
            set => serialId = value;
        }

        public float Time
        {
            get => audioSource.time;
            set
            {
                if (value < 0) 
                    value = 0;
                audioSource.time = value;
            }
        }

        public float VolumeInGroup
        {
            get => volumeInGroup;
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
            get => muteInGroup;
            set { muteInGroup = value; RefreshMute(); }
        }

        public bool Loop
        {
            get => audioSource.loop;
            set => audioSource.loop = value;
        }

        public int Priority
        {
            get => 128 - audioSource.priority;
            set => audioSource.priority = 128 - value;
        }

        public float Pitch
        {
            get => audioSource.pitch;
            set => audioSource.pitch = value;
        }

        public float PanStereo
        {
            get => audioSource.panStereo;
            set => audioSource.panStereo = value; 
        }

        public float SpatialBlend
        {
            get => audioSource.spatialBlend;
            set => audioSource.spatialBlend = value;
        }

        public float MaxDistance
        {
            get => audioSource.maxDistance;
            set => audioSource.maxDistance = value;
        }

        public float DopplerLevel
        {
            get => audioSource.dopplerLevel;
            set => audioSource.dopplerLevel = value;
        }


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
            isPause = true;
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
            isPause = false;
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
            if (!(audioSource.clip == null))
            {
                //todo txy 释放audioSource.clip 资源
                audioSource.clip = null;
                
            }
            isPause = false;
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
            audioSource.clip = null;
            followTransform = null;
            cacheTransform.localPosition = Vector3.zero;
            pauseVolume = 0f;
            serialId = 0;
        }

        public bool SetSoundAsset(AudioClip soundAsset)
        {
            if (soundAsset == null) return false;
            Reset();
            audioSource.clip = soundAsset;
            return true;
        }

        public void SetSoundGroup(SoundGroup group)
        {
            if(soundGroup==null) return;
            Reset();
            soundGroup = group;
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