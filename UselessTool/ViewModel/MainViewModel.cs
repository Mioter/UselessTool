using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UselessTool.Common;
using UselessTool.Model;
using UselessTool.Theme.Tools;
using static UselessTool.Amusing.TextProcess.EmotionEmoticonGenerator;
using static UselessTool.Theme.Tools.DefaultTheme;

namespace UselessTool.ViewModel;

public partial class MainViewModel : ObservableObject
{
    public TitleBarViewMode TitleBarViewMode { get; } = TitleBarViewMode.GetInstance;

    public ThemeControl ThemeControl { get; } = ThemeControl.GetInstance;

    public MainViewModel()
    {
        foreach (string themeType in Enum.GetNames<ColorsThemeType>())
            ComboBoxItems.Add(themeType);

        // 为什么要使用 GetCurrentThemeInfo(ThemeResourceType.Colors).themeType，因为此时主题已经更改，所以需要手动获取当前主题。
        ComboBoxSelectedItem = ThemeControl.ThemeManager.GetCurrentThemeInfo(ThemeResourceType.Colors).themeType;
        // 在订阅主题更改事件后的每次更改主题时，都会触发此事件修改 ComboBoxSelectedItem 的值。
        ThemeControl.ThemeManager.ThemeChanged += (_, arg) =>
        {
            if (arg.ResourceType == ThemeResourceType.Colors)
                ComboBoxSelectedItem = ComboBoxItems.FirstOrDefault(p => p == arg.ThemeType);
        };
    }

    [ObservableProperty]
    private string? _tipsText;

    [ObservableProperty]
    private string? _nameOfThemesAdded;

    [ObservableProperty]
    private string? _comboBoxSelectedItem;

    [ObservableProperty]
    private List<string> _comboBoxItems = [];

    #region 主题管理

    /// <summary>
    /// 添加主题
    /// </summary>
    [RelayCommand]
    private void AddTheme()
    {
        if (string.IsNullOrEmpty(NameOfThemesAdded))
        {
            TipsText = AddRandomEmoticon("请输入有效的主题名称！", EmotionType.Angry, AdditionalRules.Cat);
            return;
        }

        if (ComboBoxSelectedItem is null)
        {
            TipsText = AddRandomEmoticon("请先选择颜色主题类型。", EmotionType.Bored, AdditionalRules.Cat);
            return;
        }
        if (
            ThemeControl.ThemeManager.IsThemeRegistered(
                ThemeResourceType.Colors,
                ComboBoxSelectedItem,
                NameOfThemesAdded
            )
        )
        {
            TipsText = AddRandomEmoticon($"{NameOfThemesAdded}主题已存在。", EmotionType.Sad, AdditionalRules.Lady);
            return;
        }
        try
        {
            ThemeControl.ThemeService.AddThemeAndSaveToFileSystem(
                ThemeResourceType.Colors,
                ComboBoxSelectedItem,
                NameOfThemesAdded,
                ThemeControl.ThemeManager.GetCurrentTheme(ThemeResourceType.Colors)
            );
            ThemeControl.LoadThemeView();
        }
        catch (Exception ex)
        {
            TipsText = AddRandomEmoticon($"{ex.Message}+_+", EmotionType.Angry, AdditionalRules.Tsundere);
        }
    }

    /// <summary>
    /// 移除主题
    /// </summary>
    /// <param name="themeButtonModel"></param>
    [RelayCommand]
    private void RemoveTheme(ThemeButtonModel themeButtonModel)
    {
        (string themeType, string themeName) = ThemeControl.ThemeManager.GetCurrentThemeInfo(ThemeResourceType.Colors);

        if (themeButtonModel.ThemeType == themeType && themeButtonModel.ThemeName == themeName)
        {
            TipsText = AddRandomEmoticon(
                $"{themeButtonModel.ThemeName}主题正在使用，无法移除。",
                EmotionType.Sad,
                AdditionalRules.Lady
            );
            return;
        }

        if (
            DefaultThemes[ThemeResourceType.Colors].TryGetValue(themeButtonModel.ThemeType, out string? value)
            && value.Contains(themeButtonModel.ThemeName)
        )
        {
            TipsText = AddRandomEmoticon(
                $"{themeButtonModel.ThemeName}主题是默认主题，无法移除。\n就算移除了每次程序启动也会重新出现的。",
                EmotionType.Happy,
                AdditionalRules.Lady
            );
            return;
        }

        try
        {
            ThemeControl.ThemeService.DeleteThemeAndDeleteFileSystemEntry(
                ThemeResourceType.Colors,
                themeButtonModel.ThemeType,
                themeButtonModel.ThemeName
            );
            ThemeControl.LoadThemeView();
        }
        catch (Exception ex)
        {
            TipsText = AddRandomEmoticon(ex.Message, EmotionType.Angry, AdditionalRules.Tsundere);
        }
    }

    #endregion
}
