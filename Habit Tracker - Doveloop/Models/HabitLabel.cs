namespace Habit_Tracker___Doveloop.Models
{
    using Newtonsoft.Json;
    public class HabitLabel
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; } = Guid.NewGuid();//unique key in database

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }//either "habit" or "label"

        [JsonProperty(PropertyName = "user")]
        public string User { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "units")]
        public string Units { get; set; }

        [JsonProperty(PropertyName = "entries")]
        public List<HabitEntry> Entries { get; set; }

        [JsonProperty(PropertyName = "relationIds")]//Habits have label ids, labels have habit ids
        public List<Guid> RelationIds { get; set; }
    }
}
