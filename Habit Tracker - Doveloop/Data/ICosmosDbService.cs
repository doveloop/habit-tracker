namespace Habit_Tracker___Doveloop.Data
{
    using Habit_Tracker___Doveloop.Models;
    public interface ICosmosDbService
    {
        public void SetUser(string user);
        public Task AddHabitLabelAsync(HabitLabel habit);
        public Task DeleteHabitLabelAsync(HabitLabel id);
        public Task<IEnumerable<HabitLabel>> GetHabitsAsync();
        public Task<IEnumerable<HabitLabel>> GetLabelsAsync();
        public Task<HabitLabel> GetHabitLabelAsync(string id);
        public Task<IEnumerable<HabitLabel>> GetHabitsLabelsAsync();
        public Task<IEnumerable<HabitLabel>> GetHabitsLabelsAsync(string query);
        public Task UpdateHabitLabelAsync(HabitLabel habit);
        public Task UpdateHabitLabelAsync(HabitLabel habit, List<Guid> oldRelationIds);
    }
}
