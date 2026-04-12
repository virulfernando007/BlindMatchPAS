using System.ComponentModel.DataAnnotations.Schema;

namespace BlindMatchPAS.Models
{
    public class SupervisorExpertise
    {
        public int Id { get; set; }

        public string SupervisorId { get; set; } = string.Empty;

        public int ResearchAreaId { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(SupervisorId))]
        public ApplicationUser? Supervisor { get; set; }

        [ForeignKey(nameof(ResearchAreaId))]
        public ResearchArea? ResearchArea { get; set; }
    }
}