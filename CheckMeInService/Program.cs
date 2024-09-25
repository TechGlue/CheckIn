using CheckMeInService.Controllers;
using CheckMeInService.Models;

// Dependency Injection, singleton pattern for intializing the database settings only once 
// Wherever that class is used it will be injected
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton(new DatabaseSettings("appsettings.json"));
builder.Services.AddSingleton<AzureSqlHandler>();
var app = builder.Build();

app.MapGet("/", () => "Tester");


// this should be the go to endpoints for everything 
app.MapGet("/AddSubscription",
    (AzureSqlHandler azureSqlHandler, string firstName, string lastName, string phoneNumber) =>
    {
        // Add the subscriber first if it does not exist
        Subscriber newSubscriber = new Subscriber
        {
             FirstName = firstName, LastName = lastName, PhoneNumber = phoneNumber
        };

        // if member does not exist add the subscriber first 
        if (!azureSqlHandler.VerifySubscriberExists(newSubscriber))
        {
            azureSqlHandler.AddNewSubscriber(newSubscriber);
            
            
            // Then add the rest of the logic for creating a new subscription here 
        }
        
       // Todo: add logic for adding a subscription, need to add the dbContext for the other models 
        
        return Results.Ok("Check on the console output");
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