using System.Collections.Generic;
using LearnLightMap.Scripts.Runtime;
using UnityEditor;
using UnityEngine;

namespace LearnLightMap.Scripts.Editor
{
    [CustomEditor(typeof(LightMapRecord))]
    public class LightMapRecordEditor : UnityEditor.Editor
    {
        private LightMapRecord _lightMapRecord;

        private void OnEnable()
        {
            _lightMapRecord = (LightMapRecord)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("记录光照数据"))
            {
                MeshRenderer[] meshRenderers = _lightMapRecord.gameObject.GetComponentsInChildren<MeshRenderer>();
                List<SingleLightMapRecord> lightMapRecords = new List<SingleLightMapRecord>();
                int index = 0;
                foreach (var meshRenderer in meshRenderers)
                {
                    lightMapRecords.Add(new SingleLightMapRecord()
                    {
                        lightmapIndex = meshRenderer.lightmapIndex,
                        lightmapTilingOffset = meshRenderer.lightmapScaleOffset,
                        meshRendererIndex = index++
                    });
                }
                _lightMapRecord.lightMapRecords = lightMapRecords.ToArray();
            }
        }
    }
}
