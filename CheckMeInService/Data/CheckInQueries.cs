using CheckMeInService.Models;
using Microsoft.EntityFrameworkCore;

namespace CheckMeInService.Data;

public class CheckInQueries : Connection
{
    public CheckInQueries(string connectionString)
    {
        _connectionString = connectionString;
    }

    // Needs re-work, had to do a last-minute check inside the method to be able to test thoroughly. Due to different contexts with testcontainers;
    public bool LogCheckIn(ActiveSubscriptions activeSubscription)
    {
        using var checkInContext = new CheckMeInContext(_connectionString);

        // First handle if the user has already checked in before 
        CheckIn? checkIn =
            checkInContext.CheckIn.SingleOrDefault(x =>
                x != null && x.ActiveSubscriptionId == activeSubscription.ActiveSubscriptionId);

        if (checkIn is null)
        {
            return false;
        }

        int previousTotal = checkIn.TotalCheckIns;

        // verify that the checkIn Date is above the future reminder hours
        if (checkIn.LastCheckInDate < checkIn.FutureCheckInDate)
        {
            return false;
        }

        checkIn.TotalCheckIns += 1; // Update the entity
        checkInContext.Update(checkIn);
        checkInContext.SaveChanges();

        bool dbChanged = checkInContext.CheckIn
            .SingleOrDefault(x => activeSubscription.ActiveSubscriptionId == x.ActiveSubscriptionId)
            .TotalCheckIns != previousTotal;
        
        // check if the changes were applied 
        if (dbChanged)
        {
            return true;
        }

        return false;
    }

    public bool CreateNewCheckIn(ActiveSubscriptions activeSubscription, int futureReminderHours)
    {
        using var checkInContext = new CheckMeInContext(_connectionString);

        if (checkInContext.CheckIn.Any(x =>
                x != null && x.ActiveSubscriptionId == activeSubscription.ActiveSubscriptionId))
        {
            return false;
        }

        CheckIn newCheckIn = new CheckIn
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
        using var checkInContext = new CheckMeInContext(_connectionString);

        CheckIn? checkIn =
            checkInContext.CheckIn.SingleOrDefault(
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