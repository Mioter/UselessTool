using System.Windows;

namespace UselessTool.Theme.Service;

public class ThemeManager
{
    internal Dictionary<
        ThemeResourceType,
        Dictionary<string, Dictionary<string, ResourceDictionary>>
    > ThemeResources
    { get; } = [];

    private Dictionary<ThemeResourceType, ThemeInfo> CurrentThemesInfo { get; } = [];

    public event EventHandler<ThemeChangingEventArgs>? ThemeChanging; // 定义主题更改前事件
    public event EventHandler<ThemeChangedEventArgs>? ThemeChanged; // 定义主题更改后事件

    public string? AssemblyName { get; init; }
    public string? ResourceBasedPath { get; init; }

    public ThemeManager()
    {
        foreach (var resourceType in Enum.GetValues<ThemeResourceType>())
        {
            ThemeResources[resourceType] = [];
            CurrentThemesInfo[resourceType] = new ThemeInfo();
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
        ThemeResources[resourceType][themeType][themeName] = resourceDict;

        // 如果主题正在使用，则立即应用以更新
        if (
            CurrentThemesInfo.TryGetValue(resourceType, out var value)
            && value.ThemeType == themeType
            && value.ThemeName == themeName
        )
        {
            ApplyTheme(resourceType, themeType, themeName);
        }
    }

    /// <summary>
    /// 注册一个新主题
    /// </summary>
    /// <param name="resourceType">资源类型</param>
    /// <param name="themeType">主题类型（例如：亮色主题、暗色主题）</param>
    /// <param name="themeName">主题名称</param>
    /// <param name="assemblyName">程序集，默认为当前程序集</param>
    /// <param name="resourcePath">程序集中的资源字典所在路径（相对路径）</param>
    public void RegisterAndUpdateTheme(
        ThemeResourceType resourceType,
        string themeType,
        string themeName,
        string? assemblyName = null,
        string resourcePath = ""
    )
    {
        if (string.IsNullOrEmpty(assemblyName))
        {
            assemblyName = AssemblyName ?? throw new ArgumentNullException(nameof(assemblyName));
        }
        if (string.IsNullOrEmpty(resourcePath))
            resourcePath = $"{ResourceBasedPath}/{resourceType}/{themeType}/{themeName}.xaml";

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
        var newTheme = GetThemeResourceDictionary(resourceType, themeType, themeName);

        if (Application.Current.Resources.MergedDictionaries.Contains(newTheme))
            return; // 如果当前主题与要应用的主题一致，则不应用

        ThemeChangingEventArgs changingArgs = new(resourceType, themeType, themeName, newTheme);

        OnThemeChanging(changingArgs); // 触发主题更改前事件
        if (changingArgs.Cancel) // 如果取消事件，则不应用新主题
            return;

        RemoveAllAppliedThemes();
        Application.Current.Resources.MergedDictionaries.Add(newTheme);

        OnThemeChanged(new ThemeChangedEventArgs(resourceType, themeType, themeName, newTheme)); // 触发主题更改事件

        CurrentThemesInfo[resourceType] = new ThemeInfo(themeType, themeName);
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

        ThemeResources[resourceType][themeType].Remove(themeName);
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

        var resourceDict = ThemeResources[resourceType][oldThemeType][oldThemeName];
        ThemeResources[resourceType][oldThemeType].Remove(oldThemeName);

        ThemeResources[resourceType][newThemeType][newThemeName] = resourceDict;

        // 如果当前主题已重命名，则更新当前主题信息
        if (CurrentThemesInfo[resourceType].Equals(new ThemeInfo(oldThemeType, oldThemeName)))
            CurrentThemesInfo[resourceType] = new ThemeInfo(newThemeType, newThemeName);
    }

    /// <summary>
    /// 获取当前应用的主题类型和主题名称
    /// </summary>
    /// <param name="resourceType">资源类型</param>
    /// <returns>包含主题类型和主题名称的元组</returns>
    public (string themeType, string themeName) GetCurrentThemeInfo(ThemeResourceType resourceType)
    {
        return (CurrentThemesInfo[resourceType].ThemeType, CurrentThemesInfo[resourceType].ThemeName);
    }

    /// <summary>
    /// 获取当前应用的主题资源字典
    /// </summary>
    /// <param name="resourceType">资源类型</param>
    /// <returns>资源字典</returns>
    public ResourceDictionary GetCurrentTheme(ThemeResourceType resourceType)
    {
        return GetThemeResourceDictionary(
            resourceType,
            CurrentThemesInfo[resourceType].ThemeType,
            CurrentThemesInfo[resourceType].ThemeName
        );
    }

    /// <summary>
    /// 获取指定主题资源类型的字典
    /// </summary>
    /// <param name="resourceType">资源类型</param>
    /// <returns>主题字典 结构：主题类型>主题名>资源字典</returns>
    public Dictionary<string, Dictionary<string, ResourceDictionary>> GetThemeResourceTypeDictionary(
        ThemeResourceType resourceType
    )
    {
        // 创建副本，避免在程序集外被修改。
        return ThemeResources[resourceType].ToDictionary();
    }

    /// <summary>
    /// 获取指定的主题资源字典
    /// </summary>
    /// <param name="resourceType">资源类型</param>
    /// <param name="themeType">主题类型</param>
    /// <param name="themeName">主题名称</param>
    /// <returns>资源字典</returns>
    public ResourceDictionary GetThemeResourceDictionary(
        ThemeResourceType resourceType,
        string themeType,
        string themeName
    )
    {
        return
            !ThemeResources.TryGetValue(resourceType, out var themesByType)
            || !themesByType.TryGetValue(themeType, out var themes)
            || !themes.TryGetValue(themeName, out var theme)
            ? throw new ArgumentException("主题未注册。")
            : theme;
    }

    /// <summary>
    /// 获取指定主题类别的第一个主题键值对。
    /// </summary>
    /// <param name="resourceType">资源类型</param>
    /// <param name="themeType">主题类型</param>
    /// <returns>键值对：（主题名，对应资源字典）</returns>
    public KeyValuePair<string, ResourceDictionary> GetFirstThemeKeyValuePair(
        ThemeResourceType resourceType,
        string themeType
    )
    {
        EnsureResourceTypeExists(resourceType);
        EnsureThemeTypeExists(resourceType, themeType);
        return ThemeResources[resourceType][themeType].First();
    }

    /// <summary>
    /// 获取指定主题类别的第一个主题名
    /// </summary>
    /// <param name="resourceType">资源类型</param>
    /// <param name="themeType">主题类型</param>
    /// <returns>键值对：（主题名，对应资源字典）</returns>
    public string GetFirstThemeName(ThemeResourceType resourceType, string themeType)
    {
        return GetFirstThemeKeyValuePair(resourceType, themeType).Key;
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
        return ThemeResources.TryGetValue(resourceType, out var themesByType)
            && themesByType.TryGetValue(themeType, out var themes)
            && themes.ContainsKey(themeName);
    }

    /// <summary>
    /// 确保资源类型存在。 如果不存在，则创建一个空的主题类型集合
    /// </summary>
    /// <param name="resourceType">资源类型</param>
    private void EnsureResourceTypeExists(ThemeResourceType resourceType)
    {
        if (!ThemeResources.ContainsKey(resourceType))
        {
            ThemeResources[resourceType] = [];
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
        if (!ThemeResources[resourceType].ContainsKey(themeType))
        {
            ThemeResources[resourceType][themeType] = [];
        }
    }

    /// <summary>
    /// 移除所有已应用的主题
    /// </summary>
    internal void RemoveAllAppliedThemes()
    {
        foreach (
            var theme in ThemeResources
                .Values.SelectMany(themesByType => themesByType.Values)
                .SelectMany(themes => themes.Values)
        )
        {
            Application.Current.Resources.MergedDictionaries.Remove(theme);
        }
    }

    /// <summary>
    /// 清空 CurrentThemesInfo 中所有 ThemeInfo 结构体的值
    /// </summary>
    internal void ClearCurrentThemesInfo()
    {
        foreach (var resourceType in CurrentThemesInfo.Keys.ToList())
        {
            CurrentThemesInfo[resourceType] = new ThemeInfo();
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
