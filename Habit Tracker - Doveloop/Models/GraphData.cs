namespace Habit_Tracker___Doveloop.Models
{
    using Newtonsoft.Json;
    public class GraphData
    {
        [JsonProperty(PropertyName = "filteredLabels")]
        public List<string> FilteredLabels { get; set; }
        [JsonProperty(PropertyName = "startDate")]
        public DateTime? StartDate { get; set; }
        [JsonProperty(PropertyName = "endDate")]
        public DateTime? EndDate { get; set; }
        [JsonProperty(PropertyName = "graphType")]
        public string GraphType { get; set; }

        public GraphData()
        {
            FilteredLabels = new List<string>();
        }
    }
}
