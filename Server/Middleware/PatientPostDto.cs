namespace Server.Middleware;

public class PatientPostDto {
    public string firstName { get; set; } = null!;
    public string lastName { get; set; } = null!;
    public string emailAddress { get; set; } = null!;
    public string diagnosis { get; set; } = null!;
    public string therapistId { get; set; } = null!;
}