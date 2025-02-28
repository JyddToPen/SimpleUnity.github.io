using System.Linq;
using UnityEditor;
using UnityEditor.Build;

namespace BuildModule.Scripts.Editor
{
    /// <summary>
    /// 构建webGl平台式剔除微信相关代码
    /// </summary>
    public class WebGlFilterWx : IFilterBuildAssemblies
    {
        public int callbackOrder => 100;

        /// <summary>
        /// 剔除微信相关
        /// </summary>
        /// <param name="buildOptions"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public string[] OnFilterAssemblies(BuildOptions buildOptions, string[] assemblies)
        {
            string symbols =
                PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (!symbols.Contains("ENABLE_WECHAT"))
            {
                string[] remove = { "Wx.dll", "wx-runtime.dll", "WxWasmSDKRuntime.dll", "LitJson.dll" };
                var ret = assemblies.Where(item => remove.All(r => !item.EndsWith(r))).ToArray();
                return ret;
            }
            else
            {
                return assemblies;
            }
        }
    }
}