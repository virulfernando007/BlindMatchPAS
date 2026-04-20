namespace BlindMatchPAS.Models
{
    public class ProjectProposal
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Abstract { get; set; }
        public string TechStack { get; set; }
        public int CompatibilityScore { get; set; }
    }
}