using CheckMeInService.Data;
using CheckMeInService.Models;
using CheckMeInService.Subscribers;
using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(args);
DatabaseSettings settings = new DatabaseSettings("appsettings.json");

builder.Services.AddSingleton(settings.GetConnectionString());
builder.Services.AddSingleton<SubscriptionQueries>();
builder.Services.AddSingleton<CheckInQueries>();
var app = builder.Build();

app.MapGet("/", () => "CheckMeIn API is running");

app.MapGroup("/api/subscribers")
    .MapSubscribersApi()
    .WithTags("Controllers Api");

app.Run();