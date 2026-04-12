using System.ComponentModel.DataAnnotations;
using BlindMatchPAS.Models;

namespace BlindMatchPAS.Models.ViewModels
{
    // ─── Authentication ───────────────────────────────────────────────────────

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;

        [Display(Name = "Student ID")]
        [RegularExpression(@"^[A-Z]{2}\d{6}$",
            ErrorMessage = "Student ID must be 2 uppercase letters followed by 6 digits. e.g. CS202301")]
        public string? StudentId { get; set; }

        [StringLength(100)]
        public string? Department { get; set; }
    }

    // ─── Project ──────────────────────────────────────────────────────────────

    public class ProjectSubmissionViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Project title is required.")]
        [StringLength(200, MinimumLength = 10, ErrorMessage = "Title must be between 10 and 200 characters.")]
        [Display(Name = "Project Title")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Short description is required.")]
        [StringLength(150, ErrorMessage = "Short description must not exceed 150 characters.")]
        [Display(Name = "Short Description")]
        public string ShortDescription { get; set; } = string.Empty;

        [Required(ErrorMessage = "Abstract is required.")]
        [StringLength(2000, MinimumLength = 50, ErrorMessage = "Abstract must be between 50 and 2000 characters.")]
        [Display(Name = "Abstract")]
        public string Abstract { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select at least one research area.")]
        [Display(Name = "Research Areas")]
        public List<int> SelectedResearchAreaIds { get; set; } = new();

        [Display(Name = "Project Type")]
        public ProjectType ProjectType { get; set; } = ProjectType.Individual;

        public IFormFile? AttachmentFile { get; set; }

        public IEnumerable<ResearchArea> ResearchAreas { get; set; } = new List<ResearchArea>();
    }

    // ─── Blind Project (Supervisor sees — NO student info) ────────────────────

    public class BlindProjectViewModel
    {
        public int ProjectId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Abstract { get; set; } = string.Empty;
        public string TechnicalStack { get; set; } = string.Empty;
        public string ResearchAreaName { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; }
        public ProjectStatus Status { get; set; }
        public int CompatibilityScore { get; set; }
        public bool HasExpressedInterest { get; set; }
    }

    // ─── Revealed Match ───────────────────────────────────────────────────────

    public class RevealedMatchViewModel
    {
        public int MatchId { get; set; }
        public int ProjectId { get; set; }
        public string ProjectTitle { get; set; } = string.Empty;
        public string ProjectAbstract { get; set; } = string.Empty;
        public string TechnicalStack { get; set; } = string.Empty;
        public string ResearchAreaName { get; set; } = string.Empty;

        // Revealed student details
        public string StudentFullName { get; set; } = string.Empty;
        public string StudentEmail { get; set; } = string.Empty;
        public string? StudentId { get; set; }

        // Revealed supervisor details
        public string SupervisorFullName { get; set; } = string.Empty;
        public string SupervisorEmail { get; set; } = string.Empty;
        public string? SupervisorDepartment { get; set; }

        public DateTime RevealedAt { get; set; }
    }

    // ─── Student Dashboard ────────────────────────────────────────────────────

    public class StudentDashboardViewModel
    {
        public string StudentName { get; set; } = string.Empty;
        public int TotalProjects { get; set; }
        public int PendingCount { get; set; }
        public int UnderReviewCount { get; set; }
        public int MatchedCount { get; set; }
        public List<ProjectStatusRow> Projects { get; set; } = new();
    }

    public class ProjectStatusRow
    {
        public int ProjectId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ResearchArea { get; set; } = string.Empty;
        public ProjectStatus Status { get; set; }
        public DateTime SubmittedAt { get; set; }
        public bool IsRevealed { get; set; }
        public RevealedMatchViewModel? RevealedMatch { get; set; }
    }

    // ─── Supervisor Dashboard ─────────────────────────────────────────────────

    public class SupervisorDashboardViewModel
    {
        public string SupervisorName { get; set; } = string.Empty;
        public int AvailableProjectsCount { get; set; }
        public int InterestedCount { get; set; }
        public int ConfirmedCount { get; set; }
        public int CapacityUsed { get; set; }
        public int CapacityMax { get; set; } = 3;
        public List<string> ExpertiseAreas { get; set; } = new();
        public List<BlindProjectViewModel> AvailableProjects { get; set; } = new();
        public List<RevealedMatchViewModel> ConfirmedMatches { get; set; } = new();
    }

    // ─── Module Leader Dashboard ──────────────────────────────────────────────

    public class ModuleLeaderDashboardViewModel
    {
        public int TotalProjects { get; set; }
        public int TotalMatches { get; set; }
        public int PendingProjects { get; set; }
        public int TotalUsers { get; set; }
        public List<MatchOverviewRow> AllMatches { get; set; } = new();
        public List<Project> UnmatchedProjects { get; set; } = new();
    }

    public class MatchOverviewRow
    {
        public int MatchId { get; set; }
        public string ProjectTitle { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string SupervisorName { get; set; } = string.Empty;
        public string ResearchArea { get; set; } = string.Empty;
        public MatchStatus MatchStatus { get; set; }
        public bool IsRevealed { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // ─── Admin ────────────────────────────────────────────────────────────────

    public class AdminUserRow
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}