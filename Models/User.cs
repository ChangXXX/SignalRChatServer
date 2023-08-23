
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SignalRChat.Models;

public class User
{
    
    // 키값
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? id { get; set; }

    // id
    public string name { get; set; } = null!;

    // 비밀번호
    public string pwd { get; set;} = null!;
}