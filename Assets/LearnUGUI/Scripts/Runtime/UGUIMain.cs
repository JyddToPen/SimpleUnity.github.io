using System;
using System.Collections;
using BuildModule.Scripts.Runtime.AssetManager;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace LearnUGUI.Scripts.Runtime
{
    public class UGUIMain : MonoBehaviour
    {
        private IEnumerator Start()
        {
            yield break;
        }

        private void OnGUI()
        {
            GUIStyle buttonGUIStyle = new GUIStyle()
            {
                fontSize = 32
            };
        }
    }
}