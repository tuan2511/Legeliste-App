using LegelisteApp.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LegelisteApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Stall> Stalls { get; set; }
    public DbSet<DailyEntry> DailyEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Unique constraint for StallId and Date (only one entry per stall per day)
        modelBuilder.Entity<DailyEntry>()
            .HasIndex(e => new { e.StallId, e.Date })
            .IsUnique();
    }
}
