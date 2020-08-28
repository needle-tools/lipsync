using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace LipSync
{
    [ExecuteInEditMode]
    public class EyeAnimator : MonoBehaviour, IOverrideableEyeIndex
    {
        [Header("Rendering")]
        [SerializeField]
        public Renderer _renderer;
        public string EyesPropertyName = "_EyeIndex";
        
        [Header("Settings")]
        [FormerlySerializedAs("HeadTransform")]
        public Transform EyePosition;

        public Transform LookAtTarget;
        public EyeDirectionIndexMap IndexMap;
        public float MinTimeBetweenUpdates = .1f;

        [Header("Sensitivity")] [Range(0, 1f)] public float Horizontal = .5f;
        [Range(0, 1f)] public float Vertical = .5f;

        [Header("AutoBlink")] public bool UseAutoBlink = true;
        public BlinkSettings BlinkSettings;
        private int autoBlinkIndex = -1;

        [FormerlySerializedAs("OverrideIndex")]
        [Header("Override")] 
        [SerializeField]
        private int _overrideIndex = -1;

        public int OverrideEyeIndex
        {
            get => autoBlinkIndex >= 0 ? autoBlinkIndex : _overrideIndex;
            set => _overrideIndex = value;
        }

        // [Header("Optional")]
        // public float HeadForwardOffset;

        [Header("Gizmos")] public Color GizmoColor = Color.yellow;
        public float Scale = 0.003f;

        private float lastUpdateTime = -100;
        private MaterialPropertyBlock block;

        private void OnValidate()
        {
            if (!_renderer) _renderer = GetComponentInChildren<SkinnedMeshRenderer>();
        }

        private void OnEnable()
        {
            if (!EyePosition) Debug.LogWarning("Missing HeadTransform: " + this, this);
            if (!LookAtTarget) Debug.LogWarning("Missing LookAt target: " + this, this);
            if (!IndexMap) Debug.LogWarning("Missing Eye Dir Index Map: " + this, this);
        }

        public Vector3 lastLookDir { get; private set; }

        private void Update()
        {
            if (!_renderer) return;

            if (block == null) block = new MaterialPropertyBlock();
            _renderer.GetPropertyBlock(block);

            UpdateAutoBlink();
            
            if (OverrideEyeIndex >= 0)
            {
                block.SetInt(EyesPropertyName, OverrideEyeIndex);
            }
            else
            {
                if (Application.isPlaying && Time.time - lastUpdateTime < MinTimeBetweenUpdates) return;
                lastUpdateTime = Time.time;
                if (!EyePosition || !LookAtTarget || !IndexMap) return;
                var rot = EyePosition.rotation;
                var dir = LookAtTarget.position - GetEyePos();
                dir = Quaternion.Inverse(rot) * dir;
                // if (dir.z > -.5f)
                {
                    var lookDir = dir.normalized;
                    lastLookDir = lookDir;
                    var index = IndexMap.GetIndex(lookDir, Horizontal, Vertical);
                    block.SetInt(EyesPropertyName, index);
                }
            }

            _renderer.SetPropertyBlock(block);
        }

        private Vector3 GetEyePos()
        {
            if (!EyePosition) return transform.position;

            var eyePos = EyePosition.position;
            // if (HeadForwardOffset != 0) eyePos += EyePosition.forward * HeadForwardOffset;
            return eyePos;
        }

        private void OnDrawGizmos()
        {
            if (!EyePosition || !LookAtTarget) return;
            Gizmos.color = GizmoColor;
            var tp = LookAtTarget.position;
            Gizmos.DrawLine(GetEyePos(), tp);
            Gizmos.DrawSphere(tp, Scale);
        }

        private float nextBlinkTime = 0;
        private float blinkEndTime;
        private BlinkData currentBlinkData;
        private void UpdateAutoBlink()
        {
            autoBlinkIndex = -1;

            if (Application.isPlaying == false) return;
            
            if (!UseAutoBlink || !BlinkSettings)
                return;

            // fallback if maxtime is too big
            if (nextBlinkTime > Time.time + BlinkSettings.MaxTimeBetweenBlink)
                nextBlinkTime = Time.time;

            if (Time.time > nextBlinkTime)
            {
                nextBlinkTime = Time.time + Mathf.Lerp( BlinkSettings.MinTimeBetweenBlink, BlinkSettings.MaxTimeBetweenBlink, Random.value);
                currentBlinkData = BlinkSettings.GetRandom();
                if (currentBlinkData != null)
                {
                    blinkEndTime = Time.time + Mathf.Lerp(currentBlinkData.MinDuration, currentBlinkData.MaxDuration, Random.value);
                }
            }

            if (currentBlinkData == null) return;
            if (Time.time < blinkEndTime) autoBlinkIndex = currentBlinkData.EyeIndex;
        }
    }
}