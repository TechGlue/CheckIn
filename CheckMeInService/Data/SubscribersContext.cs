using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CheckMeInService.Models;

public class SubscribersContext : DbContext
{
    public DbSet<Subscriber> Subscribers { get; set; }
    public DbSet<ActiveSubscriptions> ActiveSubscriptions { get; set; }
    public DbSet<OfferedSubscriptions> OfferedSubscriptions { get; set; }
    
    private string ConnectionString { get; set; } 

    public SubscribersContext(string connectionString)
    {
        ConnectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(ConnectionString
        );
    }
}