namespace UselessTool.Theme.Tools;

/// <summary>
/// 主题资源类型
/// </summary>
public enum ThemeResourceType
{
    Colors,
    Styles,
    Icons,
}

/// <summary>
/// 主题信息
/// </summary>
/// <param name="themeType">主题类型</param>
/// <param name="themeName">主题名称</param>
public readonly struct ThemeInfo(string themeType, string themeName) : IEquatable<ThemeInfo>
{
    public string ThemeType { get; } = themeType ?? throw new ArgumentNullException(nameof(themeType));
    public string ThemeName { get; } = themeName ?? throw new ArgumentNullException(nameof(themeName));

    public bool Equals(ThemeInfo other)
    {
        return ThemeType == other.ThemeType && ThemeName == other.ThemeName;
    }

    public override bool Equals(object? obj)
    {
        return obj is ThemeInfo info && Equals(info);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ThemeType, ThemeName);
    }

    public static bool operator ==(ThemeInfo left, ThemeInfo right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ThemeInfo left, ThemeInfo right)
    {
        return !(left == right);
    }
}

/// <summary>
/// 主题信息扩展方法
/// </summary>
public static class ThemeInfoExtensions
{
    /// <summary>
    /// 元组转换为主题信息
    /// </summary>
    /// <param name="tuple">元组</param>
    /// <returns>ThemeInfo结构</returns>
    public static ThemeInfo ToThemeInfo(this (string themeType, string themeName) tuple)
    {
        return new ThemeInfo(tuple.themeType, tuple.themeName);
    }
}
