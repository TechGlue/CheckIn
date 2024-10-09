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
                // Fetch Subscriber
                Subscriber? subscriber = azureSqlHandler.FetchExistingSubscriber(phoneNumber);

                if (subscriber is null)
                {
                    return Results.BadRequest("Subscriber not found");
                }

                // Fetch Subscription
                OfferedSubscriptions? subscription = azureSqlHandler.GetSubscription(subscriptionName);

                if (subscription is null)
                {
                    return Results.BadRequest("Subscription not found");
                }

                // Remove Subscription
                bool response = azureSqlHandler.RemoveActiveSubscription(subscriber, subscription);

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
                Subscriber newSubscriber = new Subscriber
                {
                    FirstName = firstName, LastName = lastName, PhoneNumber = phoneNumber
                };

                OfferedSubscriptions? offeredSubscription = subscriptionQueries.GetSubscription(subscriptionName);

                if (offeredSubscription is null)
                {
                    return Results.BadRequest("Subscription not found");
                }

                if (!subscriptionQueries.VerifySubscriberExists(newSubscriber))
                {
                    // We have the subscriber here
                    subscriptionQueries.AddNewSubscriber(newSubscriber);

                    // Now we can add the subscription 
                    subscriptionQueries.AddNewMemberSubscription(newSubscriber, offeredSubscription);

                    bool res = subscriptionQueries.AddNewMemberSubscription(newSubscriber, offeredSubscription);
                    bool checkIn =
                        checkInQueries.CreateNewCheckIn(subscriptionQueries.GetActiveSubscriptions(phoneNumber), 24);

                    // return the result for the new subscriber
                    return res && checkIn
                        ? Results.BadRequest("Subscription failed to add for new subscriber")
                        : Results.Ok("Subscription added successfully for new subscriber");
                }

                Subscriber? existingSubscriber = subscriptionQueries.FetchExistingSubscriber(phoneNumber);

                if (existingSubscriber is null)
                {
                    return Results.BadRequest("Subscriber not found");
                }

                bool newMemberResponse =
                    subscriptionQueries.AddNewMemberSubscription(existingSubscriber, offeredSubscription);

                // create a user check-in entry to track  
                bool newCheckInResponse =
                    checkInQueries.CreateNewCheckIn(subscriptionQueries.GetActiveSubscriptions(phoneNumber), 24);

                return newMemberResponse && newCheckInResponse
                        ? Results.Ok("Subscription added successfully")
                        : Results.BadRequest("Subscription failed to add due to existing subscription")
                    ;
            });

        return group;
    }
}