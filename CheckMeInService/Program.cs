using CheckMeInService.Data;
using CheckMeInService.Models;
using CheckMeInService.Subscribers;

var builder = WebApplication.CreateBuilder(args);
DatabaseSettings settings = new DatabaseSettings("appsettings.json");

builder.Services.AddSingleton(settings.GetConnectionString());
builder.Services.AddSingleton<AzureSqlHandler>();
var app = builder.Build();

app.MapGet("/", () => "Subscribers API is running");

app.MapGroup("/api/subscribers")
    .MapSubscribersApi()
    .WithTags("Subscribers Api");

app.Run();