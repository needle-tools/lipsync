using System.IO;
using LipSync;
using UnityEngine;
#if !UNITY_2020_2_OR_NEWER
using UnityEditor.Experimental.AssetImporters;
#else
using UnityEditor.AssetImporters;
#endif

namespace Editor.LipSync
{
    [ScriptedImporter(1, "rhubarb")]
    public class RhubarbImporter : ScriptedImporter
    {
        public AudioClip correspondingClip;

        public override void OnImportAsset(AssetImportContext ctx)
        {
            if (ctx == null) return;

            var text = File.ReadAllText(ctx.assetPath);
            var instance = ScriptableObject.CreateInstance<LipSyncData>();
            instance.SetCorrespondingClip(correspondingClip);
            instance.SetText(text);

            ctx.AddObjectToAsset("LipSyncData", instance);
            ctx.SetMainObject(instance);
        }

    }
}