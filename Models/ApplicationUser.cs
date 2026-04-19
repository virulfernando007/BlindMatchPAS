using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BlindMatchPAS.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [StringLength(20)]
        public string? StudentId { get; set; }

        [StringLength(100)]
        public string? Department { get; set; }

        [StringLength(50)]
        public string? Batch { get; set; }

        [StringLength(100)]
        public string? Faculty { get; set; }

        /*[StringLength(200)]
        public string? DegreeName { get; set; }*/
        [StringLength(200)]
        [Display(Name = "Degree Name")]
        public string? DegreeName { get; set; }

        [StringLength(100)]
        [Display(Name = "University")]
        public string? University { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        public ICollection<Project> SubmittedProjects { get; set; } = new List<Project>();
        public ICollection<SupervisorExpertise> SupervisorExpertises { get; set; } = new List<SupervisorExpertise>();
        public ICollection<Match> SupervisedMatches { get; set; } = new List<Match>();
    }
}