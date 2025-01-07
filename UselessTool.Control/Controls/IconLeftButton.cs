using System.Windows;

namespace UselessTool.Control.Controls;

public class IconLeftButton : CheckBase
{
    static IconLeftButton()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(IconLeftButton), new FrameworkPropertyMetadata(typeof(IconLeftButton)));
    }

    //  是否显示图标
    public static readonly DependencyProperty IconVisibilityProperty = DependencyProperty.Register(nameof(IconVisibility), typeof(Visibility), typeof(IconLeftButton));
    public Visibility IconVisibility
    {
        get => (Visibility)GetValue(IconVisibilityProperty);
        set => SetValue(IconVisibilityProperty, value);
    }
}
