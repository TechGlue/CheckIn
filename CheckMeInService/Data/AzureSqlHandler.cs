using CheckMeInService.Models;

namespace CheckMeInService.Data;

public class AzureSqlHandler
{
    private readonly string _connectionString;

    public AzureSqlHandler(string connectionString)
    {
        _connectionString = connectionString;
    }

    public bool AddNewSubscriber(Subscriber newSubscriber)
    {
        using var subscriberContext = new SubscribersContext(_connectionString);
        if (subscriberContext.Database.CanConnect() is false)
        {
            return false;
        }

        // Create a new subscriber
        subscriberContext.Subscribers.Add(newSubscriber);
        subscriberContext.SaveChanges();

        return true;
    }

    public OfferedSubscriptions? GetSubscription(string subscriptionName)
    {
        using SubscribersContext subscriberContext = new SubscribersContext(_connectionString);
        if (subscriberContext.Database.CanConnect() is false)
            return null;

        return subscriberContext.OfferedSubscriptions
            .FirstOrDefault(x => x.SubscriptionName.ToLower() == subscriptionName.ToLower());
    }

    public bool VerifySubscriberExists(Subscriber subscriber)
    {
        using SubscribersContext subscriberContext = new SubscribersContext(_connectionString);
        if (subscriberContext.Database.CanConnect() is false)
        {
            return false;
        }

        var checkSubs = subscriberContext.Subscribers.FirstOrDefault(x => x.PhoneNumber == subscriber.PhoneNumber);

        return checkSubs != null;
    }

    public bool CheckForExistingSubscription(Subscriber subscriber, OfferedSubscriptions offeredSubscriptions)
    {
        using SubscribersContext subscriberContext = new SubscribersContext(_connectionString);
        var checkSubscription = subscriberContext.ActiveSubscriptions.FirstOrDefault(x =>
            x.SubscriberId == subscriber.SubscriberId && x.SubscriptionId == offeredSubscriptions.SubscriptionId);
        return checkSubscription != null;
    }

    public bool AddNewMemberSubscription(Subscriber subscriber, OfferedSubscriptions subscription)
    {
        if (CheckForExistingSubscription(subscriber, subscription))
        {
            return false;
        }

        using SubscribersContext subscriberContext = new SubscribersContext(_connectionString);
        ActiveSubscriptions newSubscription = new ActiveSubscriptions
        {
            SubscriberId = subscriber.SubscriberId,
            SubscriptionId = subscription.SubscriptionId,
            SubscriptionStartDate = DateTime.Now,
            PhoneNumber = subscriber.PhoneNumber
        };

        subscriberContext.ActiveSubscriptions.Add(newSubscription);
        subscriberContext.SaveChanges();

        return true;
    }

    public Subscriber? FetchExistingSubscriber(string phoneNumber)
    {
        using SubscribersContext subscriberContext = new SubscribersContext(_connectionString);
        if (subscriberContext.Database.CanConnect() is false)
        {
            return null;
        }

        var subscriber = subscriberContext.Subscribers.FirstOrDefault(x => x.PhoneNumber == phoneNumber);

        return subscriber;
    }

    public bool RemoveActiveSubscription(Subscriber subscriber, OfferedSubscriptions subscription)
    {
        using SubscribersContext subscribersContext = new SubscribersContext(_connectionString);
        if (subscribersContext.Database.CanConnect() is false)
        {
            return false;
        }
            
        var activeSubscription = subscribersContext.ActiveSubscriptions.FirstOrDefault(x =>
            x.SubscriberId == subscriber.SubscriberId && x.SubscriptionId == subscription.SubscriptionId);

        if (activeSubscription is null)
        {
            return false;
        }
            
        subscribersContext.ActiveSubscriptions.Remove(activeSubscription);
        subscribersContext.SaveChanges();
        return true;
    }
}