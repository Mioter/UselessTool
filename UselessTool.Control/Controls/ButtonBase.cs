using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UselessTool.Control.Controls;

public class ButtonBase : Button
{
    #region Icon

    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(nameof(Icon), typeof(ImageSource), typeof(ButtonBase));
    public ImageSource Icon
    {
        get => (ImageSource)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly DependencyProperty HoverIconProperty =
        DependencyProperty.Register(nameof(HoverIcon), typeof(ImageSource), typeof(ButtonBase));
    public ImageSource HoverIcon
    {
        get => (ImageSource)GetValue(HoverIconProperty);
        set => SetValue(HoverIconProperty, value);
    }

    public static readonly DependencyProperty PushIconProperty =
        DependencyProperty.Register(nameof(PushIcon), typeof(ImageSource), typeof(ButtonBase));
    public ImageSource PushIcon
    {
        get => (ImageSource)GetValue(PushIconProperty);
        set => SetValue(PushIconProperty, value);
    }

    public static readonly DependencyProperty DisableIconProperty =
        DependencyProperty.Register(nameof(DisableIcon), typeof(ImageSource), typeof(ButtonBase));
    public ImageSource DisableIcon
    {
        get => (ImageSource)GetValue(DisableIconProperty);
        set => SetValue(DisableIconProperty, value);
    }

    /// <summary>
    /// 图标间距
    /// </summary>
    public static readonly DependencyProperty IconMarginProperty =
        DependencyProperty.Register(nameof(IconMargin), typeof(Thickness), typeof(ButtonBase));
    public Thickness IconMargin
    {
        get => (Thickness)GetValue(IconMarginProperty);
        set => SetValue(IconMarginProperty, value);
    }

    /// <summary>
    /// 图标宽度
    /// </summary>
    public static readonly DependencyProperty IconWidthProperty =
        DependencyProperty.Register(nameof(IconWidth), typeof(double), typeof(ButtonBase));
    public double IconWidth
    {
        get => (int)GetValue(IconWidthProperty);
        set => SetValue(IconWidthProperty, value);
    }

    /// <summary>
    /// 图标高度
    /// </summary>
    public static readonly DependencyProperty IconHeightProperty =
        DependencyProperty.Register(nameof(IconHeight), typeof(double), typeof(ButtonBase));
    public double IconHeight
    {
        get => (int)GetValue(IconHeightProperty);
        set => SetValue(IconHeightProperty, value);
    }

    #endregion

    #region Background

    /// <summary>
    /// Hover 背景
    /// </summary>
    public static readonly DependencyProperty HoverBackgroundProperty =
       DependencyProperty.Register(nameof(HoverBackground), typeof(Brush), typeof(ButtonBase));
    public Brush HoverBackground
    {
        get => (Brush)GetValue(HoverBackgroundProperty);
        set => SetValue(HoverBackgroundProperty, value);
    }

    /// <summary>
    /// Push 背景
    /// </summary>
    public static readonly DependencyProperty PushBackgroundProperty =
       DependencyProperty.Register(nameof(PushBackground), typeof(Brush), typeof(ButtonBase));
    public Brush PushBackground
    {
        get => (Brush)GetValue(PushBackgroundProperty);
        set => SetValue(PushBackgroundProperty, value);
    }

    /// <summary>
    /// Disable 背景
    /// </summary>
    public static readonly DependencyProperty DisableBackgroundProperty =
       DependencyProperty.Register(nameof(DisableBackground), typeof(Brush), typeof(ButtonBase));
    public Brush DisableBackground
    {
        get => (Brush)GetValue(DisableBackgroundProperty);
        set => SetValue(DisableBackgroundProperty, value);
    }

    #endregion
}

public class TextButtonBase : ButtonBase
{
    #region Text

    /// <summary>
    /// Hover 文字颜色
    /// </summary>
    public static readonly DependencyProperty HoverForegroundProperty =
        DependencyProperty.Register(nameof(HoverForeground), typeof(Brush), typeof(TextButtonBase));
    public Brush HoverForeground
    {
        get => (Brush)GetValue(HoverForegroundProperty);
        set => SetValue(HoverForegroundProperty, value);
    }

