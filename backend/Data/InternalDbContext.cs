using backend.Models.Internal;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public class InternalDbContext : DbContext
{
    public InternalDbContext(DbContextOptions<InternalDbContext> options) : base(options) { }

    public DbSet<Request> Requests { get; set; }
    public DbSet<Rule> Rules { get; set; }
    public DbSet<RuleFullName> RuleFullNames { get; set; }
    public DbSet<RuleSubstring> RuleSubstrings { get; set; }
    public DbSet<NotificationEmail> NotificationEmails { get; set; }
    public DbSet<NotificationTemplate> NotificationTemplates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Rule (1) -> RuleFullNames (Many)
        modelBuilder.Entity<Rule>()
            .HasMany(r => r.RuleFullNames)
            .WithOne()
            .HasForeignKey(f => f.RuleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Rule (1) -> RuleSubstrings (Many)
        modelBuilder.Entity<Rule>()

            .HasMany(r => r.RuleSubstrings)
            .WithOne()
            .HasForeignKey(s => s.RuleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<NotificationEmail>()
            .HasIndex(x => x.Id)
            .IsUnique();

        modelBuilder.Entity<NotificationTemplate>()
            .HasIndex(x => x.Id)
            .IsUnique();

    }
}
