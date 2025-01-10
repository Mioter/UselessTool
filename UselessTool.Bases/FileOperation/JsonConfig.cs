using System.IO;
using System.Text.Json;

namespace UselessTool.Bases.FileOperation;

/// <remarks>
/// Json配置类，提供了Json读写方法。
/// </remarks>
/// <param name="paths">路径数组，用于构建文件路径。</param>
public class JsonConfig<T>(params string[] paths)
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true };
    private readonly string _filePath = Path.Combine(Directory.GetCurrentDirectory(), Path.Combine(paths));

    /// <summary>
    /// 从 JSON 文件中加载数据（异步）。
    /// </summary>
    /// <typeparam name="T">要加载的数据类型。</typeparam>
    /// <returns>加载的数据对象。</returns>
    /// <exception cref="FileNotFoundException">当 JSON 文件不存在时抛出。</exception>
    public async Task<T?> LoadFromJsonAsync()
    {
        FileSystemHelper.EnsureFileExists(_filePath);

        try
        {
            await using FileStream fs = new(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return await JsonSerializer.DeserializeAsync<T>(fs);
        }
        catch (Exception ex)
        {
            // 处理异常，例如记录日志
            throw new IOException("加载 JSON 文件时出错。", ex);
        }
    }

    /// <summary>
    /// 从 JSON 文件中加载数据（同步）。
    /// </summary>
    /// <typeparam name="T">要加载的数据类型。</typeparam>
    /// <returns>加载的数据对象。</returns>
    /// <exception cref="FileNotFoundException">当 JSON 文件不存在时抛出。</exception>
    public T? LoadFromJson()
    {
        FileSystemHelper.EnsureFileExists(_filePath);

        try
        {
            string jsonContent = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<T>(jsonContent);
        }
        catch (Exception ex)
        {
            // 处理异常，例如记录日志
            throw new IOException("加载 JSON 文件时出错。", ex);
        }
    }

    /// <summary>
    /// 将数据保存到 JSON 文件中（异步）。
    /// </summary>
    /// <typeparam name="T">要保存的数据类型。</typeparam>
    /// <param name="data">要保存的数据对象。</param>
    public async Task SaveToJsonAsync(T data)
    {
        FileSystemHelper.EnsureFileExists(_filePath);

        try
        {
            await using FileStream fs = new(_filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            await JsonSerializer.SerializeAsync(fs, data, _jsonSerializerOptions);
        }
        catch (Exception ex)
        {
            // 处理异常，例如记录日志
            throw new IOException("保存 JSON 文件时出错。", ex);
        }
    }

    /// <summary>
    /// 将数据保存到 JSON 文件中（同步）。
    /// </summary>
    /// <typeparam name="T">要保存的数据类型。</typeparam>
    /// <param name="data">要保存的数据对象。</param>
    public void SaveToJson(T data)
    {
        FileSystemHelper.EnsureFileExists(_filePath);

        try
        {
            string jsonContent = JsonSerializer.Serialize(data, _jsonSerializerOptions);
            File.WriteAllText(_filePath, jsonContent);
        }
        catch (Exception ex)
        {
            // 处理异常，例如记录日志
            throw new IOException("保存 JSON 文件时出错。", ex);
        }
    }
}
