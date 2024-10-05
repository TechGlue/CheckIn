using CheckMeInService.Models;

namespace CheckMeInService.Data;

public class CheckInQueries : Queries
{
    public CheckInQueries(string connectionString)
    {
        _connectionString = connectionString;
    }

    public bool CreateNewCheckIn(ActiveSubscriptions activeSubscription, int futureReminderHours)
    {
        using var checkInContext = new CheckMeInContext(_connectionString);

        // check if check in already exists
        if (checkInContext.CheckIn.Any(x => x.ActiveSubscriptionId == activeSubscription.ActiveSubscriptionId))
        {
            return false;
        }

        CheckIn newCheckIn = new CheckIn
        {
            ActiveSubscriptionId = activeSubscription.ActiveSubscriptionId,
            LastCheckInDate = activeSubscription.SubscriptionStartDate,
            FutureCheckInDate = activeSubscription.SubscriptionStartDate.AddHours(futureReminderHours),
            CurrentStreak = 0
        };

        checkInContext.CheckIn.Add(newCheckIn);
        checkInContext.SaveChanges();
        return true;
    }

    public bool RemoveCheckIn(ActiveSubscriptions activeSubscription)
    {
        using var checkInContext = new CheckMeInContext(_connectionString);

        CheckIn? checkIn =
            checkInContext.CheckIn.FirstOrDefault(
                x => x != null && x.ActiveSubscriptionId == activeSubscription.ActiveSubscriptionId);

        if (checkIn is not null)
        {
            checkInContext.Remove(checkIn);
            checkInContext.SaveChanges();
            return true;
        }

        return false;
    }

    public List<CheckIn?> FetchAllActiveCheckIns()
    {
        using var checkInContext = new CheckMeInContext(_connectionString);
        return checkInContext.CheckIn.Select(x => x).ToList();
    }
}