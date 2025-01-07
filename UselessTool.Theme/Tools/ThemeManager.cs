using System.Windows;

namespace UselessTool.Theme.Tools;

public class ThemeManager
{
    public Dictionary<
        ThemeResourceType,
        Dictionary<string, Dictionary<string, ResourceDictionary>>
    > Resources
    { get; } = [];

    public Dictionary<ThemeResourceType, ThemeInfo> CurrentThemes { get; } = [];

    public event EventHandler<ThemeChangingEventArgs>? ThemeChanging; // 定义主题更改前事件
    public event EventHandler<ThemeChangedEventArgs>? ThemeChanged; // 定义主题更改后事件

    public ThemeManager()
    {
        foreach (ThemeResourceType resourceType in Enum.GetValues<ThemeResourceType>())
        {
            Resources[resourceType] = [];
            CurrentThemes[resourceType] = new ThemeInfo("", "");
        }
    }

    /// <summary>
    /// 注册并更新一个新主题
    /// </summary>
    /// <param name="resourceType">资源类型</param>
    /// <param name="themeType">主题类型（例如：亮色主题、暗色主题）</param>
    /// <param name="themeName">主题名称</param>
    /// <param name="resourceDict">资源字典</param>
    public void RegisterAndUpdateTheme(
        ThemeResourceType resourceType,
        string themeType,
        string themeName,
        ResourceDictionary resourceDict
    )
    {
        EnsureResourceTypeExists(resourceType);
        EnsureThemeTypeExists(resourceType, themeType);
        Resources[resourceType][themeType][themeName] = resourceDict;
    }

    /// <summary>
    /// 注册一个新主题
    /// </summary>
    /// <param name="resourceType">资源类型</param>
    /// <param name="themeType">主题类型（例如：亮色主题、暗色主题）</param>
    /// <param name="themeName">主题名称</param>
    /// <param name="assemblyName">程序集</param>
    /// <param name="resourcePath">程序集中的资源字典所在路径（相对路径）</param>
    public void RegisterAndUpdateTheme(
        ThemeResourceType resourceType,
        string themeType,
        string themeName,
        string assemblyName,
        string resourcePath
    )
    {
        string uri = $"/{assemblyName};component/{resourcePath}";
        RegisterAndUpdateTheme(
            resourceType,
            themeType,
            themeName,
            new ResourceDictionary { Source = new Uri(uri, UriKind.RelativeOrAbsolute) }
        );
    }

    /// <summary>
    /// 应用指定的主题
    /// </summary>
    /// <param name="resourceType">资源类型</param>
    /// <param name="themeType">主题类型（例如：亮色主题、暗色主题）</param>
    /// <param name="themeName">主题名称</param>
    internal void ApplyTheme(ThemeResourceType resourceType, string themeType, string themeName)
    {
        ThemeInfo themeInfo = new(themeType, themeName);

        if (CurrentThemes[resourceType].Equals(themeInfo))
            return; // 如果当前主题与要应用的主题一致，则不应用

        ThemeChangingEventArgs? changingArgs = new(
            resourceType,
            themeType,
            themeName,
            GetTheme(resourceType, themeType, themeName)
        );
        OnThemeChanging(changingArgs); // 触发主题更改前事件
        if (changingArgs.Cancel) // 如果取消事件，则不应用新主题
            return;

        ResourceDictionary? theme = GetTheme(resourceType, themeType, themeName);

        RemoveAllThemes();
        Application.Current.Resources.MergedDictionaries.Add(theme);

        OnThemeChanged(new ThemeChangedEventArgs(resourceType, themeType, themeName, theme)); // 触发主题更改事件

        CurrentThemes[resourceType] = themeInfo;
    }

    /// <summary>
    /// 移除指定主题
    /// </summary>
    /// <param name="resourceType">资源类型</param>
    /// <param name="themeType">主题类型（例如：亮色主题、暗色主题）</param>
    /// <param name="themeName">主题名称</param>
    internal void RemoveTheme(ThemeResourceType resourceType, string themeType, string themeName)
    {
        if (string.IsNullOrEmpty(themeType) || string.IsNullOrEmpty(themeName))
            return;
        EnsureResourceTypeExists(resourceType);
        EnsureThemeTypeExists(resourceType, themeType);

        if (Resources[resourceType][themeType].Count <= 1)
        {
            throw new InvalidOperationException($"不能移除{themeType}类型的唯一主题。");
        }

        Resources[resourceType][themeType].Remove(themeName);
    }

    /// <summary>
    /// 修改指定主题的名称和类型
    /// </summary>
    /// <param name="resourceType">资源类型</param>
    /// <param name="oldThemeType">旧主题类型</param>
    /// <param name="oldThemeName">旧主题名称</param>
    /// <param name="newThemeType">新主题类型</param>
    /// <param name="newThemeName">新主题名称</param>
    internal void RenameTheme(
        ThemeResourceType resourceType,
        string oldThemeType,
        string oldThemeName,
        string newThemeType,
        string newThemeName
    )
    {
        EnsureResourceTypeExists(resourceType);
        EnsureThemeTypeExists(resourceType, oldThemeType);
        EnsureThemeTypeExists(resourceType, newThemeType);

        ResourceDictionary? resourceDict = Resources[resourceType][oldThemeType][oldThemeName];
        Resources[resourceType][oldThemeType].Remove(oldThemeName);

        Resources[resourceType][newThemeType][newThemeName] = resourceDict;

        // 如果当前主题已重命名，则更新当前主题信息
        if (CurrentThemes[resourceType].Equals(new ThemeInfo(oldThemeType, oldThemeName)))
        {
            CurrentThemes[resourceType] = new ThemeInfo(newThemeType, newThemeName);
        }
    }

