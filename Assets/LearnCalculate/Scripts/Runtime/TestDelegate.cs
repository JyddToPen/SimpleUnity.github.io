using System;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace LearnCalculate.Scripts.Runtime
{
    public class TestDelegate
    {
        public event Action UpdateLoopHandle;
        private GUIStyle _normalStyle;

        public void OnGUI()
        {
            GUILayout.BeginVertical("box");
            _normalStyle ??= new GUIStyle()
            {
                richText = true,
                fontSize = 25,
            };
            if (GUILayout.Button("批量添加委托"))
            {
                for (int i = 0; i < 1000; i++)
                {
                    UpdateLoopHandle += OnUpdate;
                }
            }

            if (GUILayout.Button("批量移除委托"))
            {
                for (int i = 0; i < 1000; i++)
                {
                    UpdateLoopHandle -= OnUpdate;
                }
            }

            if (GUILayout.Button("移除单个"))
            {
                UpdateLoopHandle -= OnUpdate;
            }

            if (GUILayout.Button("添加单个"))
            {
                UpdateLoopHandle -= OnUpdate;
            }

            GUILayout.EndVertical();
        }

        private void OnUpdate()
        {
        }
    }
}