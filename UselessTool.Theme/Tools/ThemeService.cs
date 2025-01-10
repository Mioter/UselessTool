using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Xml;
using static UselessTool.Bases.FileOperation.FileSystemHelper;
using static UselessTool.Theme.Tools.DefaultTheme;

namespace UselessTool.Theme.Tools;

public class ThemeService
{
    private readonly Dictionary<ThemeResourceType, string> _resourcePaths;
    private readonly ThemeManager _themeManager;
    private readonly UserPreferences _userPreferences;

    public ThemeService(ThemeManager themeManager, string themesBasePath)
    {
        string baseDirectory = Directory.GetCurrentDirectory();
        _resourcePaths = [];
        foreach (var resourceType in Enum.GetValues<ThemeResourceType>())
        {
            _resourcePaths[resourceType] = Path.Combine(baseDirectory, themesBasePath, resourceType.ToString());
        }

        _themeManager = themeManager;
        _userPreferences = new UserPreferences(themesBasePath);
    }

    /// <summary>
    /// 从文件系统加载主题并注册到 ThemeManager
    /// </summary>
    public void LoadThemesFromFileSystem()
    {
        foreach (var resourceType in Enum.GetValues<ThemeResourceType>())
        {
            string basePath = _resourcePaths[resourceType];
            EnsureDirectoryExists(basePath);
            foreach (string themeDir in Directory.GetDirectories(basePath))
            {
                string themeType = Path.GetFileName(themeDir);
                foreach (string themeFile in Directory.GetFiles(themeDir, "*.xaml"))
                {
                    string themeName = Path.GetFileNameWithoutExtension(themeFile);
                    var resourceDict = Load(themeFile);

                    _themeManager.RegisterAndUpdateTheme(resourceType, themeType, themeName, resourceDict);
                }
            }
        }
    }

    /// <summary>
    /// 从默认主题加载主题并注册到 ThemeManager
    /// </summary>
    private void LoadThemesFromDefaultThemes()
    {
        foreach (var resourceType in Enum.GetValues<ThemeResourceType>())
        {
            if (!DefaultThemes.TryGetValue(resourceType, out Dictionary<string, string>? value))
                continue;

            foreach (var themeInfo in value)
            {
                string themeType = themeInfo.Key;
                string themeName = themeInfo.Value;

                _themeManager.RegisterAndUpdateTheme(resourceType, themeType, themeName);
            }
        }
    }

    /// <summary>
    /// 从文件系统重新加载并注册所有主题
    /// </summary>
    public void ReloadAllThemesFromFileSystem()
    {
        _themeManager.RemoveAllAppliedThemes();
        _themeManager.ClearCurrentThemesInfo();
        _themeManager.ThemeResources.Clear();

        LoadThemesFromDefaultThemes();
        LoadThemesFromFileSystem();
        ApplyAllPreferredThemes();
    }

    /// <summary>
    /// 从文件系统重新加载并注册特定主题
    /// </summary>
    /// <param name="themeResourceType">资源类型</param>
    /// <param name="themeType">主题类型</param>
    /// <param name="themeName">主题名称</param>
    public void ReloadThemeFromFileSystem(ThemeResourceType themeResourceType, string themeType, string themeName)
    {
        string themeFilePath = Path.Combine(_resourcePaths[themeResourceType], themeType, $"{themeName}.xaml");
        if (File.Exists(themeFilePath))
        {
            var resourceDict = Load(themeFilePath);
            _themeManager.RegisterAndUpdateTheme(themeResourceType, themeType, themeName, resourceDict);
        }
        else
        {
            throw new FileNotFoundException($"主题文件未找到：{themeFilePath}");
        }
    }

