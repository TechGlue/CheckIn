// Creating a minimal api to send daily notis 
// First figure out how to subsribe to a route


// Tomorrows Sep 17, setup the db layer for the subscriber object


var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapGet("/tester", () => "Tester");

app.Run();