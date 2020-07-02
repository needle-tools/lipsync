using LipSync;
using UnityEditor;

namespace Editor.LipSync
{
    [CustomEditor(typeof(LipAnimator))]
    public class LipAnimatorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            var t = target as LipAnimator;

            using(new EditorGUI.DisabledScope(true))
            {
                if (t != null) EditorGUILayout.IntField("Current Index", t.lastIndex);
            }
        }
    }
}