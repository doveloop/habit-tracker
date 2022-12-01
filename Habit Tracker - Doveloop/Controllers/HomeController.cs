using Habit_Tracker___Doveloop.Data;
using Habit_Tracker___Doveloop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Habit_Tracker___Doveloop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICosmosDbService _cosmosDbService;
        public HomeController(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        public IActionResult Index()
        {
            _cosmosDbService.SetUser(HttpContext.User.Identity.Name);
            ViewBag.UserName = HttpContext.User.Identity.Name;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}