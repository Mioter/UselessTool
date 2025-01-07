using UselessTool.Common;

namespace UselessTool.View.UserControls;

/// <summary>
/// ThemeToggleView.xaml 的交互逻辑
/// </summary>
public partial class ThemeToggleView
{
    public ThemeToggleView()
    {
        InitializeComponent();
        DataContext = ThemeControl.GetInstance;
    }
}