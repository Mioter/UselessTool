using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace UselessTool.Model.ThemeControlModel;

public class ColorsDisplayNameModel : ObservableObject
{
    public string Key { get; set; }

    public string DisplayName { get; }

    public ColorsDisplayNameModel(string key, string displayName)
    {
        Key = key;
        DisplayName = displayName;
        SelectedBrush = Common.ThemeControl.CurrentThemeDict[Key] as SolidColorBrush;
    }

    private SolidColorBrush? _selectedBrush;
    public SolidColorBrush? SelectedBrush
    {
        get => _selectedBrush;
        set
        {
            if (SetProperty(ref _selectedBrush, value))
            {
                Common.ThemeControl.CurrentThemeDict[Key] = SelectedBrush;
            }
        }
    }

}