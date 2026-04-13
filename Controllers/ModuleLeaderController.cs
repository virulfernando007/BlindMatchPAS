using Microsoft.AspNetCore.Mvc;

namespace BlindMatchPAS.Controllers
{
    using System.Collections.Generic;
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



        public IActionResult Users()
        {
            var users = new List<object>
            {
                new { Name = "Alice", Role = "Student" },
                new { Name = "Bob", Role = "Supervisor" },
                new { Name = "Charlie", Role = "Student" }
            };
            return View(users);
        }
    }

}