using System;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Habit_Tracker___Doveloop.Models;
using Habit_Tracker___Doveloop.Models.ViewModels;
using Habit_Tracker___Doveloop.Data;

namespace Habit_Tracker___Doveloop.Controllers
{
    public class CosmosHabitLabelController : Controller
    {
        private readonly ICosmosDbService _cosmosDbService;
        public CosmosHabitLabelController(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        private CosmosHabitViewModel CreateHabitViewModel(HabitLabel habit, List<HabitLabel> habitsLabels)
        {
            CosmosHabitViewModel viewModel = new CosmosHabitViewModel();
            viewModel.Habit = habit;
            List<HabitLabel> labels = new List<HabitLabel>();
            if (habit.RelationIds != null) habit.RelationIds.ForEach(id => labels.Add(habitsLabels.Single(l => l.Id == id)));
            viewModel.Labels = labels;
            return viewModel;
        }

        public async Task<IActionResult> Index()
        {
            _cosmosDbService.SetUser(HttpContext.User.Identity.Name);
            var habitsLabels = await _cosmosDbService.GetHabitsLabelsAsync("Select * FROM c WHERE c.user = \"" + HttpContext.User.Identity.Name + "\"");
            List<CosmosHabitViewModel> viewModels = new List<CosmosHabitViewModel>();
            foreach (HabitLabel habit in habitsLabels.ToList().Where(h => h.Type == "habit"))
            {
                viewModels.Add(CreateHabitViewModel(habit, habitsLabels.ToList()));
            }
            return View(viewModels);
        }

        public IActionResult Create()
        {
            HabitLabel habit = new HabitLabel();
            habit.User = HttpContext.User.Identity.Name;
            return View(habit);
        }

        [HttpPost]
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync([Bind("Id,Type,User,Name")] HabitLabel habit)
        {
            habit.RelationIds = new List<Guid>();
            if(!string.IsNullOrEmpty(habit.Name) && habit.User != null && habit.User == HttpContext.User.Identity.Name)
            {
                await _cosmosDbService.AddHabitLabelAsync(habit);
                return RedirectToAction("Index");
            }
            return View(habit);
        }

        [ActionName("Edit")]
        public async Task<IActionResult> EditAsync(string id)
        {
            if(id == null)
            {
                return NotFound();
            }

            HabitLabel habit = await _cosmosDbService.GetHabitLabelAsync(id);
            if(habit == null)
            {
                return NotFound();
            }

            return View(habit);
        }

        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync([Bind("Id,Type,User,Name")] HabitLabel habit)
        {
            if(ModelState.IsValid)
            {
                await _cosmosDbService.UpdateHabitLabelAsync(habit);
                return RedirectToAction("Index");
            }
            return View(habit);
        }

        [ActionName("Delete")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            if(id == null)
            {
                return NotFound();
            }

            HabitLabel habit = await _cosmosDbService.GetHabitLabelAsync(id);
            if (habit == null)
            {
                return NotFound();
            }

            return View(habit);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedAsync([Bind("Id")] string id)
        {
            await _cosmosDbService.DeleteHabitLabelAsync(id);
            return RedirectToAction("Index");
        }

        [ActionName("Details")]
        public async Task<IActionResult> DetailsAsync(string id)
        {
            CosmosHabitViewModel viewModel = new CosmosHabitViewModel();
            viewModel.Habit = await _cosmosDbService.GetHabitLabelAsync(id);
            return View(viewModel);
        }
    }
}
