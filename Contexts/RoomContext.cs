
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Room>()
            .Property(x => x.Users)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
            );
    }
}