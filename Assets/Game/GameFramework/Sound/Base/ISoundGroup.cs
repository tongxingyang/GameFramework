using UnityEngine;

namespace GameFramework.Sound.Base
{
    public interface ISoundGroup
    {
        string Name { get; }
        int SoundCount { get; }
        bool AddWhenDontHaveEnoughSound { get; set; }
        bool Mute { get; set; }
        float Volume { get; set; }
        ISound PlaySound( AudioClip soundAsset, PlaySoundParams playSoundParams);
        bool StopSound(int serialId);
        bool StopSound(int serialId, float fadeOutTime);
        bool PauseSound(int serialId);
        bool PauseSound(int serialId, float fadeOutTime);
        bool ResumeSound(int serialId);
        bool ResumeSound(int serialId, float fadeInTime);
        void PauseAllSound();
        void PauseAllSound(float fadeOutTime);
        void ResumeAllSound();
        void ResumeAllSound(float fadeInTime);
        void StopAllSound();
        void StopAllSound(float fadeOutTime);
        void ResetAllSound();
    }
}