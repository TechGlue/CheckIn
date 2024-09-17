using CheckMeInService.Models;

namespace CheckMeInService.Controllers;

public class AzureSqlHandler
{
    private DatabaseSettings _databaseSettings;
    public AzureSqlHandler (DatabaseSettings settings)
    {
        _databaseSettings = settings;
    }
    
    public void AddNewSubscription (string subscriptionName)
    {
    }
}