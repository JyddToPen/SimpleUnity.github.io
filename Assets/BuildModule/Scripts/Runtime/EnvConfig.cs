using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BuildModule.Scripts.Runtime
{
    [CreateAssetMenu(fileName = EnvConfig.EnvConfigPath)]
    public class EnvConfig : ScriptableObject
    {
        private const string EnvConfigPath = "Assets/Resources/BuildModule/EnvConfig.asset";

        public bool isAb;

        /// <summary>
        /// 运行时平台信息
        /// </summary>
        public RuntimePlatform[] runtimeTargets;

        /// <summary>
        /// 运行时平台名
        /// </summary>
        public string[] runtimeTargetNames;

        /// <summary>
        /// 平台名映射
        /// </summary>
        private Dictionary<RuntimePlatform, string> _runtimePlatformMaps;

        public Dictionary<RuntimePlatform, string> RuntimePlatformMaps
        {
            get
            {
                if (_runtimePlatformMaps != null)
                {
                    return _runtimePlatformMaps;
                }

                _runtimePlatformMaps = new Dictionary<RuntimePlatform, string>();
                if (runtimeTargets is { Length: > 0 })
                {
                    for (int i = 0; i < runtimeTargets.Length; i++)
                    {
                        _runtimePlatformMaps.TryAdd(runtimeTargets[i], runtimeTargetNames[i]);
                    }
                }

                return _runtimePlatformMaps;
            }
        }

        private static EnvConfig _instance;

        public static EnvConfig Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = Resources.Load<EnvConfig>(EnvConfigPath.Replace("Assets/Resources/", "")
                        .Replace(".asset", ""));
                    if (!_instance)
                    {
                        _instance = CreateInstance<EnvConfig>();
#if UNITY_EDITOR
                        string folder = Path.GetDirectoryName(EnvConfigPath);
                        if (!string.IsNullOrEmpty(folder))
                        {
                            Directory.CreateDirectory($"{Application.dataPath}/../{folder}");
                        }

                        UnityEditor.AssetDatabase.CreateAsset(_instance, EnvConfigPath);
#endif
                    }
                }

                return _instance;
            }
        }
    }
}