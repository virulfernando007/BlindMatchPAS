using Microsoft.AspNetCore.Mvc;
using BlindMatchPAS.Data;
using Microsoft.EntityFrameworkCore;

namespace BlindMatchPAS.Controllers
{
    public class ModuleLeaderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ModuleLeaderController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var totalProjects = await _context.Projects.CountAsync();
            var totalMatches = await _context.Matches.CountAsync();
            var pendingProjects = await _context.Projects
                .Where(p => p.Status == "Pending")
                .CountAsync();
            var totalUsers = await _context.Users.CountAsync();

            ViewBag.TotalProjects = totalProjects;
            ViewBag.TotalMatches = totalMatches;
            ViewBag.PendingProjects = pendingProjects;
            ViewBag.TotalUsers = totalUsers;

            return View();
        }
    }
}