    /// <summary>
    /// 应用指定类别用户偏好的主题
    /// </summary>
    public void ApplyPreferredTheme(ThemeResourceType themeResourceType)
    {
        var preferences = _userPreferences.LoadUserPreferences();
        if (
            preferences[themeResourceType].TryGetValue("PreferenceThemeType", out string? currentThemeType)
            && preferences[themeResourceType].TryGetValue(currentThemeType, out string? preferredThemeName)
            && _themeManager.IsThemeRegistered(themeResourceType, currentThemeType, preferredThemeName)
        )
        {
            _themeManager.ApplyTheme(themeResourceType, currentThemeType, preferredThemeName);
        }
        else
        {
            if (DefaultThemes.TryGetValue(themeResourceType, out Dictionary<string, string>? value))
            {
                var defaultTheme = value.FirstOrDefault();
                _themeManager.ApplyTheme(themeResourceType, defaultTheme.Key, defaultTheme.Value);
            }
        }
    }

    /// <summary>
    /// 应用所有类别用户偏好的主题
    /// </summary>
    private void ApplyAllPreferredThemes()
    {
        foreach (var resourceType in Enum.GetValues<ThemeResourceType>())
        {
            ApplyPreferredTheme(resourceType);
        }
    }

    /// <summary>
    /// 保存当前应用的主题为用户偏好
    /// </summary>
    private void SaveCurrentThemeAsPreference(ThemeResourceType themeResourceType)
    {
        try
        {
            (string themeType, string themeName) = _themeManager.GetCurrentThemeInfo(themeResourceType);
            var preferences = _userPreferences.LoadUserPreferences();
            preferences[themeResourceType][themeType] = themeName;
            preferences[themeResourceType]["PreferenceThemeType"] = themeType;
            _userPreferences.SaveUserPreferences(preferences);
        }
        catch
        {
            throw; // TODO 处理异常
        }
    }

    /// <summary>
    /// 获取指定主题资源类型的偏好设置
    /// </summary>
    /// <returns>用户偏好设置字典</returns>
    public Dictionary<string, string> GetUserPreferences(ThemeResourceType themeResourceType)
    {
        return _userPreferences.LoadUserPreferences()[themeResourceType];
    }

    /// <summary>
    /// 尝试应用主题并保存偏好
    /// </summary>
    /// <param name="themeResourceType">资源类型</param>
    /// <param name="themeType">主题类型</param>
    /// <param name="themeName">主题名称</param>
    public void TryApplyTheme(ThemeResourceType themeResourceType, string themeType, string themeName, bool isChangePreference = true)
    {
        try
        {
            _themeManager.ApplyTheme(themeResourceType, themeType, themeName);
            if (isChangePreference) SaveCurrentThemeAsPreference(themeResourceType);
        }
        catch (ArgumentException ex)
        {
            throw new InvalidOperationException($"无法应用主题：{ex.Message}", ex);
        }
    }

    /// <summary>
    /// 切换到指定类别的首选主题。
    /// </summary>
    /// <param name="themeResourceType">资源类型。</param>
    /// <param name="themeType">主题类型。</param>
    public void SwitchToPreferredTheme(ThemeResourceType themeResourceType, string themeType, bool isChangePreference = true)
    {
        string? preferredThemeName = GetPreferredThemeName(themeResourceType, themeType);
        if (preferredThemeName != null)
        {
            TryApplyTheme(themeResourceType, themeType, preferredThemeName, isChangePreference);
        }
    }

    /// <summary>
    /// 获取指定类别的首选主题名称。
    /// </summary>
    /// <param name="themeResourceType">资源类型。</param>
    /// <param name="themeType">主题类型。</param>
    /// <returns>首选主题名称，如果不存在则返回 null。</returns>
    private string? GetPreferredThemeName(ThemeResourceType themeResourceType, string themeType)
    {
        if (
            !_themeManager
                .ThemeResources[themeResourceType]
                .TryGetValue(themeType, out Dictionary<string, ResourceDictionary>? themeList)
            || themeList.Count == 0
        )
            return null;

        Dictionary<string, string> preferences = GetUserPreferences(themeResourceType);
        return preferences.GetValueOrDefault(themeType) ?? themeList.Keys.FirstOrDefault();
    }

