using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UselessTool.Bases.FileOperation;
using UselessTool.Model;
using UselessTool.Model.ThemeControlModel;
using UselessTool.Theme.Service;

namespace UselessTool.Common;

#region 枚举

public enum ColorsThemeType
{
    Light,
    Dark,
}

#endregion

/// <summary>
/// 主题控制相关类。
/// </summary>
public partial class ThemeControl : ObservableObject
{
    private static readonly Lazy<ThemeControl> Instance = new(() => new ThemeControl(), true);

    public static ThemeControl GetInstance => Instance.Value;

    private ThemeControl()
    {
        ThemeManager = new ThemeManager { AssemblyName = "UselessTool.Assets", ResourceBasedPath = "Themes" };
        ThemeService = new ThemeService(ThemeManager, DefaultThemes, ThemePath);

        ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;

        LoadIsFollowSystemDarkModeConfig();

        InitializeColorsThemeConfig();
        LoadThemeView();
        InitializeColorsDictionary();
        InitializeConfigurationPanel();
    }

    ~ThemeControl()
    {
        ThemeManager.ThemeChanged -= ThemeManager_ThemeChanged;
    }

    /// <summary>
    /// 主题管理器主题变更事件。
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ThemeManager_ThemeChanged(object? sender, ThemeChangedEventArgs e)
    {
        if (e.ResourceType != ThemeResourceType.Colors)
            return;

        CurrentColorsThemeResourceDictionary = e.NewTheme; // 更新当前颜色主题字典
        CurrentColorsThemeInfo = (e.ThemeType, e.ThemeName);
        SetIsNightChecked(e.ThemeType);
    }

    #region 常量

    private const string ThemePath = "Theme";

    #endregion

    #region 属性

    public static Dictionary<ThemeResourceType, Dictionary<string, string[]>> DefaultThemes { get; } =
        new()
        {
            {
                ThemeResourceType.Colors,
                new Dictionary<string, string[]> { { "Light", ["White"] }, { "Dark", ["Black"] } }
            },
        };
    public static Dictionary<ColorsThemeType, string> ThemeTypeDic { get; } =
        new() { { ColorsThemeType.Light, "Light" }, { ColorsThemeType.Dark, "Dark" } };
    public static ResourceDictionary CurrentColorsThemeResourceDictionary { get; private set; } = [];
    public static (string themeType, string themeName) CurrentColorsThemeInfo { get; private set; }
    private JsonConfig<Dictionary<string, bool>> OtherJsonConfig { get; } = new(ThemePath, "other_config.json");
    public ThemeManager ThemeManager { get; }
    public ThemeService ThemeService { get; }

    #region 通知属性

    /// <summary>
    /// 当前主题是否为暗色主题。
    /// </summary>
    [ObservableProperty]
    private bool _isNightChecked;

    /// <summary>
    /// 是否弹出主题视图。
    /// </summary>
    [ObservableProperty]
    private bool _isOpenThemeView;

    /// <summary>
    /// 主题项集合。
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<ThemeItemsModel> _themeItems = [];

    /// <summary>
    /// 是否允许连续随机化颜色。
    /// </summary>
    [ObservableProperty]
    private bool _isContinuousRandomizationColorsAllowed;

    /// <summary>
    /// 是否跟随系统暗色模式。
    /// </summary>
    [ObservableProperty]
    private bool _isFollowSystemDarkMode;

    #endregion
    #endregion

    #region 方法

    #region 命令处理

    /// <summary>
    /// 切换主题视图显示状态。
    /// </summary>
    [RelayCommand]
    private void ToggleThemeView()
    {
        IsOpenThemeView = !IsOpenThemeView;
    }

    /// <summary>
    /// 刷新主题视图。
    /// </summary>
    [RelayCommand]
    private void RefreshThemeView()
    {
        ThemeService.ReloadAllThemesFromFileSystem();
        LoadThemeView();
    }

    /// <summary>
    /// 切换日间模式和夜间模式。
    /// </summary>
    [RelayCommand]
    private void ToggleDayModeNightMode()
    {
        IsNightChecked = !IsNightChecked;
        IsFollowSystemDarkMode = false;
        ThemeService.SwitchToPreferredTheme(
            ThemeResourceType.Colors,
            IsNightChecked ? ThemeTypeDic[ColorsThemeType.Dark] : ThemeTypeDic[ColorsThemeType.Light]
        );
    }

