using Microsoft.Data.SqlClient;

namespace CheckMeInService.Models;

public class DatabaseSettings
{
    public string? DataSource { get; }
    public string? UserId { get; }
    public string? Password { get; }
    public string? InitialCatalog { get; }

    public DatabaseSettings(string settingsFile)
    {
        // Read app settings.json
        var config = new ConfigurationBuilder()
            .AddJsonFile(settingsFile)
            .Build();

        // Propagate the config values to the members
        DataSource = config.GetSection("AzureSql:DataSource").Value;
        UserId = config.GetSection("AzureSql:UserId").Value;
        Password = config.GetSection("AzureSql:Password").Value;
        InitialCatalog = config.GetSection("AzureSql:InitialCatalog").Value;
    }
    
    public string GetConnection()
    {
        var builder = new SqlConnectionStringBuilder
        {
            DataSource = DataSource,
            UserID = UserId,
            Password = Password,
            InitialCatalog = InitialCatalog
        };

        return builder.ConnectionString;
    }
    
    public bool TestConnection()
    {
        using var connection = new SqlConnection(GetConnection());
        try
        {
            // open the connection but specify the time to wait before terminating the attempt to connect
            connection.OpenAsync().Wait(5000);
            return true;
        }
        catch (SqlException)
        {
            return false;
        }
    }
}