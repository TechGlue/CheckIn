using CheckMeInService.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CheckMeInServices_Tests;

public class CustomWebApplicationFactory<Program>: WebApplicationFactory<Program>  where Program : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<CheckMeInContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
            
            // create a new service provider
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();
            
            // add a database context using in-memory database for testing
            services.AddDbContext<CheckMeInContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
                options.UseInternalServiceProvider(serviceProvider);
            });
            
            // build the service provider
            var sp = services.BuildServiceProvider();

            // Create a scope to obtain a reference to the database context
            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var appDbContext = scopedServices.GetRequiredService<CheckMeInContext>();
                
                // Ensure the database is created
                try
                {
                    appDbContext.Database.EnsureCreated();
                    SeedData.PopulateTestData(appDbContext);
                }
                catch (Exception ex)
                {
                    throw new Exception("Seed data populate test data failing");
                }
                
            }
        });
    } 
    
    
    
}