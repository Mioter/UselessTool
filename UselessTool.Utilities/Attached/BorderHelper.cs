using System.Windows;
using System.Windows.Controls;

namespace UselessTool.Utilities.Attached;

public class BorderHelper
{
    // 依赖属性：是否自动调整角半径
    public static readonly DependencyProperty AutoCornerRadiusProperty =
        DependencyProperty.RegisterAttached(
            "AutoCornerRadius",
            typeof(bool),
            typeof(BorderHelper),
            new PropertyMetadata(false, OnAutoCornerRadiusChanged)
        );

    // 依赖属性：角半径与实际尺寸的比例
    public static readonly DependencyProperty CornerRadiusRatioProperty =
        DependencyProperty.RegisterAttached(
            "CornerRadiusRatio",
            typeof(double),
            typeof(BorderHelper),
            new PropertyMetadata(0.5, OnCornerRadiusRatioChanged)
        );

    // 依赖属性：是否仅在宽度变化时更新角半径
    public static readonly DependencyProperty UpdateOnWidthChangedProperty =
        DependencyProperty.RegisterAttached(
            "UpdateOnWidthChanged",
            typeof(bool),
            typeof(BorderHelper),
            new PropertyMetadata(true)
        );

    // 依赖属性：是否仅在高度变化时更新角半径
    public static readonly DependencyProperty UpdateOnHeightChangedProperty =
        DependencyProperty.RegisterAttached(
            "UpdateOnHeightChanged",
            typeof(bool),
            typeof(BorderHelper),
            new PropertyMetadata(true)
        );

    // 获取/设置 AutoCornerRadius 属性
    public static bool GetAutoCornerRadius(DependencyObject obj) => (bool)obj.GetValue(AutoCornerRadiusProperty);
    public static void SetAutoCornerRadius(DependencyObject obj, bool value) => obj.SetValue(AutoCornerRadiusProperty, value);

    // 获取/设置 CornerRadiusRatio 属性
    public static double GetCornerRadiusRatio(DependencyObject obj) => (double)obj.GetValue(CornerRadiusRatioProperty);
    public static void SetCornerRadiusRatio(DependencyObject obj, double value) => obj.SetValue(CornerRadiusRatioProperty, value);

    // 获取/设置 UpdateOnWidthChanged 属性
    public static bool GetUpdateOnWidthChanged(DependencyObject obj) => (bool)obj.GetValue(UpdateOnWidthChangedProperty);
    public static void SetUpdateOnWidthChanged(DependencyObject obj, bool value) => obj.SetValue(UpdateOnWidthChangedProperty, value);

    // 获取/设置 UpdateOnHeightChanged 属性
    public static bool GetUpdateOnHeightChanged(DependencyObject obj) => (bool)obj.GetValue(UpdateOnHeightChangedProperty);
    public static void SetUpdateOnHeightChanged(DependencyObject obj, bool value) => obj.SetValue(UpdateOnHeightChangedProperty, value);

    private static void OnAutoCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var border = d as Border;
        bool newValue = (bool)e.NewValue;
        if (border == null) return;
        
        if (newValue)
        {
            border.SizeChanged += Border_SizeChanged;
            UpdateCornerRadius(border);
        }
        else
        {
            border.SizeChanged -= Border_SizeChanged;
        }
    }

    private static void OnCornerRadiusRatioChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Border border && GetAutoCornerRadius(border))
        {
            UpdateCornerRadius(border);
        }
    }

    private static void Border_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (sender is not Border border || !GetAutoCornerRadius(border)) return;
        bool update = GetUpdateOnWidthChanged(border) && e.PreviousSize.Width != e.NewSize.Width;
        update = update || GetUpdateOnHeightChanged(border) && e.PreviousSize.Height != e.NewSize.Height;
        if (update)
        {
            UpdateCornerRadius(border);
        }
    }

    private static void UpdateCornerRadius(Border border)
    {
        double ratio = GetCornerRadiusRatio(border);
        double minDimension = Math.Min(border.ActualWidth, border.ActualHeight) * ratio;
        border.CornerRadius = new CornerRadius(minDimension);
    }
}