using System.Windows;
using System.Windows.Media;

namespace UselessTool.Control.Controls;

public class TextButton : TextButtonBase
{
    static TextButton()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(TextButton), new FrameworkPropertyMetadata(typeof(TextButton)));
    }

    public VerticalAlignment TextVerAlignment
    {
        get => (VerticalAlignment)GetValue(TextVerAlignmentProperty);
        set => SetValue(TextVerAlignmentProperty, value);
    }

    // Using a DependencyProperty as the backing store for TextHoriaAlignment.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty TextVerAlignmentProperty =
        DependencyProperty.Register(nameof(TextVerAlignment), typeof(VerticalAlignment), typeof(TextButtonBase));


    public static readonly DependencyProperty HoverBorderBrushProperty =
        DependencyProperty.Register(nameof(HoverBorderBrush), typeof(Brush), typeof(TextButtonBase));
    public Brush HoverBorderBrush
    {
        get => (Brush)GetValue(HoverBorderBrushProperty);
        set => SetValue(HoverBorderBrushProperty, value);
    }

    public static readonly DependencyProperty PushBorderBrushProperty =
        DependencyProperty.Register(nameof(PushBorderBrush), typeof(Brush), typeof(TextButtonBase));
    public Brush PushBorderBrush
    {
        get => (Brush)GetValue(PushBorderBrushProperty);
        set => SetValue(PushBorderBrushProperty, value);
    }

    public static readonly DependencyProperty TextBtnTrimmingProperty = DependencyProperty.Register(nameof(TextBtnTrimming), typeof(TextTrimming), typeof(TextButtonBase));
    public TextTrimming TextBtnTrimming
    {
        get => (TextTrimming)GetValue(TextBtnTrimmingProperty);
        set => SetValue(TextBtnTrimmingProperty, value);
    }
}
