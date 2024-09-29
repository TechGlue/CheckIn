using CheckMeInService.Data;
using CheckMeInService.Models;

namespace CheckMeInService.Subscribers;

public static class SubscribersApi
{
    public static RouteGroupBuilder MapSubscribersApi(this RouteGroupBuilder group)
    {
        group.MapGet("/AddSubscription",
            (AzureSqlHandler azureSqlHandler, string firstName, string lastName, string phoneNumber) =>
            {
                // Add the subscriber first if it does not exist
                Subscriber newSubscriber = new Subscriber
                {
                    FirstName = firstName, LastName = lastName, PhoneNumber = phoneNumber
                };

                OfferedSubscriptions? offeredSubscription = azureSqlHandler.GetSubscription("Exercise");

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
                    ? Results.BadRequest("Subscription failed to add due to existing subscription")
                    : Results.Ok("Subscription added successfully");
            });

        return group;
    }
}