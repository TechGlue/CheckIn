using CheckMeInService.Data;
using CheckMeInService.Models;

namespace CheckMeInService.Subscribers;

// Controller responsible for managing the subscribers and their initials subscriptions
public static class SubscribersApi
{
    public static RouteGroupBuilder MapSubscribersApi(this RouteGroupBuilder group)
    {
        group.MapDelete("/RemoveSubscription",
            (AzureSqlHandler azureSqlHandler, string phoneNumber, string subscriptionName) =>
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
            (AzureSqlHandler azureSqlHandler, string firstName, string lastName, string phoneNumber,
                string subscriptionName) =>
            {
                // Add the subscriber first if it does not exist
                Subscriber newSubscriber = new Subscriber
                {
                    FirstName = firstName, LastName = lastName, PhoneNumber = phoneNumber
                };

                OfferedSubscriptions? offeredSubscription = azureSqlHandler.GetSubscription(subscriptionName);

                if (offeredSubscription is null)
                {
                    return Results.BadRequest("Subscription not found");
                }

                if (!azureSqlHandler.VerifySubscriberExists(newSubscriber))
                {
                    // We have the subscriber here
                    azureSqlHandler.AddNewSubscriber(newSubscriber);

                    // Now we can add the subscription 
                    azureSqlHandler.AddNewMemberSubscription(newSubscriber, offeredSubscription);

                    bool res = azureSqlHandler.AddNewMemberSubscription(newSubscriber, offeredSubscription);

                    // return the result for the new subscriber
                    return res
                        ? Results.BadRequest("Subscription failed to add for new subscriber")
                        : Results.Ok("Subscription added successfully for new subscriber");
                }

                Subscriber? existingSubscriber = azureSqlHandler.FetchExistingSubscriber(phoneNumber);

                if (existingSubscriber is null)
                {
                    return Results.BadRequest("Subscriber not found");
                }

                bool response = azureSqlHandler.AddNewMemberSubscription(existingSubscriber, offeredSubscription);

                return response
                        ? Results.Ok("Subscription added successfully")
                        : Results.BadRequest("Subscription failed to add due to existing subscription")
                    ;
            });

        return group;
    }
}