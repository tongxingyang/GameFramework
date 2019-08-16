using System.Collections.Generic;
using GameFramework.Base;
using GameFramework.Debug;
using GameFramework.Res;
using GameFramework.Res.Base;
using UnityEngine;

namespace GameFramework.Sound.Base
{
    public sealed class SoundManager : ISoundManager
    {
        public static int Serial = 0;

        private Dictionary<string, SoundGroup> soundGroups;
        private List<int> soundBeingLoaded;
        private List<int> soundToReleaseOnLoad;
        private IResourceManager resourceManager;
        private LoadCallback loadCallback;
        public float DefaultSoundDuration = 1f;
        private int musicSerialId = -1;

        public int SoundGroupCount => soundGroups.Count;

        public SoundManager()
        {
            soundGroups = new Dictionary<string, SoundGroup>();
            soundBeingLoaded = new List<int>();
            soundToReleaseOnLoad = new List<int>();
            resourceManager = null;
            loadCallback = new LoadCallback(LoadSoundSuccessCallback, LoadSoundFailureCallback);

        }

        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            if (soundGroups.Count > 0)
            {
                foreach (KeyValuePair<string,SoundGroup> keyValuePair in soundGroups)
                {
                    keyValuePair.Value.OnUpdate(elapseSeconds, realElapseSeconds);
                }
            }
        }

        public void Shutdown()
        {
            StopAllSounds(0);
            foreach (KeyValuePair<string,SoundGroup> keyValuePair in soundGroups)
            {
                keyValuePair.Value.ResetAllSound();
            }
            soundGroups.Clear();
            soundBeingLoaded.Clear();
            soundToReleaseOnLoad.Clear();
        }
        
        public void SetResourceManager(IResourceManager resource)
        {
            resourceManager = resource;
        }

        public bool HasSoundGroup(string soundGroupName)
        {
            return soundGroups.ContainsKey(soundGroupName);
        }

        public ISoundGroup GetSoundGroup(string soundGroupName)
        {
            if(soundGroups.TryGetValue(soundGroupName, out var soundGroup))
            {
                return soundGroup;
            }
            return null;
        }
        
        public ISoundGroup[] GetAllSoundGroups()
        {
            int index = 0;
            ISoundGroup[] groups = new ISoundGroup[soundGroups.Count];
            foreach (KeyValuePair<string,SoundGroup> keyValuePair in soundGroups)
            {
                groups[index++] = keyValuePair.Value;
            }
            return groups;
        }

        public void GetAllSoundGroups(List<ISoundGroup> results)
        {
            results.Clear();
            foreach (KeyValuePair<string,SoundGroup> soundGroup in soundGroups)
            {
                results.Add(soundGroup.Value);
            }
        }

        public bool AddSoundGroup(ISoundGroup soundGroup)
        {
            if (GetSoundGroup(soundGroup.Name) != null) return false;
            soundGroups.Add(soundGroup.Name,(SoundGroup)soundGroup);
            return true;
        }

        public int[] GetAllSoundSerialIds()
        {
            return soundBeingLoaded.ToArray();
        }

        public void GetAllSoundSerialIds(List<int> results)
        {
            results.Clear();
            results.AddRange(soundBeingLoaded);
        }

        public bool IsLoadingSound(int serialId)
        {
            return soundBeingLoaded.Contains(serialId);
        }

        public int PlaySound(ResourceLoadInfo resourceLoadInfo,PlaySoundParams playSoundParams)
        {
            if (playSoundParams == null)
            {
                return -1;
            }
            int serialId = Serial++;
            SoundGroup soundGroup = (SoundGroup)GetSoundGroup(playSoundParams.SoundGroupName);
            if (soundGroup == null)
            {
                Debuger.LogError(Utility.StringUtility.Format("Sound group '{0}' is not exist.", playSoundParams.SoundGroupName));
                return -1;
            }
            if (soundGroup.SoundCount <= 0)
            {
                Debuger.LogError(Utility.StringUtility.Format("Sound group '{0}' is have no sound agent.", playSoundParams.SoundGroupName));
                return -1;
            }
          
            playSoundParams.SerialId = serialId;
            soundBeingLoaded.Add(serialId);
            resourceManager.LoadAsset<AudioClip>(resourceLoadInfo,loadCallback,playSoundParams);// todo txy
            return serialId;
            
        }

        public bool StopSound(int serialId)
        {
            return StopSound(serialId, Constant.DefaultFadeOutSeconds);
        }

        public bool StopSound(int serialId, float fadeOutTime)
        {
            if (IsLoadingSound(serialId))
            {
                soundToReleaseOnLoad.Add(serialId);
                return true;
            }
            foreach (KeyValuePair<string,SoundGroup> soundGroup in soundGroups)
            {
                if (soundGroup.Value.StopSound(serialId, fadeOutTime))
                {
                    return true;
                }
            }
            return false;
        }

        public void StopAllLoadingSounds()
        {
            foreach (int id in soundBeingLoaded)
            {
                soundToReleaseOnLoad.Add(id);
            }
        }
        
        public void StopAllSounds()
        {
            StopAllSounds(Constant.DefaultFadeOutSeconds);
        }

        public void StopAllSounds(float fadeOutTime)
        {
            foreach (KeyValuePair<string,SoundGroup> soundGroup in soundGroups)
            {
                soundGroup.Value.StopAllSound(fadeOutTime);
            }
        }

