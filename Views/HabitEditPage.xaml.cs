using HabitTracker.Models;
using HabitTracker.ViewModels;

namespace HabitTracker.Views;

public partial class HabitEditPage : ContentPage
{
    public HabitEditPage(HabitEditViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    private async void OnCancelClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("..");

    private void OnEmojiTapped(object sender, TappedEventArgs e)
    {
        if (sender is BindableObject b && b.BindingContext is string emoji &&
            BindingContext is HabitEditViewModel vm)
            vm.SelectIconCommand.Execute(emoji);
    }

    private void OnBackgroundColorTapped(object sender, TappedEventArgs e)
    {
        if (sender is BindableObject b && b.BindingContext is string color &&
            BindingContext is HabitEditViewModel vm)
            vm.SelectBackgroundColorCommand.Execute(color);
    }

    private void OnAccentColorTapped(object sender, TappedEventArgs e)
    {
        if (sender is BindableObject b && b.BindingContext is string color &&
            BindingContext is HabitEditViewModel vm)
            vm.SelectAccentColorCommand.Execute(color);
    }

    private void OnPhotoTapped(object sender, TappedEventArgs e)
    {
        if (sender is BindableObject b && b.BindingContext is UnsplashPhoto photo &&
            BindingContext is HabitEditViewModel vm)
            vm.SelectBackgroundImageCommand.Execute(photo);
    }
}
