using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace UselessTool.Control.Controls;

// 定义输入限制类型的枚举
public enum InputRestrictionType
{
    None,
    OnlyNumbers,
    FileNameAllowedCharacters,
    Hexadecimal
}

// 自定义 TextBox 控件，继承自 WPF 的 TextBox
public class EnhancedTextBox : TextBox
{
    #region 依赖属性 InputRestrictionProperty

    /// <summary>
    /// 依赖属性，用于设置输入限制类型。
    /// </summary>
    public static readonly DependencyProperty InputRestrictionProperty =
        DependencyProperty.Register(
            nameof(InputRestriction), // 使用 nameof 关键字获取属性名称，提高可读性和可维护性
            typeof(InputRestrictionType),
            typeof(EnhancedTextBox),
            new PropertyMetadata(InputRestrictionType.None, OnInputRestrictionPropertyChanged)
        );

    /// <summary>
    /// 获取或设置输入限制类型。
    /// </summary>
    public InputRestrictionType InputRestriction
    {
        get => (InputRestrictionType)GetValue(InputRestrictionProperty);
        set => SetValue(InputRestrictionProperty, value);
    }

    /// <summary>
    /// 当 InputRestriction 属性发生变化时调用的回调方法。
    /// </summary>
    private static void OnInputRestrictionPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e
        )
    {
        if (d is EnhancedTextBox textBox)
        {
            // 根据新值订阅或取消订阅 PreviewTextInput 事件
            if ((InputRestrictionType)e.NewValue != InputRestrictionType.None)
            {
                textBox.PreviewTextInput += TxtInput; // 订阅事件以处理输入
            }
            else
            {
                textBox.PreviewTextInput -= TxtInput; // 取消订阅事件
            }
        }
    }

    /// <summary>
    /// 处理文本输入事件，根据输入限制类型过滤输入字符。
    /// </summary>
    private static void TxtInput(object sender, TextCompositionEventArgs e)
    {
        if (sender is EnhancedTextBox textBox)
        {
            e.Handled = textBox.InputRestriction switch
            {
                InputRestrictionType.OnlyNumbers => !char.IsDigit(e.Text[0]), // 仅允许数字输入
                InputRestrictionType.FileNameAllowedCharacters => !IsValidFileNameCharacter(
                    e.Text[0]
                ), // 仅允许文件名中允许的字符
                InputRestrictionType.Hexadecimal => !IsHexadecimalCharacter(e.Text[0]), // 仅允许十六进制字符（0-9, A-F, a-f）
                _ => false // 默认情况下不阻止任何输入
            };
        }
    }

    /// <summary>
    /// 检查字符是否为有效的文件名字符。
    /// </summary>
    private static bool IsValidFileNameCharacter(char c)
    {
        char[] invalidChars = Path.GetInvalidFileNameChars(); // 获取无效的文件名字符数组
        return Array.IndexOf(invalidChars, c) < 0; // 返回字符是否不在无效字符数组中
    }

    /// <summary>
    /// 检查字符是否为有效的十六进制字符。
    /// </summary>
    private static bool IsHexadecimalCharacter(char c)
    {
        return char.IsDigit(c) || c is >= 'A' and <= 'F' || c is >= 'a' and <= 'f'; // 检查字符是否为 0-9, A-F, a-f
    }

    #endregion

    #region 依赖属性 CurrentCharCountProperty

    /// <summary>
    /// 依赖属性，用于存储当前字符数。
    /// </summary>
    public static readonly DependencyProperty CurrentCharCountProperty =
        DependencyProperty.Register(
            nameof(CurrentCharCount), // 使用 nameof 关键字获取属性名称，提高可读性和可维护性
            typeof(int),
            typeof(EnhancedTextBox),
            new PropertyMetadata(0)
        );

    /// <summary>
    /// 获取当前字符数。
    /// </summary>
    public int CurrentCharCount
    {
        get => (int)GetValue(CurrentCharCountProperty);
        set => SetValue(CurrentCharCountProperty, value);
    }

    #endregion

    #region 依赖属性 IsMaxLengthReachedProperty

    /// <summary>
    /// 依赖属性，用于指示当前字符数是否达到最大长度。
    /// </summary>
    public static readonly DependencyProperty IsMaxLengthReachedProperty =
        DependencyProperty.Register(
            nameof(IsMaxLengthReached), // 使用 nameof 关键字获取属性名称，提高可读性和可维护性
            typeof(bool),
            typeof(EnhancedTextBox),
            new PropertyMetadata(false)
        );

    /// <summary>
    /// 获取当前字符数是否达到最大长度。
    /// </summary>
    public bool IsMaxLengthReached
    {
        get => (bool)GetValue(IsMaxLengthReachedProperty);
        set => SetValue(IsMaxLengthReachedProperty, value);
    }

    #endregion

    #region 依赖属性 WatermarkProperty

    /// <summary>
    /// 依赖属性，用于存储水印内容（可以是字符串或控件）。
    /// </summary>
    public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register(
        nameof(Watermark), // 使用 nameof 关键字获取属性名称，提高可读性和可维护性
        typeof(object),
        typeof(EnhancedTextBox),
        new PropertyMetadata(null, OnWatermarkPropertyChanged)
    );

    /// <summary>
    /// 获取或设置水印内容。
    /// </summary>
    public object Watermark
    {
        get => GetValue(WatermarkProperty);
        set => SetValue(WatermarkProperty, value);
    }

    #endregion

    #region 依赖属性 WatermarkOpacityProperty

    /// <summary>
    /// 依赖属性，用于设置水印的透明度。
    /// </summary>
    public static readonly DependencyProperty WatermarkOpacityProperty =
        DependencyProperty.Register(
            nameof(WatermarkOpacity), // 使用 nameof 关键字获取属性名称，提高可读性和可维护性
            typeof(double),
            typeof(EnhancedTextBox),
            new PropertyMetadata(0.5, OnWatermarkPropertyChanged)
        );

    /// <summary>
    /// 获取或设置水印的透明度。
    /// </summary>
    public double WatermarkOpacity
    {
        get => (double)GetValue(WatermarkOpacityProperty);
        set => SetValue(WatermarkOpacityProperty, value);
    }

    #endregion

    #region 依赖属性 WatermarkMarginProperty

    /// <summary>
    /// 依赖属性，用于设置水印的边距。
    /// </summary>
    public static readonly DependencyProperty WatermarkMarginProperty =
        DependencyProperty.Register(
            nameof(WatermarkMargin), // 使用 nameof 关键字获取属性名称，提高可读性和可维护性
            typeof(Thickness),
            typeof(EnhancedTextBox),
            new PropertyMetadata(new Thickness(2, 0, 0, 0), OnWatermarkPropertyChanged)
        );

    /// <summary>
    /// 获取或设置水印的边距。
    /// </summary>
    public Thickness WatermarkMargin
    {
        get => (Thickness)GetValue(WatermarkMarginProperty);
        set => SetValue(WatermarkMarginProperty, value);
    }

    #endregion

    #region 依赖属性 WatermarkForegroundProperty

    /// <summary>
    /// 依赖属性，用于设置水印的前景色。
    /// </summary>
    public static readonly DependencyProperty WatermarkForegroundProperty =
        DependencyProperty.Register(
            nameof(WatermarkForeground), // 使用 nameof 关键字获取属性名称，提高可读性和可维护性
            typeof(Brush),
            typeof(EnhancedTextBox),
            new PropertyMetadata(Brushes.Gray, OnWatermarkPropertyChanged)
        );

    /// <summary>
    /// 获取或设置水印的前景色。
    /// </summary>
    public Brush WatermarkForeground
    {
        get => (Brush)GetValue(WatermarkForegroundProperty);
        set => SetValue(WatermarkForegroundProperty, value);
    }

    #endregion

    #region 依赖属性 WatermarkHorizontalAlignmentProperty

    /// <summary>
    /// 依赖属性，用于设置水印的水平对齐方式。
    /// </summary>
    public static readonly DependencyProperty WatermarkHorizontalAlignmentProperty =
        DependencyProperty.Register(
            nameof(WatermarkHorizontalAlignment), // 使用 nameof 关键字获取属性名称，提高可读性和可维护性
            typeof(HorizontalAlignment),
            typeof(EnhancedTextBox),
            new PropertyMetadata(HorizontalAlignment.Left, OnWatermarkPropertyChanged)
        );

    /// <summary>
    /// 获取或设置水印的水平对齐方式。
    /// </summary>
    public HorizontalAlignment WatermarkHorizontalAlignment
    {
        get => (HorizontalAlignment)GetValue(WatermarkHorizontalAlignmentProperty);
        set => SetValue(WatermarkHorizontalAlignmentProperty, value);
    }

    #endregion

    #region 依赖属性 WatermarkVerticalAlignmentProperty

    /// <summary>
    /// 依赖属性，用于设置水印的垂直对齐方式。
    /// </summary>
    public static readonly DependencyProperty WatermarkVerticalAlignmentProperty =
        DependencyProperty.Register(
            nameof(WatermarkVerticalAlignment), // 使用 nameof 关键字获取属性名称，提高可读性和可维护性
            typeof(VerticalAlignment),
            typeof(EnhancedTextBox),
            new PropertyMetadata(VerticalAlignment.Center, OnWatermarkPropertyChanged)
        );

    /// <summary>
    /// 获取或设置水印的垂直对齐方式。
    /// </summary>
    public VerticalAlignment WatermarkVerticalAlignment
    {
        get => (VerticalAlignment)GetValue(WatermarkVerticalAlignmentProperty);
        set => SetValue(WatermarkVerticalAlignmentProperty, value);
    }

    #endregion

    #region 依赖属性 WatermarkVisibilityProperty

    /// <summary>
    /// 依赖属性，用于设置水印的可见性。
    /// </summary>
    public static readonly DependencyProperty WatermarkVisibilityProperty =
        DependencyProperty.Register(
            nameof(WatermarkVisibility), // 使用 nameof 关键字获取属性名称，提高可读性和可维护性
            typeof(Visibility),
            typeof(EnhancedTextBox),
            new PropertyMetadata(Visibility.Visible, OnWatermarkPropertyChanged)
        );

    /// <summary>
    /// 获取或设置水印的可见性。
    /// </summary>
    public Visibility WatermarkVisibility
    {
        get => (Visibility)GetValue(WatermarkVisibilityProperty);
        set => SetValue(WatermarkVisibilityProperty, value);
    }

    #endregion

    /// <summary>
    /// 当 Watermark 相关属性发生变化时调用的回调方法。
    /// </summary>
    private static void OnWatermarkPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e
        )
    {
        if (d is EnhancedTextBox textBox)
        {
            textBox.UpdateWatermarkVisibility();
        }
    }

    /// <summary>
    /// 更新水印的可见性。
    /// </summary>
    private void UpdateWatermarkVisibility()
    {
        WatermarkVisibility = string.IsNullOrEmpty(Text)
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    /// <summary>
    /// 更新当前字符数和最大长度标志。
    /// </summary>
    private void UpdateCurrentCharCountAndMaxLengthReached()
    {
        CurrentCharCount = Text.Length;
        IsMaxLengthReached = MaxLength > 0 && CurrentCharCount >= MaxLength;
    }

    /// <summary>
    /// 构造函数，初始化事件处理程序并应用 ControlTemplate。
    /// </summary>
    public EnhancedTextBox()
    {
        Loaded += (_, _) => UpdateWatermarkVisibility(); // 加载时更新水印可见性
        TextChanged += (_, _) =>
        {
            UpdateWatermarkVisibility();
            UpdateCurrentCharCountAndMaxLengthReached();
        }; // 文本变化时更新水印可见性和字符计数

        // 应用自定义的 ControlTemplate
        DefaultStyleKey = typeof(EnhancedTextBox);
    }
}