using System.ComponentModel.DataAnnotations.Schema;

namespace BlindMatchPAS.Models
{
    public class Match
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }

        public string SupervisorId { get; set; } = string.Empty;

        public bool IsRevealed { get; set; } = false;

        public MatchStatus Status { get; set; } = MatchStatus.Interested;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? RevealedAt { get; set; }

        public string? ModuleLeaderNote { get; set; }

        [ForeignKey(nameof(ProjectId))]
        public Project? Project { get; set; }

        [ForeignKey(nameof(SupervisorId))]
        public ApplicationUser? Supervisor { get; set; }
    }

    public enum MatchStatus
    {
        Interested = 0,
        Confirmed = 1,
        Reassigned = 2
    }
}