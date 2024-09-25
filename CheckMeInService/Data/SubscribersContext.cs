using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CheckMeInService.Models;

public class SubscribersContext : DbContext
{
    public DbSet<Subscriber> Subscribers { get; set; }
    public DbSet<Subscriptions> Subscriptions { get; set; }
    public DbSet<OfferedSubscriptions> OfferedSubscriptions { get; set; }
    
    private SqlConnectionStringBuilder DatabaseSettings { get;}

    public SubscribersContext(SqlConnectionStringBuilder settings)
    {
        DatabaseSettings = settings;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(DatabaseSettings.ConnectionString);
    }
}