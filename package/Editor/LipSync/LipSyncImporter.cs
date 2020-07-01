using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Modules.LipSync
{
    public class LipSyncImporter : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (var imported in importedAssets)
            {
                if (imported.EndsWith(".json"))
                {
                    var text = File.ReadAllText(imported);
                    var data = JsonUtility.FromJson<Data>(text);
                    ProcessData(imported, text, data);
                }
            }
        }

        private static void ProcessData(string path, string text, Data data)
        {
            if (data == null) return;
            if (string.IsNullOrEmpty(data.metadata.fileName)) return;
            var name = data.metadata.fileName;
            var currentAssets = AssetDatabase.FindAssets("t:" + nameof(LipSyncData))
                .Select(l => AssetDatabase.LoadAssetAtPath<LipSyncData>(AssetDatabase.GUIDToAssetPath(l))).Where(a => a.FileName == name).ToList();

            var exists = false;
            foreach (var cur in currentAssets)
            {
                cur.SetText(text);
                EditorUtility.SetDirty(cur);
                exists = true;
                Debug.Log("Updated " + AssetDatabase.GetAssetPath(cur), cur);
            }

            if (exists)
            {
                AssetDatabase.DeleteAsset(path);
                AssetDatabase.SaveAssets();
                return;
            }

            var instance = ScriptableObject.CreateInstance<LipSyncData>();
            instance.SetText(text);
            var instancePath = Path.ChangeExtension(path, ".asset");
            AssetDatabase.CreateAsset(instance, instancePath);
            Debug.Log("Created " + instancePath, instance);
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.SaveAssets();
        }
    }
}
