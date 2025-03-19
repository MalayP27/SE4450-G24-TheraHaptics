using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Server.Models;

// Defines an exercise inside an Exercise Program, allowing custom values
public class ExerciseInProgram {
    public string ExerciseId { get; set; }  // Reference to the preset exercise
    public string Name { get; set; }        // Exercise name
    public string Instructions { get; set; } // Instructions
    public int TargetReps { get; set; }     // Custom target reps for this patient
    public int TargetDuration { get; set; } // Custom target duration for this patient
    public string Intensity { get; set; }   // Intensity level

    public ExerciseInProgram(string exerciseId, string name, string instructions, int targetReps, int targetDuration, string intensity) {
        this.ExerciseId = exerciseId;
        this.Name = name;
        this.Instructions = instructions;
        this.TargetReps = targetReps;
        this.TargetDuration = targetDuration;
        this.Intensity = intensity;
    }
}

public class ExerciseProgram {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string ProgramID { get; set; }

    [BsonElement("name")]
    public string Name { get; set; }

    [BsonElement("patient_id")]
    public string PatientId { get; set; } // Reference to Patient.patientID

    [BsonElement("exercises")]
    public List<ExerciseInProgram> Exercises { get; set; } = new List<ExerciseInProgram>();

    [BsonElement("start_date")]
    public DateTime StartDate { get; set; }

    [BsonElement("end_date")]
    public DateTime EndDate { get; set; }

    [BsonElement("plan_goals")]
    public string PlanGoals { get; set; }

    [BsonElement("intensity")]
    public string Intensity { get; set; }

    [BsonElement("estimated_time")]
    public int EstimatedTime { get; set; } // Estimated time in minutes

    [JsonConstructor]
    public ExerciseProgram(string programID, string name, string patientId, List<ExerciseInProgram> exercises, DateTime startDate, DateTime endDate, string planGoals, string intensity, int estimatedTime) {
        this.ProgramID = programID ?? throw new ArgumentNullException(nameof(programID));
        this.Name = name ?? throw new ArgumentNullException(nameof(name));
        this.PatientId = patientId ?? throw new ArgumentNullException(nameof(patientId));
        this.Exercises = exercises ?? throw new ArgumentNullException(nameof(exercises));
        this.StartDate = startDate;
        this.EndDate = endDate;
        this.PlanGoals = planGoals ?? throw new ArgumentNullException(nameof(planGoals));
        this.Intensity = intensity ?? throw new ArgumentNullException(nameof(intensity));
        this.EstimatedTime = estimatedTime;
    }
}
