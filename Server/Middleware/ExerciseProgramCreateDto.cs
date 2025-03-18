namespace Server.Middleware;

public class ExerciseProgramCreateDto {
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string AssignedTo { get; set; } = null!; // Reference to Patient.patientID
    public List<string> Exercises { get; set; } = new List<string>();
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string PlanGoals { get; set; } = null!;
    public string Intensity { get; set; } = null!;
    public int EstimatedTime { get; set; } // Estimated time in minutes
}