namespace Habit_Tracker___Doveloop.Data
{
    using Habit_Tracker___Doveloop.Models;
    public interface ICosmosDbService
    {
        public void SetUser(string user);
        Task AddHabitLabelAsync(HabitLabel habit);
        Task DeleteHabitLabelAsync(HabitLabel id);
        Task<IEnumerable<HabitLabel>> GetHabitsAsync();
        Task<IEnumerable<HabitLabel>> GetLabelsAsync();
        Task<HabitLabel> GetHabitLabelAsync(string id);
        Task<IEnumerable<HabitLabel>> GetHabitsLabelsAsync();
        Task<IEnumerable<HabitLabel>> GetHabitsLabelsAsync(string query);
        Task UpdateHabitLabelAsync(HabitLabel habit);
        Task UpdateHabitLabelAsync(HabitLabel habit, List<Guid> oldRelationIds);
    }
}
