
namespace SignalRChat.Models;

public class ChatDatabaseSettings
{

    public static string SectionName = "ChatDatabase";

    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string UsersCollectionName { get; set; } = null!;
}