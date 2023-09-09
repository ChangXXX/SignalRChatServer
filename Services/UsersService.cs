using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SignalRChat.Models;

namespace SignalRChat.Services;

public class UsersService
{

    private readonly IMongoCollection<User> _usersCollection;

    public UsersService(
        IOptions<ChatDatabaseSettings> chatDatabaseSettings
    ) {

        var mongoClient = new MongoClient(chatDatabaseSettings.Value.ConnectionString);

        var mongoDb = mongoClient.GetDatabase(chatDatabaseSettings.Value.DatabaseName);

        _usersCollection = mongoDb.GetCollection<User>(chatDatabaseSettings.Value.UsersCollectionName);
    }

    public async Task CreateAsync(User newUser) {
        await _usersCollection.InsertOneAsync(newUser);
    }

    public async Task<List<User>> GetAsync() =>
        await _usersCollection.Find(_ => true).ToListAsync();

    public async Task<User?> GetAsync(string name) =>
        await _usersCollection.Find(user => user.Name == name).FirstOrDefaultAsync();

    public async Task<User?> GetAsync(string name, string pwd) =>
        await _usersCollection.Find(user => user.Name == name && user.Pwd == pwd).FirstOrDefaultAsync();
}