namespace Habit_Tracker___Doveloop.Data
{
    using Habit_Tracker___Doveloop.Models;
    public interface ICosmosDbService
    {
        public void SetUser(string user);

        public Task<UserProfile> GetProfileAsync();
        public Task UpdateProfileAsync(UserProfile profile);
        public Task AddHabitEntryAsync(string id, DateTime time, float units);
        public Task AddHabitLabelAsync(HabitLabel habitLabel);
        public Task DeleteHabitLabelAsync(HabitLabel habitLabel);
        public Task<IEnumerable<HabitLabel>> GetHabitsAsync();
        public Task<IEnumerable<HabitLabel>> GetLabelsAsync();
        public Task<HabitLabel> GetHabitLabelAsync(string id);
        public Task<IEnumerable<HabitLabel>> GetHabitsLabelsAsync();
        public Task<IEnumerable<HabitLabel>> GetHabitsLabelsAsync(string query);
        public Task UpdateHabitLabelAsync(HabitLabel habitLabel);
        public Task UpdateHabitLabelAsync(HabitLabel habitLabel, List<Guid> oldRelationIds);
    }
}
