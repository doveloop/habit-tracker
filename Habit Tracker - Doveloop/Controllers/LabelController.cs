using System;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Habit_Tracker___Doveloop.Models;
using Habit_Tracker___Doveloop.Models.ViewModels;
using Habit_Tracker___Doveloop.Data;

namespace Habit_Tracker___Doveloop.Controllers
{
    public class LabelController : Controller
    {
        private readonly ICosmosDbService _cosmosDbService;
        public LabelController(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        private LabelViewModel CreateLabelViewModel(HabitLabel label, IEnumerable<HabitLabel> habitsLabels)
        {
            List<HabitLabel> habits = new List<HabitLabel>();
            if (label.RelationIds != null) label.RelationIds.ForEach(id => habits.Add(habitsLabels.Single(h => h.Id == id)));
            return new LabelViewModel(label, habits);
        }

        private IEnumerable<LabelViewModel> CreateLabelViewModels(IEnumerable<HabitLabel> habitsLabels)
        {
            List<LabelViewModel> viewModels = new List<LabelViewModel>();
            habitsLabels.Where(l => l.Type == "label").ToList().ForEach(label => viewModels.Add(CreateLabelViewModel(label, habitsLabels)));
            return viewModels;
        }

        public async Task<IActionResult> Index()
        {
            _cosmosDbService.SetUser(HttpContext.User.Identity.Name);
            return View(CreateLabelViewModels(await _cosmosDbService.GetHabitsLabelsAsync()));
        }

        public IActionResult Create()
        {
            HabitLabel label = new HabitLabel();
            label.User = HttpContext.User.Identity.Name;
            label.Type = "label";
            return View(label);
        }

        [HttpPost]
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync([Bind("Id,Type,User,Name")] HabitLabel label)
        {
            label.RelationIds = new List<Guid>();
            if (!string.IsNullOrEmpty(label.Name) && label.Type == "label" && label.User != null && label.User == HttpContext.User.Identity.Name)
            {
                await _cosmosDbService.AddHabitLabelAsync(label);
                return RedirectToAction("Index");
            }
            return View(label);
        }

        [ActionName("Edit")]
        public async Task<IActionResult> EditAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            IEnumerable<HabitLabel> habitsLabels = await _cosmosDbService.GetHabitsLabelsAsync();
            HabitLabel label = habitsLabels.Single(l => l.Id.ToString() == id);
            if (label == null || label.Type != "label" || label.User != HttpContext.User.Identity.Name)
            {
                return NotFound();
            }

            return View(PopulateAppliedHabitData(label, habitsLabels.Where(l => l.Type == "habit").ToList()));
        }

        private LabelViewModel PopulateAppliedHabitData(HabitLabel label, List<HabitLabel> allHabits)
        {
            LabelViewModel viewModel = CreateLabelViewModel(label, allHabits);
            List<AppliedData> habitViewModel = new List<AppliedData>();
            allHabits.ForEach(habit =>
                habitViewModel.Add(
                    new AppliedData
                    {
                        Id = habit.Id,
                        Name = habit.Name
                    }
                )
            );

            ViewBag.HabitViewModel = habitViewModel;
            return viewModel;
        }

        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync(string id, string labelName, string[] selectedHabits)
        {
            if (id == null)
            {
                return NotFound();
            }
            IEnumerable<HabitLabel> habitsLabels = await _cosmosDbService.GetHabitsLabelsAsync();
            HabitLabel label = habitsLabels.Single(l => l.Id.ToString() == id);

            if (label == null || label.Type != "label" || label.User != HttpContext.User.Identity.Name)
            {
                return NotFound();
            }

            try
            {
                label.Name = labelName;
                List<Guid> oldHabitIds = label.RelationIds;
                label.RelationIds = selectedHabits.ToList().ConvertAll(Guid.Parse);
                await _cosmosDbService.UpdateHabitLabelAsync(label, oldHabitIds);
                //update all the habits to add/remove the label
                return RedirectToAction("Details", new { id = id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to save changes");
            }

            return View(PopulateAppliedHabitData(label, habitsLabels.Where(l => l.Type == "label").ToList()));
        }

        [ActionName("Delete")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            HabitLabel label = await _cosmosDbService.GetHabitLabelAsync(id);
            if (label == null)
            {
                return NotFound();
            }

            return View(label);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedAsync([Bind("Id,Type,User,Name,RelationIds")] HabitLabel label)
        {
            await _cosmosDbService.DeleteHabitLabelAsync(label);
            return RedirectToAction("Index");
        }

        [ActionName("Details")]
        public async Task<IActionResult> DetailsAsync(string id)
        {
            IEnumerable<HabitLabel> habitLabels = await _cosmosDbService.GetHabitsLabelsAsync();
            HabitLabel label = habitLabels.Single(l => l.Id.ToString() == id);
            return View(new LabelViewModel(label, habitLabels.Where(h => label.RelationIds.Contains(h.Id))));
        }
    }
}