    /// <summary>
    /// 添加新主题并保存到文件系统
    /// </summary>
    /// <param name="themeResourceType">资源类型</param>
    /// <param name="themeType">主题类型（例如：亮色主题、暗色主题）</param>
    /// <param name="themeName">主题名称</param>
    /// <param name="resourceDict">新的资源字典</param>
    public void AddThemeAndSaveToFileSystem(
        ThemeResourceType themeResourceType,
        string themeType,
        string themeName,
        ResourceDictionary resourceDict
    )
    {
        string filePath = Path.Combine(_resourcePaths[themeResourceType], themeType, $"{themeName}.xaml");
        EnsureDirectoryExists(Path.GetDirectoryName(filePath)!);
        Save(resourceDict, filePath);
        _themeManager.RegisterAndUpdateTheme(themeResourceType, themeType, themeName, resourceDict);
    }

    /// <summary>
    /// 删除指定主题及其对应的 XAML 文件
    /// </summary>
    /// <param name="themeResourceType">资源类型</param>
    /// <param name="themeType">主题类型（例如：亮色主题、暗色主题）</param>
    /// <param name="themeName">主题名称</param>
    public void DeleteThemeAndDeleteFileSystemEntry(
        ThemeResourceType themeResourceType,
        string themeType,
        string themeName
    )
    {
        string filePath = Path.Combine(_resourcePaths[themeResourceType], themeType, $"{themeName}.xaml");
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        _themeManager.RemoveTheme(themeResourceType, themeType, themeName);
    }

    /// <summary>
    /// 更新指定主题并保存到文件系统
    /// </summary>
    /// <param name="themeResourceType">资源类型</param>
    /// <param name="themeType">主题类型（例如：亮色主题、暗色主题）</param>
    /// <param name="themeName">主题名称</param>
    /// <param name="newResourceDict">新的资源字典</param>
    public void UpdateThemeAndSaveToFileSystem(
        ThemeResourceType themeResourceType,
        string themeType,
        string themeName,
        ResourceDictionary? newResourceDict = null
    )
    {
        string filePath = Path.Combine(_resourcePaths[themeResourceType], themeType, $"{themeName}.xaml");
        EnsureFileExists(Path.GetDirectoryName(filePath)!);
        newResourceDict ??= _themeManager.GetSpecifiedTheme(themeResourceType, themeType, themeName);
        Save(newResourceDict, filePath);
        _themeManager.RegisterAndUpdateTheme(themeResourceType, themeType, themeName, newResourceDict);
    }

    /// <summary>
    /// 修改指定主题的主题名和主题类型，并保存到文件系统
    /// </summary>
    /// <param name="themeResourceType">资源类型</param>
    /// <param name="oldThemeType">旧主题类型</param>
    /// <param name="oldThemeName">旧主题名称</param>
    /// <param name="newThemeType">新主题类型</param>
    /// <param name="newThemeName">新主题名称</param>
    public void RenameThemeAndSaveToFileSystem(
        ThemeResourceType themeResourceType,
        string oldThemeType,
        string oldThemeName,
        string newThemeType,
        string newThemeName
    )
    {
        try
        {
            (string finalThemeType, string finalThemeName) = ValidateAndPrepareRename(
                themeResourceType,
                oldThemeType,
                oldThemeName,
                newThemeType,
                newThemeName
            );
            _themeManager.RenameTheme(themeResourceType, oldThemeType, oldThemeName, finalThemeType, finalThemeName);
            if (oldThemeType != finalThemeType)
            {
                MoveThemeFileOnDisk(themeResourceType, oldThemeType, oldThemeName, finalThemeType, finalThemeName);
            }
        }
        catch (InvalidOperationException ex)
        {
            throw new Exception($"无效操作：{ex.Message}", ex);
        }
    }

