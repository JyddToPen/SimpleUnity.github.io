using System.Collections.Generic;
using System.Linq;
using LearnLightMap.Scripts.Runtime;
using UnityEditor;
using UnityEngine;

namespace LearnLightMap.Scripts.Editor
{
    /// <summary>
    /// uv2记录可以解决unity2021产生的uv2数据和unity2022无法兼容的问题
    /// 实现方法便是，如果该项目在unity2021中通过generate LightUv生成uv数据，并且用于烘焙光照贴图，那么当
    /// 项目迁移到unity2022.3时会出现顶点数据错乱的问题，这是因为uv2的计算方式行了改变导致。
    /// </summary>
    [CustomEditor(typeof(UV2Record))]
    public class UV2RecordEditor : UnityEditor.Editor
    {
        private UV2Record _uV2Record;

        private void OnEnable()
        {
            _uV2Record = (UV2Record)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("记录顶点数据"))
            {
                MeshFilter meshFilter = _uV2Record.gameObject.GetComponentInChildren<MeshFilter>();
                _uV2Record.vertices = meshFilter.sharedMesh.vertices?.ToArray();
                _uV2Record.triangles = meshFilter.sharedMesh.triangles?.ToArray();
                _uV2Record.uv2 = meshFilter.sharedMesh.uv2?.ToArray();
                _uV2Record.uv = meshFilter.sharedMesh.uv?.ToArray();
                _uV2Record.normals = meshFilter.sharedMesh.normals?.ToArray();
                _uV2Record.tangents = meshFilter.sharedMesh.tangents?.ToArray();
                PrefabUtility.SavePrefabAsset(_uV2Record.gameObject);
            }

            if (GUILayout.Button("对比顶点数据"))
            {
                CompareVertices();
            }

            if (GUILayout.Button("还原顶点数据"))
            {
                MeshFilter meshFilter = _uV2Record.gameObject.GetComponentInChildren<MeshFilter>();
                meshFilter.sharedMesh.vertices = _uV2Record.vertices?.ToArray();
                meshFilter.sharedMesh.triangles = _uV2Record.triangles?.ToArray();
                meshFilter.sharedMesh.uv = _uV2Record.uv?.ToArray();
                meshFilter.sharedMesh.uv2 = _uV2Record.uv2?.ToArray();
                meshFilter.sharedMesh.normals = _uV2Record.normals?.ToArray();
                meshFilter.sharedMesh.tangents = _uV2Record.tangents?.ToArray();
            }
        }

        private void CompareVertices()
        {
            MeshFilter meshFilter = _uV2Record.gameObject.GetComponentInChildren<MeshFilter>();
            if (meshFilter.sharedMesh.vertices == null || _uV2Record.vertices == null ||
                meshFilter.sharedMesh.vertices.Length != _uV2Record.vertices.Length)
            {
                Debug.LogError(
                    $"Compare Result 1:sharedMesh:{meshFilter.sharedMesh.vertices}--_uV2Record:{_uV2Record.vertices}");
                return;
            }

            for (int i = 0; i < meshFilter.sharedMesh.vertices.Length; i++)
            {
                if (meshFilter.sharedMesh.vertices[i] != _uV2Record.vertices[i])
                {
                    Debug.LogError(
                        $"Compare Result 2: Index:{i} sharedMesh:{meshFilter.sharedMesh.vertices[i]}--_uV2Record:{_uV2Record.vertices[i]}");
                }
            }

            Debug.Log($"Compare Result 3: completely identical");
        }
    }
}