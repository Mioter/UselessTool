using System.IO;

namespace UselessTool.Bases.SystemIO;

public static class FileSystemHelper
{
    /// <summary>
    /// 确保文件目录存在，如果不存在则创建。
    /// </summary>
    /// <param name="directoryPath">目录路径</param>
    public static void EnsureDirectoryExists(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
    }
}