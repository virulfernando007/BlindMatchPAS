using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace BlindMatchPAS.Controllers
{
    // Notice there is NO [Authorize] tag here, so it won't ask you to log in!
    public class SupervisorController : Controller
    {
        public IActionResult BlindReview()
        {
            // We are manually passing fake UI data so the database is completely ignored.
            var mockProjects = new List<dynamic>
            {
                new { 
                    Title = "AI Traffic Analyzer", 
                    Abstract = "Using computer vision to optimize campus traffic flow.", 
                    TechStack = "Python, TensorFlow", 
                    CompatibilityScore = 95 
                },
                new { 
                    Title = "Campus Shuttle Tracker", 
                    Abstract = "Real-time GPS tracking for university bus routes.", 
                    TechStack = "Flutter, Firebase", 
                    CompatibilityScore = 65 
                },
                new { 
                    Title = "Secure Vote Blockchain", 
                    Abstract = "A decentralized voting system for student council elections.", 
                    TechStack = "Solidity, React", 
                    CompatibilityScore = 30 
                }
            };

            return View(mockProjects);
        }
    }
}