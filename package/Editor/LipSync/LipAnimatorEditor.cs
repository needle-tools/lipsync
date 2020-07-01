using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Modules.LipSync
{
    [CustomEditor(typeof(LipAnimator))]
    public class LipAnimatorEditor : Editor
    {
        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            var t = target as LipAnimator;

            using(new EditorGUI.DisabledScope(true)) {
                EditorGUILayout.IntField("Current Index", t.lastIndex);
            }
        }
    }
}