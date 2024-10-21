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
            connection.Open();
            return true;
        }
        catch (SqlException)
        {
            return false;
        }
    }
}