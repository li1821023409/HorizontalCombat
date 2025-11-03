using UnityEngine;

namespace FileIO
{
    /// <summary>
    /// csv文本路径
    /// </summary>
    public class FilePaths
    {
        public static readonly string root = $"{Application.dataPath}/Common/GameData/";
    }

    /// <summary>
    /// csv文本对应脚本路径
    /// </summary>
    public class FileDataPaths
    {
        public static readonly string root = $"{Application.dataPath}/Scripts/FileData/";
    }
}
