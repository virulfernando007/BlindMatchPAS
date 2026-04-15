using BlindMatchPAS.Models;
using System.Collections.Generic;

namespace BlindMatchPAS.Services
{
    public interface IBlindReviewService
    {
        IEnumerable<ProjectProposal> GetBlindReviewProjects();
    }
}