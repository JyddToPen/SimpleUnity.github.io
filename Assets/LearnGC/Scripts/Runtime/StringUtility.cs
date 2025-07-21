using System;
using System.Text;

namespace LearnGC.Scripts.Runtime
{
    public class StringUtility
    {
        private readonly StringBuilder _abPathSb = new();

        /// <summary>
        /// 解析路径
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public (string ab, string name) ParseFromAssetPath(string assetPath, string prefix = "")
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                return (string.Empty, string.Empty);
            }

            _abPathSb.Clear();
            int lastSplitChar = assetPath.LastIndexOf("/", StringComparison.Ordinal);
            string directory = assetPath[..lastSplitChar];
            if (string.IsNullOrEmpty(directory))
            {
                return (string.Empty, string.Empty);
            }

            int extensionPos = assetPath.LastIndexOf(".", StringComparison.Ordinal);
            string name = assetPath.Substring(lastSplitChar + 1,
                (extensionPos >= 0 ? extensionPos : assetPath.Length) - lastSplitChar - 1);

            if (!string.IsNullOrEmpty(prefix))
            {
                _abPathSb.Append(prefix);
                if (!prefix.EndsWith("/"))
                {
                    _abPathSb.Append(".");
                }
            }

            _abPathSb.Append(directory);
            _abPathSb.Append(".assetbundle");
            Span<char> buffer = stackalloc char[256];
            string abPath = _abPathSb.ToString();
            abPath.ToLowerInBuffer(buffer);
            abPath = abPath.Replace("/", ".");
            return (abPath, name);
        }
    }
}