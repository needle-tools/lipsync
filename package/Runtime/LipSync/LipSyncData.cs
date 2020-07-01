using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Modules.LipSync
{
    [CreateAssetMenu(menuName = "LipSyncData", fileName = "LipSyncData", order = 0)]
    [System.Serializable]
    public class LipSyncData : ScriptableObject
    {
        [SerializeField]
        private AudioClip correspondingClip;

        [SerializeField]
        private string filename;
        
        [SerializeField]
        private Data data;

        public string FileName => filename;
        public AudioClip CorrespondingClip => correspondingClip;

        public string GetValue(double time, ref int currentIndex)
        {
            if (currentIndex >= data.mouthCues.Count | currentIndex < 0)
                currentIndex = 0;
            
            var currentCue = data.mouthCues[currentIndex];
            if (currentCue.start > time)
            {
                for (var i = currentIndex; i >= 0; i--)
                {
                    var cue = data.mouthCues[i];
                    if (cue.start <= time && cue.end > time)
                    {
                        currentIndex = i;
                        return cue.value;
                    }
                }
            }
            else
            {
                for (var i = currentIndex; i <= data.mouthCues.Count; i++)
                {
                    var cue = data.mouthCues[i];
                    if (cue.start <= time && cue.end > time)
                    {
                        currentIndex = i;
                        return cue.value;
                    }
                }
            }
            
            return null;
        }
        
        #if UNITY_EDITOR
        public void SetText(string txt)
        {
            this.text = txt;
            Process();
        }

        public void SetCorrespondingClip(AudioClip clip) 
        {
            this.correspondingClip = clip;
        }
        
        [SerializeField, Multiline(5)]
        private string text;

        private void OnValidate()
        {
            Process();
        }

        private void Process()
        {
            data = JsonUtility.FromJson<Data>(text);
            filename = data.metadata.fileName;
        }
        #endif
    }

    [System.Serializable]
    public class Data
    {
        [SerializeField]
        public Meta metadata;
        
        [SerializeField]
        public List<MouthCue> mouthCues;
    }

    [Serializable]
    public class Meta
    {
        public string soundFile;

        public string fileName => Path.GetFileNameWithoutExtension(soundFile);
    }

    [System.Serializable]
    public class MouthCue
    {
        public float start, end;
        public string value;
    }
}