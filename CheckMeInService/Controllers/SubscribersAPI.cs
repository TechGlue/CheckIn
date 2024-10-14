using CheckMeInService.Data;
using CheckMeInService.Models;

namespace CheckMeInService.Subscribers;

// Controller responsible for managing the subscribers and their initials subscriptions
public static class SubscribersAPI
{
    public static RouteGroupBuilder MapSubscribersApi(this RouteGroupBuilder group)
    {
        group.MapDelete("/RemoveSubscription",
            (SubscriptionQueries azureSqlHandler, string phoneNumber, string subscriptionName) =>
            {
                var subscriber = azureSqlHandler.FetchExistingSubscriber(phoneNumber);

                if (subscriber is null) return Results.BadRequest("Subscriber not found");

                var subscription = azureSqlHandler.GetSubscription(subscriptionName);

                if (subscription is null) return Results.BadRequest("Subscription not found");

                var response = azureSqlHandler.RemoveActiveSubscription(subscriber, subscription);

                return response
                    ? Results.Ok("Subscription removed successfully")
                    : Results.BadRequest("Failed to remove subscription");
            });

        group.MapPost("/AddSubscription",
            (SubscriptionQueries subscriptionQueries, CheckInQueries checkInQueries, string firstName, string lastName,
                string phoneNumber,
                string subscriptionName) =>
            {
                // Add the subscriber first if it does not exist
                var newSubscriber = new Subscriber
                {
                    FirstName = firstName, LastName = lastName, PhoneNumber = phoneNumber
                };

                var offeredSubscription = subscriptionQueries.GetSubscription(subscriptionName);

                if (offeredSubscription is null) return Results.BadRequest("Subscription not found");

                if (!subscriptionQueries.VerifySubscriberExists(newSubscriber))
                {
                    // We have the subscriber here
                    subscriptionQueries.AddNewSubscriber(newSubscriber);

                    // Now we can add the subscription 
                    subscriptionQueries.AddNewMemberSubscription(newSubscriber, offeredSubscription);

                    var res = subscriptionQueries.AddNewMemberSubscription(newSubscriber, offeredSubscription);

                    var userSubscription = subscriptionQueries.GetActiveSubscriptions(phoneNumber);

                    if (userSubscription is null) return Results.BadRequest("No active user subscription found.");

                    var checkIn =
                        checkInQueries.CreateNewCheckIn(
                            userSubscription, 24);

                    // return the result for the new subscriber
                    return res && checkIn
                        ? Results.BadRequest("Subscription failed to add for new subscriber")
                        : Results.Ok("Subscription added successfully for new subscriber");
                }

                var existingSubscriber = subscriptionQueries.FetchExistingSubscriber(phoneNumber);

                if (existingSubscriber is null) return Results.BadRequest("Subscriber not found");

                var newMemberResponse =
                    subscriptionQueries.AddNewMemberSubscription(existingSubscriber, offeredSubscription);

                var activeSubscription = subscriptionQueries.GetActiveSubscriptions(phoneNumber);

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