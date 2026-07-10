namespace HabitTracker.Models;

public class Habit
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;

    // Can be an emoji, image URL, or gif URL
    public string? Icon { get; set; }

    // Header/cover: choose one — color name or image URL
    public string? HeaderColor { get; set; }
    public string? HeaderImageUrl { get; set; }

    // Background: choose one — color name or image URL
    public string? BackgroundColor { get; set; }
    public string? BackgroundImageUrl { get; set; }

    public string? AccentColor { get; set; }   // null = default green
    public bool HasCover { get; set; } = true;
    public string? Notes { get; set; }

    public List<DateTime> CompletedDates { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.Today;

    public int Total => CompletedDates.Count(d => d.Date <= DateTime.Today);

    public int CurrentStreak
    {
        get
        {
            var dates = CompletedDates.Select(x => x.Date).ToHashSet();
            var s = 0;
            var d = DateTime.Today;
            while (dates.Contains(d)) { s++; d = d.AddDays(-1); }
            return s;
        }
    }
}
