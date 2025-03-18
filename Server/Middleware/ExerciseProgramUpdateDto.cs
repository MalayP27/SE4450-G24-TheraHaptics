namespace Server.Middleware;

public class ExerciseProgramUpdateDto {
    public string Name { get; set; } = null!;
    public string PatientId { get; set; } = null!; // Reference to Patient.patientID
    public List<ExerciseInProgramDto> Exercises { get; set; } = new List<ExerciseInProgramDto>();
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string PlanGoals { get; set; } = null!;
    public string Intensity { get; set; } = null!;
    public int EstimatedTime { get; set; } // Estimated time in minutes
}