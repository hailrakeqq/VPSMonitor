using Microsoft.EntityFrameworkCore;

namespace VPSMonitor.API;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }
    
    
}