using System.IO;
using UnityEngine;

namespace BaseLib.Scripts.Runtime
{
    public static class FileUtility
    {
        /// <summary>
        /// 复制文件夹
        /// </summary>
        /// <param name="sourceFolderPath"></param>
        /// <param name="targetFolderPath"></param>
        /// <param name="cleanupDestination"></param>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public static void CopyFolder(string sourceFolderPath, string targetFolderPath, bool cleanupDestination = false)
        {
            if (!Directory.Exists(sourceFolderPath))
            {
                throw new DirectoryNotFoundException(
                    $"BaseLib.Scripts.Runtime.CopyFolder error, Source directory:{sourceFolderPath} does not exist.");
            }

            sourceFolderPath = new DirectoryInfo(sourceFolderPath).FullName;
            targetFolderPath = new DirectoryInfo(targetFolderPath).FullName;

            string targetParent = Path.GetDirectoryName(targetFolderPath);
            if (string.IsNullOrEmpty(targetParent) || !Directory.Exists(targetParent))
            {
                throw new DirectoryNotFoundException(
                    $"BaseLib.Scripts.Runtime.CopyFolder error, Target parent directory:{targetParent} does not exist.");
            }

            if (Directory.Exists(targetFolderPath) && cleanupDestination)
            {
                Directory.Delete(targetFolderPath, true);
            }

            if (!Directory.Exists(targetFolderPath))
            {
                Debug.Log(
                    $"BaseLib.Scripts.Runtime.CopyFolder,start to create destination directory:{targetFolderPath}.");
                Directory.CreateDirectory(targetFolderPath);
            }

            foreach (string subSourceDirPath in Directory.GetDirectories(sourceFolderPath, "*",
                         SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(subSourceDirPath.Replace(sourceFolderPath, targetFolderPath));
            }

            foreach (string newPath in Directory.GetFiles(sourceFolderPath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourceFolderPath, targetFolderPath), true);
            }

            Debug.Log(
                $"BaseLib.Scripts.Runtime.CopyFolder success, sourceFolderPath:{sourceFolderPath} targetFolderPath:{targetFolderPath}.");
        }
    }
}