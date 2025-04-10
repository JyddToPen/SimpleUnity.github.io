using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BuildModule.Scripts.Editor.AssetBundle;
using BuildModule.Scripts.Runtime;
using UnityEditor;
using UnityEngine;

namespace BuildModule.Scripts.Editor
{
    /// <summary>
    /// 场景数据
    /// </summary>
    internal struct SceneData
    {
        /// <summary>
        /// 场景路径
        /// </summary>
        public string ScenePath { get; set; }

        /// <summary>
        /// 场景名
        /// </summary>
        public string SceneName { get; set; }

        /// <summary>
        /// 根目录
        /// </summary>
        public string RootFolder { get; set; }

        /// <summary>
        /// ab包目录
        /// </summary>
        public string AssetBundleOutput { get; set; }

        /// <summary>
        /// 是否选择
        /// </summary>
        public bool IsSelect { get; set; }

        /// <summary>
        /// 选择id
        /// </summary>
        public int SelectIndex { get; set; }

        public SceneData(string scene, bool isSelect, int selectIndex)
        {
            ScenePath = scene;
            AssetBundleOutput = EditorAssetBundleUtility.AssetBundleOutput(scene);
            RootFolder = EditorAssetBundleUtility.GetRootFolder(scene);
            IsSelect = isSelect;
            SelectIndex = selectIndex;
            SceneName = Path.GetFileNameWithoutExtension(scene);
        }
    }

    public class BuildWindow : EditorWindow
    {
        private bool _isDevelopBuild;
        private bool _isAutoProfile;
        private bool _isAutoRunPlayer;
        private PlatformBuild _platformBuild;
        private SceneData[] _sceneData;
        private Queue<string> _bootScene;
        private string[] BootScene => _bootScene?.ToArray();
        private int _selectSceneIndex;
        private Vector2 _scrollPos;
        private SerializedObject _sEnvConfig;
        private SerializedProperty _sIsAb;

        private void OnEnable()
        {
            _sEnvConfig = new SerializedObject(EnvConfig.Instance);
            _sIsAb = _sEnvConfig.FindProperty(nameof(EnvConfig.isAb));
            _bootScene = new Queue<string>();
            _sceneData = AssetDatabase.GetAllAssetPaths()
                .Where(item => item.EndsWith(".unity") && item.StartsWith("Assets/")).OrderBy(item => item).Select(
                    (item, index) =>
                    {
                        var sceneData = new SceneData(item, false, index);
                        return sceneData;
                    }).ToArray();
        }

        private void OnGUI()
        {
            OnGUIBootScene();
            _sEnvConfig.Update();
            EditorGUILayout.PropertyField(_sIsAb);
            _sEnvConfig.ApplyModifiedProperties();
            switch (EditorUserBuildSettings.selectedBuildTargetGroup)
            {
                case BuildTargetGroup.WebGL:
                {
                    GUIPlatformBuild<WxBuild>("微信小游戏");
                    GUIPlatformBuild<WebGlBuild>("webGl");
                }
                    break;
                case BuildTargetGroup.Standalone:
                {
                    GUIPlatformBuild<StandaloneBuild>("单机");
                }
                    break;
            }
        }


        /// <summary>
        /// 平台构建
        /// </summary>
        private void GUIPlatformBuild<T>(string platformName) where T : PlatformBuild, new()
        {
            EditorGUILayout.BeginHorizontal("box");
            if (GUILayout.Button(platformName))
            {
                _platformBuild = new T();
                _platformBuild.HandleBuild(BuildOptions.None, BootScene);
                return;
            }

            if (GUILayout.Button($"{platformName}(AutoRun)"))
            {
                _platformBuild = new T();
                _platformBuild.HandleBuild(BuildOptions.AutoRunPlayer, BootScene);
                return;
            }

            if (GUILayout.Button($"{platformName}(Dev)"))
            {
                _platformBuild = new T();
                _platformBuild.HandleBuild(BuildOptions.Development | BuildOptions.ConnectWithProfiler, BootScene);
                return;
            }

            if (GUILayout.Button($"{platformName}(Dev-AutoRun)"))
            {
                _platformBuild = new T();
                _platformBuild.HandleBuild(BuildOptions.Development | BuildOptions.ConnectWithProfiler |
                                           BuildOptions.AutoRunPlayer, BootScene);
                return;
            }

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 启动场景
        /// </summary>
        private void OnGUIBootScene()
        {
            if (_sceneData is not { Length: > 0 })
            {
                return;
            }

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("添加启动场景");
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            for (var i = 0; i < _sceneData.Length; i++)
            {
                EditorGUILayout.BeginHorizontal("box");
                EditorGUI.BeginChangeCheck();
                SceneData sceneData = _sceneData[i];
                sceneData.IsSelect =
                    EditorGUILayout.ToggleLeft(sceneData.SceneName, sceneData.IsSelect, GUILayout.Width(150));
                string abState = Directory.Exists(sceneData.AssetBundleOutput) ? "* " : " ";
                if (GUILayout.Button($"{abState}Build AB", GUILayout.Width(80)))
                {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndScrollView();
                    AssetBundleBuildEditor.BuildAssetBundleBasedOnScene(sceneData.ScenePath, sceneData.RootFolder);
                    return;
                }

                EditorGUILayout.LabelField(sceneData.ScenePath);
                if (EditorGUI.EndChangeCheck())
                {
                    if (sceneData.IsSelect)
                    {
                        _bootScene.Enqueue(sceneData.ScenePath);
                    }
                    else
                    {
                        _bootScene.Clear();
                        for (int j = 0; j < _sceneData.Length; j++)
                        {
                            if (_sceneData[j].IsSelect)
                            {
                                _bootScene.Enqueue(_sceneData[j].ScenePath);
                            }
                        }
                    }
                }

                _sceneData[i] = sceneData;
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("清理选择"))
            {
                for (int i = 0; i < _sceneData.Length; i++)
                {
                    SceneData sceneData = _sceneData[i];
                    sceneData.IsSelect = false;
                    _sceneData[i] = sceneData;
                }

                _bootScene.Clear();
            }

            EditorGUILayout.LabelField("当前选择的场景");
            EditorGUILayout.BeginVertical("box");
            foreach (var bootScene in _bootScene)
            {
                EditorGUILayout.LabelField(bootScene);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        }
    }
}