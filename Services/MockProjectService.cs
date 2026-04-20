using BlindMatchPAS.Models;
using System.Collections.Generic;
using System.Linq;

namespace BlindMatchPAS.Services
{
    public class MockProjectService : IBlindReviewService
    {
        public IEnumerable<ProjectProposal> GetBlindReviewProjects()
        {
            var mockProjects = new List<ProjectProposal>
            {
                new ProjectProposal 
                { 
                    Id = 1, 
                    Title = "AI Traffic Analyzer", 
                    Abstract = "Using computer vision to optimize campus traffic flow.", 
                    TechStack = "Python, TensorFlow", 
                    CompatibilityScore = 95,
                    SubmittedDate = DateTime.Now.AddDays(-5),
                    ResearchAreas = new List<ResearchArea> { new ResearchArea { Name = "Artificial Intelligence" }, new ResearchArea { Name = "Computer Vision" } }
                },
                new ProjectProposal 
                { 
                    Id = 2, 
                    Title = "Campus Shuttle Tracker", 
                    Abstract = "Real-time GPS tracking for university bus routes.", 
                    TechStack = "Flutter, Firebase", 
                    CompatibilityScore = 65,
                    SubmittedDate = DateTime.Now.AddDays(-3),
                    ResearchAreas = new List<ResearchArea> { new ResearchArea { Name = "Mobile Development" }, new ResearchArea { Name = "GPS Technology" } }
                },
                new ProjectProposal 
                { 
                    Id = 3, 
                    Title = "Secure Vote Blockchain", 
                    Abstract = "A decentralized voting system for student council elections.", 
                    TechStack = "Solidity, React", 
                    CompatibilityScore = 30,
                    SubmittedDate = DateTime.Now.AddDays(-1),
                    ResearchAreas = new List<ResearchArea> { new ResearchArea { Name = "Blockchain" }, new ResearchArea { Name = "Web Development" } }
                }
            };

            return mockProjects.OrderByDescending(p => p.CompatibilityScore).ToList();
        }
    }
}
