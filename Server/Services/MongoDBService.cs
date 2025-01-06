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
    private readonly IMongoCollection<ProductKey> _productKeyCollection;
    private readonly IMongoCollection<User> _userCollection;
    private readonly IMongoCollection<Therapist> _therapistCollection;
    private readonly IMongoCollection<Patient> _patientCollection;

    public MongoDBService(IOptions<MongoDBSettings> mongoDBSettings) {
        MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
        IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
        _playlistCollection = database.GetCollection<Playlist>(mongoDBSettings.Value.PlaylistCollectionName);
        _productKeyCollection = database.GetCollection<ProductKey>(mongoDBSettings.Value.ProductKeyCollectionName);
        _userCollection = database.GetCollection<User>(mongoDBSettings.Value.UserCollectionName);
        _therapistCollection = database.GetCollection<Therapist>(mongoDBSettings.Value.TherapistCollectionName);
        _patientCollection = database.GetCollection<Patient>(mongoDBSettings.Value.PatientCollectionName);

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
    
    // Product Key Endpoints DB Integration
    public async Task CreateProductKeyAsync(ProductKey productKey) {
        await _productKeyCollection.InsertOneAsync(productKey);
    }

    public async Task<ProductKey> GetProductKeyAsync(string productKey) {
        return await _productKeyCollection.Find(pk => pk.productKey == productKey).FirstOrDefaultAsync();
    }

    public async Task<ProductKey> GetProductKeyByIdAsync(string productKeyId) {
        return await _productKeyCollection.Find(pk => pk.productKeyId == productKeyId).FirstOrDefaultAsync();
    }

    public async Task UpdateProductKeyAsync(ProductKey productKey) {
        var filter = Builders<ProductKey>.Filter.Eq(pk => pk.productKeyId, productKey.productKeyId);
        await _productKeyCollection.ReplaceOneAsync(filter, productKey);
    }

    //User Endpoints DB Integration
    public async Task CreateUserAsync(User user) {
        await _userCollection.InsertOneAsync(user);
        return;
    }

    public async Task<User> GetUserByEmailAsync(string emailAddress) {
        return await _userCollection.Find(u => u.emailAddress == emailAddress).FirstOrDefaultAsync();
    }

    public async Task<User> UpdateUserAsync(User user) {
        await _userCollection.ReplaceOneAsync(u => u.userId == user.userId, user);
        return user;
    }

    //Therapist Endpoints DB Integration
    public async Task CreateTherapistAsync(Therapist therapist) { 
        await _therapistCollection.InsertOneAsync(therapist);
        return;
    }

    public async Task<Therapist> GetTherapistByIdAsync(string therapistId) { 
        return await _therapistCollection.Find(t => t.therapistId == therapistId).FirstOrDefaultAsync();
    }

    public async Task<Therapist> GetTherapistByEmailAsync(string emailAddress) {
        return await _therapistCollection.Find(t => t.emailAddress == emailAddress).FirstOrDefaultAsync();
    }

    public async Task UpdateTherapistInformationAsync(Therapist therapist) {
        var filter = Builders<Therapist>.Filter.Eq(t => t.therapistId, therapist.therapistId);
        var update = Builders<Therapist>.Update
            .Set(t => t.firstName, therapist.firstName)
            .Set(t => t.lastName, therapist.lastName)
            .Set(t => t.emailAddress, therapist.emailAddress)
            .Set(t => t.productKeyId, therapist.productKeyId);

        await _therapistCollection.UpdateOneAsync(filter, update);
    }

    public async Task DeleteTherapistAsync(string therapistId) {
        var filter = Builders<Therapist>.Filter.Eq(t => t.therapistId, therapistId);
        await _therapistCollection.DeleteOneAsync(filter);
    }

    //Patient Endpoints DB Integration
    public async Task CreatePatientAsync(Patient patient) {
        await _patientCollection.InsertOneAsync(patient);
        return;
    }

    public async Task<Patient> GetPatientByEmailAsync(string emailAddress) {
        return await _patientCollection.Find(p => p.emailAddress == emailAddress).FirstOrDefaultAsync();
    }
}