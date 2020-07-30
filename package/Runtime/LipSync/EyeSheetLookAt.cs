using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace LipSync
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(LipAnimator))]
    public class EyeSheetLookAt : MonoBehaviour
    {
        [FormerlySerializedAs("HeadTransform")] public Transform EyePosition;
        public Transform LookAtTarget;
        public EyeDirectionIndexMap IndexMap;
        
        // [Header("Optional")]
        // public float HeadForwardOffset;

        [Header("Gizmos")] public Color GizmoColor = Color.yellow;
        public float Scale = 0.01f;

        private LipAnimator lipAnimator;

        private void OnEnable()
        {
            lipAnimator = GetComponent<LipAnimator>();
            if(!EyePosition) Debug.LogWarning("Missing HeadTransform: " + this, this);
            if(!LookAtTarget) Debug.LogWarning("Missing LookAt target: " + this, this);
            if(!IndexMap) Debug.LogWarning("Missing Eye Dir Index Map: " + this, this);
        }

        private void Update()
        {
            if (!EyePosition || !LookAtTarget || !IndexMap) return;
            lipAnimator.OverwriteEyeIndex = IndexMap.GetIndex(GetEyePos(), LookAtTarget.position);
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