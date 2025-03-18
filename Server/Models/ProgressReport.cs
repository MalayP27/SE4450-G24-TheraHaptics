using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Server.Models;

public class ProgressReport {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? ProgressReportID { get; set; }

    [BsonElement("patient_id")]
    public string PatientID { get; set; } // Reference to Patient.patientID

    [BsonElement("date_reported")]
    public DateTime DateReported { get; set; }

    [BsonElement("description")]
    public string Description { get; set; }

    [BsonElement("time_progression")]
    public string TimeProgression { get; set; }

    [BsonElement("rep_progression")]
    public string RepProgression { get; set; }

    [BsonElement("difficulty_improvement")]
    public string DifficultyImprovement { get; set; }

    // Default constructor
    public ProgressReport() { }

    // Parameterized constructor
    [JsonConstructor]
    public ProgressReport(string progressReportID, string patientID, DateTime dateReported, string description, string timeProgression, string repProgression, string difficultyImprovement) {
        this.ProgressReportID = progressReportID ?? throw new ArgumentNullException(nameof(progressReportID));
        this.PatientID = patientID ?? throw new ArgumentNullException(nameof(patientID));
        this.DateReported = dateReported;
        this.Description = description ?? throw new ArgumentNullException(nameof(description));
        this.TimeProgression = timeProgression ?? throw new ArgumentNullException(nameof(timeProgression));
        this.RepProgression = repProgression ?? throw new ArgumentNullException(nameof(repProgression));
        this.DifficultyImprovement = difficultyImprovement ?? throw new ArgumentNullException(nameof(difficultyImprovement));
    }
}