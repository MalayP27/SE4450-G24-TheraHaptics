namespace Server.Middleware;

public class LoginDto {
    public string emailAddress { get; set; } = null!;
    public string password { get; set; } = null!;
}