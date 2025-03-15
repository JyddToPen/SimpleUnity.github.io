using System.Collections.Generic;
using System.Linq;
using LearnLightMap.Scripts.Runtime;
using UnityEditor;
using UnityEngine;

namespace LearnLightMap.Scripts.Editor
{
    [CustomEditor(typeof(LightMapRecord))]
    public class LightMapRecordEditor : UnityEditor.Editor
    {
        private LightMapRecord _lightMapRecord;
        private GameObject _bakeObject;

        private GameObject BakeObject
        {
            get
            {
                if (!_bakeObject)
                {
                    _bakeObject = GameObject.Find(_lightMapRecord.gameObject.name);
                    if (!_bakeObject)
                    {
                        _bakeObject = Object.Instantiate(_lightMapRecord.gameObject);
                        _bakeObject.name = _lightMapRecord.gameObject.name;
                    }
                }

                return _bakeObject;
            }
        }

        private void OnEnable()
        {
            _lightMapRecord = (LightMapRecord)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("烘焙光照贴图"))
            {
                Bake();
            }
        }

        /// <summary>
        /// 开始烘焙
        /// </summary>
        private void Bake()
        {
            foreach (var transform in BakeObject.GetComponentsInChildren<Transform>())
            {
                transform.gameObject.isStatic = true;
            }

            Lightmapping.bakeCompleted -= OnBakeLightMapComplete;
            Lightmapping.bakeCompleted += OnBakeLightMapComplete;
            Lightmapping.BakeAsync();
        }

        /// <summary>
        /// 生成光照贴图配置
        /// </summary>
        private void OnBakeLightMapComplete()
        {
            if (!_lightMapRecord || !BakeObject)
            {
                return;
            }

            List<SingleLightMapRecord> lightingMapInfos = new List<SingleLightMapRecord>();
            int meshIndex = -1;
            foreach (var meshRenderer in BakeObject.GetComponentsInChildren<MeshRenderer>())
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
            if (Lightmapping.lightingDataAsset)
            {
                _lightMapRecord.lightingColorTexture2Ds =
                    LightmapSettings.lightmaps.Select(item => item.lightmapColor).ToArray();
                _lightMapRecord.lightingDirTexture2Ds =
                    LightmapSettings.lightmaps.Select(item => item.lightmapDir).ToArray();
            }

            Object.DestroyImmediate(_bakeObject);
            PrefabUtility.SaveAsPrefabAsset(_lightMapRecord.gameObject,
                AssetDatabase.GetAssetPath(_lightMapRecord.gameObject));
        }
    }
}