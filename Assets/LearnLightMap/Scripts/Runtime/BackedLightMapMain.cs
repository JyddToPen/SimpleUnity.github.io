using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LearnLightMap.Scripts.Runtime
{
    public class BackedLightMapMain : MonoBehaviour
    {
        private void Awake()
        {
            Object.FindObjectOfType<Light>().enabled = false;
        }

        void Start()
        {
            GameObject bakedArch = Resources.Load<GameObject>("LearnLightMap/Arch");
            if (!bakedArch) return;
            RecoveryLightMap(bakedArch.GetComponent<LightMapRecord>());
            GameObject bakedArchIns = GameObject.Instantiate<GameObject>(bakedArch);
            GameObject realLightArch = Resources.Load<GameObject>("LearnLightMap/ArchLitRealLight");
            if (realLightArch)
            {
                GameObject.Instantiate<GameObject>(realLightArch);
            }
        }

        /// <summary>
        /// 还原光照贴图数据
        /// </summary>
        /// <param name="lightMapRecord"></param>
        private void RecoveryLightMap(LightMapRecord lightMapRecord)
        {
            if (!lightMapRecord || lightMapRecord.lightingColorTexture2Ds is not { Length: > 0 }) return;
            List<LightmapData> lightmapDatas = new List<LightmapData>();
            for (int i = 0; i < lightMapRecord.lightingColorTexture2Ds.Length; i++)
            {
                LightmapData lightmapData = new LightmapData();
                if (lightMapRecord.lightingDirTexture2Ds is { Length: > 0 } &&
                    lightMapRecord.lightingColorTexture2Ds.Length ==
                    lightMapRecord.lightingDirTexture2Ds.Length)
                {
                    lightmapData.lightmapDir = lightMapRecord.lightingDirTexture2Ds[i];
                }

                lightmapData.lightmapColor = lightMapRecord.lightingColorTexture2Ds[i];

                lightmapDatas.Add(lightmapData);
            }

            LightmapSettings.lightmaps = lightmapDatas.ToArray();
            foreach (var lightmapData in LightmapSettings.lightmaps)
            {
                Debug.Log($"lightmapData lightmapColor:{lightmapData.lightmapColor}");
            }
#if UNITY_EDITOR
            // UnityEditor.Lightmapping.lightingDataAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEditor.LightingDataAsset>("Assets/Resources/LearnLightMap/Tex/LightingData.asset");
#endif
        }
    }
}