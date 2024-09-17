using CheckMeInService.Models;
using Microsoft.AspNetCore.Http.HttpResults;
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
    }

    public void AddNewSubscription(string subscriptionName)
    {
        Console.WriteLine("something subscription...");
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