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

    [BsonElement("is_temporary_password")]
    public bool isTemporaryPassword { get; set; } 

    public string role { get; set; } = null!;

    [BsonElement("account_created")]
    public DateTime accountCreated { get; set; }

    [BsonElement("last_logged_in")]
    public DateTime? lastLoggedIn { get; set; } = null!;

    [JsonConstructor]
    public User(string userId, string emailAddress, string passwordSalt, string passwordHash, string role, DateTime accountCreated, bool? isTemporaryPassword, DateTime? lastLoggedIn = null) {
        this.userId = userId ?? throw new ArgumentNullException(nameof(userId));
        this.emailAddress = emailAddress ?? throw new ArgumentNullException(nameof(emailAddress));
        this.passwordSalt = passwordSalt ?? throw new ArgumentNullException(nameof(passwordSalt));
        this.passwordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
        this.role = role ?? throw new ArgumentNullException(nameof(role));
        this.accountCreated = accountCreated != default ? accountCreated : throw new ArgumentException("Invalid account creation date", nameof(accountCreated));
        this.lastLoggedIn = lastLoggedIn != default ? lastLoggedIn : throw new ArgumentException("Invalid last logged in date", nameof(lastLoggedIn));
        this.isTemporaryPassword = isTemporaryPassword ?? throw new ArgumentNullException(nameof(isTemporaryPassword));
    }
}