// Creating a minimal api to send daily notis 
// First figure out how to subsribe to a route
using CheckMeInService.Controllers;
using CheckMeInService.Models;


// Dependency Injection, singleton pattern for intializing the database settings only once 
// Wherever that class is used it will be injected
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton(new DatabaseSettings("appsettings.json"));
builder.Services.AddSingleton<AzureSqlHandler>();
var app = builder.Build();

// end points
app.MapGet("/", () => "Tester");

app.MapGet("/DE", (AzureSqlHandler azureSqlHandler) =>
{
    azureSqlHandler.AddNewSubscription("Excercise");
    return Results.Ok("Check on the console output");
});

app.Run();