        public bool PauseSound(int serialId)
        {
            return PauseSound(serialId, Constant.DefaultFadeOutSeconds);
        }

        public bool PauseSound(int serialId, float fadeOutTime)
        {
            foreach (KeyValuePair<string,SoundGroup> keyValuePair in soundGroups)
            {
                if (keyValuePair.Value.PauseSound(serialId, fadeOutTime))
                {
                    return true;
                }
            }
            return false;
        }

        public void PauseAllSounds()
        {
            PauseAllSounds(Constant.DefaultFadeOutSeconds);
        }

        public void PauseAllSounds(float fadeOutTime)
        {
            foreach (var soundGroup in soundGroups)
            {
                soundGroup.Value.PauseAllSound(fadeOutTime);
            }
        }

        public bool ResumeSound(int serialId)
        {
            return ResumeSound(serialId, Constant.DefaultFadeInSeconds);
        }

        public bool ResumeSound(int serialId, float fadeInTime)
        {
            foreach (var soundGroup in soundGroups)
            {
                if (soundGroup.Value.ResumeSound(serialId, fadeInTime))
                {
                    return true;
                }
            }
            return false;
        }

        public void ResumeAllSounds()
        {
            ResumeAllSounds(Constant.DefaultFadeInSeconds);
        }

        public void ResumeAllSounds(float fadeInTime)
        {
            foreach (var soundGroup in soundGroups)
            {
                soundGroup.Value.ResumeAllSound(fadeInTime);
            }
        }

        public int PlayMusic(ResourceLoadInfo resourceLoadInfo)
        {
            StopMusic();
            PlaySoundParams playSoundParams = new PlaySoundParams
            {
                Priority = 64,
                Loop = true,
                VolumeInSoundGroup = 1f,
                FadeInSeconds = DefaultSoundDuration,
                SpatialBlend = 0f,
                SoundGroupName = "Music"
            };
            musicSerialId = PlaySound(resourceLoadInfo, playSoundParams);
            return musicSerialId;
        }
        
        public int PlayWorldSound(ResourceLoadInfo resourceLoadInfo,Vector3 worldPos)
        {
            PlaySoundParams playSoundParams = new PlaySoundParams
            {
                Priority = 0,
                Loop = false,
                VolumeInSoundGroup = 1f,
                FadeInSeconds = DefaultSoundDuration,
                SpatialBlend = 1f,
                WorldVector3 = worldPos,
                SoundGroupName = "Sound",
            };
            return PlaySound(resourceLoadInfo, playSoundParams);
        }
        
        
        public int PlayFollowSound(ResourceLoadInfo resourceLoadInfo,Transform followPos)
        {
            PlaySoundParams playSoundParams = new PlaySoundParams
            {
                Priority = 0,
                Loop = false,
                VolumeInSoundGroup = 1f,
                FadeInSeconds = DefaultSoundDuration,
                SpatialBlend = 1f,
                FollowTransform = followPos,
                SoundGroupName = "Sound",
            };
            return PlaySound(resourceLoadInfo, playSoundParams);
        }

        public void StopMusic()
        {
            if(musicSerialId==-1)return;
            StopSound(musicSerialId, DefaultSoundDuration);
            musicSerialId = -1;
        }

        public int PlayUISound(ResourceLoadInfo resourceLoadInfo)
        {
            PlaySoundParams playSoundParams = new PlaySoundParams
            {
                Priority = 0,
                Loop = false,
                VolumeInSoundGroup = 1f,
                FadeInSeconds = DefaultSoundDuration,
                SpatialBlend = 0f,
                SoundGroupName = "UISound",
            };
            return PlaySound(resourceLoadInfo, playSoundParams);
        }

        private void LoadSoundSuccessCallback(string soundAssetName, object soundAsset, float duration, object userData)
        {
            PlaySoundParams playSoundParams = (PlaySoundParams) userData;
            if (playSoundParams == null)
            {
                Debuger.LogError("Play sound info is invalid.");
                return;
            }

            soundBeingLoaded.Remove(playSoundParams.SerialId);
            if (soundToReleaseOnLoad.Contains(playSoundParams.SerialId))
            {
                Debuger.LogError(string.Format("Release sound '{0}' on loading success.", playSoundParams.SerialId.ToString()));
                soundToReleaseOnLoad.Remove(playSoundParams.SerialId);
                //resourceManager.ReleaseSoundAsset(soundAsset);   todo txy
                return;
            }

            ISound soundAgent = GetSoundGroup(playSoundParams.SoundGroupName).PlaySound((AudioClip)soundAsset,
                playSoundParams);
            if (soundAgent != null)
            {

            }
            else
            {
                //resourceManager.ReleaseSoundAsset(soundAsset);    todo txy
                Debuger.LogError(Utility.StringUtility.Format("Sound group '{0}' play sound '{1}' failure.",playSoundParams.SoundGroupName, soundAssetName));
            }
        }

        private void LoadSoundFailureCallback(string soundAssetName, string errorMessage,object userData)
        {
            PlaySoundParams playSoundParams = (PlaySoundParams) userData;
            if (playSoundParams == null)
            {
                Debuger.LogError("Play sound info is invalid.");
                return;
            }

            soundBeingLoaded.Remove(playSoundParams.SerialId);
            soundToReleaseOnLoad.Remove(playSoundParams.SerialId);
            Debuger.LogError(Utility.StringUtility.Format("Load sound failure, asset name '{0}', error message '{1}'.",soundAssetName,errorMessage));

        }
    }
}