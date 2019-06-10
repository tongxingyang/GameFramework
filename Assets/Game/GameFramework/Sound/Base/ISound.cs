using UnityEngine;

namespace GameFramework.Sound.Base
{
    public interface ISound
    {
        bool IsPlaying { get; }
        string SoundName { get; set; }
        ISoundGroup SoundGroup { get; }
        int SerialId { get; set; }
        float Length { get; }
        float Time { get; set; }
        float VolumeInGroup { get; set; }
        bool MuteInGroup { get; set; }
        bool Loop { get; set; }
        int Priority { get; set; }
        float Pitch { get; set; }
        float PanStereo { get; set; }
        float SpatialBlend { get; set; }
        float MaxDistance { get; set; }
        float DopplerLevel { get; set; }
        void Play();
        void Play(float fadeInTime);
        void Stop();
        void Stop(float fadeOutTime);
        void Pause();
        void Pause(float fadeOutTime);
        void Resume();
        void Resume(float fadeInTime);
        void Reset();
        bool SetSoundAsset(AudioClip soundAsset);
        void SetFollowObject(Transform followTransform);
        void SetWorldPosition(Vector3 worldVector3);
    }
}