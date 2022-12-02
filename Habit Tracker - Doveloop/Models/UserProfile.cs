namespace Habit_Tracker___Doveloop.Models
{
    using Newtonsoft.Json;
    public class UserProfile
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; } = Guid.NewGuid();
        [JsonProperty(PropertyName = "user")]
        public string User { get; set; }
        [JsonProperty(PropertyName = "graphData")]
        public GraphData GraphData { get; set; }

        public UserProfile(string _user)
        {
            User = _user;
            GraphData = new GraphData();
        }
    }
}
