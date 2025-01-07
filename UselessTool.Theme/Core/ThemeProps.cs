using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace UselessTool.Theme.Core;

public class ThemeProps
{
    // 定义Background附加属性
    public static readonly DependencyProperty BackgroundProperty =
        DependencyProperty.RegisterAttached(
            "Background",
            typeof(Brush),
            typeof(ThemeProps),
            new PropertyMetadata(null, OnBrushPropertyChanged)
        );

    // 定义Foreground附加属性
    public static readonly DependencyProperty ForegroundProperty =
        DependencyProperty.RegisterAttached(
            "Foreground",
            typeof(Brush),
            typeof(ThemeProps),
            new PropertyMetadata(null, OnBrushPropertyChanged)
        );

    // 定义BorderBrush附加属性
    public static readonly DependencyProperty BorderBrushProperty =
        DependencyProperty.RegisterAttached(
            "BorderBrush",
            typeof(Brush),
            typeof(ThemeProps),
            new PropertyMetadata(null, OnBrushPropertyChanged)
        );

    // 定义Fill附加属性
    public static readonly DependencyProperty FillProperty =
        DependencyProperty.RegisterAttached(
            "Fill",
            typeof(Brush),
            typeof(ThemeProps),
            new PropertyMetadata(null, OnBrushPropertyChanged)
        );

    // 获取Background属性值的方法
    public static Brush GetBackground(DependencyObject obj) => (Brush)obj.GetValue(BackgroundProperty);
    public static void SetBackground(DependencyObject obj, Brush value) => obj.SetValue(BackgroundProperty, value);

    // 获取Foreground属性值的方法
    public static Brush GetForeground(DependencyObject obj) => (Brush)obj.GetValue(ForegroundProperty);
    public static void SetForeground(DependencyObject obj, Brush value) => obj.SetValue(ForegroundProperty, value);

    // 获取BorderBrush属性值的方法
    public static Brush GetBorderBrush(DependencyObject obj) => (Brush)obj.GetValue(BorderBrushProperty);
    public static void SetBorderBrush(DependencyObject obj, Brush value) => obj.SetValue(BorderBrushProperty, value);

    // 获取Fill属性值的方法
    public static Brush GetFill(DependencyObject obj) => (Brush)obj.GetValue(FillProperty);
    public static void SetFill(DependencyObject obj, Brush value) => obj.SetValue(FillProperty, value);

    // 刷子属性改变时触发的方法
    private static void OnBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FrameworkElement element && e.NewValue is SolidColorBrush newBrush)
        {
            AnimateBrushProperty(element, newBrush, e.Property);
        }
    }

    // 动画化刷子属性的方法
    private static void AnimateBrushProperty(FrameworkElement element, SolidColorBrush newBrush, DependencyProperty dp)
    {
        string? propertyName = dp.Name;
        PropertyInfo? propertyInfo = element.GetType().GetProperty(propertyName);

        if (propertyInfo == null)
            return;

        // 获取当前刷子，如果为空或冻结则创建一个新的刷子
        SolidColorBrush? currentValue = propertyInfo.GetValue(element) as SolidColorBrush ?? new SolidColorBrush(newBrush.Color);
        if (currentValue.IsFrozen)
        {
            currentValue = new SolidColorBrush(currentValue.Color);
        }

        // 设置新的刷子
        propertyInfo.SetValue(element, currentValue);

        ColorAnimation? animation = new()
        {
            To = newBrush.Color,
            Duration = TimeSpan.FromSeconds(0.3),
            EasingFunction = new CircleEase { EasingMode = EasingMode.EaseInOut }
        };
        currentValue.BeginAnimation(SolidColorBrush.ColorProperty, animation);
    }
}