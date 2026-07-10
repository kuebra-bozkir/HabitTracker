using HabitTracker.Views;

namespace HabitTracker;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(HabitPage),     typeof(HabitPage));
        Routing.RegisterRoute(nameof(HabitEditPage), typeof(HabitEditPage));
    }
}
