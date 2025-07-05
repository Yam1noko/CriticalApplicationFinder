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
        modelBuilder.Entity<RuleFullName>()
            .HasIndex(x => x.FullName)
            .IsUnique();

        modelBuilder.Entity<RuleSubstring>()
            .HasIndex(x => x.Substring)
            .IsUnique();

        // RuleFullName.fullname → Rule.fullname
        modelBuilder.Entity<Rule>()
            .HasOne(r => r.RuleFullName)
            .WithMany(f => f.Rules)
            .HasForeignKey(r => r.FullName)
            .HasPrincipalKey(f => f.FullName)
            .OnDelete(DeleteBehavior.Restrict);

        // RuleSubstring.substring → Rule.substring
        modelBuilder.Entity<Rule>()
            .HasOne(r => r.RuleSubstring)
            .WithMany(s => s.Rules)
            .HasForeignKey(r => r.Substring)
            .HasPrincipalKey(s => s.Substring)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<NotificationEmail>()
            .HasIndex(x => x.Id)
            .IsUnique();

        modelBuilder.Entity<NotificationTemplate>()
            .HasIndex(x => x.Id)
            .IsUnique();
    }
}
