using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CheckMeInService.Models;

public class SubscribersContext : DbContext
{
    public DbSet<Subscriber> Subscribers { get; set; }
    private SqlConnectionStringBuilder _databaseSettings { get; set; }

    public SubscribersContext(SqlConnectionStringBuilder settings)
    {
        _databaseSettings = settings;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_databaseSettings.ConnectionString);
    }
}