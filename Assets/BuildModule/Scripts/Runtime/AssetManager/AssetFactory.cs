namespace BuildModule.Scripts.Runtime.AssetManager
{
    public class AssetFactory
    {
        private IAssetLoad _assetLoad;

        public IAssetLoad AssetLoad
        {
            get
            {
                if (_assetLoad != null) return _assetLoad;
                if (EnvConfig.Instance.isAb)
                {
                    _assetLoad = new AssetBundleLoad();
                    return _assetLoad;
                }
#if UNITY_EDITOR
                _assetLoad = new EditorLoad();
#else
                _assetLoad = new ResourcesLoad();
#endif

                return _assetLoad;
            }
        }

        public static readonly AssetFactory Instance = new AssetFactory();
    }
}