    /// <summary>
    /// Push 文字颜色
    /// </summary>
    public static readonly DependencyProperty PushForegroundProperty =
        DependencyProperty.Register(nameof(PushForeground), typeof(Brush), typeof(TextButtonBase));
    public Brush PushForeground
    {
        get => (Brush)GetValue(PushForegroundProperty);
        set => SetValue(PushForegroundProperty, value);
    }

    /// <summary>
    /// Disable 文字颜色
    /// </summary>
    public static readonly DependencyProperty DisableForegroundProperty =
        DependencyProperty.Register(nameof(DisableForeground), typeof(Brush), typeof(TextButtonBase));
    public Brush DisableForeground
    {
        get => (Brush)GetValue(DisableForegroundProperty);
        set => SetValue(DisableForegroundProperty, value);
    }

    /// <summary>
    /// 文字对齐方式
    /// </summary>
    public static readonly DependencyProperty HorAlignmentProperty =
        DependencyProperty.Register(nameof(HorAlignment), typeof(HorizontalAlignment), typeof(TextButtonBase));
    public HorizontalAlignment HorAlignment
    {
        get => (HorizontalAlignment)GetValue(HorAlignmentProperty);
        set => SetValue(HorAlignmentProperty, value);
    }

    /// <summary>
    /// 文字边距
    /// </summary>
    public static readonly DependencyProperty TextMarginProperty =
        DependencyProperty.Register(nameof(TextMargin), typeof(Thickness), typeof(TextButtonBase));
    public Thickness TextMargin
    {
        get => (Thickness)GetValue(TextMarginProperty);
        set => SetValue(TextMarginProperty, value);
    }

    /// <summary>
    /// 文字对齐方式
    /// </summary>
    public static readonly DependencyProperty TextHoriaAlignmentProperty =
        DependencyProperty.Register(nameof(TextHoriaAlignment), typeof(HorizontalAlignment), typeof(TextButtonBase));
    public HorizontalAlignment TextHoriaAlignment
    {
        get => (HorizontalAlignment)GetValue(TextHoriaAlignmentProperty);
        set => SetValue(TextHoriaAlignmentProperty, value);
    }

    #endregion

    /// <summary>
    /// 圆角
    /// </summary>
    public static readonly DependencyProperty BorderRadiusProperty =
        DependencyProperty.Register(nameof(BorderRadius), typeof(CornerRadius), typeof(TextButtonBase));
    public CornerRadius BorderRadius
    {
        get => (CornerRadius)GetValue(BorderRadiusProperty);
        set => SetValue(BorderRadiusProperty, value);
    }
}

public class CheckBase : TextButtonBase
{
    #region Check

    /// <summary>
    /// 选中状态
    /// </summary>
    public static readonly DependencyProperty IsCheckedProperty =
        DependencyProperty.Register(nameof(IsChecked), typeof(bool), typeof(CheckBase));
    public bool IsChecked
    {
        get => (bool)GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }

    /// <summary>
    /// Check 状态图标
    /// </summary>
    public static readonly DependencyProperty CheckIconProperty =
      DependencyProperty.Register(nameof(CheckIcon), typeof(ImageSource), typeof(CheckBase));
    public ImageSource CheckIcon
    {
        get => (ImageSource)GetValue(CheckIconProperty);
        set => SetValue(CheckIconProperty, value);
    }

    /// <summary>
    /// Check 文字颜色
    /// </summary>
    public static readonly DependencyProperty CheckForegroundProperty =
        DependencyProperty.Register(nameof(CheckForeground), typeof(Brush), typeof(CheckBase));
    public Brush CheckForeground
    {
        get => (Brush)GetValue(CheckForegroundProperty);
        set => SetValue(CheckForegroundProperty, value);
    }

    /// <summary>
    /// Check 背景
    /// </summary>
    public static readonly DependencyProperty CheckBackgroundProperty =
       DependencyProperty.Register(nameof(CheckBackground), typeof(Brush), typeof(CheckBase));
    public Brush CheckBackground
    {
        get => (Brush)GetValue(CheckBackgroundProperty);
        set => SetValue(CheckBackgroundProperty, value);
    }


    #endregion

}
