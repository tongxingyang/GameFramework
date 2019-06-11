using System.Collections.Generic;
using GameFramework.Res.Base;
using UnityEngine;

namespace GameFramework.Sound.Base
{
    public interface ISoundManager
    {
        int SoundGroupCount { get; }
        void SetResourceManager(IResourceManager resourceManager);
        bool HasSoundGroup(string soundGroupName);
        ISoundGroup GetSoundGroup(string soundGroupName);
        ISoundGroup[] GetAllSoundGroups();
        void GetAllSoundGroups(List<ISoundGroup> results);
        bool AddSoundGroup(ISoundGroup soundGroup);
        int[] GetAllSoundSerialIds();
        void GetAllSoundSerialIds(List<int> results);
        bool IsLoadingSound(int serialId);
        int PlaySound(ResourceLoadInfo resourceLoadInfo,PlaySoundParams playSoundParams);
        bool StopSound(int serialId);
        bool StopSound(int serialId, float fadeOutTime);
        void StopAllLoadingSounds();
        void StopAllSounds();
        void StopAllSounds(float fadeOutTime);
        bool PauseSound(int serialId);
        bool PauseSound(int serialId, float fadeOutTime);
        void PauseAllSounds();
        void PauseAllSounds(float fadeOutTime);
        bool ResumeSound(int serialId);
        bool ResumeSound(int serialId, float fadeOutTime);
        void ResumeAllSounds();
        void ResumeAllSounds(float fadeOutTime);
        int PlayMusic(ResourceLoadInfo resourceLoadInfo);
        int PlayFollowSound(ResourceLoadInfo resourceLoadInfo, Transform followPos);
        int PlayWorldSound(ResourceLoadInfo resourceLoadInfo, Vector3 worldPos);
        void StopMusic();
        int PlayUISound(ResourceLoadInfo resourceLoadInfo);
    }
}