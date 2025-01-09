using System.Reflection;
using System.Windows.Media;

namespace UselessTool.Theme.Core;

public static class BasicColor
{
    
    /// <summary>
    /// 预设颜色名与画刷键值对。
    /// </summary>
    public static IEnumerable<ColorBrushPair> Brushes { get; } =
        typeof(Colors)
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Select(pi => new ColorBrushPair(pi.Name, new SolidColorBrush((Color)pi.GetValue(null)!)));


    /// <summary>
    /// 颜色名与画刷键值对。
    /// </summary>
    /// <param name="name">颜色名</param>
    /// <param name="brush">画刷</param>
    public class ColorBrushPair(string name, SolidColorBrush brush)
    {
        public string Name { get; set; } = name;
        public SolidColorBrush Brush { get; set; } = brush;
    }

}