    /// <summary>
    /// 切换主题。
    /// </summary>
    /// <param name="themeButtonModel">包含主题信息的模型。</param>
    [RelayCommand]
    private void ToggleTheme(ThemeButtonModel themeButtonModel)
    {
        ThemeService.TryApplyTheme(ThemeResourceType.Colors, themeButtonModel.ThemeType, themeButtonModel.ThemeName);
        IsFollowSystemDarkMode = false;
    }

    /// <summary>
    /// 更新主题。
    /// </summary>
    [RelayCommand]
    private void UpdateTheme()
    {
        if (
            ThemeService.IsDefaultTheme(
                ThemeResourceType.Colors,
                CurrentColorsThemeInfo.themeType,
                CurrentColorsThemeInfo.themeName
            )
        )
        {
            MessageBox.Show("无法修改默认主题！");
            return;
        }
        ThemeService.UpdateThemeAndSaveToFileSystem(
            ThemeResourceType.Colors,
            CurrentColorsThemeInfo.themeType,
            CurrentColorsThemeInfo.themeName
        );
    }

    /// <summary>
    /// 重新加载当前主题。
    /// </summary>
    [RelayCommand]
    private void ReloadCurrentTheme()
    {
        try
        {
            ThemeService.ReloadThemeFromFileSystem(
                ThemeResourceType.Colors,
                CurrentColorsThemeInfo.themeType,
                CurrentColorsThemeInfo.themeName
            );
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{ex.Message}\n");
        }
    }

