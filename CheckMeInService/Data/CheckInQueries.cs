using CheckMeInService.Models;

namespace CheckMeInService.Data;

public class CheckInQueries : Connection
{
    public CheckInQueries(string connectionString)
    {
        ConnectionString = connectionString;
    }

    public void CreateCheckInHistoryEntry(ActiveSubscriptions activeSubscriptions, CheckIn checkIn,
        string subscriptionName)
    {
        using var checkInContext = new CheckMeInContext(ConnectionString);

        var newCheckInHistory = new CheckInHistory(activeSubscriptions.SubscriptionId, activeSubscriptions.SubscriberId,
            checkIn.LastCheckInDate, subscriptionName);

        checkInContext.CheckInHistory.Add(newCheckInHistory);
        checkInContext.SaveChanges();
    }

    // Needs re-work, had to do a last-minute check inside the method to be able to test thoroughly. Due to different contexts with testcontainers;
    public bool LogCheckIn(ActiveSubscriptions activeSubscription)
    {
        using var checkInContext = new CheckMeInContext(ConnectionString);

        var checkIn =
            checkInContext.CheckIn.SingleOrDefault(x =>
                x.ActiveSubscriptionId == activeSubscription.ActiveSubscriptionId);

        if (checkIn is null) return false;

        var previousTotal = checkIn.TotalCheckIns;

        // verify that the checkIn Date is above the future reminder hours
        if (checkIn.LastCheckInDate < checkIn.FutureCheckInDate) return false;

        checkIn.LastCheckInDate = DateTime.Now;
        checkIn.FutureCheckInDate = checkIn.LastCheckInDate.AddHours(24);
        checkIn.TotalCheckIns += 1; // Update the entity
        checkInContext.Update(checkIn);
        checkInContext.SaveChanges();

        var dbChanged = checkInContext.CheckIn
            .SingleOrDefault(x => activeSubscription.ActiveSubscriptionId == x.ActiveSubscriptionId);

        if (dbChanged is null) return false;

        if (dbChanged.TotalCheckIns != previousTotal) return true;

        return false;
    }

    public bool CreateNewCheckIn(ActiveSubscriptions activeSubscription, int futureReminderHours)
    {
        using var checkInContext = new CheckMeInContext(ConnectionString);

        if (checkInContext.CheckIn.Any(x =>
                x.ActiveSubscriptionId == activeSubscription.ActiveSubscriptionId))
            return false;

        var newCheckIn = new CheckIn
        {
            ActiveSubscriptionId = activeSubscription.ActiveSubscriptionId,
            LastCheckInDate = activeSubscription.SubscriptionStartDate,
            FutureCheckInDate = activeSubscription.SubscriptionStartDate.AddHours(futureReminderHours),
            TotalCheckIns = 0
        };

        checkInContext.CheckIn.Add(newCheckIn);
        checkInContext.SaveChanges();
        return true;
    }

    public bool RemoveCheckIn(ActiveSubscriptions activeSubscription)
    {
        using var checkInContext = new CheckMeInContext(ConnectionString);

        var checkIn =
            checkInContext.CheckIn.SingleOrDefault(
                x => x.ActiveSubscriptionId == activeSubscription.ActiveSubscriptionId);

        if (checkIn is not null)
        {
            checkInContext.Remove(checkIn);
            checkInContext.SaveChanges();
            return true;
        }

        return false;
    }

    public List<CheckIn> FetchAllActiveCheckIns()
    {
        using var checkInContext = new CheckMeInContext(ConnectionString);
        return checkInContext.CheckIn.Select(x => x).ToList();
    }

    public CheckIn GetCheckIn(Guid activeSubscriptionId)
    {
        using var checkInContext = new CheckMeInContext(ConnectionString);
        var checkin = checkInContext.CheckIn.SingleOrDefault(x => x.ActiveSubscriptionId == activeSubscriptionId) ??
                      new CheckIn();

        return checkin;
    }
}