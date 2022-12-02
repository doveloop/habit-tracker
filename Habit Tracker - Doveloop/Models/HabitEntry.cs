namespace Habit_Tracker___Doveloop.Models
{
    using Newtonsoft.Json;
    public class HabitEntry
    {
        [JsonProperty(PropertyName = "dateTime")]
        public DateTime dateTime { get; set; }

        [JsonProperty(PropertyName = "units")]
        public float Units { get; set; }
    }
}
