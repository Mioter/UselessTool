using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UselessTool.Control.Controls;

public abstract class IconTextButtonBase : Button
{

    #region Icon

    /// <summary>
    /// Normal 状态图标
    /// </summary>
    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(nameof(Icon), typeof(ImageSource), typeof(IconTextButtonBase));
    public ImageSource Icon
    {
        get => (ImageSource)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>
    /// Hover 状态图标
    /// </summary>
    public static readonly DependencyProperty HoverIconProperty =
       DependencyProperty.Register(nameof(HoverIcon), typeof(ImageSource), typeof(IconTextButtonBase));
    public ImageSource HoverIcon
    {
        get => (ImageSource)GetValue(HoverIconProperty);
        set => SetValue(HoverIconProperty, value);
    }

    /// <summary>
    /// Push 状态图标
    /// </summary>
    public static readonly DependencyProperty PushIconProperty =
       DependencyProperty.Register(nameof(PushIcon), typeof(ImageSource), typeof(IconTextButtonBase));
    public ImageSource PushIcon
    {
        get => (ImageSource)GetValue(PushIconProperty);
        set => SetValue(PushIconProperty, value);
    }

    /// <summary>
    /// Disable 状态图标
    /// </summary>
    public static readonly DependencyProperty DisableIconProperty =
        DependencyProperty.Register(nameof(DisableIcon), typeof(ImageSource), typeof(IconTextButtonBase));
    public ImageSource DisableIcon
    {
        get => (ImageSource)GetValue(DisableIconProperty);
        set => SetValue(DisableIconProperty, value);
    }

    /// <summary>
    /// 图标间距
    /// </summary>
    public static readonly DependencyProperty IconMarginProperty =
        DependencyProperty.Register(nameof(IconMargin), typeof(Thickness), typeof(IconTextButtonBase));
    public Thickness IconMargin
    {
        get => (Thickness)GetValue(IconMarginProperty);
        set => SetValue(IconMarginProperty, value);
    }

    /// <summary>
    /// 图标宽度
    /// </summary>
    public static readonly DependencyProperty IconWidthProperty =
        DependencyProperty.Register(nameof(IconWidth), typeof(double), typeof(IconTextButtonBase));
    public double IconWidth
    {
        get => (int)GetValue(IconWidthProperty);
        set => SetValue(IconWidthProperty, value);
    }

    /// <summary>
    /// 图标高度
    /// </summary>
    public static readonly DependencyProperty IconHeightProperty =
        DependencyProperty.Register(nameof(IconHeight), typeof(double), typeof(IconTextButtonBase));
    public double IconHeight
    {
        get => (int)GetValue(IconHeightProperty);
        set => SetValue(IconHeightProperty, value);
    }

    #endregion

    #region Text

    /// <summary>
    /// Hover 文字颜色
    /// </summary>
    public static readonly DependencyProperty HoverForegroundProperty =
        DependencyProperty.Register(nameof(HoverForeground), typeof(Brush), typeof(IconTextButtonBase));
    public Brush HoverForeground
    {
        get => (Brush)GetValue(HoverForegroundProperty);
        set => SetValue(HoverForegroundProperty, value);
    }

    /// <summary>
    /// Push 文字颜色
    /// </summary>
    public static readonly DependencyProperty PushForegroundProperty =
        DependencyProperty.Register(nameof(PushForeground), typeof(Brush), typeof(IconTextButtonBase));
    public Brush PushForeground
    {
        get => (Brush)GetValue(PushForegroundProperty);
        set => SetValue(PushForegroundProperty, value);
    }

    /// <summary>
    /// Disable 文字颜色
    /// </summary>
    public static readonly DependencyProperty DisableForegroundProperty =
        DependencyProperty.Register(nameof(DisableForeground), typeof(Brush), typeof(IconTextButtonBase));
    public Brush DisableForeground
    {
        get => (Brush)GetValue(DisableForegroundProperty);
        set => SetValue(DisableForegroundProperty, value);
    }

    /// <summary>
    /// 文字对齐方式
    /// </summary>
    public static readonly DependencyProperty HorAlignmentProperty =
        DependencyProperty.Register(nameof(HorAlignment), typeof(HorizontalAlignment), typeof(IconTextButtonBase));
    public HorizontalAlignment HorAlignment
    {
        get => (HorizontalAlignment)GetValue(HorAlignmentProperty);
        set => SetValue(HorAlignmentProperty, value);
    }

    /// <summary>
    /// 文字间距
    /// </summary>
    public static readonly DependencyProperty TextMarginProperty =
        DependencyProperty.Register(nameof(TextMargin), typeof(Thickness), typeof(IconTextButtonBase));
    public Thickness TextMargin
    {
        get => (Thickness)GetValue(TextMarginProperty);
        set => SetValue(TextMarginProperty, value);
    }

    #endregion

    #region Background

    /// <summary>
    /// Hover 背景
    /// </summary>
    public static readonly DependencyProperty HoverBackgroundProperty =
       DependencyProperty.Register(nameof(HoverBackground), typeof(Brush), typeof(IconTextButtonBase));
    public Brush HoverBackground
    {
        get => (Brush)GetValue(HoverBackgroundProperty);
        set => SetValue(HoverBackgroundProperty, value);
    }

    /// <summary>
    /// Push 背景
    /// </summary>
    public static readonly DependencyProperty PushBackgroundProperty =
       DependencyProperty.Register(nameof(PushBackground), typeof(Brush), typeof(IconTextButtonBase));
    public Brush PushBackground
    {
        get => (Brush)GetValue(PushBackgroundProperty);
        set => SetValue(PushBackgroundProperty, value);
    }

    /// <summary>
    /// Disable 背景
    /// </summary>
    public static readonly DependencyProperty DisableBackgroundProperty =
       DependencyProperty.Register(nameof(DisableBackground), typeof(Brush), typeof(IconTextButtonBase));
    public Brush DisableBackground
    {
        get => (Brush)GetValue(DisableBackgroundProperty);
        set => SetValue(DisableBackgroundProperty, value);
    }

    #endregion

}
