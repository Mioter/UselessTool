using System.IO;
using System.Text.Json;
using static UselessTool.Theme.Tools.FileSystemHelper;

namespace UselessTool.Theme.Tools;

public class UserPreferences(string filePath)
{
    private readonly string _filePath = Path.Combine(filePath, "UserPreferences.json");
    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true };

    /// <summary>
    /// 加载用户偏好设置
    /// </summary>
    /// <returns>用户偏好设置字典</returns>
    public Dictionary<ThemeResourceType, Dictionary<string, string>> LoadUserPreferences()
    {
        if (!File.Exists(_filePath))
        {
            return CreateDefaultPreferences();
        }

        try
        {
            string jsonContent = File.ReadAllText(_filePath);
            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                return CreateDefaultPreferences();
            }

            using JsonDocument? doc = JsonDocument.Parse(jsonContent);
            JsonElement root = doc.RootElement;

            return root.ValueKind != JsonValueKind.Object
                ? throw new InvalidDataException("JSON 文件格式不正确。根元素应为对象。")
                : DeserializePreferences(root);
        }
        catch (JsonException ex)
        {
            throw new InvalidDataException("JSON 解析错误。", ex);
        }
    }

    /// <summary>
    /// 保存用户偏好设置
    /// </summary>
    /// <param name="preferences">用户偏好设置字典</param>
    public void SaveUserPreferences(Dictionary<ThemeResourceType, Dictionary<string, string>> preferences)
    {
        EnsureDirectoryExists(Path.GetDirectoryName(_filePath)!);
        string jsonContent = JsonSerializer.Serialize(preferences, _jsonSerializerOptions);
        File.WriteAllText(_filePath, jsonContent);
    }

    /// <summary>
    /// 反序列化用户偏好设置
    /// </summary>
    /// <param name="root">JSON 根元素</param>
    /// <returns>用户偏好设置字典</returns>
    /// <exception cref="InvalidOperationException">如果 JSON 结构不正确</exception>
    private static Dictionary<ThemeResourceType, Dictionary<string, string>> DeserializePreferences(JsonElement root)
    {
        Dictionary<ThemeResourceType, Dictionary<string, string>>? preferences = new();

        foreach (JsonProperty property in root.EnumerateObject())
        {
            if (Enum.TryParse(property.Name, out ThemeResourceType resourceType))
            {
                Dictionary<string, string>? resourcePreferences = new();

                if (property.Value.ValueKind != JsonValueKind.Object)
                {
                    throw new InvalidOperationException($"属性值必须是一个对象。属性名: {property.Name}");
                }

                foreach (JsonProperty preferenceProperty in property.Value.EnumerateObject())
                {
                    resourcePreferences[preferenceProperty.Name] =
                        preferenceProperty.Value.GetString() ?? throw new InvalidOperationException("属性值不能为空。");
                }

                preferences[resourceType] = resourcePreferences;
            }
            else
            {
                throw new InvalidOperationException($"未知的资源类型: {property.Name}");
                // 未知的资源类型，可以获取写入日志而不是抛出异常，避免影响程序的正常运行。
            }
        }

        return preferences;
    }

    /// <summary>
    /// 创建默认的用户偏好设置字典
    /// </summary>
    /// <returns>默认的用户偏好设置字典</returns>
    private static Dictionary<ThemeResourceType, Dictionary<string, string>> CreateDefaultPreferences()
    {
        Dictionary<ThemeResourceType, Dictionary<string, string>> preferences = [];
        foreach (ThemeResourceType resourceType in Enum.GetValues<ThemeResourceType>())
        {
            preferences[resourceType] = [];
        }
        return preferences;
    }
}
