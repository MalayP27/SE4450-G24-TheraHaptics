using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Server.Models;

public class Session {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? SessionID { get; set; }

    [BsonElement("patient_id")]
    public string PatientID { get; set; } // Reference to Patient.patientID

    [BsonElement("date")]
    public DateTime Date { get; set; }

    [BsonElement("duration")]
    public TimeSpan Duration { get; set; }

    [BsonElement("feedback")]
    public string Feedback { get; set; }

    [BsonElement("reps_completed")]
    public int RepsCompleted { get; set; }

    // Default constructor
    public Session() { }

    // Parameterized constructor
    [JsonConstructor]
    public Session(string sessionID, string patientID, DateTime date, TimeSpan duration, string feedback, int repsCompleted) {
        this.SessionID = sessionID ?? throw new ArgumentNullException(nameof(sessionID));
        this.PatientID = patientID ?? throw new ArgumentNullException(nameof(patientID));
        this.Date = date;
        this.Duration = duration;
        this.Feedback = feedback ?? throw new ArgumentNullException(nameof(feedback));
        this.RepsCompleted = repsCompleted;
    }
}