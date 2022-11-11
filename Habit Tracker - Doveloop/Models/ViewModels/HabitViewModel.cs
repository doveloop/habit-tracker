namespace Habit_Tracker___Doveloop.Models.ViewModels
{
    public class HabitViewModel
    {
        public HabitLabel Habit { get; set; }
        public List<HabitLabel> Labels { get; set; }

        public HabitViewModel(HabitLabel habit, IEnumerable<HabitLabel> labels)
        {
            Habit = habit;
            Labels = labels.ToList();
        }
    }
}
