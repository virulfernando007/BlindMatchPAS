namespace BlindMatchPAS.Models
{
    public class ProjectProposal
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Abstract { get; set; } = string.Empty;
        public string TechStack { get; set; } = string.Empty;
        public int CompatibilityScore { get; set; }
        public DateTime SubmittedDate { get; set; }
        public ICollection<ResearchArea> ResearchAreas { get; set; } = new List<ResearchArea>();
    }
}