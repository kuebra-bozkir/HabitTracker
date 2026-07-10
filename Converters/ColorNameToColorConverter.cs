using System.Globalization;

namespace HabitTracker.Converters;

/// <summary>
/// Maps a color name string (e.g. "PastelBlue") to a MAUI Color.
/// Returns null for unknown names so the caller can fall back gracefully.
/// </summary>
public class ColorNameToColorConverter : IValueConverter
{
    public static readonly Dictionary<string, Color> Map = new()
    {
        { "Yellow",       Color.FromArgb("#FFC107") },
        { "Orange",       Color.FromArgb("#FF6D00") },
        { "Red",          Color.FromArgb("#F44336") },
        { "Pink",         Color.FromArgb("#E91E8C") },
        { "Purple",       Color.FromArgb("#9C27B0") },
        { "Blue",         Color.FromArgb("#2196F3") },
        { "Green",        Color.FromArgb("#4CAF50") },
        { "Black",        Color.FromArgb("#212121") },
        { "White",        Color.FromArgb("#FAFAFA") },
        { "PastelYellow", Color.FromArgb("#FFF9C4") },
        { "PastelOrange", Color.FromArgb("#FFE0B2") },
        { "PastelRed",    Color.FromArgb("#FFCDD2") },
        { "PastelPink",   Color.FromArgb("#F8BBD0") },
        { "PastelPurple", Color.FromArgb("#E1BEE7") },
        { "PastelBlue",   Color.FromArgb("#BBDEFB") },
        { "PastelGreen",  Color.FromArgb("#C8E6C9") },
        { "PastelTeal",   Color.FromArgb("#B2DFDB") },
    };

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string name && Map.TryGetValue(name, out var color))
            return color;
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
