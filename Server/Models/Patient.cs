using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Server.Models;

public class Patient {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? patientId { get; set; }

    [BsonElement("first_name")]
    public string firstName { get; set; }

    [BsonElement("last_name")]
    public string lastName { get; set; }

    [BsonElement("email_address")]
    public string emailAddress { get; set; }

    [BsonElement("phone_number")]
    public string phoneNumber { get; set; } = null!;

    [BsonElement("address")]
    public string address { get; set; } = null!;

    [BsonElement("date_of_birth")]
    public DateTime? dateOfBirth { get; set; }

    [BsonElement("gender")]
    public string gender { get; set; } = null!;

    [BsonElement("diagnosis")]
    public string diagnosis { get; set; }

    [BsonElement("date_joined")]
    public DateTime? dateJoined { get; set; }

    [BsonElement("patient_goal_id")]
    public string patientGoalId { get; set; } = null!;

    //[JsonPropertyName("items")]
    //public List<string> assignedPatients { get; set; } = null!;

    [JsonConstructor]
    public Patient(string patientId, string firstName, string lastName, string emailAddress, string diagnosis) {
        this.patientId = patientId ?? throw new ArgumentNullException(nameof(patientId));
        this.firstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        this.lastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        this.emailAddress = emailAddress ?? throw new ArgumentNullException(nameof(emailAddress));
        this.diagnosis = diagnosis ?? throw new ArgumentNullException(nameof(diagnosis));
    }
}