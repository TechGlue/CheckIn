using Microsoft.EntityFrameworkCore;

namespace CheckMeInService.Models;

public class CheckMeInContext : DbContext
{
    public DbSet<Subscriber> Subscribers { get; set; }
    public DbSet<ActiveSubscriptions> ActiveSubscriptions { get; set; }
    public DbSet<OfferedSubscriptions> OfferedSubscriptions { get; set; }
    public DbSet<CheckIn?> CheckIn { get; set; }

    private string ConnectionString { get; set; }

    public CheckMeInContext(string connectionString)
    {
        ConnectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(ConnectionString);
    }
}