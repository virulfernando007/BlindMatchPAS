using Microsoft.AspNetCore.Mvc;

namespace BlindMatchPAS.Controllers
{
    public class ModuleLeaderController : Controller
    {
        public IActionResult Dashboard()
        {
            ViewBag.TotalProjects = 10;
            ViewBag.TotalMatches = 5;
            ViewBag.PendingProjects = 3;
            ViewBag.TotalUsers = 20;

            return View();
        }
    }
}