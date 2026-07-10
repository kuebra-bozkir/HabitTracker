using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HabitTracker.Models;
using HabitTracker.Services;

namespace HabitTracker.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private readonly IHabitService _habitService;

    [ObservableProperty]
    private ObservableCollection<Habit> _habits = new();

    // Bound to CollectionView.SelectedItem; navigation fires in the partial method.
    [ObservableProperty]
    private Habit? _selectedHabit;

    partial void OnSelectedHabitChanged(Habit? value)
    {
        if (value is null) return;
        // Clear selection first so the same item can be tapped again after returning.
        SelectedHabit = null;
        _ = Shell.Current.GoToAsync($"{nameof(Views.HabitPage)}?id={value.Id}");
    }

    public MainViewModel(IHabitService habitService)
    {
        _habitService = habitService;
    }

    [RelayCommand]
    public async Task LoadHabitsAsync()
    {
        var habits = await _habitService.GetHabitsAsync();
        Habits = new ObservableCollection<Habit>(habits);
    }

    [RelayCommand]
    private async Task CreateHabitAsync()
    {
        var name = await Shell.Current.DisplayPromptAsync(
            "New Habit",
            null,
            placeholder: "e.g. Drink water",
            accept: "Create",
            cancel: "Cancel",
            maxLength: 60);

        if (string.IsNullOrWhiteSpace(name)) return;

        var habit = new Habit { Name = name.Trim() };
        await _habitService.SaveHabitAsync(habit);
        await Shell.Current.GoToAsync($"{nameof(Views.HabitPage)}?id={habit.Id}");
    }

    [RelayCommand]
    private async Task DeleteHabitAsync(Habit habit)
    {
        var confirmed = await Shell.Current.DisplayAlert(
            "Delete Habit",
            $"Delete \"{habit.Name}\"? This cannot be undone.",
            "Delete", "Cancel");
        if (!confirmed) return;
        await _habitService.DeleteHabitAsync(habit.Id);
        Habits.Remove(habit);
    }
}
