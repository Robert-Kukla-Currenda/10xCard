using Microsoft.EntityFrameworkCore;
using TenXCards.API.Data.Models;

namespace TenXCards.API.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public DbSet<User> Users { get; set; } = default!;
    public DbSet<Card> Cards { get; set; } = default!;
    public DbSet<ErrorLog> ErrorLogs { get; set; } = default!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=localhost:5433;Database=ten_x_cards;Username=postgres;Password=postgres");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure indexes
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => new { u.FirstName, u.LastName });

        modelBuilder.Entity<Card>()
            .HasIndex(c => c.UserId);

        modelBuilder.Entity<Card>()
            .HasIndex(c => c.GeneratedBy);

        // Configure relationships
        modelBuilder.Entity<Card>()
            .HasOne(c => c.User)
            .WithMany(u => u.Cards)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ErrorLog>()
            .HasOne(e => e.Card)
            .WithMany(c => c.ErrorLogs)
            .HasForeignKey(e => e.CardId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure check constraints
        modelBuilder.Entity<Card>()
            .HasCheckConstraint("CK_Card_GeneratedBy", "\"GeneratedBy\" IN ('AI', 'human')");
    }    
}
