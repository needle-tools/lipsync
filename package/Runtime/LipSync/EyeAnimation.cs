using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace LipSync
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(LipAnimator))]
    public class EyeAnimation : MonoBehaviour, IOverrideableEyeIndex
    {
        [FormerlySerializedAs("HeadTransform")] public Transform EyePosition;
        public Transform LookAtTarget;
        public EyeDirectionIndexMap IndexMap;
        public float MinTimeBetweenUpdates = .1f;
        
        [Header("Override")]
        public int OverrideIndex = -1;

        public int OverrideEyeIndex
        {
            get => OverrideIndex;
            set => OverrideIndex = value;
        }
        
        // [Header("Optional")]
        // public float HeadForwardOffset;

        [Header("Gizmos")] public Color GizmoColor = Color.yellow;
        public float Scale = 0.003f;

        private LipAnimator lipAnimator;
        private float lastUpdateTime = -100;

        private void OnEnable()
        {
            lipAnimator = GetComponent<LipAnimator>();
            if (!EyePosition) Debug.LogWarning("Missing HeadTransform: " + this, this);
            if(!LookAtTarget) Debug.LogWarning("Missing LookAt target: " + this, this);
            if(!IndexMap) Debug.LogWarning("Missing Eye Dir Index Map: " + this, this);
        }

        
        private void Update()
        {
            if (OverrideIndex >= 0)
            {
                lipAnimator.OverrideEyeIndex = OverrideIndex;
                return;
            }
            
            if (Time.time - lastUpdateTime < MinTimeBetweenUpdates) return;
            lastUpdateTime = Time.time;
            if (!EyePosition || !LookAtTarget || !IndexMap) return;
            lipAnimator.OverrideEyeIndex = IndexMap.GetIndex(GetEyePos(), LookAtTarget.position);
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