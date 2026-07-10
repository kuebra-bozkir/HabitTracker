using HabitTracker.Models;

namespace HabitTracker.Services;

public interface IHabitService
{
    Task<List<Habit>> GetHabitsAsync();
    Task SaveHabitAsync(Habit habit);
    Task DeleteHabitAsync(string id);
}
