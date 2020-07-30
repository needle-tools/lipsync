using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace LipSync
{
    [ExecuteAlways]
    public class LipAnimator : MonoBehaviour
    {
        [Header("LipSync")] public LipSyncData LipData;
        public MouthShapeIndexMap MouthMap;
        public PlayableDirector Director;

        [Tooltip("StrictMode=true means AudioClip name in timeline and in LipData MUST match")]
        public bool StrictMode = false;

        [Header("Rendering")] public Renderer Renderer;
        public string MouthPropertyName = "_MouthIndex";
        public string EyesPropertyName = "_EyeIndex";

        [Header("Overwrites")]
        [Range(-1,20), Tooltip("Set to -1 to disable overwrite")]
        public int OverwriteMouthIndex = -1;
        [Range(-1,20), Tooltip("Set to -1 to disable overwrite")]
        public int OverwriteEyeIndex = -1;


        private int currentIndex;
        private MaterialPropertyBlock block;

        private readonly Dictionary<PlayableAsset, IEnumerable<AudioTrack>> tracksCache = new Dictionary<PlayableAsset, IEnumerable<AudioTrack>>();

        #if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Renderer) Renderer = GetComponentInChildren<SkinnedMeshRenderer>();
        }
        #endif

        private void OnEnable()
        {
            
        }

        private void OnDisable()
        {
            if (Renderer && block != null)
            {
                block = null;
                Renderer.SetPropertyBlock(null);
            }
        }
        // UpdateIndices is called once per frame
        private void Update()
        {
            if (!LipData) return;
            if (!MouthMap) return;
            if (!Renderer) return;

            if (!LipData.CorrespondingClip)
            {
                Debug.LogWarning("No clip assigned to lipsync data", LipData);
            }
            else
            {
                FindActiveClipTime(out var time, LipData.CorrespondingClip);
                UpdateIndices((float) time);
            }

        }

        public int lastMouthIndex { get; private set; }
        public int lastEyeIndex { get; private set; }

        private void UpdateIndices(double time)
        {
            if (block == null)
            {
                block = new MaterialPropertyBlock();
                Renderer.GetPropertyBlock(block);
            }

            if (OverwriteMouthIndex < 0)
            {
                var val = LipData.GetValue(time, ref currentIndex);
                if (val != null) 
                    lastMouthIndex = MouthMap.GetIndex(val);
            }
            else
            {
                lastMouthIndex = OverwriteMouthIndex;
            }
            
            block.SetInt(MouthPropertyName, lastMouthIndex);

            
            if (OverwriteEyeIndex >= 0)
            {
                lastEyeIndex = OverwriteEyeIndex;
            }
            
            block.SetInt(EyesPropertyName, lastEyeIndex);
            
            
            Renderer.SetPropertyBlock(block);
        }

          private TimelineClip FindActiveClipTime(out double currentTimeInClip, AudioClip clip)
        {
            if(Director.playableAsset == null)
            {
                currentTimeInClip = -1;
                return null;
            }

            if (Director.playableAsset != null && Director.playableAsset is TimelineAsset timelineAsset && !tracksCache.ContainsKey(Director.playableAsset))
            {
                var outputTracks = timelineAsset.GetOutputTracks();
                var audioTracks = outputTracks
                    .Where(x => x is AudioTrack)
                    .Cast<AudioTrack>();
                tracksCache.Add(Director.playableAsset, audioTracks);
            }

            var tracks = tracksCache[Director.playableAsset];

            // now we have all tracks affecting our source,
            // lets figure out which clip is active
            // and which time it's at.
            // (we assume only the first track holds data relevant to voice - the others could e.g. be noises, footsteps, ...)
            foreach (var track in tracks)
            {
                if (track == null) continue;
                var time = Director.time;
                var activeClip = track.GetClips().FirstOrDefault(x => x.start <= time && x.end >= time);

                // got a clip. time is based on start and clipIn property (might be cut off in the beginning)
                if (activeClip == null) continue;
                if (clip && activeClip.asset && activeClip.asset is AudioPlayableAsset audioAsset)
                {
                    currentTimeInClip = (Director.time - activeClip.start) + activeClip.clipIn;

                    // check if the clip is the same OR the clip name and the clip samples match
                    if (audioAsset.clip == clip)
                    {
                        // this is what we can base the mouth animation off.
                        return activeClip;
                    }

                    if (!StrictMode && clip != null && audioAsset.clip.samples == clip.samples &&
                        (audioAsset.clip.name.Contains(clip.name) || clip.name.Contains(audioAsset.clip.name)))
                    {
                        return activeClip;
                    }
                }
            }


            currentTimeInClip = -1;
            return null;
        }

        [ContextMenu(nameof(FindActiveClipTime))]
        private void FindActiveClip()
        {
            var c = FindActiveClipTime(out var currentClipTime, LipData ? LipData.CorrespondingClip : null);
            if (c != null)
                Debug.Log("active clip: " + (c?.displayName ?? "null") + " @ " + currentClipTime);
            else
                Debug.Log("no active clip");
        }
    }
}