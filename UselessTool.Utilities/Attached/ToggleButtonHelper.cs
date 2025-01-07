using System.Windows;

namespace UselessTool.Utilities.Attached;

public static class ToggleButtonHelper
{
    // 定义附加属性
    public static readonly DependencyProperty CheckedContentProperty =
        DependencyProperty.RegisterAttached(
            "CheckedContent",
            typeof(object),
            typeof(ToggleButtonHelper),
            new PropertyMetadata(null, OnCheckedContentChanged));

    public static object GetCheckedContent(DependencyObject obj) => obj.GetValue(CheckedContentProperty);
    public static void SetCheckedContent(DependencyObject obj, object value) => obj.SetValue(CheckedContentProperty, value);

    private static void OnCheckedContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is System.Windows.Controls.Primitives.ToggleButton toggleButton)
        {
            toggleButton.Checked += (_, _) => toggleButton.Content = e.NewValue;
        }
    }
}