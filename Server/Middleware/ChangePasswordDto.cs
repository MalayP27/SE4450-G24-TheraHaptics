namespace Server.Middleware;

public class ChangePasswordDto {
    public string emailAddress { get; set; } = null!;
    public string tempPassword { get; set; } = null!;
    public string newPassword { get; set; } = null!;
}