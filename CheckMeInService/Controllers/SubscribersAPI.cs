using CheckMeInService.Data;
using CheckMeInService.Models;

namespace CheckMeInService.Subscribers;

public static class SubscribersApi
{
    public static RouteGroupBuilder MapSubscribersApi(this RouteGroupBuilder group)
    {
        group.MapGet("/test",
            () => Results.Ok("API is working"));
        
        group.MapPost("/addsubscription",
            (SubscriptionQueries subscriptionQueries, CheckInQueries checkInQueries, string firstName, string lastName,
                string phoneNumber,
                string subscriptionName) =>
            {
                var newSubscriber = new Subscriber
                {
                    FirstName = firstName, LastName = lastName, PhoneNumber = phoneNumber
                };

                var offeredSubscription = subscriptionQueries.GetSubscription(subscriptionName);

                if (offeredSubscription is null) return Results.BadRequest("Subscription not found");

                if (!subscriptionQueries.VerifySubscriberExists(newSubscriber))
                {
                    subscriptionQueries.AddNewSubscriber(newSubscriber);

                    subscriptionQueries.AddNewMemberSubscription(newSubscriber, offeredSubscription);

                    var res = subscriptionQueries.AddNewMemberSubscription(newSubscriber, offeredSubscription);

                    var userSubscription = subscriptionQueries.GetActiveSubscriptions(phoneNumber, subscriptionName);

                    if (userSubscription is null) return Results.BadRequest("No active user subscription found.");

                    var checkIn =
                        checkInQueries.CreateNewCheckIn(
                            userSubscription, 24);

                    return res && checkIn
                        ? Results.BadRequest("Subscription failed to add for new subscriber")
                        : Results.Ok("Subscription added successfully for new subscriber");
                }

                var existingSubscriber = subscriptionQueries.FetchExistingSubscriber(phoneNumber);

                if (existingSubscriber is null) return Results.BadRequest("Subscriber not found");

                var newMemberResponse =
                    subscriptionQueries.AddNewMemberSubscription(existingSubscriber, offeredSubscription);

                var activeSubscription = subscriptionQueries.GetActiveSubscriptions(phoneNumber, subscriptionName);

                if (activeSubscription is null) return Results.BadRequest("Active subscription not found");

                // create a user check-in entry to track  
                var newCheckInResponse =
                    checkInQueries.CreateNewCheckIn(activeSubscription, 24);

                return newMemberResponse && newCheckInResponse
                        ? Results.Ok("Subscription added successfully")
                        : Results.BadRequest("Subscription failed to add due to existing subscription")
                    ;
            });

        return group;
    }
}