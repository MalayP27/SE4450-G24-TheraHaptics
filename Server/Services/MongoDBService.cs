using Server.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Server.Services;

public class MongoDBService {
    // REFERENCE
    // NOT ACTUAL CODE FOR PROJECT, JUST AN EXAMPLE API IMPLEMENTATION
    //*************************************************************************

    private readonly IMongoCollection<Playlist> _playlistCollection;
    private readonly IMongoCollection<Therapist> _therapistCollection;

    public MongoDBService(IOptions<MongoDBSettings> mongoDBSettings) {
        MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
        IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
        _playlistCollection = database.GetCollection<Playlist>(mongoDBSettings.Value.PlaylistCollectionName);
        _therapistCollection = database.GetCollection<Therapist>(mongoDBSettings.Value.TherapistCollectionName);

        var collections = database.ListCollections().ToList();
        System.Console.WriteLine("Successfully connected to MongoDB. Collections found: " + collections.Count);
    }

    public async Task<List<Playlist>> GetAsync() { 
        return await _playlistCollection.Find(new BsonDocument()).ToListAsync();
    }
    public async Task CreateAsync(Playlist playlist) { 
        await _playlistCollection.InsertOneAsync(playlist);
        return;
    }
    public async Task AddToPlaylistAsync(string id, string movieId) {
        FilterDefinition<Playlist> filter = Builders<Playlist>.Filter.Eq("Id", id);
        UpdateDefinition<Playlist> update = Builders<Playlist>.Update.AddToSet<string>("movieIds", movieId);
        await _playlistCollection.UpdateOneAsync(filter, update);
        return;
    }
    public async Task DeleteAsync(string id) { 
        FilterDefinition<Playlist> filter = Builders<Playlist>.Filter.Eq("Id", id);
        await _playlistCollection.DeleteOneAsync(filter);
        return;
    }
    
    // ******************************END OF EXAMPLE CODE********************

    public async Task CreateTherapistAsync(Therapist therapist) { 
        await _therapistCollection.InsertOneAsync(therapist);
        return;
    }

    public async Task<Therapist> GetTherapistAsync(string therapistId) { 
        return await _therapistCollection.Find(t => t.therapistId == therapistId).FirstOrDefaultAsync();
    }

}