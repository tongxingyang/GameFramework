using System.Collections.Generic;
using GameFramework.Base;
using GameFramework.Debug;
using GameFramework.Res;
using GameFramework.Res.Base;
using GameFramework.Sound.Base;
using GameFramework.Utility.Singleton;
using UnityEngine;

namespace GameFramework.Sound
{
    [DisallowMultipleComponent]
    public class SoundComponent : GameFrameworkComponent
    {
        private SoundManager soundManager;
        private Transform instanceRoot;
        
        public override int Priority => SingletonMono<GameFramework>.GetInstance().SoundPriority;
        
        public override void OnAwake()
        {
            base.OnAwake();
            soundManager = new SoundManager();
            if (instanceRoot == null)
            {
                instanceRoot = (new GameObject("SoundGroup Root")).transform;
                instanceRoot.SetParent(gameObject.transform);
                instanceRoot.localScale = Vector3.one;
                instanceRoot.localPosition = Vector3.zero;
            }
            for (int i = 0; i < SingletonMono<GameFramework>.GetInstance().SoundGroupInfos.Length; i++)
            {
                if (!AddSoundGroup(SingletonMono<GameFramework>.GetInstance().SoundGroupInfos[i]))
                {
                    Debuger.LogError("Add sound group '{0}' failure.",LogColor.Red, SingletonMono<GameFramework>.GetInstance().SoundGroupInfos[i].Name);
                    continue;
                }
            }
        }

        private bool AddSoundGroup(SoundGroupInfo info)
        {
            if (HasSoundGroup(info.Name)) return false;
            SoundGroup soundGroup = (new GameObject(name)).AddComponent<SoundGroup>();
            soundGroup.Name = name;
            soundGroup.AddWhenDontHaveEnoughSound = info.AddWhenDontHaveEnoughSound;
            soundGroup.Mute = info.Mute;
            soundGroup.Volume = info.Volume;
            soundGroup.gameObject.transform.SetParent(instanceRoot);
            soundGroup.gameObject.transform.localPosition = Vector3.zero;
            soundGroup.gameObject.transform.localScale = Vector3.one;
            soundManager.AddSoundGroup(soundGroup);
            for (int i = 0; i < info.SoundCount; i++)
            {
                AddSound(name + "_" + i, soundGroup);
            }
            return true;

        }

        private void AddSound(string name,SoundGroup soundGroup)
        {
            Base.Sound sound = new GameObject(name).AddComponent<Base.Sound>();
            sound.SoundName = name;
            sound.transform.SetParent(soundGroup.transform);
            sound.transform.localPosition = Vector3.zero;
            sound.transform.localScale = Vector3.one;
            sound.SetSoundGroup(soundGroup);
            soundGroup.AddSoundToGroup(sound);
        }
        
        public bool HasSoundGroup(string name)
        {
            return soundManager.HasSoundGroup(name);
        }
        
        public override void OnStart()
        {
            base.OnStart();
            soundManager.SetResourceManager(Singleton<GameEntry>.GetInstance().GetComponent<ResourceComponent>().GetResourceManager());
        }

        public override void Shutdown()
        {
            base.Shutdown();
            soundManager.Shutdown();
        }

        public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            soundManager.OnUpdate(elapseSeconds,realElapseSeconds);
        }

        public ISoundGroup GetSoundGroup(string soundGroupName)
        {
            return soundManager.GetSoundGroup(soundGroupName);
        }

        public ISoundGroup[] GetAllSoundGroups()
        {
            return soundManager.GetAllSoundGroups();
        }

        public void GetAllSoundGroups(List<ISoundGroup> results)
        {
            soundManager.GetAllSoundGroups(results);
        }

        public int[] GetAllSoundSerialIds()
        {
            return soundManager.GetAllSoundSerialIds();
        }

        public void GetAllSoundSerialIds(List<int> results)
        {
            soundManager.GetAllSoundSerialIds(results);
        }

        public bool IsLoadingSound(int serialId)
        {
            return soundManager.IsLoadingSound(serialId);
        }

        public int PlaySound(ResourceLoadInfo resourceLoadInfo, PlaySoundParams playSoundParams)
        {
            return soundManager.PlaySound(resourceLoadInfo, playSoundParams);
        }

        public bool StopSound(int serialId)
        {
            return soundManager.StopSound(serialId);
        }

        public bool StopSound(int serialId, float fadeOutTime)
        {
            return soundManager.StopSound(serialId, fadeOutTime);
        }

        public void StopAllLoadingSounds()
        {
            soundManager.StopAllLoadingSounds();
        }

        public void StopAllSounds()
        {
            soundManager.StopAllSounds();
        }

        public void StopAllSounds(float fadeOutTime)
        {
            soundManager.StopAllSounds(fadeOutTime);
        }

        public bool PauseSound(int serialId)
        {
            return soundManager.PauseSound(serialId);
        }

        public bool PauseSound(int serialId, float fadeOutTime)
        {
            return soundManager.PauseSound(serialId, fadeOutTime);
        }

        public void PauseAllSounds()
        {
            soundManager.PauseAllSounds();
        }

        public void PauseAllSounds(float fadeOutTime)
        {
            soundManager.PauseAllSounds(fadeOutTime);
        }

        public bool ResumeSound(int serialId)
        {
            return soundManager.ResumeSound(serialId);
        }

        public bool ResumeSound(int serialId, float fadeOutTime)
        {
            return soundManager.ResumeSound(serialId, fadeOutTime);
        }

        public void ResumeAllSounds()
        {
            soundManager.ResumeAllSounds();
        }

        public void ResumeAllSounds(float fadeOutTime)
        {
            soundManager.ResumeAllSounds(fadeOutTime);
        }

        public int PlayMusic(ResourceLoadInfo resourceLoadInfo,string assetPath)
        {
            return soundManager.PlayMusic(resourceLoadInfo,assetPath);
        }

        public void StopMusic()
        {
            soundManager.StopMusic();
        }

        public int PlayUISound(ResourceLoadInfo resourceLoadInfo,string assetPath)
        {
            return soundManager.PlayUISound(resourceLoadInfo,assetPath);
        }
        public int PlayWorldSound(ResourceLoadInfo resourceLoadInfo,string assetPath,Vector3 worldPos)
        {
            return soundManager.PlayWorldSound(resourceLoadInfo, assetPath, worldPos);
        }
        
        public int PlayFollowSound(ResourceLoadInfo resourceLoadInfo,string assetPath,Transform followPos)
        {
            return soundManager.PlayFollowSound(resourceLoadInfo, assetPath, followPos);
        }
        
    }
}