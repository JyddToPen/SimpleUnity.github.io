using System.Collections.Generic;
using System.IO;
using System.Linq;
using LearnLightMap.Scripts.Runtime;
using UnityEditor;
using UnityEngine;

namespace LearnLightMap.Scripts.Editor
{
    public static class LightMapEditor
    {
        private static LightMapRecord _lightMapRecord;
        private const string LightMapRecordPath = "Assets/Resources/LearnLightMap/Arch.prefab";

        // [MenuItem("Assets/LightMap/Bake")]
        private static void BuildWeChat()
        {
            _lightMapRecord = Object.FindObjectOfType<LightMapRecord>();
            if (!_lightMapRecord)
            {
                var asset = AssetDatabase.LoadAssetAtPath<GameObject>(LightMapRecordPath);
                _lightMapRecord = Object.Instantiate(asset).GetComponent<LightMapRecord>();
            }

            foreach (var transform in _lightMapRecord.gameObject.GetComponentsInChildren<Transform>())
            {
                transform.gameObject.isStatic = true;
            }

            // Lightmapping.ClearDiskCache();
            // Lightmapping.ClearLightingDataAsset();
            Lightmapping.bakeCompleted -= OnBakeLightMapComplete;
            Lightmapping.bakeCompleted += OnBakeLightMapComplete;
            Lightmapping.BakeAsync();
        }

        /// <summary>
        /// 生成光照贴图配置
        /// </summary>
        private static void GenerateLightingMapsConfigs()
        {
            if (!_lightMapRecord)
            {
                return;
            }

            List<SingleLightMapRecord> lightingMapInfos = new List<SingleLightMapRecord>();
            int meshIndex = -1;
            foreach (var meshRenderer in _lightMapRecord.gameObject.GetComponentsInChildren<MeshRenderer>())
            {
                meshIndex++;
                if (!meshRenderer.gameObject.isStatic)
                {
                    continue;
                }

                lightingMapInfos.Add(new SingleLightMapRecord()
                {
                    meshRendererIndex = meshIndex,
                    lightmapIndex = meshRenderer.lightmapIndex,
                    lightmapTilingOffset = meshRenderer.lightmapScaleOffset
                });
            }

            _lightMapRecord.lightMapRecords = lightingMapInfos.ToArray();

            string outputFolder = $"Assets/Resources/LearnLightMap/Tex";
            if (AssetDatabase.IsValidFolder(outputFolder))
            {
                Directory.Delete($"{Application.dataPath}/../{outputFolder}", true);
            }

            Directory.CreateDirectory($"{Application.dataPath}/../{outputFolder}");
            if (Lightmapping.lightingDataAsset)
            {
                string filePath = AssetDatabase.GetAssetPath(Lightmapping.lightingDataAsset);
                string folder = Path.GetDirectoryName(filePath)?.Replace("\\", "/");
                if (string.IsNullOrEmpty(folder))
                {
                    return;
                }

                string[] allLightMapTex = AssetDatabase.FindAssets("*", new string[] { folder })
                    .Select(AssetDatabase.GUIDToAssetPath).ToArray();
                foreach (var lightMapTex in allLightMapTex)
                {
                    string moveTarget = lightMapTex.Replace(folder, outputFolder);
                    AssetDatabase.MoveAsset(lightMapTex, moveTarget);
                }

                //保留场景数据
                _lightMapRecord.lightingColorTexture2Ds =
                    LightmapSettings.lightmaps.Select(item => item.lightmapColor).ToArray();            
                _lightMapRecord.lightingDirTexture2Ds =
                    LightmapSettings.lightmaps.Select(item => item.lightmapDir).ToArray();
                Lightmapping.lightingDataAsset = null;
                // Lightmapping.ClearDiskCache();
                AssetDatabase.DeleteAsset(folder);
                LightmapParameters lightmapParameters =
                    LightmapParameters.GetLightmapParametersForLightingSettings(Lightmapping.lightingSettings);
            }
        }

        /// <summary>
        /// 烘焙结束
        /// </summary>
        private static void OnBakeLightMapComplete()
        {
            GenerateLightingMapsConfigs();
            Lightmapping.bakeCompleted -= OnBakeLightMapComplete;
            PrefabUtility.SaveAsPrefabAsset(_lightMapRecord.gameObject, LightMapRecordPath);
            Object.DestroyImmediate(_lightMapRecord.gameObject);
        }
    }
}