using System.Reflection;
using System.Windows.Media;

namespace UselessTool.Theme.Core;

public static class BasicColor
{
    /// <summary>
    /// 预设颜色名与画刷键值对。
    /// </summary>
    public static IEnumerable<object> Brushes { get; } =
        typeof(Colors)
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Select(pi => new { pi.Name, Brush = new SolidColorBrush((Color)pi.GetValue(null)!) });
}
