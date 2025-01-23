using System.Windows;
using UselessTool.Bases.FileOperation;

namespace UselessTool.Theme.Service;

public class UserPreferences(Dictionary<string, Dictionary<string, string[]>> defaultThemes, params string[] filePath)
{
    private readonly JsonConfig<Dictionary<string, Dictionary<string, string>>> _jsonConfig = new(
        [.. filePath, "userPreferences.json"]
    );

    /// <summary>
    /// 加载用户偏好设置
    /// </summary>
    /// <returns>用户偏好设置字典</returns>
    public Dictionary<string, Dictionary<string, string>> LoadUserPreferences()
    {
        try
        {
            return _jsonConfig.LoadFromJson() ?? CreateDefaultPreferences();
        }
        catch (Exception)
        {
            return CreateDefaultPreferences();
        }
    }

    /// <summary>
    /// 保存用户偏好设置
    /// </summary>
    /// <param name="preferences">用户偏好设置字典</param>
    public void SaveUserPreferences(Dictionary<string, Dictionary<string, string>> preferences)
    {
        try
        {
            _jsonConfig.SaveToJson(preferences);
        }
        catch (Exception ex)
        {
            // 处理无法保存的情况。
            MessageBox.Show($"{ex.Message}", "保存出错");
        }
    }

    /// <summary>
    /// 创建默认的用户偏好设置字典
    /// </summary>
    /// <returns>默认的用户偏好设置字典</returns>
    private Dictionary<string, Dictionary<string, string>> CreateDefaultPreferences()
    {
        return defaultThemes.ToDictionary(
            resourceType => resourceType.Key,
            resourceType =>
                resourceType.Value.Where(kv => kv.Value.Length > 0).ToDictionary(kv => kv.Key, kv => kv.Value.First())
        );
    }
}
