// REFERENCE CLASS
// NOT ACTUAL CODE FOR PROJECT, JUST AN EXAMPLE API IMPLEMENTATION

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Server.Models;

public class Playlist {
    // Notes to Remember:
    // [Bson] represents the stuff reflected in MongoDB
    // [Json] represents the stuff reflected in the body/response of the API request

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string username { get; set; } = null!;

    [BsonElement("items")]
    [JsonPropertyName("items")]
    public List<string> movieIds { get; set; } = null!;

}