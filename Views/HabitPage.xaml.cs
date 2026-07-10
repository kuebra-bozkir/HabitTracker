using HabitTracker.ViewModels;

namespace HabitTracker.Views;

public partial class HabitPage : ContentPage
{
    private readonly HabitViewModel _vm;
    private bool _firstAppear = true;

    public HabitPage(HabitViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_firstAppear)
        {
            // OnHabitIdChanged already triggered the first load; skip the duplicate.
            _firstAppear = false;
            return;
        }
        // Reload after returning from the edit page.
        if (!string.IsNullOrEmpty(_vm.HabitId))
            await _vm.LoadHabitCommand.ExecuteAsync(null);
    }

    private async void OnBackClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("..");
}
