using System.IO;
using System.Text.Json;

namespace UselessTool.Common;

/// <remarks>
///  Json配置类，提供了Json读写方法。
/// </remarks>
/// <param name="paths">路径数组，用于构建文件路径。</param>
public class JsonConfig(params string[] paths)
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true };
    private readonly string _filePath = Path.Combine([Directory.GetCurrentDirectory(), .. paths]);

    /// <summary>
    /// 从 JSON 文件中加载数据。
    /// </summary>
    /// <typeparam name="T">要加载的数据类型。</typeparam>
    /// <returns>加载的数据对象。</returns>
    /// <exception cref="FileNotFoundException">当 JSON 文件不存在时抛出。</exception>
    public T? LoadFromJson<T>()
    {
        if (!File.Exists(_filePath))
        {
            throw new FileNotFoundException("JSON 文件不存在！", _filePath);
        }

        string jsonContent = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<T>(jsonContent);
    }

    /// <summary>
    /// 将数据保存到 JSON 文件中。
    /// </summary>
    /// <typeparam name="T">要保存的数据类型。</typeparam>
    /// <param name="data">要保存的数据对象。</param>
    public void SaveToJson<T>(T data)
    {
        string jsonContent = JsonSerializer.Serialize(data, _jsonSerializerOptions);
        File.WriteAllText(_filePath, jsonContent);
    }
}