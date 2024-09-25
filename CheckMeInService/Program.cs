using CheckMeInService.Data;
using CheckMeInService.Models;

// Dependency Injection, singleton pattern for intializing the database settings only once 
// Wherever that class is used it will be injected
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton(new DatabaseSettings("appsettings.json"));
builder.Services.AddSingleton<AzureSqlHandler>();
var app = builder.Build();

app.MapGet("/", () => "Tester");

// Todo: add all these paths and any logic to a respective controllers folder

app.MapGet("/AddSubscription",
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
        }

        Subscriber? existingSubscriber = azureSqlHandler.FetchExistingSubscriber(phoneNumber);

        if (existingSubscriber is null)
        {
            return Results.BadRequest("Subscriber not found");
        }

        // Todo: refactor by adding a catch for people adding existing subscriptions
        azureSqlHandler.AddNewMemberSubscription(existingSubscriber, offeredSubscription);

        return Results.Ok("Reached the end... Verify the table to ensure that the data is there.");
    });

app.MapGet("/TestConnection", (AzureSqlHandler azureSqlHandler) =>
{
    try
    {
        return Results.Ok(azureSqlHandler.TestConnection());
    }
    catch (Exception e)
    {
        return Results.BadRequest("Sql connection failed: " + e.Message);
    }
});

app.Run();