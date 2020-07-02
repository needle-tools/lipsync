using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace LipSync
{
    [ExecuteAlways]
    public class LipAnimator : MonoBehaviour
    {
        [Header("LipSync")]
        public LipSyncData LipData;
        public MouthShapeIndexMap MouthMap;
        public PlayableDirector Director;
        public bool StrictMode = false;
        
        [Header("Rendering")]
        public Renderer Renderer;
        public string MouthPropertyName = "_MouthIndex";

        private int currentIndex;
        private MaterialPropertyBlock block;
        public AudioSource audioSource;

        private readonly Dictionary<PlayableAsset, IEnumerable<AudioTrack>> tracksCache = new Dictionary<PlayableAsset, IEnumerable<AudioTrack>>();

        private TimelineClip FindActiveClip(out double currentTimeInClip, [CanBeNull] AudioClip clip)
        {
            if (Director.playableAsset == null)
            {
                currentTimeInClip = -1;
                return null;
            }
            
            if (Director.playableAsset != null && Director.playableAsset is TimelineAsset timelineAsset && !tracksCache.ContainsKey(Director.playableAsset))
            {
                var outputTracks = timelineAsset.GetOutputTracks();
                var audioTracks = outputTracks
                    .Where(x => x is AudioTrack t && Director.GetGenericBinding(x) == audioSource)
                    .Cast<AudioTrack>();
                tracksCache.Add(Director.playableAsset, audioTracks);
            }

            var tracks = tracksCache[Director.playableAsset];

            // now we have all tracks affecting our source,
            // lets figure out which clip is active
            // and which time it's at.
            // (we assume only the first track holds data relevant to voice - the others could e.g. be noises, footsteps, ...)
            var activeTrack = tracks.FirstOrDefault();
            if (activeTrack != null)
            {
                var time = Director.time;
                var activeClip = activeTrack.GetClips().FirstOrDefault(x => x.start <= time && x.end >= time);
            
                // got a clip. time is based on start and clipIn property (might be cut off in the beginning)
                if(activeClip != null) 
                {
                    if (clip && activeClip.asset && activeClip.asset is AudioPlayableAsset audioAsset)
                    {
                        currentTimeInClip = (Director.time - activeClip.start) + activeClip.clipIn;
                        
                        // check if the clip is the same OR the clip name and the clip samples match
                        if (audioAsset.clip == clip)
                        {
                            // this is what we can base the mouth animation off.
                            return activeClip;
                        }
                        if (!StrictMode && clip != null && audioAsset.clip.samples == clip.samples && (audioAsset.clip.name.Contains(clip.name) || clip.name.Contains(audioAsset.clip.name)))
                        {
                            return activeClip;
                        }
                    }
                    
                }
            }

            currentTimeInClip = -1;
            return null;
        }


        // Update is called once per frame
        private void Update()
        {
            if(!LipData) return;
            if(!MouthMap) return;
            if(!Renderer) return;
            if(!Director.playableGraph.IsValid()) return;

            var activeClip = FindActiveClip(out var time, LipData.CorrespondingClip);
            if (activeClip == null) return;
            Update((float)time);
        }

        public int lastIndex { get; private set; }

        private void Update(double time)
        {
            if(block == null) block = new MaterialPropertyBlock();
            
            // time += TimeOffset;
            var val = LipData.GetValue(time, ref currentIndex);
            lastIndex = MouthMap.GetIndex(val);
            block.SetInt(MouthPropertyName, lastIndex);
            Renderer.SetPropertyBlock(block);
        }
        
        
        [ContextMenu(nameof(FindActiveClip))]
        private void FindActiveClip() 
        {
            var c = FindActiveClip(out var currentClipTime, LipData ? LipData.CorrespondingClip : null);
            if(c != null)
                Debug.Log("active clip: " + (c?.displayName ?? "null") + " @ " + currentClipTime);
            else
                Debug.Log("no active clip");
        }

    }
}
