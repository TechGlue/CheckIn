using System.ComponentModel;
using System.Data.Common;
using CheckMeInService.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CheckMeInServices_Tests;

using Microsoft.AspNetCore.Mvc.Testing;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionstring;

    public CustomWebApplicationFactory(string connectionstring)
    {
        _connectionstring = connectionstring;
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<CheckMeInContext>));

            services.Remove(dbContextDescriptor);

            var dbConnectionDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbConnection));

            services.Remove(dbConnectionDescriptor);

            // Create and open AzureEdge Connection so EF won't automatically close it. 
            services.AddSingleton<DbConnection>(container =>
            {
                var connection = new SqlConnection(_connectionstring);
                connection.Open();
                return connection;
            });

            services.AddDbContext<CheckMeInContext>((container, options) =>
            {
                var connection = container.GetRequiredService<DbConnection>();
                options.UseSqlServer(connection);
            });
        });
    }
}