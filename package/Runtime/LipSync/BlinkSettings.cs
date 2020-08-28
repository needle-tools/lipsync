using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace LipSync
{
    [System.Serializable]
    public class BlinkData
    {
        public int EyeIndex;
        [Range(0, 1)] public float Probability = 1;
            
        public float MinDuration = .05f;
        public float MaxDuration = .1f;
    }
    
    [CreateAssetMenu(menuName = "Lipsync/" + nameof(BlinkSettings), fileName = "BlinkSettings", order = 0)]
    public class BlinkSettings : ScriptableObject
    {
        public float MinTimeBetweenBlink = 1;
        public float MaxTimeBetweenBlink = 3;
        
        [FormerlySerializedAs("Data")] [SerializeField]
        private List<BlinkData> data;

        private float totalWeight = -1;

        #if UNITY_EDITOR
        private void OnValidate()
        {
            CalculateProbability();
        }
        #endif

        private void Awake()
        {
            CalculateProbability();
        }

        public bool HasData()
        {
            if (data == null)
            {
                totalWeight = -1;
                return false;
            }

            return true;
        }

        public void CalculateProbability()
        {
            if (!HasData()) return;

            totalWeight = 0;
            for (var i = 0; i < data.Count; i++)
            {
                var entry = data[i];
                totalWeight += entry.Probability;
            }
        }

        public BlinkData GetRandom()
        {
            if (!HasData()) return null;
            var rnd = Random.value * totalWeight;
            var sum = 0f;
            for (var i = 0; i < data.Count; i++)
            {
                var entry = data[i];
                sum += entry.Probability;
                if (rnd <= sum) return entry;
            }

            return null;
        }
    }
}