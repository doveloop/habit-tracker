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
            ViewBag.PreviousGraph = (await _cosmosDbService.GetProfileAsync()).GraphData;
            return View(viewModels);
        }
    }
}
