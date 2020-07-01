using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Modules.LipSync
{
    [ExecuteAlways]
    public class LipAnimator : MonoBehaviour
    {
        public LipSyncData LipData;
        public MouthShapeIndexMap MouthMap;
        public Renderer Renderer;
        public string MouthPropertyName = "_MouthIndex";
        public PlayableDirector Director;

        private int currentIndex;
        private MaterialPropertyBlock block;

        public AudioSource audioSource;

        private TimelineClip TimelineMagic(out double currentTimeInClip) {
            
            var audioTracks = ((TimelineAsset) Director.playableAsset)
                .GetOutputTracks()
                .Where(x => x is AudioTrack && Director.GetGenericBinding(x) == audioSource)
                .Cast<AudioTrack>();

            // now we have all tracks affecting our source,
            // lets figure out which clip is active
            // and which time it's at.
            // (we assume only the first track holds data relevant to voice - the others could e.g. be noises, footsteps, ...)
            var activeTrack = audioTracks.FirstOrDefault();
            activeClip = activeTrack
                .GetClips()
                .Where(x => x.start <= Director.time && x.end >= Director.time)
                .FirstOrDefault();
            
            // got a clip. time is based on start and clipIn property (might be cut off in the beginning)
            if(activeClip != null) {
                currentTimeInClip = (Director.time - activeClip.start) + activeClip.clipIn;

                // this is what we can base the mouth animation off.
                return activeClip;
            }
            else {
                currentTimeInClip = 0;
                return null;
            }
        }

        TimelineClip activeClip;

        // Update is called once per frame
        private void Update()
        {
            if(!LipData) return;
            if(!MouthMap) return;
            if(!Renderer) return;
            if(!Director.playableGraph.IsValid()) return;
            

            activeClip = TimelineMagic(out double time);

            if(activeClip != null)
            {
                if(((activeClip.asset) as AudioPlayableAsset).clip != LipData.CorrespondingClip) {
                    // TODO we could find the right LipData here that corresponds to the currently playing audio clip.
                    Debug.LogError("Playing a different clip than assigned in the LipData. This will result in incorrect visuals.");
                }

                Update((float)time);
            }
        }

        [HideInInspector]
        public int lastIndex;

        private void Update(float time)
        {
            if(block == null) block = new MaterialPropertyBlock();
            
            // time += TimeOffset;
            var val = LipData.GetValue(time, ref currentIndex);
            var index = MouthMap.GetIndex(val);
            block.SetInt(MouthPropertyName, index);
            Renderer.SetPropertyBlock(block);
        }
        
        
        [ContextMenu("Timeline Magic")]
        private void DoTimelineMagic() {
            var c = TimelineMagic(out var currentClipTime);
            if(c != null)
                Debug.Log("active clip: " + (activeClip?.displayName ?? "null") + " @ " + currentClipTime);
            else
                Debug.Log("no active clip");
        }

    }
}
