using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Server.Models;

public class Therapist {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? therapistId { get; set; }

    [BsonElement("first_name")]
    public string firstName { get; set; }

    [BsonElement("last_name")]
    public string lastName { get; set; }

    [BsonElement("email_address")]
    public string emailAddress { get; set; }

    [BsonElement("product_key_id")]
    public string productKeyId { get; set; }

    [BsonElement("assigned_patients")]
    public List<string> assignedPatients { get; set; } = new List<string>();
    
    [JsonConstructor]
    public Therapist(string therapistId, string firstName, string lastName, string emailAddress, string productKeyId, List<string> assignedPatients) {
        this.therapistId = therapistId ?? throw new ArgumentNullException(nameof(therapistId));
        this.firstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        this.lastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        this.emailAddress = emailAddress ?? throw new ArgumentNullException(nameof(emailAddress));
        this.productKeyId = productKeyId ?? throw new ArgumentNullException(nameof(productKeyId));
        this.assignedPatients = assignedPatients ?? new List<string>();
    }
    //[JsonPropertyName("items")]
    //public List<string> assignedPatients { get; set; } = null!;

    // [JsonConstructor]
    // public Therapist(string therapistId, string firstName, string lastName, string emailAddress, string productKeyId, List<string> assignedPatients) {
    //     this.therapistId = therapistId ?? throw new ArgumentNullException(nameof(therapistId));
    //     this.firstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
    //     this.lastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
    //     this.emailAddress = emailAddress ?? throw new ArgumentNullException(nameof(emailAddress));
    //     this.productKeyId = productKeyId ?? throw new ArgumentNullException(nameof(productKeyId));
    //     this.assignedPatients = assignedPatients ?? throw new ArgumentNullException(nameof(assignedPatients));
    // }

   
}