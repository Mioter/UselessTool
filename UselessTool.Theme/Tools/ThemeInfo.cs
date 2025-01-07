namespace UselessTool.Theme.Tools;

public enum ThemeResourceType
{
    Colour,
    Styles,
    Icons
}

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
