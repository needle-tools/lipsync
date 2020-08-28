using LipSync;
using UnityEditor;

namespace Editor.LipSync
{
    [CustomEditor(typeof(EyeAnimator))]
    public class EyeAnimatorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            var t = target as EyeAnimator;

            using(new EditorGUI.DisabledScope(true))
            {
                if (t != null)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Internal", EditorStyles.boldLabel);
                    EditorGUILayout.Vector3Field("LookDir", t.lastLookDir);
                    EditorGUILayout.IntField("EyeIndex", t.OverrideEyeIndex);
                }
            }
        }
    }
}