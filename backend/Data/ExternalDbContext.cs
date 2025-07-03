using backend.Models.External;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public class ExternalDbContext : DbContext
{
    public ExternalDbContext(DbContextOptions<ExternalDbContext> options) : base(options) { }

    public DbSet<ExternalRequest> Requests { get; set; }
}
