using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BaseLib.Scripts.Runtime
{
    public class MonoUtility : MonoBehaviour
    {
        private static MonoUtility _instance;

        public static MonoUtility Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = new GameObject(nameof(MonoUtility)).AddComponent<MonoUtility>();
                    Object.DontDestroyOnLoad(_instance.gameObject);
                }

                return _instance;
            }
        }

        public event Action UpdateLoopEvent;

        private void Update()
        {
            UpdateLoopEvent?.Invoke();
        }
    }
}