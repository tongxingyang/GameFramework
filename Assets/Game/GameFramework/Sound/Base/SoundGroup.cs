using System.Collections.Generic;
using GameFramework.Debug;
using UnityEngine;

namespace GameFramework.Sound.Base
{
    public sealed class SoundGroup : MonoBehaviour, ISoundGroup
    {
        [SerializeField]
        private string soundGroupName;
        [SerializeField]
        private List<Sound> sounds;
        [SerializeField]
        private bool mute;
        [SerializeField]
        private bool addWhenDontHaveEnoughSound;
        [SerializeField]
        private float volume;

        public bool AddWhenDontHaveEnoughSound
        {
            get => addWhenDontHaveEnoughSound;
            set => addWhenDontHaveEnoughSound = value;
        }
        public string Name
        {
            get => soundGroupName;
            set => soundGroupName = value;
        }
        public int SoundCount => sounds.Count;

        public bool Mute
        {
            get => mute;
            set
            {
                mute = value;
                foreach (Sound sound in sounds)
                {
                    sound.RefreshMute();
                }
            }
        }

        public float Volume
        {
            get => volume;
            set
            {
                volume = value;
                foreach (Sound sound in sounds)
                {
                    sound.RefreshVolume();
                }
            }
        }

        public void Awake()
        {
            sounds = new List<Sound>();
        }

        public bool HasSoundInGroup(string name)
        {
            foreach (Sound sound in sounds)
            {
                if (sound.SoundName == name)
                {
                    return true;
                }
            }
            return false;
        }

        public Sound GetSound(string name)
        {
            foreach (Sound sound in sounds)
            {
                if (sound.SoundName == name)
                {
                    return sound;
                }
            }
            return null;
        }

        public void AddSoundToGroup(Sound sound)
        {
            if(HasSoundInGroup(sound.name)) return;
            sounds.Add(sound);
        }
        
        public ISound PlaySound(AudioClip soundAsset, PlaySoundParams playSoundParams)
        {
            Sound currentSound = null;
            foreach (Sound sound in sounds)
            {
                if (!sound.IsPlaying)
                {
                    currentSound = sound;
                    break;
                }
                if (sound.Priority < playSoundParams.Priority)
                {
                    if (currentSound == null || sound.Priority < currentSound.Priority)
                    {
                        currentSound = sound;
                    }
                }
            }
            if (currentSound == null)
            {
                if (addWhenDontHaveEnoughSound)
                {
                    string str = Name + "_" + sounds.Count;
                    currentSound = new GameObject(str).AddComponent<Sound>();
                    currentSound.SoundName = str;
                    currentSound.transform.SetParent(transform);
                    currentSound.transform.localPosition = Vector3.zero;
                    currentSound.transform.localScale = Vector3.one;
                    currentSound.SetSoundGroup(this);
                    AddSoundToGroup(currentSound);
                }
                else
                {
                    Debuger.LogError("play sound error dont have other sound ");
                    return null;
                }
               
            }
            currentSound.SetSoundAsset(soundAsset);
            currentSound.SerialId = playSoundParams.SerialId;
            currentSound.Time = playSoundParams.Time;
            currentSound.MuteInGroup = playSoundParams.MuteInSoundGroup;
            currentSound.Loop = playSoundParams.Loop;
            currentSound.Priority = playSoundParams.Priority;
            currentSound.VolumeInGroup = playSoundParams.VolumeInSoundGroup;
            currentSound.Pitch = playSoundParams.Pitch;
            currentSound.PanStereo = playSoundParams.PanStereo;
            currentSound.SpatialBlend = playSoundParams.SpatialBlend;
            currentSound.MaxDistance = playSoundParams.MaxDistance;
            currentSound.DopplerLevel = playSoundParams.DopplerLevel;
            currentSound.Play(playSoundParams.FadeInSeconds);
            if (playSoundParams.FollowTransform == null)
            {
                currentSound.SetFollowObject(playSoundParams.FollowTransform);
            }
            else
            {
                currentSound.SetWorldPosition(playSoundParams.WorldVector3);
            }
            return currentSound;
        }

        public bool StopSound(int serialId)
        {
            return StopSound(serialId, Constant.DefaultFadeOutSeconds);
        }

        public bool StopSound(int serialId, float fadeOutTime)
        {
            foreach (Sound sound in sounds)
            {
                if (sound.SerialId == serialId)
                {
                    sound.Stop(fadeOutTime);
                    return true;
                }
            }
            return false;
        }

        public bool PauseSound(int serialId)
        {
            return PauseSound(serialId, Constant.DefaultFadeOutSeconds);
        }

        public bool PauseSound(int serialId, float fadeOutTime)
        {
            foreach (Sound sound in sounds)
            {
                if (sound.SerialId == serialId)
                {
                    sound.Pause(fadeOutTime);
                    return true;
                }
            }
            return false;
        }

        public bool ResumeSound(int serialId)
        {
            return ResumeSound(serialId, Constant.DefaultFadeInSeconds);
        }

        public bool ResumeSound(int serialId, float fadeInTime)
        {
            foreach (Sound sound in sounds)
            {
                if (sound.SerialId == serialId)
                {
                    sound.Resume(fadeInTime);
                    return true;
                }
            }
            return false;
        }

        public void PauseAllSound()
        {
            PauseAllSound(Constant.DefaultFadeOutSeconds);
        }

        public void PauseAllSound(float fadeOutTime)
        {
            foreach (Sound sound in sounds)
            {
                sound.Pause(fadeOutTime);
            }
        }

        public void ResumeAllSound()
        {
            ResumeAllSound(Constant.DefaultFadeInSeconds);
        }

        public void ResumeAllSound(float fadeInTime)
        {
            foreach (Sound sound in sounds)
            {
                sound.Resume(fadeInTime);
            }
        }

        public void StopAllSound()
        {
            StopAllSound(Constant.DefaultFadeOutSeconds);
        }

        public void StopAllSound(float fadeOutTime)
        {
            foreach (Sound sound in sounds)
            {
                sound.Stop(fadeOutTime);
            }
        }
        
        public void ResetAllSound()
        {
            foreach (Sound sound in sounds)
            {
                sound.Reset();
            }
        }
        
    }
}