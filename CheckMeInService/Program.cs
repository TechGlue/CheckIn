using CheckMeInService.CheckIns;
using CheckMeInService.Data;
using CheckMeInService.Models;
using CheckMeInService.Subscribers;

var builder = WebApplication.CreateBuilder(args);
DatabaseSettings settings = new DatabaseSettings("appsettings.json");

builder.Services.AddSingleton(settings.GetConnectionString());
builder.Services.AddSingleton<SubscriptionQueries>();
builder.Services.AddSingleton<CheckInQueries>();
var app = builder.Build();

app.MapGroup("/api/subscribers")
    .MapSubscribersApi()
    .WithTags("Controllers Api");

app.MapGroup("/api/checkins")
    .MapCheckInApi()
    .WithTags("CheckIn");

app.Run();