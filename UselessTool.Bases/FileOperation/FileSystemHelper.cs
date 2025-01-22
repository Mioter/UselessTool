using System.IO;

namespace UselessTool.Bases.FileOperation;

public static class FileSystemHelper
{
    /// <summary>
    /// 确保文件目录存在，如果不存在则创建。
    /// </summary>
    /// <param name="directoryPath">目录路径</param>
    /// <exception cref="ArgumentNullException">当 <paramref name="directoryPath"/> 为 null 时抛出。</exception>
    /// <exception cref="ArgumentException">当 <paramref name="directoryPath"/> 包含无效字符时抛出。</exception>
    /// <exception cref="IOException">当目录创建失败时抛出（例如，路径已存在但不是目录，或者没有写权限）。</exception>
    public static void EnsureDirectoryExists(string directoryPath)
    {
        if (string.IsNullOrWhiteSpace(directoryPath))
            throw new ArgumentNullException(nameof(directoryPath), "目录路径不能为空或仅包含空白字符。");

        if (Directory.Exists(directoryPath)) return;
        try
        {
            Directory.CreateDirectory(directoryPath);
        }
        catch (Exception ex)
        {
            throw new IOException($"创建目录失败：{ex.Message}", ex);
        }
    }

    /// <summary>
    /// 确保文件存在，如果不存在则创建。
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <exception cref="ArgumentNullException">当 <paramref name="filePath"/> 为 null 时抛出。</exception>
    /// <exception cref="ArgumentException">当 <paramref name="filePath"/> 包含无效字符或格式不正确时抛出。</exception>
    /// <exception cref="IOException">当文件创建失败时抛出（例如，路径已存在但不是文件，或者没有写权限）。</exception>
    public static void EnsureFileExists(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentNullException(nameof(filePath), "文件路径不能为空或仅包含空白字符。");

        var fileInfo = new FileInfo(filePath);
        string directoryPath = Path.GetDirectoryName(filePath) ?? throw new InvalidOperationException("无法获取文件目录路径。");

        EnsureDirectoryExists(directoryPath);

        if (fileInfo.Exists) return;
        try
        {
            // 使用 File.Create 创建一个空文件，并立即关闭文件流
            using (File.Create(filePath)) {}
        }
        catch (Exception ex)
        {
            throw new IOException($"创建文件失败：{ex.Message}", ex);
        }
    }
}
