using System;
using System.IO;
using System.Linq;
using System.Reflection;
using BaseLib.Scripts.Runtime.URP;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using Object = System.Object;

namespace BaseLib.Scripts.Editor
{
    internal class TemplateGenerator : EditorWindow
    {
        private string _templateTitle;
        private string _templateRealName;
        private const string LearnName = "Learn";
        private const string TemplateName = "Template";

        [MenuItem("Tools/Template Generator")]
        private static void Generate()
        {
            EditorWindow.GetWindow<TemplateGenerator>();
        }

        private void OnGUI()
        {
            _templateTitle = EditorGUILayout.TextField("标题", _templateTitle);
            if (!string.IsNullOrEmpty(_templateTitle) && GUILayout.Button("生成"))
            {
                if (AssetDatabase.IsValidFolder($"Assets/{_templateTitle}"))
                {
                    return;
                }

                if (!_templateTitle.StartsWith(LearnName))
                {
                    _templateTitle = $"{LearnName}{_templateTitle}";
                }

                _templateRealName = _templateTitle.Replace(LearnName, "");

                if (AssetDatabase.CopyAsset($"Assets/{LearnName}{TemplateName}", $"Assets/{_templateTitle}"))
                {
                    Rename(
                        $"Assets/{_templateTitle}/URP/{LearnName}{TemplateName} Render Pipeline Asset_Renderer.asset");
                    Rename(
                        $"Assets/{_templateTitle}/URP/{LearnName}{TemplateName} Universal Render Pipeline Asset.asset");

                    Rename($"Assets/{_templateTitle}/Scenes/{TemplateName}Scene.unity");

                    UniversalRenderPipelineAsset renderPipelineAsset =
                        AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(
                            $"Assets/{_templateTitle}/URP/{_templateTitle} Universal Render Pipeline Asset.asset");
                    if (typeof(UniversalRenderPipelineAsset)
                            .GetField("m_RendererDataList", BindingFlags.Instance | BindingFlags.NonPublic)
                            ?.GetValue(renderPipelineAsset) is ScriptableRendererData[] { Length: > 0 } list)
                    {
                        list[0] = AssetDatabase.LoadAssetAtPath<UniversalRendererData>(
                            $"Assets/{_templateTitle}/URP/{_templateTitle} Render Pipeline Asset_Renderer.asset");
                    }

                    Scene scene =
                        EditorSceneManager.OpenScene($"Assets/{_templateTitle}/Scenes/{_templateRealName}Scene.unity");
                    GameObject[] roots = scene.GetRootGameObjects();
                    GameObject mainCamera = roots.FirstOrDefault(item => item.GetComponent<Camera>());
                    if (mainCamera)
                    {
                        UrpEnvSwitch urpEnvSwitch = mainCamera.GetComponent<UrpEnvSwitch>();
                        if (urpEnvSwitch)
                        {
                            urpEnvSwitch.m_PipelineAsset = renderPipelineAsset;
                        }
                    }

                    ReplaceTextContent(
                        $"Assets/{_templateTitle}/Scripts/Editor/{LearnName}{TemplateName}.Editor.asmdef.txt");
                    ReplaceTextContent(
                        $"Assets/{_templateTitle}/Scripts/Editor/{LearnName}{TemplateName}Editor.cs.txt");

                    ReplaceTextContent(
                        $"Assets/{_templateTitle}/Scripts/Runtime/{LearnName}{TemplateName}.Runtime.asmdef.txt");
                    ReplaceTextContent($"Assets/{_templateTitle}/Scripts/Runtime/{LearnName}{TemplateName}Main.cs.txt");
                    AssetDatabase.Refresh();
                }
            }
        }

        /// <summary>
        /// 替换文本
        /// </summary>
        private void ReplaceTextContent(string shortPath)
        {
            string content = AssetDatabase.LoadAssetAtPath<TextAsset>(shortPath).text;
            content = content.Replace(TemplateName, _templateRealName);
            string target =
                $"{Application.dataPath}/../{shortPath.Replace(TemplateName, _templateRealName).Replace(".txt", "")}";
            File.WriteAllText(target, content);
            AssetDatabase.DeleteAsset(shortPath);
        }

        /// <summary>
        /// 重命名
        /// </summary>
        /// <param name="shortPath"></param>
        private void Rename(string shortPath)
        {
            string targetName = Path.GetFileName(shortPath).Replace(TemplateName, _templateRealName);
            string ret = AssetDatabase.RenameAsset(shortPath, targetName);
            if (!string.IsNullOrEmpty(ret))
            {
                Debug.LogError($"shortPath:{shortPath}--targetName:{targetName}--error:{ret}");
            }
        }
    }
}