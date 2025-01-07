using CheckMeInService.Data;
using CheckMeInService.Models;

namespace CheckMeInService.CheckIns;

public static class CheckInApi
{
    public static RouteGroupBuilder MapCheckInApi(this RouteGroupBuilder group)
    {
        group.MapGet("/test",
            () => Results.Ok("API is working"));

        // Un-Enroll Active Subscription 
        group.MapDelete("/unenroll",
            (SubscriptionQueries subscriptionQueries, CheckInQueries checkInQueries, string phoneNumber, string subscriptionName) =>
            {
                // First identify the active subscription 
                var activeSubscriptions = subscriptionQueries.GetActiveSubscriptions(phoneNumber, subscriptionName);

                if (activeSubscriptions is null)
                    return Results.BadRequest($"Subscription not found for user with phone number: {phoneNumber}");

                // remove check-in for the associated active subscription
                _ = checkInQueries.RemoveCheckIn(activeSubscriptions);

                // Now we can remove the active subscription
                var result = subscriptionQueries.RemoveActiveSubscription(activeSubscriptions);

                return result
                    ? Results.Ok("Successfully, deleted the active subscription")
                    : Results.BadRequest("Unable to un-enroll active subscription");
            });

        group.MapPut("/logDaily",
            (SubscriptionQueries subscriptionQueries, CheckInQueries checkInQueries, string phoneNumber, string subscriptionName) =>
            {
                // First identify the active subscription 
                var activeSubscriptions = subscriptionQueries.GetActiveSubscriptions(phoneNumber, subscriptionName);

                if (activeSubscriptions is null)
                    return Results.BadRequest($"Subscription not found for: {phoneNumber}");

                // Create Check-in and make an entry in the history table
                var result = checkInQueries.LogCheckIn(activeSubscriptions);
                if (result is false) return Results.BadRequest("Already checked in for the day");

                var checkIn = checkInQueries.GetCheckIn(activeSubscriptions.ActiveSubscriptionId);
                var offeredSubscription =
                    subscriptionQueries.GetOfferedSubscription(activeSubscriptions.SubscriptionId);
                
                checkInQueries.CreateCheckInHistoryEntry(activeSubscriptions, checkIn, offeredSubscription);

                if (checkIn == new CheckIn())
                    return Results.BadRequest("Unable to find check-in for the active subscription");

                return
                    Results.Ok("Successfully CheckedIn");
            });

        return group;
    }
}