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
}