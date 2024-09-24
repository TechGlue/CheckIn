using CheckMeInService.Models;
using Microsoft.Data.SqlClient;

namespace CheckMeInService.Controllers;

public class AzureSqlHandler
{
    SqlConnectionStringBuilder builder = new();

    public AzureSqlHandler(DatabaseSettings settings)
    {
        builder.DataSource = settings.DataSource;
        builder.UserID = settings.UserId;
        builder.Password = settings.Password;
        builder.InitialCatalog = settings.InitialCatalog;
        builder.CommandTimeout = 180;
        builder.ConnectTimeout = 30;
    }
    
    // Todo: figure out why entity framework can't bind the data from the SQL. It looks to be connecting so that's pretty good. 
    public void AddNewSubscriber(string name, string phoneNumber)
    {
        // Todo: Write logic for creating a new subscriber
        using (var dbContext = new SubscribersContext(builder))
        {
            string canConnect = dbContext.Database.CanConnect() ?  "Connected to DB": "Could not connect";
            var output = dbContext.Subscribers.ToList();
            
            Console.WriteLine(canConnect);
        }
    }
    
    // Working entity framework sample it's binding correctly 
    // public void AddNewSubscriber(string name, string phoneNumber)
    // {
    //     // Todo: Write logic for creating a new subscriber
    //     using (var dbContext = new SubscribersContext(builder))
    //     {
    //         string canConnect = dbContext.Database.CanConnect() ?  "Connected to DB": "Could not connect";
    //         var output = dbContext.Subscribers.ToList();
    //         
    //         Console.WriteLine(canConnect);
    //     }
    // }
    
    public void RemoveSubscriber(string phoneNumber)
    {
        // Todo: Write logic for fetching and deleting a subscriber based on their phone number 
        
    }

    // It's going to be just subscriptions 
    public void AddNewSubscription(string subscriptionName, Subscriber subscriber )
    {
        // Todo: Write logic for adding a subscription for a subscriber
        
    }
    
    public string TestConnection()
    {
        using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
        {
            connection.Open();
            String sql = "SELECT name, collation_name FROM sys.databases";

            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader.GetString(0) == builder.InitialCatalog)
                        {
                            return "Database found: " + reader.GetString(0);
                        }
                    }
                }
            }
        }

        return $"Connection to Azure SQL DB {builder.InitialCatalog} failed...";
    }
}