using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace GameFramework.Editor.Core.AssetImportSetting
{
    public class AudioClipImportManager
    {
        public string AudioClipImportRulePath = "Assets/AudioClipImportRules.asset";

        public class AudioClipImportRule : ScriptableObject
        {
            [Serializable]
            public class AudioClipImportData
            {
                public string AssetPath = String.Empty;
                public string FileFilter = String.Empty;
                public int Index = 0;
                public bool IsForceToMono = false;
                public bool IsLoadInBackground = false;
                public bool IsAmbisonic = false;
                public bool IsPreloadAudioClip = false;
                public int Quality = 100;

                public AudioCompressionFormat AndroidCompressionFormat = AudioCompressionFormat.MP3;
                public AudioCompressionFormat iPhoneCompressionFormat = AudioCompressionFormat.MP3;
                public AudioClipLoadType AndroidAudioClipLoadType = AudioClipLoadType.CompressedInMemory;
                public AudioClipLoadType iPhoneClipLoadType = AudioClipLoadType.CompressedInMemory;

                public bool IsMatch(string name)
                {
                    return Regex.IsMatch(name, FileFilter);
                }
            }

            public List<AudioClipImportData> AudioClipImportDatas = new List<AudioClipImportData>();

            public void Sort()
            {
                AudioClipImportDatas.Sort((a, b) => a.Index.CompareTo(b.Index));
            }

            public int GetNextIndex()
            {
                int next = 0;
                for (int i = 0; i < AudioClipImportDatas.Count; i++)
                {
                    if (AudioClipImportDatas[i].Index >= next)
                    {
                        next = AudioClipImportDatas[i].Index + 1;
                    }
                }
                return next;
            }

            public AudioClipImportData GetRule(int index)
            {
                return AudioClipImportDatas.FirstOrDefault(t => t.Index == index);
            }

            public AudioClipImportData GetRule(string path, string fileName)
            {
                AudioClipImportData rule = null;
                for (int i = 0; i < AudioClipImportDatas.Count; i++)
                {
                    if (path.Equals(AudioClipImportDatas[i].AssetPath))
                    {
                        if (AudioClipImportDatas[i].IsMatch(fileName))
                        {
                            rule = AudioClipImportDatas[i];
                        }
                    }
                }
                return rule;
            }

            public void Delete(int index)
            {
                for (int i = AudioClipImportDatas.Count - 1; i >= 0; i--)
                {
                    if (AudioClipImportDatas[i].Index == index)
                    {
                        AudioClipImportDatas.RemoveAt(i);
                    }
                }
            }

            public void Add(AudioClipImportData data, bool isSort = false)
            {
                AudioClipImportDatas.Add(data);
                if (isSort)
                {
                    Sort();
                }
            }
        }

        private AudioClipImportRule importRule;

        public AudioClipImportRule ImportRule
        {
            get
            {
                if (importRule == null)
                {
                    importRule = AssetDatabase.LoadAssetAtPath<AudioClipImportRule>(AudioClipImportRulePath);
                    if (importRule == null)
                    {
                        importRule = ScriptableObject.CreateInstance<AudioClipImportRule>();
                        AssetDatabase.CreateAsset(importRule, AudioClipImportRulePath);
                        AssetDatabase.SaveAssets();
                    }
                }
                return importRule;
            }
        }

        private static AudioClipImportManager instance;

        public static AudioClipImportManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AudioClipImportManager();
                }
                return instance;
            }
        }

        public static void ReImportAudioClips(AudioClipImportRule.AudioClipImportData data)
        {
            if (data == null) return;
            string[] guids = AssetDatabase.FindAssets("t:AudioClip", new string[] {data.AssetPath});
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                if (string.IsNullOrEmpty(path))
                {
                    continue;
                }
                string dir = path.Remove(path.LastIndexOf('/'));
                if (!dir.Equals(data.AssetPath))
                {
                    continue;
                }
                string fileName = path.Substring(path.LastIndexOf('/') + 1);
                if (!data.IsMatch(fileName))
                {
                    continue;
                }
                AudioImporter audioClipImporter = AssetImporter.GetAtPath(path) as AudioImporter;
                if (null != audioClipImporter)
                {
                    ApplyRulesToAudioClip(audioClipImporter, data);
                }
            }
        }

        public static void AudioClipImport(AudioImporter audioClipImporter)
        {
            if (null == audioClipImporter) return;
            string dir = audioClipImporter.assetPath.Remove(audioClipImporter.assetPath.LastIndexOf('/'));
            string name = audioClipImporter.assetPath.Substring(audioClipImporter.assetPath.LastIndexOf('/') + 1);
            AudioClipImportRule.AudioClipImportData data = Instance.ImportRule.GetRule(dir, name);
            if (data != null)
            {
                ApplyRulesToAudioClip(audioClipImporter, data);
            }
        }

        public static void ApplyRulesToAudioClip(AudioImporter audioClipImporter,
            AudioClipImportRule.AudioClipImportData data)
        {
            if (null == audioClipImporter) return;
            audioClipImporter.forceToMono = data.IsForceToMono;
            audioClipImporter.loadInBackground = data.IsLoadInBackground;
            audioClipImporter.ambisonic = data.IsAmbisonic;
            audioClipImporter.preloadAudioData = data.IsPreloadAudioClip;

            AudioImporterSampleSettings settingAndroid = audioClipImporter.GetOverrideSampleSettings("Android");
            settingAndroid.compressionFormat = data.AndroidCompressionFormat;
            settingAndroid.loadType = data.AndroidAudioClipLoadType;
            settingAndroid.quality = data.Quality;
            audioClipImporter.SetOverrideSampleSettings("Android", settingAndroid);

            AudioImporterSampleSettings iPhoneAndroid = audioClipImporter.GetOverrideSampleSettings("iOS");
            iPhoneAndroid.compressionFormat = data.AndroidCompressionFormat;
            iPhoneAndroid.loadType = data.AndroidAudioClipLoadType;
            iPhoneAndroid.quality = data.Quality;
            audioClipImporter.SetOverrideSampleSettings("iOS", iPhoneAndroid);

            audioClipImporter.SaveAndReimport();
        }
    }
}