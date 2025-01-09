using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace UselessTool.Utilities.Converter;

public class ValueToVisibilityConverter : IValueConverter
{
    private const char Colon = ':';
    private const char Equal = '#';
    private const char InvertMarker = '!';
    private const char HiddenMarker = 'H';

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (parameter is not string parameterString)
            return GetDefaultVisibility(GetBooleanValue(value));

        var settings = ParseParameter(parameterString);
        bool isVisible = EvaluateValueAndParameter(value, parameterString, settings);

        return GetVisibility(isVisible, settings);
    }

    private static (bool UseCollapsed, bool InvertValue) ParseParameter(string parameterString)
    {
        bool invertValue = parameterString.Contains(InvertMarker);
        bool useCollapsed = !parameterString.Contains(HiddenMarker);
        return (useCollapsed, invertValue);
    }

    private static bool EvaluateValueAndParameter(object? value, string parameterString, (bool UseCollapsed, bool InvertValue) settings)
    {
        bool isVisible = GetBooleanValue(value);    // 参数不为字符串的条件下将值条件转换为bool
        if (!parameterString.Contains(Colon))
            return ApplyInversion(isVisible, settings.InvertValue);

        string[] parts = parameterString.Split(Colon);
        string leftSide = parts[0].Trim();

        isVisible = parts[0].Contains(Equal)
            ? EvaluateEquality(leftSide)
            : EvaluateLeftSide(leftSide, value?.ToString() ?? string.Empty);

        return ApplyInversion(isVisible, settings.InvertValue);
    }

    private static bool EvaluateEquality(string leftSide)
    {
        int equalIndex = leftSide.IndexOf(Equal);
        string leftPart = leftSide[..equalIndex].Trim();
        string rightPart = leftSide[(equalIndex + 1)..].Trim();
        return string.Equals(leftPart, rightPart, StringComparison.OrdinalIgnoreCase);
    }

    private static bool EvaluateLeftSide(string leftSide, string value)
    {
        return string.Equals(leftSide, value, StringComparison.OrdinalIgnoreCase);
    }

    private static bool ApplyInversion(bool isVisible, bool invertValue)
    {
        return invertValue ? !isVisible : isVisible;
    }

    private static Visibility GetVisibility(bool isVisible, (bool UseCollapsed, bool InvertValue) settings)
    {
        return isVisible 
            ? Visibility.Visible 
            : settings.UseCollapsed ? Visibility.Collapsed : Visibility.Hidden;
    }

    private static bool GetBooleanValue(object? value)
    {
        return value switch
        {
            bool bVal => bVal,
            string sVal => !string.IsNullOrWhiteSpace(sVal),
            0 => false,
            _ => true
        };
    }

    private static Visibility GetDefaultVisibility(bool isVisible)
    {
        return isVisible ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Visibility visibility)
            throw new ArgumentException("Value must be of type Visibility", nameof(value));

        return visibility == Visibility.Visible;
    }
}