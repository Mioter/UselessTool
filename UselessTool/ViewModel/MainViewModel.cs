using System.Collections;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UselessTool.Common;
using UselessTool.Model;
using UselessTool.Theme.Tools;

namespace UselessTool.ViewModel;

public partial class MainViewModel : ObservableObject
{
    public TitleBarViewMode TitleBarViewMode { get; } = TitleBarViewMode.GetInstance;

    public ThemeControl ThemeControl { get; } = ThemeControl.GetInstance;

    public MainViewModel()
    {
        ThemeControl.ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;
    }

    private void ThemeManager_ThemeChanged(object? sender, ThemeChangedEventArgs e)
    {
        _currentThemeDict = e.NewTheme;
    }

    // 测试

    [ObservableProperty]
    private string? _tipsText;

    [ObservableProperty]
    private string? _nameOfThemesAdded;

    [ObservableProperty]
    private ComplexInfoModel? _combboxItem;

    [ObservableProperty]
    private ObservableCollection<ComplexInfoModel> _combboxList =
    [
        new() { Text = "Light" },
        new() { Text = "Dark" },
        new() { Text = "HighContrast" },
    ];

    #region 测试添加主题

    [RelayCommand]
    private void RefreshThemeView()
    {
        ThemeControl.RefreshThemeView();
    }

    [RelayCommand]
    private void TestAddTheme()
    {
        if (NameOfThemesAdded == null || CombboxItem?.Text == null) { }
    }

    [RelayCommand]
    private void RemoveTheme(ThemeButtonModel themeButtonModel)
    {
        (string themeType, string themeName) = ThemeControl.ThemeManager.GetCurrentThemeInfo(ThemeResourceType.Colour);

        if (string.IsNullOrEmpty(themeButtonModel.ThemeType) || string.IsNullOrEmpty(themeButtonModel.ThemeType))
        {
            TipsText = "请输入有效的主题类型和主题名称。";
            return;
        }

        if (themeButtonModel.ThemeType == themeType && themeButtonModel.ThemeName == themeName)
        {
            TipsText = $"{themeButtonModel.ThemeName}主题正在使用，无法移除。";
            return;
        }

        try
        {
            ThemeControl.ThemeService.DeleteThemeAndDeleteFileSystemEntry(
                ThemeResourceType.Colour,
                themeButtonModel.ThemeType,
                themeButtonModel.ThemeName
            );
            ThemeControl.LoadThemeView();
        }
        catch (Exception ex)
        {
            TipsText = ex.Message;
        }
    }

    #endregion

    public class ComplexInfoModel
    {
        public String? Text { get; set; }
    }

    private StackPanel? _configurationPanel = new();

    [RelayCommand]
    private void SetConfigurationPanel(object? stackPanel)
    {
        _configurationPanel = stackPanel as StackPanel;
        InitializeConfigurationPanel();
    }

    private ResourceDictionary _currentThemeDict = [];

    private readonly Dictionary<string, string> _resourceKeyToDisplayNameMap = new()
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
    };

    private void InitializeConfigurationPanel()
    {
        _currentThemeDict = ThemeControl.ThemeManager.GetCurrentTheme(ThemeResourceType.Colour);
        Dictionary<string, StackPanel> categories = new()
        {
            {
                "基础颜色",
                new StackPanel { Orientation = Orientation.Vertical }
            },
            {
                "控件颜色",
                new StackPanel { Orientation = Orientation.Vertical }
            },
            {
                "文本颜色",
                new StackPanel { Orientation = Orientation.Vertical }
            },
            {
                "边框颜色",
                new StackPanel { Orientation = Orientation.Vertical }
            },
            {
                "交互颜色",
                new StackPanel { Orientation = Orientation.Vertical }
            },
            {
                "其他颜色",
                new StackPanel { Orientation = Orientation.Vertical }
            },
        };

        foreach (DictionaryEntry kvp in _currentThemeDict)
        {
            string key = kvp.Key.ToString() ?? string.Empty;
            object? value = kvp.Value;

            if (!_resourceKeyToDisplayNameMap.TryGetValue(key, out string? displayName) || value == null)
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

            categories[category].Children.Add(CreateControlForResource(key, displayName, value));
        }

        foreach (KeyValuePair<string, StackPanel> category in categories)
        {
            EleCho.WpfSuite.Controls.Expander expander = new()
            {
                Header = category.Key,
                Content = category.Value,
                Margin = new Thickness(0, 0, 0, 10),
                Style = (Style)Application.Current.Resources["Expander.Base.Style"],
            };
            _configurationPanel?.Children.Add(expander);
        }
    }

    private StackPanel CreateControlForResource(string key, string displayName, object value)
    {
        StackPanel stackPanel = new() { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 5) };

        Label label = new()
        {
            Content = displayName,
            Width = 150,
            HorizontalAlignment = HorizontalAlignment.Left,
            Style = (Style)Application.Current.Resources["Label.Base.Style"],
        };
        stackPanel.Children.Add(label);

        if (value is SolidColorBrush solidColorBrush)
        {
            var brushes = typeof(Colors)
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Select(pi => new { pi.Name, Brush = new SolidColorBrush((Color)pi.GetValue(null)!) })
                .ToList();

            EleCho.WpfSuite.Controls.ComboBox colorPicker = new()
            {
                Width = 200,
                ItemsSource = brushes,
                SelectedValuePath = "Brush",
                DisplayMemberPath = "Name",
                SelectedValue = solidColorBrush,
                Style = (Style)Application.Current.Resources["ComboBox.Colors.Style"]
            };

            colorPicker.SelectionChanged += (_, _) =>
            {
                if (colorPicker.SelectedValue is SolidColorBrush selectedBrush)
                    _currentThemeDict[key] = selectedBrush;
            };

            stackPanel.Children.Add(colorPicker);
        }

        return stackPanel;
    }
}
