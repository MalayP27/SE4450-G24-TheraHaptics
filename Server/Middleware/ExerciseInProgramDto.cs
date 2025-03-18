namespace Server.Middleware;

public class ExerciseInProgramDto {
    public string ExerciseId { get; set; } = null!;  // Reference to the preset exercise
    public string Name { get; set; } = null!;        // Exercise name
    public string Instructions { get; set; } = null!; // Instructions
    public int TargetReps { get; set; }              // Custom target reps for this patient
    public int TargetDuration { get; set; }          // Custom target duration for this patient
    public string Intensity { get; set; } = null!;   // Intensity level
}