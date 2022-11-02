namespace Habit_Tracker___Doveloop.Models.CosmosModels
{
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations.Schema;

    public class CosmosLabel
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; } = Guid.NewGuid();//unique key in database

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "user")]
        public string User { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}