    /// <summary>
    /// 验证并准备重命名操作
    /// </summary>
    /// <param name="themeResourceType">资源类型</param>
    /// <param name="oldThemeType">旧主题类型</param>
    /// <param name="oldThemeName">旧主题名称</param>
    /// <param name="newThemeType">新主题类型</param>
    /// <param name="newThemeName">新主题名称</param>
    /// <returns>返回最终的主题类型和名称</returns>
    private (string finalThemeType, string finalThemeName) ValidateAndPrepareRename(
        ThemeResourceType themeResourceType,
        string oldThemeType,
        string oldThemeName,
        string newThemeType,
        string newThemeName
    )
    {
        if (string.IsNullOrEmpty(oldThemeType) || string.IsNullOrEmpty(oldThemeName))
        {
            throw new ArgumentException("旧主题类型和名称不能同时为空或空白。");
        }
        if (string.IsNullOrWhiteSpace(newThemeType) && string.IsNullOrWhiteSpace(newThemeName))
        {
            throw new ArgumentException("新主题类型和名称不能同时为空或空白。");
        }
        if (oldThemeType == newThemeType && oldThemeName == newThemeName)
        {
            throw new ArgumentException("新主题类型/名称至少有一项与旧主题不同。");
        }
        _themeManager.EnsureThemeTypeExists(themeResourceType, oldThemeType);
        if (!_themeManager.IsThemeRegistered(themeResourceType, oldThemeType, oldThemeName))
        {
            throw new ArgumentException("旧主题未找到。");
        }
        string finalThemeType = string.IsNullOrWhiteSpace(newThemeType) ? oldThemeType : newThemeType.Trim();
        string finalThemeName = string.IsNullOrWhiteSpace(newThemeName) ? oldThemeName : newThemeName.Trim();
        return _themeManager.IsThemeRegistered(themeResourceType, finalThemeType, finalThemeName)
            ? throw new ArgumentException("新主题已存在。")
            : (finalThemeType, finalThemeName);
    }

    /// <summary>
    /// 在磁盘上移动主题文件以反映重命名操作
    /// </summary>
    /// <param name="themeResourceType">资源类型</param>
    /// <param name="oldThemeType">旧主题类型</param>
    /// <param name="oldThemeName">旧主题名称</param>
    /// <param name="newThemeType">新主题类型</param>
    /// <param name="newThemeName">新主题名称</param>
    private void MoveThemeFileOnDisk(
        ThemeResourceType themeResourceType,
        string oldThemeType,
        string oldThemeName,
        string newThemeType,
        string newThemeName
    )
    {
        string oldFilePath = Path.Combine(_resourcePaths[themeResourceType], oldThemeType, $"{oldThemeName}.xaml");
        string newFilePath = Path.Combine(_resourcePaths[themeResourceType], newThemeType, $"{newThemeName}.xaml");

        if (oldFilePath == newFilePath)
            return;

        EnsureFileExists(newFilePath);
        File.Move(oldFilePath, newFilePath);
    }

    /// <summary>
    /// 加载资源字典
    /// </summary>
    /// <param name="filePath">XAML 文件路径</param>
    /// <returns>资源字典</returns>
    private static ResourceDictionary Load(string filePath)
    {
        using FileStream stream = new(filePath, FileMode.Open, FileAccess.Read);
        return (ResourceDictionary)XamlReader.Load(stream) ?? throw new InvalidDataException("无法加载资源字典。");
    }

    /// <summary>
    /// 保存资源字典到文件
    /// </summary>
    /// <param name="resourceDict">资源字典</param>
    /// <param name="filePath">目标文件路径</param>
    private static void Save(ResourceDictionary resourceDict, string filePath)
    {
        XmlWriterSettings settings = new()
        {
            Indent = true,
            IndentChars = " ",
            NewLineOnAttributes = false,
            Encoding = Encoding.UTF8,
        };
        using FileStream stream = new(filePath, FileMode.Create, FileAccess.Write);
        using var writer = XmlWriter.Create(stream, settings);
        XamlWriter.Save(resourceDict, writer);
    }
}
