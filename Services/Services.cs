using BlindMatchPAS.Data;
using BlindMatchPAS.Models;
using BlindMatchPAS.Models.ViewModels;
using BlindMatchPAS.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BlindMatchPAS.Services
{
    // ─── Matching Service ─────────────────────────────────────────────────────

    public interface IMatchingService
    {
        Task<bool> ExpressInterestAsync(string supervisorId, int projectId);
        Task<RevealedMatchViewModel?> ConfirmMatchAsync(string supervisorId, int matchId);
        Task<bool> CanSupervisorConfirmMoreMatchesAsync(string supervisorId);
    }

    public class MatchingService : IMatchingService
    {
        private readonly IMatchRepository _matchRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly ApplicationDbContext _context;
        private const int MaxConfirmedMatchesPerSupervisor = 3;

        public MatchingService(
            IMatchRepository matchRepository,
            IProjectRepository projectRepository,
            ApplicationDbContext context)
        {
            _matchRepository   = matchRepository;
            _projectRepository = projectRepository;
            _context           = context;
        }

        /// <summary>
        /// Phase 1 — Supervisor expresses interest.
        /// Project moves to UnderReview. Identity still hidden.
        /// </summary>
        public async Task<bool> ExpressInterestAsync(string supervisorId, int projectId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null || project.Status != ProjectStatus.Pending)
                return false;

            var existingMatch = await _context.Matches
                .FirstOrDefaultAsync(m => m.SupervisorId == supervisorId
                                       && m.ProjectId == projectId);
            if (existingMatch != null)
                return false;

            var match = new Match
            {
                ProjectId    = projectId,
                SupervisorId = supervisorId,
                IsRevealed   = false,
                Status       = MatchStatus.Interested,
                CreatedAt    = DateTime.UtcNow
            };

            await _matchRepository.AddAsync(match);

            project.Status    = ProjectStatus.UnderReview;
            project.UpdatedAt = DateTime.UtcNow;
            _context.Projects.Update(project);

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Phase 2 — THE REVEAL.
        /// Supervisor confirms. IsRevealed flips to true.
        /// Both parties can now see each other's full details.
        /// </summary>
        public async Task<RevealedMatchViewModel?> ConfirmMatchAsync(string supervisorId, int matchId)
        {
            if (!await CanSupervisorConfirmMoreMatchesAsync(supervisorId))
                return null;

            var match = await _matchRepository.GetMatchWithDetailsAsync(matchId);
            if (match == null || match.SupervisorId != supervisorId)
                return null;

            if (match.Status == MatchStatus.Confirmed)
                return BuildRevealedViewModel(match);

            // === IDENTITY REVEAL ===
            match.IsRevealed  = true;
            match.Status      = MatchStatus.Confirmed;
            match.RevealedAt  = DateTime.UtcNow;

            match.Project!.Status    = ProjectStatus.Matched;
            match.Project.UpdatedAt  = DateTime.UtcNow;

            await _matchRepository.UpdateAsync(match);
            await _context.SaveChangesAsync();

            return BuildRevealedViewModel(match);
        }

        public async Task<bool> CanSupervisorConfirmMoreMatchesAsync(string supervisorId)
        {
            var count = await _matchRepository
                .GetConfirmedMatchCountForSupervisorAsync(supervisorId);
            return count < MaxConfirmedMatchesPerSupervisor;
        }

        private static RevealedMatchViewModel BuildRevealedViewModel(Match match) => new()
        {
            MatchId             = match.Id,
            ProjectId           = match.ProjectId,
            ProjectTitle        = match.Project!.Title,
            ProjectAbstract     = match.Project.Abstract,
           TechnicalStack      = match.Project.ResearchAreaIds,
            ResearchAreaName    = match.Project.ResearchArea?.Name ?? "",
            StudentFullName     = match.Project.Student!.FullName,
            StudentEmail        = match.Project.Student.Email!,
            StudentId           = match.Project.Student.StudentId,
            SupervisorFullName  = match.Supervisor!.FullName,
            SupervisorEmail     = match.Supervisor.Email!,
            SupervisorDepartment = match.Supervisor.Department,
            RevealedAt          = match.RevealedAt ?? DateTime.UtcNow
        };
    }

    // ─── Project Service ──────────────────────────────────────────────────────

    public interface IProjectService
    {
        Task<int> SubmitProjectAsync(string studentId, ProjectSubmissionViewModel model, string? attachmentPath = null);
        Task<bool> UpdateProjectAsync(string studentId, ProjectSubmissionViewModel model);
        Task<bool> WithdrawProjectAsync(string studentId, int projectId);
        Task<bool> DeleteProjectAsync(string studentId, int projectId);
        Task<StudentDashboardViewModel> GetStudentDashboardAsync(string studentId, string studentName);
        Task<SupervisorDashboardViewModel> GetSupervisorDashboardAsync(string supervisorId, string supervisorName);
        Task<ModuleLeaderDashboardViewModel> GetModuleLeaderDashboardAsync();
        Task<Project?> GetProjectForEditAsync(string studentId, int projectId);
    }

    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IMatchRepository   _matchRepository;
        private readonly ApplicationDbContext _context;

        public ProjectService(
            IProjectRepository projectRepository,
            IMatchRepository matchRepository,
            ApplicationDbContext context)
        {
            _projectRepository = projectRepository;
            _matchRepository   = matchRepository;
            _context           = context;
        }

        public async Task<int> SubmitProjectAsync(string studentId, ProjectSubmissionViewModel model, string? attachmentPath = null)
        {
            var project = new Project
            {
                Title             = model.Title,
                ShortDescription  = model.ShortDescription,
                Abstract          = model.Abstract,
                ResearchAreaIds   = string.Join(",", model.SelectedResearchAreaIds),
                ProjectType       = model.ProjectType,
                AttachmentPath    = attachmentPath,
                StudentId         = studentId,
                Status            = ProjectStatus.Pending,
                SubmittedAt       = DateTime.UtcNow
            };

            await _projectRepository.AddAsync(project);
            await _projectRepository.SaveChangesAsync();
            return project.Id;
        }

        public async Task<bool> UpdateProjectAsync(string studentId, ProjectSubmissionViewModel model)
        {
            var project = await _projectRepository.GetByIdAsync(model.Id);
            if (project == null || project.StudentId != studentId)
                return false;

            if (project.Status == ProjectStatus.Matched)
                return false;

            project.Title            = model.Title;
            project.ShortDescription = model.ShortDescription;
            project.Abstract         = model.Abstract;
            project.ResearchAreaIds  = string.Join(",", model.SelectedResearchAreaIds);
            project.ProjectType      = model.ProjectType;

            await _projectRepository.UpdateAsync(project);
            await _projectRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> WithdrawProjectAsync(string studentId, int projectId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null || project.StudentId != studentId)
                return false;

            if (project.Status == ProjectStatus.Matched)
                return false;

            project.Status = ProjectStatus.Withdrawn;
            await _projectRepository.UpdateAsync(project);
            await _projectRepository.SaveChangesAsync();
            return true;
        }

        public async Task<StudentDashboardViewModel> GetStudentDashboardAsync(
            string studentId, string studentName)
        {
            var projects = (await _projectRepository
                .GetProjectsByStudentAsync(studentId)).ToList();

            var rows = projects.Select(p =>
            {
                RevealedMatchViewModel? revealedMatch = null;

                if (p.Match != null && p.Match.IsRevealed)
                {
                    revealedMatch = new RevealedMatchViewModel
                    {
                        MatchId              = p.Match.Id,
                        ProjectId            = p.Id,
                        SupervisorFullName   = p.Match.Supervisor?.FullName ?? "",
                        SupervisorEmail      = p.Match.Supervisor?.Email ?? "",
                        SupervisorDepartment = p.Match.Supervisor?.Department,
                        RevealedAt           = p.Match.RevealedAt ?? DateTime.UtcNow
                    };
                }

                return new ProjectStatusRow
                {
                    ProjectId    = p.Id,
                    Title        = p.Title,
                    ResearchArea = p.ResearchArea?.Name ?? "",
                    ShortDescription = p.ShortDescription,
                    Status       = p.Status,
                    SubmittedAt  = p.SubmittedAt,
                    IsRevealed   = p.Match?.IsRevealed ?? false,
                    RevealedMatch = revealedMatch
                };
            }).ToList();

            return new StudentDashboardViewModel
            {
                StudentName      = studentName,
                TotalProjects    = projects.Count,
                PendingCount     = projects.Count(p => p.Status == ProjectStatus.Pending),
                UnderReviewCount = projects.Count(p => p.Status == ProjectStatus.UnderReview),
                MatchedCount     = projects.Count(p => p.Status == ProjectStatus.Matched),
                Projects         = rows
            };
        }

        public async Task<SupervisorDashboardViewModel> GetSupervisorDashboardAsync(
            string supervisorId, string supervisorName)
        {
            var expertiseAreas = await _context.SupervisorExpertises
                .Include(se => se.ResearchArea)
                .Where(se => se.SupervisorId == supervisorId)
                .Select(se => se.ResearchArea!.Name)
                .ToListAsync();

            var blindProjects = (await _projectRepository
                .GetBlindProjectsForSupervisorAsync(supervisorId)).ToList();

            var supervisorMatches = (await _matchRepository
                .GetMatchesBySupervisorAsync(supervisorId)).ToList();

            var blindViewModels = blindProjects.Select(p => new BlindProjectViewModel
            {
                ProjectId          = p.Id,
                Title              = p.Title,
                Abstract           = p.Abstract,
                TechnicalStack     = p.ResearchAreaIds,
                ResearchAreaName   = p.ResearchArea?.Name ?? "",
                SubmittedAt        = p.SubmittedAt,
                Status             = p.Status,
                CompatibilityScore = expertiseAreas.Contains(p.ResearchArea?.Name ?? "") ? 100 : 0
            }).ToList();

            var confirmedMatches = supervisorMatches
                .Where(m => m.IsRevealed)
                .Select(m => new RevealedMatchViewModel
                {
                    MatchId            = m.Id,
                    ProjectId          = m.ProjectId,
                    ProjectTitle       = m.Project?.Title ?? "",
                    ProjectAbstract    = m.Project?.Abstract ?? "",
                    ResearchAreaName   = m.Project?.ResearchArea?.Name ?? "",
                    StudentFullName    = m.Project?.Student?.FullName ?? "",
                    StudentEmail       = m.Project?.Student?.Email ?? "",
                    StudentId          = m.Project?.Student?.StudentId,
                    SupervisorFullName = supervisorName,
                    SupervisorEmail    = "",
                    RevealedAt         = m.RevealedAt ?? DateTime.UtcNow
                }).ToList();

            return new SupervisorDashboardViewModel
            {
                SupervisorName         = supervisorName,
                AvailableProjectsCount = blindProjects.Count,
                InterestedCount        = supervisorMatches.Count(m => m.Status == MatchStatus.Interested),
                ConfirmedCount         = supervisorMatches.Count(m => m.Status == MatchStatus.Confirmed),
                CapacityUsed           = supervisorMatches.Count(m => m.Status == MatchStatus.Confirmed),
                ExpertiseAreas         = expertiseAreas,
                AvailableProjects      = blindViewModels,
                ConfirmedMatches       = confirmedMatches
            };
        }

        public async Task<ModuleLeaderDashboardViewModel> GetModuleLeaderDashboardAsync()
        {
            var allMatches  = (await _matchRepository.GetAllMatchesWithDetailsAsync()).ToList();
            var allProjects = (await _projectRepository.GetAllAsync()).ToList();
            var totalUsers  = await _context.Users.CountAsync();

            var matchRows = allMatches.Select(m => new MatchOverviewRow
            {
                MatchId        = m.Id,
                ProjectTitle   = m.Project?.Title ?? "",
                StudentName    = m.Project?.Student?.FullName ?? "Anonymous",
                SupervisorName = m.Supervisor?.FullName ?? "",
                ResearchArea   = m.Project?.ResearchArea?.Name ?? "",
                MatchStatus    = m.Status,
                IsRevealed     = m.IsRevealed,
                CreatedAt      = m.CreatedAt
            }).ToList();

            return new ModuleLeaderDashboardViewModel
            {
                TotalProjects    = allProjects.Count,
                TotalMatches     = allMatches.Count,
                PendingProjects  = allProjects.Count(p => p.Status == ProjectStatus.Pending),
                TotalUsers       = totalUsers,
                AllMatches       = matchRows,
                UnmatchedProjects = allProjects
                    .Where(p => p.Status == ProjectStatus.Pending).ToList()
            };
        }

        public async Task<bool> DeleteProjectAsync(string studentId, int projectId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null || project.StudentId != studentId)
                return false;

            if (project.Status == ProjectStatus.Matched)
                return false;

            await _projectRepository.DeleteAsync(project);
            await _projectRepository.SaveChangesAsync();
            return true;
        }

        public async Task<Project?> GetProjectForEditAsync(string studentId, int projectId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null || project.StudentId != studentId)
                return null;
            return project;
        }
    }
}