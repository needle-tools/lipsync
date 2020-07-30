using System;
using UnityEngine;

namespace LipSync
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(LipAnimator))]
    public class EyeSheetLookAt : MonoBehaviour
    {
        public Transform HeadTransform;
        public Transform LookAtTarget;
        public EyeDirectionIndexMap IndexMap;

        private LipAnimator lipAnimator;

        private void OnEnable()
        {
            lipAnimator = GetComponent<LipAnimator>();
            if(!HeadTransform) Debug.LogWarning("Missing HeadTransform: " + this, this);
            if(!LookAtTarget) Debug.LogWarning("Missing LookAt target: " + this, this);
            if(!IndexMap) Debug.LogWarning("Missing Eye Dir Index Map: " + this, this);
        }

        private void Update()
        {
            if (!HeadTransform || !LookAtTarget || !IndexMap) return;
            lipAnimator.OverwriteEyeIndex = IndexMap.GetIndex(HeadTransform.position, LookAtTarget.position);
        }
    }
}