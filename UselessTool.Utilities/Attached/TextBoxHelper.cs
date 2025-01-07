using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UselessTool.Utilities.Attached;

public static class TextBoxHelper
{
    #region 附加属性 IsClearButtonProperty

    /// <summary>
    /// 附加属性，是否带清空按钮。
    /// </summary>
    public static readonly DependencyProperty IsClearButtonProperty =
        DependencyProperty.RegisterAttached(
            "IsClearButton",
            typeof(bool),
            typeof(TextBoxHelper),
            new PropertyMetadata(false, OnIsClearButtonPropertyChanged)
        );

    public static bool GetIsClearButton(DependencyObject obj) => (bool)obj.GetValue(IsClearButtonProperty);
    public static void SetIsClearButton(DependencyObject obj, bool value) => obj.SetValue(IsClearButtonProperty, value);

    private static void OnIsClearButtonPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e
        )
    {
        if (d is not FrameworkElement element) return;
        // 尝试找到父级 TextBox
        TextBox? textBox = FindParent<TextBox>(element);

        if (textBox == null) return;
        // 订阅或取消订阅 ClearButton 的 Click 事件
        if (element is not Button clearButton) return;
            
        if ((bool)e.NewValue)
        {
            clearButton.Click += ClearTextClicked;
        }
        else
        {
            clearButton.Click -= ClearTextClicked;
        }
    }

    private static void ClearTextClicked(object sender, RoutedEventArgs e)
    {
        if (sender is not Button clearButton) return;
        
        // 查找父级 TextBox 并清空内容
        TextBox? textBox = FindParent<TextBox>(clearButton);
        textBox?.Clear();
    }

    #endregion

    private static T? FindParent<T>(DependencyObject child) where T : DependencyObject
    {
        while (true)
        {
            DependencyObject? parentObject = VisualTreeHelper.GetParent(child);

            switch (parentObject)
            {
                case null:
                    return null;
                case T parent:
                    return parent;
                default:
                    child = parentObject;
                    break;
            }
        }
    }
}