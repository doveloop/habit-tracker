using Microsoft.AspNetCore.Mvc;

namespace Habit_Tracker___Doveloop.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            if(HttpContext.User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");
            return View();
        }
    }
}
