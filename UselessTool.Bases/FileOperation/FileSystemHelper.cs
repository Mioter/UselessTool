using System.IO;

namespace UselessTool.Bases.FileOperation;

public static class FileSystemHelper
{
    /// <summary>
    /// 确保文件目录存在，如果不存在则创建。
    /// </summary>
    /// <param name="directoryPath">目录路径</param>
    public static void EnsureDirectoryExists(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);
    }

    /// <summary>
    /// 确保文件存在，如果不存在则创建。
    /// </summary>
    /// <param name="filePath">文件路径</param>
    public static void EnsureFileExists(string filePath)
    {
        // 使用 FileInfo 类来获取文件信息
        FileInfo fileInfo = new(filePath);

        // 获取文件的目录路径，并确保目录存在
        string? directoryPath = fileInfo.DirectoryName;
        if (directoryPath == null)
            EnsureDirectoryExists(directoryPath);

        if (fileInfo.Exists)
            return;

        using var stream = fileInfo.Create();
    }
}
