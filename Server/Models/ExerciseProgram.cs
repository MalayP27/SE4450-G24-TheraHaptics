using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Server.Models;

public class ExerciseProgram {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string ProgramID { get; set; }

    [BsonElement("name")]
    public string Name { get; set; }

    [BsonElement("assigned_to")]
    public string AssignedTo { get; set; } // Reference to Patient.patientID

    [BsonElement("exercises")]
    public List<string> Exercises { get; set; } = new List<string>();

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
    public ExerciseProgram(string programID, string name, string assignedTo, List<string> exercises, DateTime startDate, DateTime endDate, string planGoals, string intensity, int estimatedTime) {
        this.ProgramID = programID ?? throw new ArgumentNullException(nameof(programID));
        this.Name = name ?? throw new ArgumentNullException(nameof(name));
        this.AssignedTo = assignedTo ?? throw new ArgumentNullException(nameof(assignedTo));
        this.Exercises = exercises ?? throw new ArgumentNullException(nameof(exercises));
        this.StartDate = startDate;
        this.EndDate = endDate;
        this.PlanGoals = planGoals ?? throw new ArgumentNullException(nameof(planGoals));
        this.Intensity = intensity ?? throw new ArgumentNullException(nameof(intensity));
        this.EstimatedTime = estimatedTime;
    }
}