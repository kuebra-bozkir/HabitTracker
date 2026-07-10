using HabitTracker.Models;
using HabitTracker.ViewModels;

namespace HabitTracker.Views;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel _vm;

    public MainPage(MainViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadHabitsCommand.ExecuteAsync(null);
    }

    private async void OnDeleteInvoked(object sender, EventArgs e)
    {
        if (sender is SwipeItem item && item.BindingContext is Habit habit)
            await _vm.DeleteHabitCommand.ExecuteAsync(habit);
    }
}
