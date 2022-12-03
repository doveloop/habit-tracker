using System;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Habit_Tracker___Doveloop.Models;
using Habit_Tracker___Doveloop.Models.ViewModels;
using Habit_Tracker___Doveloop.Data;

namespace Habit_Tracker___Doveloop.Controllers
{
    public class HabitController : Controller
    {
        private readonly ICosmosDbService _cosmosDbService;
        public HabitController(ICosmosDbService cosmosDbService)
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

        private IEnumerable<HabitViewModel> CreateHabitViewModels(IEnumerable<HabitLabel> habitsLabels)
        {
            List<HabitViewModel> viewModels = new List<HabitViewModel>();
            habitsLabels.Where(h => h.Type == "habit").ToList().ForEach(h => viewModels.Add(CreateHabitViewModel(h, habitsLabels)));
            return viewModels;
        }

        private void CreateLabelViewModel(List<HabitLabel> allLabels)
        {
            List<AppliedData> labelViewModel = new List<AppliedData>();
            allLabels.ForEach(label =>
                labelViewModel.Add(
                    new AppliedData
                    {
                        Id = label.Id,
                        Name = label.Name
                    }
                )
            );

            ViewBag.LabelViewModel = labelViewModel;
        }

        private HabitViewModel PopulateAppliedLabelData(HabitLabel habit, List<HabitLabel> allLabels)
        {
            HabitViewModel habitViewModel = CreateHabitViewModel(habit, allLabels);
            CreateLabelViewModel(allLabels);
            return habitViewModel;
        }

        public async Task<IActionResult> AddHabitEntry(string id, float units)
        {
            await _cosmosDbService.AddHabitEntryAsync(id, DateTime.UtcNow, units);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Index()
        {
            _cosmosDbService.SetUser(HttpContext.User.Identity.Name);
            return View(CreateHabitViewModels(await _cosmosDbService.GetHabitsLabelsAsync()));
        }

        public async Task<IActionResult> Create()
        {
            HabitLabel habit = new HabitLabel();
            habit.User = HttpContext.User.Identity.Name;
            habit.Type = "habit";
            //habit.Units = "";;//could make a default unit
            CreateLabelViewModel((await _cosmosDbService.GetLabelsAsync()).ToList());
            return View(habit);
        }

        [HttpPost]
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync([Bind("Id,Type,User,Name,Units")] HabitLabel habit)
        {
            habit.RelationIds = new List<Guid>();
            habit.Entries = new List<HabitEntry>();
            if (!string.IsNullOrEmpty(habit.Name) && !string.IsNullOrEmpty(habit.Units) && habit.Type == "habit" && habit.User != null && habit.User == HttpContext.User.Identity.Name)
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
            IEnumerable<HabitLabel> habitsLabels = await _cosmosDbService.GetHabitsLabelsAsync();
            HabitLabel habit = habitsLabels.Single(h => h.Id.ToString() == id);
            if(habit == null || habit.Type != "habit" || habit.User != HttpContext.User.Identity.Name)
            {
                return NotFound();
            }
            
            return View(PopulateAppliedLabelData(habit, habitsLabels.Where(l => l.Type == "label").ToList()));
        }

        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync(string id, string habitName, string habitUnits, string[] selectedLabels)
        {
            if (id == null)
            {
                return NotFound();
            }
            IEnumerable<HabitLabel> habitsLabels = await _cosmosDbService.GetHabitsLabelsAsync();
            HabitLabel habit = habitsLabels.Single(h => h.Id.ToString() == id);

            if (habit == null || habit.Type != "habit" || habit.User != HttpContext.User.Identity.Name)
            {
                return NotFound();
            }

            try
            {
                habit.Name = habitName;
                habit.Units = habitUnits;
                List<Guid> oldLabelIds = habit.RelationIds;
                habit.RelationIds = selectedLabels.ToList().ConvertAll(Guid.Parse);
                await _cosmosDbService.UpdateHabitLabelAsync(habit, oldLabelIds);
                return RedirectToAction("Details", new {id = id});
            } catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to save changes");
            }

            return View(PopulateAppliedLabelData(habit, habitsLabels.Where(l => l.Type == "label").ToList()));
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
        public async Task<IActionResult> DeleteConfirmedAsync([Bind("Id,Type,User,Name,RelationIds")] HabitLabel habit)
        {
            await _cosmosDbService.DeleteHabitLabelAsync(habit);
            return RedirectToAction("Index");
        }

        [ActionName("Details")]
        public async Task<IActionResult> DetailsAsync(string id)
        {
            IEnumerable<HabitLabel> habitLabels = await _cosmosDbService.GetHabitsLabelsAsync();
            HabitLabel habit = habitLabels.Single(h => h.Id.ToString() == id);
            return View(new HabitViewModel(habit, habitLabels.Where(l => habit.RelationIds.Contains(l.Id))));
        }
    }
}
