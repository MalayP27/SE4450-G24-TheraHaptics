using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Server.Models;

public class Goal {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? GoalID { get; set; }

    [BsonElement("patient_id")]
    public string PatientID { get; set; } // Reference to Patient.patientID

    [BsonElement("description")]
    public string Description { get; set; }

    // Default constructor
    public Goal() { }

    // Parameterized constructor
    [JsonConstructor]
    public Goal(string goalID, string patientID, string description) {
        this.GoalID = goalID ?? throw new ArgumentNullException(nameof(goalID));
        this.PatientID = patientID ?? throw new ArgumentNullException(nameof(patientID));
        this.Description = description ?? throw new ArgumentNullException(nameof(description));
    }
}