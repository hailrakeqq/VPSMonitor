using Microsoft.EntityFrameworkCore;
using VPSMonitor.API.Entities;

namespace VPSMonitor.API;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<User> users { get; set; }
    public DbSet<AuthRefreshToken> refreshtokens { get; set; }
}