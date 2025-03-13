using System;
using UnityEditor;
using UnityEngine;

namespace LearnAssetProcess.Scripts.Editor
{
    public class ModelImporter : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            foreach (var importedAsset in importedAssets)
            {
                // Debug.Log($"OnPostprocessAllAssets {importedAsset}--{AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(importedAsset)}");
            }
        }

        private void OnPreprocessAsset()
        {
            // Debug.Log($"OnPreprocessAsset {assetPath}--{AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath)}");
        }


        private void OnPostprocessModel(GameObject g)
        {
            // Debug.Log($"OnPostprocessModel {g}");
        }
    }
}