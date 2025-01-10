using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace UselessTool.Model.ThemeControlModel;

public class ColorsDisplayNameModel : ObservableObject
{
    private string Key { get; }

    public string DisplayName { get; }

    public ColorsDisplayNameModel(string key, string displayName)
    {
        Key = key;
        DisplayName = displayName;
        SelectedBrush = Common.ThemeControl.CurrentColorsThemeResourceDictionary[Key] as SolidColorBrush;
    }

    private SolidColorBrush? _selectedBrush;
    public SolidColorBrush? SelectedBrush
    {
        get => _selectedBrush;
        set
        {
            if (SetProperty(ref _selectedBrush, value))
            {
                Common.ThemeControl.CurrentColorsThemeResourceDictionary[Key] = SelectedBrush;
            }
        }
    }

}