using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UselessTool.Control.Controls;

public class IconRadioButton : RadioButton
{
    static IconRadioButton()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(IconRadioButton), new FrameworkPropertyMetadata(typeof(IconRadioButton)));
    }

    //  未选中
    public static readonly DependencyProperty UnSelectedIconProperty =
        DependencyProperty.Register(nameof(UnSelectedIcon), typeof(ImageSource), typeof(IconRadioButton));

    public ImageSource UnSelectedIcon
    {
        get => (ImageSource)GetValue(UnSelectedIconProperty);
        set => SetValue(UnSelectedIconProperty, value);
    }

    //  未选中hover
    public static readonly DependencyProperty UnSelectedIconHoverProperty =
        DependencyProperty.Register(nameof(UnSelectedIconHover), typeof(ImageSource), typeof(IconRadioButton));

    public ImageSource UnSelectedIconHover
    {
        get => (ImageSource)GetValue(UnSelectedIconHoverProperty);
        set => SetValue(UnSelectedIconHoverProperty, value);
    }

    //  选中
    public static readonly DependencyProperty SelectedIconProperty =
        DependencyProperty.Register(nameof(SelectedIcon), typeof(ImageSource), typeof(IconRadioButton));

    public ImageSource SelectedIcon
    {
        get => (ImageSource)GetValue(SelectedIconProperty);
        set => SetValue(SelectedIconProperty, value);
    }

    //  选中hover
    public static readonly DependencyProperty SelectedIconHoverProperty =
        DependencyProperty.Register(nameof(SelectedIconHover), typeof(ImageSource), typeof(IconRadioButton));

    public ImageSource SelectedIconHover
    {
        get => (ImageSource)GetValue(SelectedIconHoverProperty);
        set => SetValue(SelectedIconHoverProperty, value);
    }

    //  图片宽度
    public static readonly DependencyProperty IconWidthProperty =
        DependencyProperty.Register(nameof(IconWidth), typeof(int), typeof(IconRadioButton));

    public int IconWidth
    {
        get => (int)GetValue(IconWidthProperty);
        set => SetValue(IconWidthProperty, value);
    }

    //图片高度
    public static readonly DependencyProperty IconHeightProperty =
        DependencyProperty.Register(nameof(IconHeight), typeof(int), typeof(IconRadioButton));
    public int IconHeight
    {
        get => (int)GetValue(IconHeightProperty);
        set => SetValue(IconHeightProperty, value);
    }
}
