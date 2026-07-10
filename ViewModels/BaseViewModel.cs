using CommunityToolkit.Mvvm.ComponentModel;

namespace HabitTracker.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isBusy;
}
