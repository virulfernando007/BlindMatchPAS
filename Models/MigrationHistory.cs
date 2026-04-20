namespace BlindMatchPAS.Models;

public class MigrationHistory
{
    public string MigrationId { get; set; } = string.Empty;
    public string ProductVersion { get; set; } = string.Empty;
    public DateTime AppliedAt { get; set; }
}
