namespace Server.Middleware;

public class SessionCreateDto {
    public DateTime Date { get; set; }
    public TimeSpan Duration { get; set; }
    public string Feedback { get; set; }
    public int RepsCompleted { get; set; }
}