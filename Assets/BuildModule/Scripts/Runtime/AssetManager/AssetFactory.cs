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
                }
                else
                {
                    _assetLoad = new ResourcesLoad();
                }

                return _assetLoad;
            }
        }

        public static readonly AssetFactory Instance = new AssetFactory();
    }
}