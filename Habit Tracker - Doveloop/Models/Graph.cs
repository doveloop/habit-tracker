namespace Habit_Tracker___Doveloop.Models
{
    public struct GraphData
    {
        public int value;
        public string label;
        public string color;
        public GraphData(int value, string label, string color)
        {
            this.value = value;
            this.label = label;
            this.color = color; 
        }
    }

    public class Graph
    {
        public GraphData[] planned { get; set; }
        public GraphData[] actual { get; set; }

        public Graph(GraphData[] planned, GraphData[] actual)
        {
            this.planned = planned;
            this.actual = actual;
        }
    }
}
