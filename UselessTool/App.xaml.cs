using System.Windows;
using Microsoft.Win32;
using UselessTool.Common;

namespace UselessTool;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private const string RegistryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
    private const string RegistryValueName = "AppsUseLightTheme";

    public static bool IsSystemInDarkMode { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
        DetectTheme();
    }

    private void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
    {
        if (e.Category == UserPreferenceCategory.General)
        {
            DetectTheme();
        }
    }

    private static void DetectTheme()
    {
        IsSystemInDarkMode = !IsLightTheme();
        ThemeControl.GetInstance.FollowSystemToggleLightOrDarkTheme();
    }

    private static bool IsLightTheme()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath);
        object? registryValueObject = key?.GetValue(RegistryValueName);
        if (registryValueObject == null) return true; // 默认返回Light Theme
        int registryValue = (int)registryValueObject;
        return registryValue == 1;
    }

    protected override void OnExit(ExitEventArgs e)
    {
        SystemEvents.UserPreferenceChanged -= OnUserPreferenceChanged;
        base.OnExit(e);
    }
}
