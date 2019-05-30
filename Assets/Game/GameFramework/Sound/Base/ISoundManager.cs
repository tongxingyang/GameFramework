using System.Collections.Generic;
using GameFramework.Res.Base;

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
        int PlaySound(string soundAssetName, int priority, PlaySoundParams playSoundParams);
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
        int PlayMusic(string assetPath);
        void StopMusic();
        int PlayUISound(string assetPath);
    }
}