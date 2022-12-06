namespace Habit_Tracker___Doveloop.Models
{
    using Newtonsoft.Json;
    public class GraphData
    {
        [JsonProperty(PropertyName = "filteredLabels")]
        public List<string> FilteredLabels { get; set; }
        [JsonProperty(PropertyName = "startDate")]
        public string StartDate { get; set; }
        [JsonProperty(PropertyName = "endDate")]
        public string EndDate { get; set; }
        [JsonProperty(PropertyName = "graphType")]
        public string GraphType { get; set; }

        /*public object Output()
        {
            return new
            {
                FilteredLabels = FilteredLabels,
                StartDate = StartDate == null ? null : StartDate.Value.ToString("yyyy-MM-dd"),
                EndDate = EndDate == null ? null : EndDate.Value.ToString("yyyy-MM-dd"),
                GraphType = GraphType
            };
        }*/

        public GraphData()
        {
            FilteredLabels = new List<string>();
            //GraphType = "bar";
            //StartDate = DateTime.Today;
            //EndDate = DateTime.Today;//.AddDays(1);
        }
    }
}
