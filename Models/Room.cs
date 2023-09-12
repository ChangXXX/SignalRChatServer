using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SignalRChat.Models;
public class Room
{
    public Room()
    {
    }

    public Room(long id, List<string> users)
    {
        this.Id = id;
        this.Users = users;
    }
    [Key]
    public long Id { get; set; }
    [NotMapped]
    public List<string> Users { get; set; } = new List<string>();
}
