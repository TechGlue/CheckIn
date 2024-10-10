using CheckMeInService.Models;

namespace CheckMeInService.Data;

public class SubscriptionQueries : Connection 
{
    public SubscriptionQueries(string connectionString)
    {
        _connectionString = connectionString;
    }

    public bool AddNewSubscriber(Subscriber newSubscriber)
    {
        using var subscriberContext = new CheckMeInContext(_connectionString);

        // Create a new subscriber
        subscriberContext.Subscribers.Add(newSubscriber);
        subscriberContext.SaveChanges();

        return true;
    }

    public OfferedSubscriptions? GetSubscription(string subscriptionName)
    {
        using CheckMeInContext checkMeInContext = new CheckMeInContext(_connectionString);
        return checkMeInContext.OfferedSubscriptions
            .SingleOrDefault(x => x.SubscriptionName.ToLower() == subscriptionName.ToLower());
    }
    
    public ActiveSubscriptions? GetActiveSubscriptions(string phoneNumber)
    {
        using CheckMeInContext checkMeInContext = new CheckMeInContext(_connectionString);
        return checkMeInContext.ActiveSubscriptions.SingleOrDefault(x => x.PhoneNumber == phoneNumber);
    }

    public bool VerifySubscriberExists(Subscriber subscriber)
    {
        using CheckMeInContext checkMeInContext = new CheckMeInContext(_connectionString);

        var checkSubs = checkMeInContext.Subscribers.SingleOrDefault(x => x.PhoneNumber == subscriber.PhoneNumber);

        return checkSubs != null;
    }

    public bool CheckForExistingSubscription(Subscriber subscriber, OfferedSubscriptions offeredSubscriptions)
    {
        using CheckMeInContext checkMeInContext = new CheckMeInContext(_connectionString);
        var checkSubscription = checkMeInContext.ActiveSubscriptions.SingleOrDefault(x =>
            x.SubscriberId == subscriber.SubscriberId && x.SubscriptionId == offeredSubscriptions.SubscriptionId);
        return checkSubscription != null;
    }

    public bool AddNewMemberSubscription(Subscriber subscriber, OfferedSubscriptions subscription)
    {
        if (CheckForExistingSubscription(subscriber, subscription))
        {
            return false;
        }

        using CheckMeInContext checkMeInContext = new CheckMeInContext(_connectionString);
        ActiveSubscriptions newSubscription = new ActiveSubscriptions
        {
            SubscriberId = subscriber.SubscriberId,
            SubscriptionId = subscription.SubscriptionId,
            SubscriptionStartDate = DateTime.Now,
            PhoneNumber = subscriber.PhoneNumber
        };

        checkMeInContext.ActiveSubscriptions.Add(newSubscription);
        checkMeInContext.SaveChanges();

        return true;
    }

    public Subscriber? FetchExistingSubscriber(string phoneNumber)
    {
        using CheckMeInContext checkMeInContext = new CheckMeInContext(_connectionString);

        var subscriber = checkMeInContext.Subscribers.SingleOrDefault(x => x.PhoneNumber == phoneNumber);

        return subscriber;
    }

    // todo: unit test this
    public bool RemoveActiveSubscription(ActiveSubscriptions activeSubscriptions)
    {
        using CheckMeInContext checkMeInContext = new CheckMeInContext(_connectionString);

        var activeSubscription = checkMeInContext.ActiveSubscriptions.SingleOrDefault(x =>
            x.ActiveSubscriptionId == activeSubscriptions.ActiveSubscriptionId);

        if (activeSubscription is null)
        {
            return false;
        }

        checkMeInContext.ActiveSubscriptions.Remove(activeSubscription);
        checkMeInContext.SaveChanges();
        return true;
    }

    public bool RemoveActiveSubscription(Subscriber subscriber, OfferedSubscriptions subscription)
    {
        using CheckMeInContext checkMeInContext = new CheckMeInContext(_connectionString);

        var activeSubscription = checkMeInContext.ActiveSubscriptions.SingleOrDefault(x =>
            x.SubscriberId == subscriber.SubscriberId && x.SubscriptionId == subscription.SubscriptionId);

        if (activeSubscription is null)
        {
            return false;
        }

        checkMeInContext.ActiveSubscriptions.Remove(activeSubscription);
        checkMeInContext.SaveChanges();
        return true;
    }
}