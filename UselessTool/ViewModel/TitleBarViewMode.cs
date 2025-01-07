using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace UselessTool.ViewModel;

public partial class TitleBarViewMode : ObservableObject
{
    private static readonly Lazy<TitleBarViewMode> Instance = new(() => new TitleBarViewMode(), true);

    private TitleBarViewMode() { }

    public static TitleBarViewMode GetInstance => Instance.Value;

    [RelayCommand]
    private static void CloseWindow(Window window) => window.Close();

    [RelayCommand]
    private static void ToggleWindowState(Window window) =>
        window.WindowState = window.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

    [RelayCommand]
    private static void MinimizeWindow(Window window) => window.WindowState = WindowState.Minimized;
}
