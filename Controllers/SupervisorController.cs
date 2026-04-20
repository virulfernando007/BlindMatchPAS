using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BlindMatchPAS.Services;

namespace BlindMatchPAS.Controllers
{
    [Authorize(Roles = "Supervisor")]
    public class SupervisorController : Controller
    {
        private readonly IBlindReviewService _blindReviewService;

        public SupervisorController(IBlindReviewService blindReviewService)
        {
            _blindReviewService = blindReviewService;
        }

        public IActionResult Dashboard()
        {
            // Placeholder for dashboard data
            ViewBag.TotalProjects = 15;
            ViewBag.PendingReviews = 5;
            ViewBag.MatchedProjects = 8;
            ViewBag.TotalStudents = 50;

            return View();
        }

        public IActionResult BlindReview()
        {
            var projects = _blindReviewService.GetBlindReviewProjects();
            return View(projects);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExpressInterest(int projectId)
        {
            // In a real application, this would create a Match record in the database
            // For now, we'll just show a success message
            TempData["Success"] = $"You have successfully expressed interest in project ID {projectId}!";
            return RedirectToAction(nameof(BlindReview));
        }
    }
}