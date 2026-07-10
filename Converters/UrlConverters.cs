using System.Globalization;

namespace HabitTracker.Converters;

/// <summary>Returns true when the string value starts with http:// or https://.</summary>
public class IsUrlConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is string s && (s.StartsWith("http://") || s.StartsWith("https://"));

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

/// <summary>Returns true when the string is NOT a URL (emoji or other text).</summary>
public class IsNotUrlConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string s && !string.IsNullOrEmpty(s))
            return !(s.StartsWith("http://") || s.StartsWith("https://"));
        return false;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
