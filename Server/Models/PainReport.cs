using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Server.Models;

public class PainReport {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? PainReportID { get; set; }

    [BsonElement("patient_id")]
    public string PatientID { get; set; } // Reference to Patient.patientID

    [BsonElement("description")]
    public string Description { get; set; }

    [BsonElement("pain_level")]
    public int PainLevel { get; set; }

    [BsonElement("date_reported")]
    public DateTime DateReported { get; set; }

    // Default constructor
    public PainReport() { }

    [JsonConstructor]
    public PainReport(string painReportID, string patientID, string description, int painLevel, DateTime dateReported) {
        this.PainReportID = painReportID ?? throw new ArgumentNullException(nameof(painReportID));
        this.PatientID = patientID ?? throw new ArgumentNullException(nameof(patientID));
        this.Description = description ?? throw new ArgumentNullException(nameof(description));
        this.PainLevel = painLevel;
        this.DateReported = dateReported;
    }
}