using CheckMeInService.CheckIns;
using CheckMeInService.Data;
using CheckMeInService.Models;
using CheckMeInService.Subscribers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);
var settings = new DatabaseSettings("appsettings.json");

builder.Services.AddSingleton(settings.GetConnection());
builder.Services.AddSingleton<SubscriptionQueries>();
builder.Services.AddSingleton<CheckInQueries>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("Entra"));

var app = builder.Build();

if (settings.TestDatabaseConnection() is false) throw new Exception("Unable to connect to the database");

app.UseAuthorization();

app.MapGet("/", () => "CheckMeIn API is running").RequireAuthorization();

app.MapGroup("/api/subscribers")
    .MapSubscribersApi()
    .RequireAuthorization()
    .RequireScope("Subscribers")
    .WithTags("Subscribers");
    

app.MapGroup("/api/checkins")
    .MapCheckInApi()
    .RequireAuthorization()
    .RequireScope("CheckIns")
    .WithTags("CheckIn");

app.Run();