using UnityEditor;
using UnityEngine;

namespace LearnLightMap.Scripts.Editor
{
    public class UV2Importer : AssetPostprocessor
    {
        void OnPreprocessModel()
        {
            if (!assetPath.StartsWith("Assets/Resources/Model"))
            {
                return;
            }

            ModelImporter modelImporter = (ModelImporter)assetImporter;
            if (!modelImporter)
            {
                return;
            }

            //TODO:这里需要关闭自动生成
            // modelImporter.generateSecondaryUV = false;
        }

        void OnPostprocessModel(GameObject go)
        {
            if (!assetPath.StartsWith("Assets/Resources/Model"))
            {
                return;
            }

            ModelImporter modelImporter = (ModelImporter)assetImporter;
            if (!modelImporter)
            {
                return;
            }
            
            //TODO:这里实现对顶点的复写
            // MeshFilter meshFilter = _uV2Record.gameObject.GetComponentInChildren<MeshFilter>();
            // meshFilter.sharedMesh.vertices = _uV2Record.vertices?.ToArray();
            // meshFilter.sharedMesh.triangles = _uV2Record.triangles?.ToArray();
            // meshFilter.sharedMesh.uv = _uV2Record.uv?.ToArray();
            // meshFilter.sharedMesh.uv2 = _uV2Record.uv2?.ToArray();
            // meshFilter.sharedMesh.normals = _uV2Record.normals?.ToArray();
            // meshFilter.sharedMesh.tangents = _uV2Record.tangents?.ToArray();
        }
    }
}