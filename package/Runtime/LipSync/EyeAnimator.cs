using System;
using UnityEngine;
using UnityEngine.Serialization;

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
        

        [Header("Override")] public int OverrideIndex = -1;

        public int OverrideEyeIndex
        {
            get => OverrideIndex;
            set => OverrideIndex = value;
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


        private void Update()
        {
            if (!_renderer) return;

            if (block == null) block = new MaterialPropertyBlock();
            _renderer.GetPropertyBlock(block);
            
            if (OverrideIndex >= 0)
            {
                block.SetInt(EyesPropertyName, OverrideIndex);
            }
            else
            {
                if (Application.isPlaying && Time.time - lastUpdateTime < MinTimeBetweenUpdates) return;
                lastUpdateTime = Time.time;
                if (!EyePosition || !LookAtTarget || !IndexMap) return;
                var rot = transform.rotation;
                var dir = LookAtTarget.position - GetEyePos();
                dir = Quaternion.Inverse(rot) * dir;
                var lookDir = dir.normalized;
                var index = IndexMap.GetIndex(lookDir, Horizontal, Vertical);
                block.SetInt(EyesPropertyName, index);
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
    }
}