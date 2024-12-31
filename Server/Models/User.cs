using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Server.Models;

public class User {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? userId { get; set; }

    [BsonElement("email_address")]
    public string emailAddress { get; set; } = null!;
    
    [BsonElement("password_salt")]
    public string passwordSalt { get; set; } = null!;

    [BsonElement("password_hash")]
    public string passwordHash { get; set; } = null!;

    public string role { get; set; } = null!;

    [BsonElement("account_created")]
    public DateTime accountCreated { get; set; }

    [BsonElement("last_logged_in")]
    public DateTime lastLoggedIn { get; set; }

    [JsonConstructor]
    public User(string userId, string emailAddress, string passwordSalt, string passwordHash, string role, DateTime accountCreated, DateTime lastLoggedIn) {
        this.userId = userId ?? throw new ArgumentNullException(nameof(userId));
        this.emailAddress = emailAddress ?? throw new ArgumentNullException(nameof(emailAddress));
        this.passwordSalt = passwordSalt ?? throw new ArgumentNullException(nameof(passwordSalt));
        this.passwordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
        this.role = role ?? throw new ArgumentNullException(nameof(role));
        this.accountCreated = accountCreated;
        this.lastLoggedIn = lastLoggedIn;
    }
}