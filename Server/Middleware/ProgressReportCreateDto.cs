namespace Server.Middleware;

public class ProgressReportCreateDto {
    public DateTime DateReported { get; set; }
    public string Description { get; set; }
    public string TimeProgression { get; set; }
    public string RepProgression { get; set; }
    public string DifficultyImprovement { get; set; }
}