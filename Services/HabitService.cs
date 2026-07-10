using System.Text.Json;
using HabitTracker.Models;

namespace HabitTracker.Services;

public class HabitService : IHabitService
{
    private readonly string _filePath = Path.Combine(FileSystem.AppDataDirectory, "habits.json");
    private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };
    private readonly SemaphoreSlim _lock = new(1, 1);
    private List<Habit>? _cache;

    public async Task<List<Habit>> GetHabitsAsync()
    {
        if (_cache is not null) return _cache;

        await _lock.WaitAsync();
        try
        {
            if (_cache is not null) return _cache;
            if (!File.Exists(_filePath)) return _cache = [];

            var json = await File.ReadAllTextAsync(_filePath);
            try
            {
                _cache = JsonSerializer.Deserialize<List<Habit>>(json) ?? [];
            }
            catch (JsonException)
            {
                _cache = [];
            }
            return _cache;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task SaveHabitAsync(Habit habit)
    {
        var habits = await GetHabitsAsync();
        var index = habits.FindIndex(h => h.Id == habit.Id);
        if (index >= 0)
            habits[index] = habit;
        else
            habits.Add(habit);

        await PersistAsync(habits);
    }

    public async Task DeleteHabitAsync(string id)
    {
        var habits = await GetHabitsAsync();
        habits.RemoveAll(h => h.Id == id);
        await PersistAsync(habits);
    }

    private async Task PersistAsync(List<Habit> habits)
    {
        _cache = habits;
        await File.WriteAllTextAsync(_filePath, JsonSerializer.Serialize(habits, _jsonOptions));
    }
}
