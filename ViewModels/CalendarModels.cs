using CommunityToolkit.Mvvm.ComponentModel;

namespace HabitTracker.ViewModels;

public partial class CalendarDay : ObservableObject
{
    public DateTime Date { get; init; }
    public int DayNumber { get; init; }
    public bool IsToday { get; init; }
    public bool IsEmpty { get; init; }
    public bool IsFuture { get; init; }
    public bool IsInteractable => !IsEmpty && !IsFuture;

    // Set by the ViewModel when the calendar is built
    public Color AccentColor { get; init; } = Color.FromArgb("#4CAF50");
    public Color DefaultBackground { get; init; } = Color.FromArgb("#F5F5F5");

    [ObservableProperty] private bool _isCompleted;

    // Recalculated whenever IsCompleted changes
    partial void OnIsCompletedChanged(bool value) => OnPropertyChanged(nameof(CellBackground));

    public Color CellBackground =>
        IsEmpty      ? Colors.Transparent :
        IsCompleted  ? AccentColor        :
                       DefaultBackground;
}
