using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Server.Models;

public class ProductKey {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? productKeyId { get; set; }

    [BsonElement("product_key")]
    public string productKey { get; set; }

    [BsonElement("is_activated")]
    public bool isActivated { get; set; }
}