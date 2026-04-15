using Microsoft.AspNetCore.Mvc;
using BlindMatchPAS.Services;

namespace BlindMatchPAS.Controllers
{
    public class SupervisorController : Controller
    {
        private readonly IBlindReviewService _blindReviewService;

        public SupervisorController(IBlindReviewService blindReviewService)
        {
            _blindReviewService = blindReviewService;
        }

        public IActionResult BlindReview()
        {
            var projects = _blindReviewService.GetBlindReviewProjects();
            return View(projects);
        }
    }
}