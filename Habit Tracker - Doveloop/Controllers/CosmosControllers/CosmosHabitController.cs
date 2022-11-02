using System;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Habit_Tracker___Doveloop.Models.CosmosModels;
using Habit_Tracker___Doveloop.Models.CosmosModels.ViewModels;
using Habit_Tracker___Doveloop.Data;

namespace Habit_Tracker___Doveloop.Controllers.CosmosControllers
{
    public class CosmosHabitController : Controller
    {
        private readonly ICosmosDbService _cosmosDbService;
        public CosmosHabitController(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        private CosmosHabitViewModel CreateHabitViewModel(CosmosHabit habit, List<CosmosHabit> habitsLabels)
        {
            CosmosHabitViewModel viewModel = new CosmosHabitViewModel();
            viewModel.Habit = habit;
            List<CosmosLabel> labels = new List<CosmosLabel>();
            foreach (Guid labelId in habit.Labels)
            {
                CosmosLabel label = new CosmosLabel();
                CosmosHabit item = habitsLabels.Single(l => l.Id == labelId);
                label.Id = item.Id;
                label.Type = item.Type;
                label.User = item.User;
                label.Name = item.Name;
                labels.Add(label);
            }
            viewModel.Labels = labels;
            return viewModel;
        }

        public async Task<IActionResult> Index()
        {
            _cosmosDbService.SetUser(HttpContext.User.Identity.Name);
            var habitsLabels = await _cosmosDbService.GetHabitsAsync("Select * FROM c WHERE c.user = \"" + HttpContext.User.Identity.Name + "\"");
            List<CosmosHabitViewModel> viewModels = new List<CosmosHabitViewModel>();
            foreach (CosmosHabit habit in habitsLabels.ToList().Where(h => h.Type == "habit"))
            {
                viewModels.Add(CreateHabitViewModel(habit, habitsLabels.ToList()));
            }
            return View(viewModels);
        }

        public IActionResult Create()
        {
            CosmosHabit habit = new CosmosHabit();
            habit.User = HttpContext.User.Identity.Name;
            return View(habit);
        }

        [HttpPost]
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync([Bind("Id,Type,User,Name")] CosmosHabit habit)
        {
            habit.Labels = new Guid[0];
            if(!string.IsNullOrEmpty(habit.Name) && habit.User != null && habit.User == HttpContext.User.Identity.Name)
            {
                await _cosmosDbService.AddHabitAsync(habit);
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

            CosmosHabit habit = await _cosmosDbService.GetHabitAsync(id);
            if(habit == null)
            {
                return NotFound();
            }

            return View(habit);
        }

        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync([Bind("Id,Type,User,Name")] CosmosHabit habit)
        {
            if(ModelState.IsValid)
            {
                await _cosmosDbService.UpdateHabitAsync(habit);
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

            CosmosHabit habit = await _cosmosDbService.GetHabitAsync(id);
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
            await _cosmosDbService.DeleteHabitAsync(id);
            return RedirectToAction("Index");
        }

        [ActionName("Details")]
        public async Task<IActionResult> DetailsAsync(string id)
        {
            CosmosHabitViewModel viewModel = new CosmosHabitViewModel();
            viewModel.Habit = await _cosmosDbService.GetHabitAsync(id);
            //viewModel.Labels = await _cosmosDbService.GetLabelsAsync(viewModel.Habit.Labels);
            return View(viewModel);
        }
    }
}
