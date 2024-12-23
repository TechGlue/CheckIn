using CheckMeInService.Models;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);
var settings = new DatabaseSettings("appsettings.json");

// builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddSwaggerGen();
// at this point no connection string is set. 
builder.Services.AddDbContext<CheckMeInContext>(options => options.UseSqlServer(settings.GetConnection()));
builder.Services.AddControllers();

builder.Services.AddOpenTelemetry()
    .ConfigureResource(res => res.AddService("CheckIn.Api"))
    .WithMetrics(m =>
    {
        m.AddAspNetCoreInstrumentation().AddHttpClientInstrumentation();
        m.AddOtlpExporter(opt => opt.Endpoint = new Uri("http://localhost:18889"));
    })
    .WithTracing(t =>
    {
        t.AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddEntityFrameworkCoreInstrumentation();
        t.AddOtlpExporter(opt => { opt.Endpoint = new Uri("http://localhost:18889"); });
    });
builder.Logging.AddOpenTelemetry(options =>
{
    options.AddConsoleExporter()
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("CheckIn.Api"));
    options.AddOtlpExporter(x => { x.Endpoint = new Uri("http://localhost:18889"); });
});

var app = builder.Build();

if (settings.TestDatabaseConnection() is false)
{ 
    app.Logger.LogError("Unable to connect to the database. Check configuration or database availability");
    throw new Exception("Unable to connect to the database");
}


app.MapControllers();

app.Run();


app.Logger.LogInformation("CheckMeIn successfully connected to database");
app.Logger.LogInformation("CheckMeIn API started");

public partial class Program
{
}