
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SignalRChat.Models;

public class User
{
    // id
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Name { get; set; } = null!;

    // 비밀번호
    public string Pwd { get; set;} = null!;
}