using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Server.Models;
public class Exercise {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? exerciseId { get; set; }

    [BsonElement("name")]
    public string Name { get; set; }

    [BsonElement("instructions")]
    public string Instructions { get; set; }

    [BsonElement("target_reps")]
    public int TargetReps { get; set; }

    [BsonElement("target_duration")]
    public int TargetDuration { get; set; } // Duration in minutes

    [BsonElement("intensity")]
    public string Intensity { get; set; }

    [JsonConstructor]
    public Exercise(string exerciseId, string name, string instructions, int targetReps, int targetDuration, string intensity) {
        this.exerciseId = exerciseId ?? throw new ArgumentNullException(nameof(exerciseId));
        this.Name = name ?? throw new ArgumentNullException(nameof(name));
        this.Instructions = instructions ?? throw new ArgumentNullException(nameof(instructions));
        this.TargetReps = targetReps;
        this.TargetDuration = targetDuration;
        this.Intensity = intensity ?? throw new ArgumentNullException(nameof(intensity));
    }
}