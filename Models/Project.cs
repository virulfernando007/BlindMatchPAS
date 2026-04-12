using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlindMatchPAS.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Project title is required.")]
        [StringLength(200, MinimumLength = 10, ErrorMessage = "Title must be between 10 and 200 characters.")]
        [Display(Name = "Project Title")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Abstract is required.")]
        [StringLength(2000, MinimumLength = 50, ErrorMessage = "Abstract must be between 50 and 2000 characters.")]
        public string Abstract { get; set; } = string.Empty;


        [StringLength(500)]
        [Display(Name = "Research Areas")]
        public string ResearchAreaIds { get; set; } = string.Empty;

        [StringLength(150)]
        [Display(Name = "Short Description")]
        public string ShortDescription { get; set; } = string.Empty;

        [Display(Name = "Project Type")]
        public ProjectType ProjectType { get; set; } = ProjectType.Individual;

        [StringLength(500)]
        [Display(Name = "Uploaded File Path")]
        public string? AttachmentPath { get; set; }

        [Required]
        public string StudentId { get; set; } = string.Empty;

        public ProjectStatus Status { get; set; } = ProjectStatus.Pending;

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public ResearchArea? ResearchArea { get; set; }

        [ForeignKey(nameof(StudentId))]
        public ApplicationUser? Student { get; set; }

        public Match? Match { get; set; }
    }

    public enum ProjectStatus
    {
        Pending = 0,
        UnderReview = 1,
        Matched = 2,
        Withdrawn = 3
    }

    public enum ProjectType
    {
        Individual = 0,
        Group = 1
    }
}