using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace LearnLightMap.Scripts.Runtime
{
    public class UV2Record : MonoBehaviour
    {
        /// <summary>
        /// 顶点
        /// </summary>
        public Vector3[] vertices;

        public Vector2[] uv;
        public Vector3[] normals;
        public Vector4[] tangents;
        public int[] triangles;

        /// <summary>
        /// uv2
        /// </summary>
        public Vector2[] uv2;
    }
}