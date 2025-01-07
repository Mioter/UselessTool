using System.Collections.ObjectModel;
using System.Windows.Media;

namespace UselessTool.Model;

public class ThemeItemsModel(string themeType, IEnumerable<ThemeButtonModel> themeButtons)
{
    public string ThemeType { get; } = themeType;
    public ObservableCollection<ThemeButtonModel> ThemeButtons { get; } = new(themeButtons);
}

public class ThemeButtonModel(
    SolidColorBrush baseAccentColor,
    string themeType,
    string themeName,
    bool isChecked = false
)
{
    public SolidColorBrush BaseAccentColor { get; } = baseAccentColor;
    public string ThemeType { get; } = themeType;
    public string ThemeName { get; } = themeName;
    public bool IsChecked { get; set; } = isChecked;
}
