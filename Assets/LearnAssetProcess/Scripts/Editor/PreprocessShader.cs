using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.Build;
using UnityEditor.Rendering;
using UnityEngine;

namespace LearnAssetProcess.Scripts.Editor
{
    public class PreprocessShader : IPreprocessShaders
    {
        // Use callbackOrder to set when Unity calls this shader preprocessor. Unity starts with the preprocessor that has the lowest callbackOrder value.
        public int callbackOrder => 0;

        private readonly string[] _detectedShader =
        {
        };

        public void OnProcessShader(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> data)
        {
            if (data.Count > 0)
            {
                for (int i = 0; i < data.Count; ++i)
                {
                    var foundKeywordSet = string.Join(" ", data[i].shaderKeywordSet.GetShaderKeywords());
                    // Debug.Log($"PreprocessShader shader:{shader} passName:{snippet.passName} passType:{snippet.passType} foundKeywordSet:{foundKeywordSet} i:{i} count:{data.Count}");
                }

                // Debug.Log(
                    // $"PreprocessShader shader:{shader} passName:{snippet.passName} passType:{snippet.passType} count:{data.Count}");
            }
            else
            {
                // Debug.Log(
                    // $"PreprocessShader shader:{shader}  passName:{snippet.passName}  passType:{snippet.passType} no Keyword set");
            }
        }
    }
}