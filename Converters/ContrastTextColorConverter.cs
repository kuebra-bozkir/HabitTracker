using System.Globalization;

namespace HabitTracker.Converters;

/// <summary>
/// Given a background Color, returns Black or White — whichever gives better contrast.
/// Uses the standard relative luminance formula (WCAG).
/// </summary>
public class ContrastTextColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Color bg)
        {
            var luminance = 0.2126 * Linearise(bg.Red)
                          + 0.7152 * Linearise(bg.Green)
                          + 0.0722 * Linearise(bg.Blue);
            return luminance > 0.179 ? Colors.Black : Colors.White;
        }
        return Colors.Black;
    }

    private static double Linearise(float channel)
    {
        double c = channel;
        return c <= 0.04045 ? c / 12.92 : Math.Pow((c + 0.055) / 1.055, 2.4);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
