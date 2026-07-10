using System.Globalization;

namespace HabitTracker.Converters;

/// <summary>
/// Converts IsEditVisible (bool) to a chevron character for the edit panel toggle.
/// </summary>
public class ChevronConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is true ? "▾" : "▸";

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
