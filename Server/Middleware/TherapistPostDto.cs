namespace Server.Middleware;
// DTO stands for Data Transfer Object
// For use in specifying the strictly necessary fields when using the POST method for Therapists in UserController.cs
public class TherapistPostDto {
    public string firstName { get; set; } = null!;
    public string lastName { get; set; } = null!;
    public string emailAddress { get; set; } = null!;
    public string password { get; set; } = null!;
    public string productKeyId { get; set; } = null!;
}