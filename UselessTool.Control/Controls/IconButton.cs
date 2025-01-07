using System.Windows;

namespace UselessTool.Control.Controls;

public class IconButton : ButtonBase
{
    static IconButton()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(IconButton), new FrameworkPropertyMetadata(typeof(IconButton)));
    }
}
