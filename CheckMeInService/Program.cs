using CheckMeInService.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var settings = new DatabaseSettings("appsettings.json");

// builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddSwaggerGen();
// at this point no connection string is set. 
builder.Services.AddDbContext<CheckMeInContext>(options => options.UseSqlServer(settings.GetConnection()));
builder.Services.AddControllers();

var app = builder.Build();

if (settings.TestDatabaseConnection() is false) throw new Exception("Unable to connect to the database");

app.MapControllers();
app.Run();

public partial class Program
{
}