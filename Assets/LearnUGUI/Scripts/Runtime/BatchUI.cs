using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace LearnUGUI.Scripts.Runtime
{
    public class BatchUI : MonoBehaviour
    {
        private RawImage _batchIcon;

        private RenderTexture _renderTexture;

        private Bounds _wholeBounds;
        private Camera _uiCamera;

        // Start is called before the first frame update
        void Start()
        {
            _batchIcon = gameObject.AddComponent<RawImage>();
            _batchIcon.enabled = false;
            _uiCamera = GameObject.Find("UICamera").GetComponent<Camera>();
        }

        /// <summary>
        /// 捕获信息
        /// </summary>
        public void CaptureDrawing()
        {
            _wholeBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(transform);
            GraphicsFormat graphicsFormat =
                SystemInfo.IsFormatSupported(GraphicsFormat.RGBA_ASTC6X6_SRGB, FormatUsage.Render)
                    ? GraphicsFormat.RGBA_ASTC6X6_SRGB
                    : GraphicsFormat.R8G8B8A8_UNorm;
            _renderTexture = RenderTexture.GetTemporary((int)_wholeBounds.size.x, (int)_wholeBounds.size.y, 0,
                graphicsFormat
            );
            _batchIcon.texture = _renderTexture;
            _batchIcon.SetNativeSize();
            _batchIcon.enabled = true;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
        }


        private void OnGUI()
        {
            if (GUILayout.Button("捕获图像"))
            {
                CaptureDrawing();
            }
        }
#endif

        // Update is called once per frame
        void Update()
        {
        }
    }
}