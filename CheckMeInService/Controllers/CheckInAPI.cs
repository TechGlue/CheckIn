using CheckMeInService.Data;
using CheckMeInService.Models;

namespace CheckMeInService.CheckIns;

public static class CheckInApi
{
    public static RouteGroupBuilder MapCheckInApi(this RouteGroupBuilder group)
    {
        // Un-Enroll Active Subscription 
        group.MapDelete("/Unenroll", (SubscriptionQueries subscriptionQueries, CheckInQueries checkInQueries, string phoneNumber) =>
        {
           // First identify the active subscription 
           ActiveSubscriptions? activeSubscriptions = subscriptionQueries.GetActiveSubscriptions(phoneNumber);

           if (activeSubscriptions is null)
           {
               return Results.BadRequest($"Subscription not found for user with phone number: {phoneNumber}");
           }
           
           // remove check-in for the associated active subscription
           _ = checkInQueries.RemoveCheckIn(activeSubscriptions);
           
           // Now we can remove the active subscription
           bool deletionOutput = subscriptionQueries.RemoveActiveSubscription(activeSubscriptions);

           return deletionOutput ? Results.Ok("Successfully, deleted the active subscription") : Results.BadRequest("Unable to un-enroll active subscription") ;
        });
        
        return group;
    }
}