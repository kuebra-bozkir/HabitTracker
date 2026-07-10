using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using HabitTracker.Converters;
using HabitTracker.Models;
using HabitTracker.Services;

namespace HabitTracker.ViewModels;

[QueryProperty(nameof(HabitId), "id")]
public partial class HabitViewModel : BaseViewModel
{
    private readonly IHabitService _habitService;
    private Habit? _habit;

    [ObservableProperty] private string _habitId = string.Empty;

    // Display
    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private string? _icon;
    [ObservableProperty] private string? _backgroundColor;
    [ObservableProperty] private string? _backgroundImageUrl;

    // Calendar
    [ObservableProperty] private ObservableCollection<CalendarDay> _calendarDays = new();
    [ObservableProperty] private int _calendarHeight;
    [ObservableProperty] private string _monthYearLabel = string.Empty;
    [ObservableProperty] private int _currentYear;
    [ObservableProperty] private int _currentMonth;

    [ObservableProperty] private CalendarDay? _selectedDay;

    partial void OnSelectedDayChanged(CalendarDay? value)
    {
        if (value is null) return;
        SelectedDay = null;
        _ = ToggleDayAsync(value);
    }

    // Notes
    [ObservableProperty] private string _notes = string.Empty;
    private CancellationTokenSource? _notesSaveCts;

    partial void OnNotesChanged(string value)
    {
        _notesSaveCts?.Cancel();
        _notesSaveCts = new CancellationTokenSource();
        _ = SaveNotesDebounced(_notesSaveCts.Token);
    }

    private async Task SaveNotesDebounced(CancellationToken token)
    {
        try
        {
            await Task.Delay(800, token);
            if (_habit is null) return;
            _habit.Notes = string.IsNullOrWhiteSpace(Notes) ? null : Notes;
            await _habitService.SaveHabitAsync(_habit);
        }
        catch (OperationCanceledException) { }
    }

    // Stats
    [ObservableProperty] private int _streak;
    [ObservableProperty] private int _total;
    [ObservableProperty] private int _percentage;

    public HabitViewModel(IHabitService habitService)
    {
        _habitService = habitService;
        var now = DateTime.Today;
        CurrentYear = now.Year;
        CurrentMonth = now.Month;
    }

    partial void OnHabitIdChanged(string value)
    {
        if (!string.IsNullOrEmpty(value))
            _ = LoadHabitAsync();
    }

    [RelayCommand]
    public async Task LoadHabitAsync()
    {
        var habits = await _habitService.GetHabitsAsync();
        _habit = habits.FirstOrDefault(h => h.Id == HabitId);
        if (_habit is null) return;

        Name = _habit.Name;
        Icon = _habit.Icon;
        BackgroundColor = _habit.BackgroundColor;
        BackgroundImageUrl = _habit.BackgroundImageUrl;
        Notes = _habit.Notes ?? string.Empty;

        RefreshCalendar();    }

    private static readonly Color _fallbackAccent = Color.FromArgb("#4CAF50");

    private void RefreshCalendar()
    {
        MonthYearLabel = new DateTime(CurrentYear, CurrentMonth, 1).ToString("MMMM yyyy");

        var firstDay = new DateTime(CurrentYear, CurrentMonth, 1);
        var daysInMonth = DateTime.DaysInMonth(CurrentYear, CurrentMonth);
        var startOffset = (int)firstDay.DayOfWeek;

        var isDark = Application.Current?.RequestedTheme == AppTheme.Dark;
        var defaultBg = isDark ? Color.FromArgb("#2C2C2E") : Color.FromArgb("#F5F5F5");
        var accent = (!string.IsNullOrEmpty(_habit?.AccentColor) &&
                      ColorNameToColorConverter.Map.TryGetValue(_habit.AccentColor, out var ac))
                     ? ac : _fallbackAccent;

        var days = new List<CalendarDay>();

        for (int i = 0; i < startOffset; i++)
            days.Add(new CalendarDay { IsEmpty = true, DefaultBackground = defaultBg, AccentColor = accent });

        for (int d = 1; d <= daysInMonth; d++)
        {
            var date = new DateTime(CurrentYear, CurrentMonth, d);
            days.Add(new CalendarDay
            {
                Date = date,
                DayNumber = d,
                IsCompleted = _habit?.CompletedDates.Any(c => c.Date == date) ?? false,
                IsToday = date == DateTime.Today,
                IsFuture = date > DateTime.Today,
                DefaultBackground = defaultBg,
                AccentColor = accent,
            });
        }

        while (days.Count % 7 != 0)
            days.Add(new CalendarDay { IsEmpty = true, DefaultBackground = defaultBg, AccentColor = accent });

        CalendarDays = new ObservableCollection<CalendarDay>(days);
        CalendarHeight = (days.Count / 7) * 44;
        CalculateStats();
    }

    private void CalculateStats()
    {
        if (_habit is null) return;

        Total = _habit.CompletedDates.Count(d => d.Date <= DateTime.Today);

        var streak = 0;
        var checkDate = DateTime.Today;
        while (_habit.CompletedDates.Any(d => d.Date == checkDate))
        {
            streak++;
            checkDate = checkDate.AddDays(-1);
        }
        Streak = streak;

        // Percentage: completed days / total days in the viewed month × 100
        var daysInMonth = DateTime.DaysInMonth(CurrentYear, CurrentMonth);
        var completedInViewed = _habit.CompletedDates
            .Count(d => d.Year == CurrentYear && d.Month == CurrentMonth);
        Percentage = (int)Math.Round((double)completedInViewed / daysInMonth * 100);
    }

    private async Task ToggleDayAsync(CalendarDay day)
    {
        if (_habit is null || !day.IsInteractable) return;

        if (day.IsCompleted)
        {
            day.IsCompleted = false;
            _habit.CompletedDates.RemoveAll(d => d.Date == day.Date);
        }
        else
        {
            day.IsCompleted = true;
            _habit.CompletedDates.Add(day.Date);
        }

        CalculateStats();
        await _habitService.SaveHabitAsync(_habit);
    }

    [RelayCommand]
    private void PreviousMonth()
    {
        if (CurrentMonth == 1) { CurrentMonth = 12; CurrentYear--; }
        else CurrentMonth--;
        RefreshCalendar();
    }

    [RelayCommand]
    private void NextMonth()
    {
        var now = DateTime.Today;
        if (CurrentYear == now.Year && CurrentMonth == now.Month) return;
        if (CurrentMonth == 12) { CurrentMonth = 1; CurrentYear++; }
        else CurrentMonth++;
        RefreshCalendar();
    }

    [RelayCommand]
    private async Task NavigateToEditAsync()
        => await Shell.Current.GoToAsync($"{nameof(Views.HabitEditPage)}?id={HabitId}");
}
