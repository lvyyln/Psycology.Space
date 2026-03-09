namespace Psycology.Space.Data;

public class IntakeResponse
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public int MoodScore { get; set; }
    public int SleepScore { get; set; }
    public int AnxietyScore { get; set; }
    public string? MainConcern { get; set; }
    public string? AdditionalNotes { get; set; }

    public ApplicationUser User { get; set; } = null!;
}
