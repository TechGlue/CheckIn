using CheckMeInService.Models;
using Microsoft.EntityFrameworkCore;
using Testcontainers.SqlEdge;

namespace CheckMeInServices_Tests;

public class AzureSqlHandler_Tests : IAsyncLifetime
{
    private readonly SqlEdgeContainer _container;
    private SubscribersContext _dbContext;

    public AzureSqlHandler_Tests()
    {
        _container = new SqlEdgeBuilder()
            .WithImage("mcr.microsoft.com/azure-sql-edge:1.0.7")
            .WithExposedPort(1433)
            .WithCleanUp(true)
            .Build();
    }

    [Fact]
    public async void TestAzureSqlHandler_TestSubscribers()
    {
        var users = await _dbContext.Subscribers.ToListAsync();
        Assert.Equal(2, users.Count);
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        _dbContext = new SubscribersContext(_container.GetConnectionString());

        await _dbContext.Database.EnsureCreatedAsync();

        await SeedTestDataSubscribers();
    }

    private async Task SeedTestDataSubscribers()
    {
        _dbContext.Subscribers.Add(new Subscriber(Guid.NewGuid(), "TesterFirst", "TesterLast", "555-555-1111"));
        _dbContext.Subscribers.Add(new Subscriber(Guid.NewGuid(), "SubscriberFirst", "SubscriberLast", "333-555-1111"));
        await _dbContext.SaveChangesAsync();
    }

    public Task DisposeAsync() =>
        _container.DisposeAsync().AsTask();
}