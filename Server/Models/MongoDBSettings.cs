namespace Server.Models;

public class MongoDBSettings {

    public string ConnectionURI { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string PlaylistCollectionName { get; set; } = null!;
    public string UserCollectionName { get; set; } = null!;
    public string TherapistCollectionName { get; set; } = null!;
    public string ProductKeyCollectionName { get; set; } = null!;
}