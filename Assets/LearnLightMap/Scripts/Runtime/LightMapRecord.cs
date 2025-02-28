using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace LearnLightMap.Scripts.Runtime
{
    /// <summary>
    /// 单个光照贴图信息
    /// </summary>
    [Serializable]
    public struct SingleLightMapRecord
    {
        public int meshRendererIndex;

        public Vector4 lightmapTilingOffset;

        public int lightmapIndex;
    }

    public class LightMapRecord : MonoBehaviour
    {
        /// <summary>
        /// 光照贴图信息
        /// </summary>
        public SingleLightMapRecord[] lightMapRecords;
        /// <summary>
        /// 光照贴图资源
        /// </summary>
        public Texture2D[] lightingColorTexture2Ds;      
        /// <summary>
        /// 光照贴图资源
        /// </summary>
        public Texture2D[] lightingDirTexture2Ds;
        
        // Start is called before the first frame update
        void Start()
        {
            if (lightMapRecords != null)
            {
                int index = 0;
                foreach (var meshRenderer in gameObject.GetComponentsInChildren<MeshRenderer>())
                {
                    SingleLightMapRecord lightMapRecord = lightMapRecords[index];
                    meshRenderer.lightmapIndex = lightMapRecord.lightmapIndex;
                    meshRenderer.lightmapScaleOffset = lightMapRecord.lightmapTilingOffset;
                    Debug.Log($"meshRenderer:{meshRenderer}--sharedMaterial:{meshRenderer.sharedMaterial}--shader:{meshRenderer.sharedMaterial.shader}");
                    string keywords = string.Join(";", meshRenderer.sharedMaterial.shaderKeywords);
                    Debug.Log($"keywords:{keywords} isLightMapOn_Global:{Shader.IsKeywordEnabled(ShaderKeywordStrings.LIGHTMAP_ON)} isLightMapOn_Local:{meshRenderer.sharedMaterial.IsKeywordEnabled(ShaderKeywordStrings.LIGHTMAP_ON)}");
                    index++;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
