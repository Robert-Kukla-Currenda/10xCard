using Microsoft.EntityFrameworkCore;
using TenXCards.API.Data.Models;

namespace TenXCards.API.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = default!;
    public DbSet<Card> Cards { get; set; } = default!;
    public DbSet<ErrorLog> ErrorLogs { get; set; } = default!;
    public DbSet<OriginalContent> OriginalContents { get; set; } = default!;

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

        modelBuilder.Entity<OriginalContent>()
            .HasIndex(o => o.UserId);

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

        modelBuilder.Entity<OriginalContent>()
            .HasOne(o => o.User)
            .WithMany(u => u.OriginalContents)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure check constraints using new syntax
        modelBuilder.Entity<Card>()
            .ToTable(tb => tb.HasCheckConstraint(
                "CK_Card_GeneratedBy", 
                "\"GeneratedBy\" IN ('AI', 'human')"
            ));

        modelBuilder.Entity<OriginalContent>()
            .ToTable(tb => tb.HasCheckConstraint(
                "CK_OriginalContent_ContentLength",
                "char_length(\"Content\") BETWEEN 1000 AND 10000"
            ));
    }    
}
