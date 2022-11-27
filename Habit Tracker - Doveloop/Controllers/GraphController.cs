using Microsoft.AspNetCore.Mvc;
using Habit_Tracker___Doveloop.Models;
using Habit_Tracker___Doveloop.Models.ViewModels;
using Habit_Tracker___Doveloop.Data;

namespace Habit_Tracker___Doveloop.Controllers
{
    public class GraphController : Controller
    {
        private readonly ICosmosDbService _cosmosDbService;
        public GraphController(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        private HabitViewModel CreateHabitViewModel(HabitLabel habit, IEnumerable<HabitLabel> habitsLabels)
        {
            List<HabitLabel> labels = new List<HabitLabel>();
            if (habit.RelationIds != null) habit.RelationIds.ForEach(id => {
                HabitLabel label = habitsLabels.SingleOrDefault(l => l.Id == id);
                if (label != null)
                    labels.Add(label);
            });
            return new HabitViewModel(habit, labels);
        }

        public async Task<IActionResult> Index()
        {
            List<HabitViewModel> viewModels = new List<HabitViewModel>();
            IEnumerable<HabitLabel> habitsLabels = await _cosmosDbService.GetHabitsLabelsAsync();
            habitsLabels.Where(h => h.Type == "habit").ToList().ForEach(h => viewModels.Add(CreateHabitViewModel(h, habitsLabels)));
            return View(viewModels);
            /*return View(new Graph(
                new GraphData[]
                {
                    new GraphData(5,    "Personal",     "#a2de6b"),
                    new GraphData(10,   "Professional", "#ffb763"),
                    new GraphData(15,   "Huh?",         "#be81f1"),
                    new GraphData(20,   "What?",        "#4de79e"),
                },
                new GraphData[]
                {
                    new GraphData(3,    "Personal",     "#aac090"),
                    new GraphData(7,    "Professional", "#c0aa90"),
                    new GraphData(12,   "Huh?",         "#aa90c0"),
                    new GraphData(15,   "What?",        "#90c0aa"),
                }
                )
            );*/
        }
    }
}
