using System.Collections;
using System.Collections.Generic;
using BuildModule.Scripts.Runtime.AssetManager;
using UnityEngine;

namespace BuildModule.Scripts.Runtime
{
    public class BuildMain : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(LoadCube1());
            StartCoroutine(LoadCube2());
        }

        IEnumerator LoadCube1()
        {
            AssetLoadResult<GameObject> assetLoadResult = new AssetLoadResult<GameObject>();
            yield return AssetFactory.Instance.AssetLoad.AsyLoadAsset("BuildModule",
                "assets.buildmodule.dependency1.assetbundle", "Cube1", ExtensionName.Prefab, assetLoadResult);
            GameObject cube = assetLoadResult.AssetObject;
            if (!cube) yield break;
            GameObject cubeIns = GameObject.Instantiate<GameObject>(cube);
        }

        IEnumerator LoadCube2()
        {
            AssetLoadResult<GameObject> assetLoadResult = new AssetLoadResult<GameObject>();
            yield return AssetFactory.Instance.AssetLoad.AsyLoadAsset("BuildModule",
                "assets.buildmodule.dependency2.assetbundle", "Cube2", ExtensionName.Prefab, assetLoadResult);
            GameObject cube = assetLoadResult.AssetObject;
            if (!cube) yield break;
            GameObject cubeIns = GameObject.Instantiate<GameObject>(cube);
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}