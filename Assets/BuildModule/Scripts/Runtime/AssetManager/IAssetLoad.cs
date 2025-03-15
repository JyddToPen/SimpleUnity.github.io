using System.Collections;

namespace BuildModule.Scripts.Runtime.AssetManager
{
    public class AssetLoadResult<T>
        where T : UnityEngine.Object
    {
        public T AssetObject { get; set; }
    }

    public interface IAssetLoad
    {
        /// <summary>
        /// 异步加载
        /// </summary>
        /// <param name="rootFolder"></param>
        /// <param name="assetBundleName"></param>
        /// <param name="assetName"></param>
        /// <param name="extension"></param>
        /// <param name="assetLoadResult"></param>
        /// <returns></returns>
        IEnumerator AsyLoadAsset<T>(string rootFolder, string assetBundleName, string assetName, string extension,
            AssetLoadResult<T> assetLoadResult)
            where T : UnityEngine.Object;
    }
}