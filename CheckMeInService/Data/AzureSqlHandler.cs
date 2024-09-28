using CheckMeInService.Models;
using Microsoft.Data.SqlClient;

namespace CheckMeInService.Data;

public class AzureSqlHandler
{
    readonly SqlConnectionStringBuilder _builder = new();

    public AzureSqlHandler(DatabaseSettings settings)
    {
        _builder.DataSource = settings.DataSource;
        _builder.UserID = settings.UserId;
        _builder.Password = settings.Password;
        _builder.InitialCatalog = settings.InitialCatalog;

        // customize the timeouts to prevent timeouts
        _builder.CommandTimeout = 180;
        _builder.ConnectTimeout = 30;
    }

    public bool AddNewSubscriber(Subscriber newSubscriber)
    {
        using (var subscriberContext = new SubscribersContext(_builder))
        {
            if (subscriberContext.Database.CanConnect() is false)
            {
                return false;
            }

            // Create a new subscriber
            subscriberContext.Subscribers.Add(newSubscriber);
            subscriberContext.SaveChanges();
        }

        // indicating a successful database interaction
        return true;
    }

    public OfferedSubscriptions? GetSubscription(string subscriptionName)
    {
        using SubscribersContext subscriberContext = new SubscribersContext(_builder);
        if (subscriberContext.Database.CanConnect() is false)
            return null;

        return subscriberContext.OfferedSubscriptions.FirstOrDefault(x => x.SubscriptionName == subscriptionName);
    }

    public bool VerifySubscriberExists(Subscriber subscriber)
    {
        using SubscribersContext subscriberContext = new SubscribersContext(_builder);
        if (subscriberContext.Database.CanConnect() is false)
        {
            return false;
        }

        var checkSubs = subscriberContext.Subscribers.FirstOrDefault(x => x.PhoneNumber == subscriber.PhoneNumber);

        return checkSubs != null;
    }


    public bool CheckForExistingSubscription(Subscriber subscriber, OfferedSubscriptions offeredSubscriptions)
    {
        using (SubscribersContext subscriberContext = new SubscribersContext(_builder))
        {
            var checkSubscription = subscriberContext.ActiveSubscriptions.FirstOrDefault(x =>
                x.SubscriberId == subscriber.SubscriberId && x.SubscriptionId == offeredSubscriptions.SubscriptionId);
            return checkSubscription != null;
        }
    }

    public void UnEnrollSubscriber(Subscriber subscriber)
    {
        // Todo: Write logic for fetching and deleting a subscriber based on their phone number 
    }

    public bool AddNewMemberSubscription(Subscriber subscriber, OfferedSubscriptions subscription)
    {
        if (CheckForExistingSubscription(subscriber, subscription))
        {
            return false;
        }

        using (SubscribersContext subscriberContext = new SubscribersContext(_builder))
        {
            ActiveSubscriptions newSubscription = new ActiveSubscriptions
            {
                SubscriberId = subscriber.SubscriberId,
                SubscriptionId = subscription.SubscriptionId,
                SubscriptionStartDate = DateTime.Now,
                PhoneNumber = subscriber.PhoneNumber
            };

            subscriberContext.ActiveSubscriptions.Add(newSubscription);
            subscriberContext.SaveChanges();
        }

        return true;
    }

    public Subscriber? FetchExistingSubscriber(string phoneNumber)
    {
        using (SubscribersContext subscriberContext = new SubscribersContext(_builder))
        {
            if (subscriberContext.Database.CanConnect() is false)
            {
                return null;
            }

            var subscriber = subscriberContext.Subscribers.FirstOrDefault(x => x.PhoneNumber == phoneNumber);

            return subscriber;
        }
    }

    public string TestConnection()
    {
        using (SqlConnection connection = new SqlConnection(_builder.ConnectionString))
        {
            connection.Open();
            String sql = "SELECT name, collation_name FROM sys.databases";

            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader.GetString(0) == _builder.InitialCatalog)
                        {
                            return "Database found: " + reader.GetString(0);
                        }
                    }
                }
            }
        }

        return $"Connection to Azure SQL DB {_builder.InitialCatalog} failed...";
    }
}