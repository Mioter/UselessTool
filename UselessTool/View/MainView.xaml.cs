using UselessTool.ViewModel;

namespace UselessTool.View;

/// <summary>
/// Interaction logic for MainView.xaml
/// </summary>
public partial class MainView
{
    public MainView()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}
