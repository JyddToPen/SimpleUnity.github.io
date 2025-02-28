using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.UI;

namespace LearnCrash.Scripts.Runtime
{
    public class CrashMain:MonoBehaviour
    {
        public Button btnCrash;

        private void Awake()
        {
            btnCrash.onClick.AddListener(OnClickCrash);
        }

        private void OnClickCrash()
        {
            UnityEngine.Debug.LogFormat($"<color=yellow>ForceCrash</color>");
            Utils.ForceCrash(ForcedCrashCategory.FatalError);
        }
    }
}