    /// <summary>
    /// 获取上一次应用的主题类型和主题名称
    /// </summary>
    /// <param name="resourceType">资源类型</param>
    /// <returns>包含主题类型和主题名称的元组</returns>
    public (string themeType, string themeName) GetCurrentThemeInfo(ThemeResourceType resourceType)
    {
        return (CurrentThemes[resourceType].ThemeType, CurrentThemes[resourceType].ThemeName);
    }

    /// <summary>
    /// 获取当前的主题
    /// </summary>
    /// <param name="resourceType">资源类型</param>
    /// <returns>资源字典</returns>
    public ResourceDictionary GetCurrentTheme(ThemeResourceType resourceType)
    {
        return GetTheme(resourceType, CurrentThemes[resourceType].ThemeType, CurrentThemes[resourceType].ThemeName);
    }

    /// <summary>
    /// 获取指定的主题
    /// </summary>
    /// <param name="resourceType">资源类型</param>
    /// <param name="themeType">主题类型</param>
    /// <param name="themeName">主题名称</param>
    /// <returns>资源字典</returns>
    private ResourceDictionary GetTheme(ThemeResourceType resourceType, string themeType, string themeName)
    {
        return
            !Resources.TryGetValue(resourceType, out Dictionary<string, Dictionary<string, ResourceDictionary>>? themesByType)
            || !themesByType.TryGetValue(themeType, out Dictionary<string, ResourceDictionary>? themes)
            || !themes.TryGetValue(themeName, out ResourceDictionary? theme)
            ? throw new ArgumentException("主题未注册。")
            : theme;
    }

    /// <summary>
    /// 检查主题是否被注册。
    /// </summary>
    /// <param name="resourceType">资源类型</param>
    /// <param name="themeType">主题类型</param>
    /// <param name="themeName">主题名称</param>
    /// <returns></returns>
    public bool IsThemeRegistered(ThemeResourceType resourceType, string themeType, string themeName)
    {
        return Resources.TryGetValue(resourceType, out Dictionary<string, Dictionary<string, ResourceDictionary>>? themesByType)
            && themesByType.TryGetValue(themeType, out Dictionary<string, ResourceDictionary>? themes)
            && themes.ContainsKey(themeName);
    }

    /// <summary>
    /// 确保资源类型存在。 如果不存在，则创建一个空的主题类型集合
    /// </summary>
    /// <param name="resourceType">资源类型</param>
    private void EnsureResourceTypeExists(ThemeResourceType resourceType)
    {
        if (!Resources.ContainsKey(resourceType))
        {
            Resources[resourceType] = [];
        }
    }

    /// <summary>
    /// 确保主题类型存在。 如果不存在，则创建一个空的主题集合
    /// </summary>
    /// <param name="resourceType">资源类型</param>
    /// <param name="themeType">主题类型</param>
    internal void EnsureThemeTypeExists(ThemeResourceType resourceType, string themeType)
    {
        EnsureResourceTypeExists(resourceType);
        if (!Resources[resourceType].ContainsKey(themeType))
        {
            Resources[resourceType][themeType] = [];
        }
    }

    /// <summary>
    /// 移除所有已应用的主题
    /// </summary>
    private void RemoveAllThemes()
    {
        foreach (
            ResourceDictionary? theme in Resources
                .Values.SelectMany(themesByType => themesByType.Values)
                .SelectMany(themes => themes.Values)
        )
        {
            Application.Current.Resources.MergedDictionaries.Remove(theme);
        }
    }

    /// <summary>
    /// 触发主题更改前事件
    /// </summary>
    /// <param name="e"></param>
    private void OnThemeChanging(ThemeChangingEventArgs e)
    {
        ThemeChanging?.Invoke(this, e);
    }

    /// <summary>
    /// 触发主题更改事件
    /// </summary>
    /// <param name="e"></param>
    private void OnThemeChanged(ThemeChangedEventArgs e)
    {
        ThemeChanged?.Invoke(this, e);
    }
}

/// <summary>
/// 自定义主题更改前事件参数类
/// </summary>
public class ThemeChangingEventArgs(
    ThemeResourceType resourceType,
    string themeType,
    string themeName,
    ResourceDictionary theme
) : EventArgs
{
    public ThemeResourceType ResourceType { get; } = resourceType;
    public string ThemeType { get; } = themeType;
    public string ThemeName { get; } = themeName;
    public ResourceDictionary OldTheme { get; } = theme;
    public bool Cancel { get; set; } = false; // 默认不取消
}

/// <summary>
/// 自定义主题更改后事件参数类
/// </summary>
public class ThemeChangedEventArgs(
    ThemeResourceType resourceType,
    string themeType,
    string themeName,
    ResourceDictionary theme
) : EventArgs
{
    public ThemeResourceType ResourceType { get; } = resourceType;
    public string ThemeType { get; } = themeType;
    public string ThemeName { get; } = themeName;
    public ResourceDictionary NewTheme { get; } = theme;
}
