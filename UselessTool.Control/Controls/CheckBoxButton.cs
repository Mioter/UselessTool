using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UselessTool.Control.Controls;

public class CheckBoxButton : CheckBox
{
    static CheckBoxButton()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(CheckBoxButton), new FrameworkPropertyMetadata(typeof(CheckBoxButton)));
    }

    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(nameof(Icon), typeof(ImageSource), typeof(CheckBoxButton));
    public ImageSource Icon
    {
        get => (ImageSource)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly DependencyProperty HoverIconProperty = DependencyProperty.Register(nameof(HoverIcon), typeof(ImageSource), typeof(CheckBoxButton));
    public ImageSource HoverIcon
    {
        get => (ImageSource)GetValue(HoverIconProperty);
        set => SetValue(HoverIconProperty, value);
    }

    public static readonly DependencyProperty CheckIconProperty = DependencyProperty.Register(nameof(CheckIcon), typeof(ImageSource), typeof(CheckBoxButton));
    public ImageSource CheckIcon
    {
        get => (ImageSource)GetValue(CheckIconProperty);
        set => SetValue(CheckIconProperty, value);
    }

    public static readonly DependencyProperty CheckHoverIconProperty = DependencyProperty.Register(nameof(CheckHoverIcon), typeof(ImageSource), typeof(CheckBoxButton));
    public ImageSource CheckHoverIcon
    {
        get => (ImageSource)GetValue(CheckHoverIconProperty);
        set => SetValue(CheckHoverIconProperty, value);
    }


    /// <summary>
    /// Check 文字颜色
    /// </summary>
    public static readonly DependencyProperty CheckForegroundProperty =
        DependencyProperty.Register(nameof(CheckForeground), typeof(Brush), typeof(CheckBoxButton));
    public Brush CheckForeground
    {
        get => (Brush)GetValue(CheckForegroundProperty);
        set => SetValue(CheckForegroundProperty, value);
    }

    /// <summary>
    /// 图标宽度
    /// </summary>
    public static readonly DependencyProperty IconWidthProperty =
        DependencyProperty.Register(nameof(IconWidth), typeof(double), typeof(CheckBoxButton));
    public double IconWidth
    {
        get => (int)GetValue(IconWidthProperty);
        set => SetValue(IconWidthProperty, value);
    }

    /// <summary>
    /// 图标高度
    /// </summary>
    public static readonly DependencyProperty IconHeightProperty =
        DependencyProperty.Register(nameof(IconHeight), typeof(double), typeof(CheckBoxButton));
    public double IconHeight
    {
        get => (int)GetValue(IconHeightProperty);
        set => SetValue(IconHeightProperty, value);
    }
}
