namespace Habit_Tracker___Doveloop.Data
{
    using Habit_Tracker___Doveloop.Models;
    public interface ICosmosDbService
    {
        public void SetUser(string user);
        Task AddHabitLabelAsync(HabitLabel habit);
        Task DeleteHabitLabelAsync(string id);
        Task<HabitLabel> GetHabitLabelAsync(string id);
        Task<IEnumerable<HabitLabel>> GetHabitsLabelsAsync(string query);
        Task UpdateHabitLabelAsync(HabitLabel habit);
    }
}