    /// <summary>
    /// 设置是否跟随系统暗色模式配置。
    /// </summary>
    [RelayCommand]
    private void UpdateIsFollowSystemDarkModeConfig()
    {
        try
        {
            var otherConfig =
                OtherJsonConfig.LoadFromJson() ?? new Dictionary<string, bool> { { "IsFollowSystemDarkMode", false } };
            otherConfig["IsFollowSystemDarkMode"] = IsFollowSystemDarkMode;

            OtherJsonConfig.SaveToJson(otherConfig);
        }
        catch (IOException iex)
        {
            MessageBox.Show($"{iex.Message}", "保存出错");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"处理主题配置>其他配置时出错：{ex.Message}");
        }
    }

    /// <summary>
    /// 设置随机颜色。
    /// </summary>
    [RelayCommand]
    private async Task SetRandomColorsAsync()
    {
        if (!IsContinuousRandomizationColorsAllowed)
            await RandomlyThemedColorsAsync();

        while (IsContinuousRandomizationColorsAllowed)
        {
            await RandomlyThemedColorsAsync();
        }
    }

    #endregion

    #region 辅助方法

    /// <summary>
    /// 加载是否跟随系统暗色模式配置。
    /// </summary>
    private void LoadIsFollowSystemDarkModeConfig()
    {
        try
        {
            var otherConfig =
                OtherJsonConfig.LoadFromJson() ?? new Dictionary<string, bool> { { "IsFollowSystemDarkMode", false } };
            IsFollowSystemDarkMode = otherConfig["IsFollowSystemDarkMode"];
        }
        catch
        {
            IsFollowSystemDarkMode = false;
            OtherJsonConfig.SaveToJson(new Dictionary<string, bool> { { "IsFollowSystemDarkMode", false } });
        }
    }

    /// <summary>
    /// 初始化主题配置。
    /// </summary>
    private void InitializeColorsThemeConfig()
    {
        ThemeManager.RegisterAndUpdateTheme(ThemeResourceType.Colors, "Light", "White");
        ThemeManager.RegisterAndUpdateTheme(ThemeResourceType.Colors, "Dark", "Black");

        ThemeService.LoadThemesFromFileSystem();

        if (IsFollowSystemDarkMode)
            FollowSystemToggleLightOrDarkTheme();
        else
            ThemeService.ApplyPreferredTheme(ThemeResourceType.Colors);
    }

    /// <summary>
    /// 加载主题视图。
    /// </summary>
    public void LoadThemeView()
    {
        ThemeItems.Clear(); // 清空现有主题项

        var themes = ThemeManager.GetThemeResourceTypeDictionary(ThemeResourceType.Colors);
        var userPreferences = ThemeService.GetUserPreferences(ThemeResourceType.Colors);

        foreach (string themeType in themes.Keys)
        {
            var themeButtons = themes[themeType]
                .Select(theme =>
                {
                    string themeName = theme.Key;
                    var resourceDict = theme.Value;

                    var baseAccentColor =
                        resourceDict.Contains("Base.AccentColor")
                        && resourceDict["Base.AccentColor"] is SolidColorBrush colorBrush
                            ? colorBrush
                            : new SolidColorBrush(Color.FromRgb(0x00, 0x7A, 0xCC)); // 默认强调颜色

                    bool isPreferredTheme =
                        userPreferences.TryGetValue(themeType, out string? preferredThemeName)
                        && preferredThemeName == themeName;

                    return new ThemeButtonModel(baseAccentColor, themeType, themeName, isPreferredTheme);
                });

            ThemeItems.Add(new ThemeItemsModel(themeType, themeButtons));
        }
    }

    /// <summary>
    /// 更新夜间模式选中状态。
    /// </summary>
    /// <param name="themeType">主题类型。</param>
    private void SetIsNightChecked(string themeType)
    {
        IsNightChecked = ThemeTypeDic[ColorsThemeType.Dark] == themeType;
    }

    /// <summary>
    /// 随机化主题颜色。
    /// </summary>
    private async Task RandomlyThemedColorsAsync()
    {
        var currentTheme = ThemeManager.GetCurrentTheme(ThemeResourceType.Colors);
        var random = new Random();
        bool isDarkMode = IsFollowSystemDarkMode ? App.IsSystemInDarkMode : IsNightChecked;

        foreach (object? key in currentTheme.Keys)
        {
            bool isTextOrAccentColor =
                key.ToString()?.Contains("Text") == true || key.ToString()?.Contains("AccentColor") == true;
            var randomColor = isDarkMode
                ? isTextOrAccentColor
                    ? Color.FromRgb(
                        (byte)random.Next(128, 256),
                        (byte)random.Next(128, 256),
                        (byte)random.Next(128, 256)
                    ) // 亮色区间
                    : Color.FromRgb((byte)random.Next(0, 128), (byte)random.Next(0, 128), (byte)random.Next(0, 128)) // 暗色区间
                : isTextOrAccentColor
                    ? Color.FromRgb((byte)random.Next(0, 128), (byte)random.Next(0, 128), (byte)random.Next(0, 128)) // 暗色区间
                    : Color.FromRgb(
                        (byte)random.Next(128, 256),
                        (byte)random.Next(128, 256),
                        (byte)random.Next(128, 256)
                    ); // 亮色区间
            currentTheme[key] = new SolidColorBrush(randomColor);
        }
        // 等待一段时间再进行下一次随机化
        await Task.Delay(500); // 500 毫秒
    }

    /// <summary>
    /// 跟随系统切换浅色/深色主题。
    /// </summary>
    public void FollowSystemToggleLightOrDarkTheme()
    {
        string themeType = App.IsSystemInDarkMode
            ? ThemeTypeDic[ColorsThemeType.Dark]
            : ThemeTypeDic[ColorsThemeType.Light];

        if (!IsFollowSystemDarkMode || themeType == CurrentColorsThemeInfo.themeType)
            return;

        ThemeService.SwitchToPreferredTheme(ThemeResourceType.Colors, themeType, false);
        IsNightChecked = App.IsSystemInDarkMode;
    }

    #endregion

    #region 主题配置

    /// <summary>
    /// 颜色类型集合。
    /// </summary>
    public ObservableCollection<ColorsCategoriesModel> ColorsCategoriesModels { get; private set; } = [];

    /// <summary>
    /// 资源键到显示名称的映射。
    /// </summary>
    private Dictionary<ThemeResourceType, Dictionary<string, string>> _resourceKeyToDisplayNameMap = [];

    /// <summary>
    /// 初始化 JsonConfig 并加载颜色名字典配置
    /// </summary>
    private void InitializeColorsDictionary()
    {
        var themeConfig = new JsonConfig<Dictionary<ThemeResourceType, Dictionary<string, string>>>(
            "Theme",
            "displayNameMap.json"
        );
        try
        {
            _resourceKeyToDisplayNameMap = themeConfig.LoadFromJson() ?? GetDefaultResourceKeyToDisplayNameMap();
        }
        catch (Exception)
        {
            _resourceKeyToDisplayNameMap = GetDefaultResourceKeyToDisplayNameMap();
            themeConfig.SaveToJson(GetDefaultResourceKeyToDisplayNameMap());
        }
    }

    /// <summary>
    /// 提供默认的资源键映射字典
    /// </summary>
    /// <returns></returns>
    private static Dictionary<ThemeResourceType, Dictionary<string, string>> GetDefaultResourceKeyToDisplayNameMap()
    {
        return new Dictionary<ThemeResourceType, Dictionary<string, string>>
        {
            {
                ThemeResourceType.Colors,
                new Dictionary<string, string>
                {
                    // 基础颜色
                    { "Base.BackgroundColor", "基础背景色" },
                    { "Base.ForegroundColor", "基础前景色" },
                    { "Base.AccentColor", "基础强调色" },
                    // 控件背景色
                    { "Control.BackgroundColor", "控件背景色" },
                    { "Control.HoverBackgroundColor", "鼠标悬停时的控件背景色" },
                    { "Control.PressedBackgroundColor", "按下时的控件背景色" },
                    // 控件边框色
                    { "Control.BorderBrushColor", "控件边框色" },
                    { "Control.HoverBorderBrushColor", "鼠标悬停时的控件边框色" },
                    { "Control.PressedBorderBrushColor", "按下时的控件边框色" },
                    // 文本颜色
                    { "Text.ForegroundColor", "文本前景色" },
                    { "Text.DeputyLevelColor", "次级文本颜色" },
                    { "Text.PlaceholderColor", "占位符文本颜色" },
                    // 边框和分隔线颜色
                    { "Border.Color", "默认边框颜色" },
                    { "Border.AccentColor", "强调边框颜色" },
                    { "Separator.Color", "分隔线颜色" },
                    // 交互元素颜色
                    { "Hyperlink.Color", "超链接颜色" },
                    { "Hyperlink.VisitedColor", "已访问超链接颜色" },
                    // 其他UI元素颜色
                    { "Header.BackgroundColor", "头部背景色" },
                    { "Header.TextColor", "头部文本颜色" },
                    { "Footer.BackgroundColor", "页脚背景色" },
                    { "Footer.TextColor", "页脚文本颜色" },
                }
            },
        };
    }

    /// <summary>
    /// 初始化颜色配置面板。
    /// </summary>
    private void InitializeConfigurationPanel()
    {
        var categoryMap = new Dictionary<string, ColorsCategoriesModel>
        {
            { "基础颜色", new ColorsCategoriesModel("基础颜色") },
            { "控件颜色", new ColorsCategoriesModel("控件颜色") },
            { "文本颜色", new ColorsCategoriesModel("文本颜色") },
            { "边框颜色", new ColorsCategoriesModel("边框颜色") },
            { "交互颜色", new ColorsCategoriesModel("交互颜色") },
            { "其他颜色", new ColorsCategoriesModel("其他颜色") },
        };

        // 按照预定义顺序填充项目
        foreach (string key in _resourceKeyToDisplayNameMap[ThemeResourceType.Colors].Keys)
        {
            if (!CurrentColorsThemeResourceDictionary.Contains(key))
                continue;

            string category = key switch
            {
                _ when key.StartsWith("Base.") => "基础颜色",
                _ when key.StartsWith("Control.") => "控件颜色",
                _ when key.StartsWith("Text.") => "文本颜色",
                _ when key.StartsWith("Border.") || key.StartsWith("Separator.") => "边框颜色",
                _ when key.StartsWith("Hyperlink.") => "交互颜色",
                _ => "其他颜色",
            };

            if (_resourceKeyToDisplayNameMap[ThemeResourceType.Colors].TryGetValue(key, out string? colorName))
            {
                categoryMap[category].ColorsNameModels.Add(new ColorsDisplayNameModel(key, colorName));
            }
        }

        ColorsCategoriesModels = new ObservableCollection<ColorsCategoriesModel>(
            categoryMap.Values.Where(c => c.ColorsNameModels.Any())
        );
    }

    #endregion

    #endregion
}
