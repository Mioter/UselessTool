using System.Windows;
using UselessTool.Bases.FileOperation;

namespace UselessTool.Theme.Tools;

public class UserPreferences(string filePath)
{
    private readonly JsonConfig<Dictionary<ThemeResourceType, Dictionary<string, string>>> _jsonConfig = new(
        filePath,
        "userPreferences.json"
    );

    /// <summary>
    /// 加载用户偏好设置
    /// </summary>
    /// <returns>用户偏好设置字典</returns>
    public Dictionary<ThemeResourceType, Dictionary<string, string>> LoadUserPreferences()
    {
        try
        {
            var preferences = _jsonConfig.LoadFromJson();
            return preferences ?? CreateDefaultPreferences();
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
    public void SaveUserPreferences(Dictionary<ThemeResourceType, Dictionary<string, string>> preferences)
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
    private static Dictionary<ThemeResourceType, Dictionary<string, string>> CreateDefaultPreferences()
    {
        Dictionary<ThemeResourceType, Dictionary<string, string>> preferences = [];
        foreach (var resourceType in Enum.GetValues<ThemeResourceType>())
        {
            preferences[resourceType] = [];
        }
        return preferences;
    }
}
