namespace Habit_Tracker___Doveloop.Models
{
    using Newtonsoft.Json;
    public class HabitEntry
    {
        [JsonProperty(PropertyName = "dateTime")]
        public string DateTime { get; set; }

        [JsonProperty(PropertyName = "units")]
        public int Units { get; set; }
    }
}
