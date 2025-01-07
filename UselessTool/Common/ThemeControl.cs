using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UselessTool.Model;
using UselessTool.Theme.Tools;

namespace UselessTool.Common;

/// <summary>
/// 主题控制相关类。
/// </summary>
public partial class ThemeControl : ObservableObject
{
    private static readonly Lazy<ThemeControl> Instance = new(() => new ThemeControl(), true);

    public static ThemeControl GetInstance => Instance.Value;

    private ThemeControl()
    {
        ThemeManager = new ThemeManager();
        ThemeService = new ThemeService(ThemeManager, ThemePath);
        Initialize();
    }

    #region 枚举

    private enum ThemeType
    {
        Light,
        Dark
    }

    #endregion

    #region 字段

    private const string ThemePath = "Theme";
    private static readonly Dictionary<ThemeType, string> ThemeTypeDic = new()
    {
        { ThemeType.Light, "Light" },
        { ThemeType.Dark, "Dark" }
    };

    #endregion

    #region 属性

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

    #endregion
    #endregion

    #region 方法

    /// <summary>
    /// 切换主题视图显示状态。
    /// </summary>
    [RelayCommand]
    private void ToggleThemeView()
    {
        IsOpenThemeView = !IsOpenThemeView;
    }

    /// <summary>
    /// 切换日间模式和夜间模式。
    /// </summary>
    [RelayCommand]
    private void ToggleDayModeNightMode()
    {
        IsNightChecked = !IsNightChecked;
        ThemeService.SwitchToPreferredTheme(ThemeResourceType.Colour, IsNightChecked ? ThemeTypeDic[ThemeType.Dark] : ThemeTypeDic[ThemeType.Light]);
    }

    /// <summary>
    /// 切换主题。
    /// </summary>
    /// <param name="themeButtonModel">包含主题信息的模型。</param>
    [RelayCommand]
    private void ToggleTheme(ThemeButtonModel themeButtonModel)
    {
        string themeType = themeButtonModel.ThemeType;
        string themeName = themeButtonModel.ThemeName;

        if (!string.IsNullOrEmpty(themeType) && !string.IsNullOrEmpty(themeName))
        {
            ThemeService.TryApplyTheme(ThemeResourceType.Colour, themeType, themeName);
        }
        else
        {
            MessageBox.Show("未能找到对应的主题。");
        }
    }

    #region 辅助方法

    /// <summary>
    /// 初始化主题控制。
    /// </summary>
    private void Initialize()
    {
        InitializeThemeConfig();
        LoadThemeView();
    }

    /// <summary>
    /// 初始化主题配置。
    /// </summary>
    private void InitializeThemeConfig()
    {
        ThemeManager.RegisterAndUpdateTheme(
            ThemeResourceType.Colour,
            "Light",
            "White",
            "UselessTool.Theme",
            "/Resources/Colour/Light/White.xaml"
        );
        ThemeManager.RegisterAndUpdateTheme(
            ThemeResourceType.Colour,
            "Dark",
            "Black",
            "UselessTool.Theme",
            "/Resources/Colour/Dark/Black.xaml"
        );

        ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;

        ThemeService.LoadThemesFromFileSystem();
        ThemeService.ApplyPreferredTheme(ThemeResourceType.Colour);
    }

    /// <summary>
    /// 主题管理器主题变更事件。
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ThemeManager_ThemeChanged(object? sender, ThemeChangedEventArgs e)
    {
        if (e.ResourceType == ThemeResourceType.Colour)
            SetIsNightChecked(e.ThemeType);
    }

    /// <summary>
    /// 刷新主题视图。
    /// </summary>
    public void RefreshThemeView()
    {
        ThemeService.LoadThemesFromFileSystem();
        LoadThemeView();
    }

    /// <summary>
    /// 加载主题视图。
    /// </summary>
    public void LoadThemeView()
    {
        ThemeItems.Clear(); // 清空现有主题项

        Dictionary<string, Dictionary<string, ResourceDictionary>> themes = ThemeManager.Resources[ThemeResourceType.Colour];
        Dictionary<string, string> userPreferences = ThemeService.GetUserPreferences(ThemeResourceType.Colour);

        foreach (string themeType in themes.Keys)
        {
            List<ThemeButtonModel> themeButtons = themes[themeType]
                .Select(theme =>
                {
                    string themeName = theme.Key;
                    ResourceDictionary resourceDict = theme.Value;

                    SolidColorBrush baseAccentColor =
                        resourceDict.Contains("Base.AccentColor")
                        && resourceDict["Base.AccentColor"] is SolidColorBrush colorBrush
                            ? colorBrush
                            : new SolidColorBrush(Color.FromRgb(0x00, 0x7A, 0xCC)); // 默认强调颜色

                    bool isPreferredTheme =
                        userPreferences.TryGetValue(themeType, out string? preferredThemeName)
                        && preferredThemeName == themeName;

                    return new ThemeButtonModel(baseAccentColor, themeType, themeName, isPreferredTheme);
                })
                .ToList();

            ThemeItems.Add(new ThemeItemsModel(themeType, themeButtons));
        }
    }

    /// <summary>
    /// 更新主题，并刷新当前主题类型。
    /// </summary>
    /// <param name="themeType">主题类型。</param>
    private void SetIsNightChecked(string themeType)
    {
        if (ThemeTypeDic[ThemeType.Light] == themeType)
            IsNightChecked = false;

        if (ThemeTypeDic[ThemeType.Dark] == themeType)
            IsNightChecked = true;
    }

    #endregion

    #endregion
}
