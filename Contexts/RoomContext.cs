
using Microsoft.EntityFrameworkCore;
using SignalRChat.Models;

namespace SignalRChat.Contexts;

public class RoomContext : DbContext
{

    public RoomContext(DbContextOptions<RoomContext> options)
        : base(options)
    {
    }

    public DbSet<Room> Rooms { get; set; } = null!;   
}