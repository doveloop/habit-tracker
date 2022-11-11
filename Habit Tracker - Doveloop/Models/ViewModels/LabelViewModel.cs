namespace Habit_Tracker___Doveloop.Models.ViewModels
{
    public class LabelViewModel
    {
        public HabitLabel Label { get; set; }
        public List<HabitLabel> Habits { get; set; }

        public LabelViewModel(HabitLabel label, IEnumerable<HabitLabel> habits)
        {
            Label = label;
            Habits = habits.ToList();
        }
    }
}
