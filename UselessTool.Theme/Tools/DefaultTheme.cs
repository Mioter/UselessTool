namespace UselessTool.Theme.Tools
{
    public class DefaultTheme
    {
        public static Dictionary<ThemeResourceType, Dictionary<string, string>> DefaultThemes { get; } =
            new()
            {
                {
                    ThemeResourceType.Colors,
                    new Dictionary<string, string>() { { "Light", "White" }, { "Dark", "Black" } }
                },
            };
    }
}
