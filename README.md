# HabitTracker

A cross-platform mobile app built with **.NET MAUI** for tracking personal habits.

## What the app does

Users can create habits and mark them as completed on a day-by-day calendar. The app tracks how consistently a habit is kept over time and shows three stats per habit: current streak, total completions, and a monthly completion percentage.

Each habit is fully customizable — users can assign an emoji or image icon, pick a background color or search for a photo via the Unsplash API, and choose an accent color for the calendar.

## Features

- **Habit list** — create, view, and swipe-to-delete habits
- **Calendar view** — tap any past or present day to toggle completion
- **Stats** — streak, total, and monthly percentage, updated live
- **Notes** — free-text notes per habit, auto-saved with debounce
- **Customization** — emoji picker, color palette, Unsplash photo search
  
## Architecture

The project follows the **MVVM** pattern using CommunityToolkit.Mvvm for observable properties and relay commands. The three screens (main list, habit detail, habit edit) each have a dedicated ViewModel. Dependency injection is handled by MAUI's built-in DI container.

Habits are persisted locally as a JSON file in the app's data directory via `HabitService`.

## Project structure

```
HabitTracker/
├── Models/          # Habit data model
├── Services/        # IHabitService / HabitService (JSON persistence)
├── ViewModels/      # MainViewModel, HabitViewModel, HabitEditViewModel
├── Views/           # XAML pages (MainPage, HabitPage, HabitEditPage)
└── Converters/      # Value converters for XAML bindings
```

## Tech stack

| | |
|---|---|
| Framework | .NET MAUI |
| MVVM | CommunityToolkit.Mvvm |
| Persistence | System.Text.Json, local file |
| External API | Unsplash (background photos) |
| Platforms | iOS, Android, macOS, Windows |
