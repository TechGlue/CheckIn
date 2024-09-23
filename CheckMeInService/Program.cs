using CheckMeInService.Controllers;
using CheckMeInService.Models;
// Dependency Injection, singleton pattern for intializing the database settings only once 
// Wherever that class is used it will be injected
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton(new DatabaseSettings("appsettings.json"));
builder.Services.AddSingleton<AzureSqlHandler>();
var app = builder.Build();

app.MapGet("/", () => "Tester");
app.MapGet("/TestEntity", (AzureSqlHandler azureSqlHandler) =>
{
    azureSqlHandler.AddNewSubscriber(null, null);
});

app.MapGet("/AddSubscription", (AzureSqlHandler azureSqlHandler, string firstName, string LastName, string phoneNumber) =>
{
    // Grab the subscriber somewhere here using the phone number
    
    
    azureSqlHandler.AddNewSubscription("Excercise", null);
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