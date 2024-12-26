using CheckMeInService.Models;
using CheckMeInService.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Testcontainers.SqlEdge;

namespace CheckMeInServices_Tests;

public class TestDatabaseFixture:IAsyncLifetime
{
    public SqlEdgeContainer _container { get; private set; }
    private CheckMeInContext _checkInContext;
    
    public TestDatabaseFixture()
    {
        _container = new SqlEdgeBuilder()
            .WithImage("mcr.microsoft.com/azure-sql-edge:1.0.7")
            .WithExposedPort(new Random().Next(41952, 65535))
            .WithCleanUp(true)
            .Build();
    }
    
    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        
        var options = new DbContextOptionsBuilder<CheckMeInContext>()
            .UseSqlServer(_container.GetConnectionString())
            .Options;
        
        _checkInContext = new CheckMeInContext(options);

        await _checkInContext.Database.EnsureCreatedAsync();

        await SeedTestDataSubscribers();
    }

    public Task DisposeAsync()
    {
        return _container.DisposeAsync().AsTask();
    }
    
    private async Task SeedTestDataSubscribers()
    {
        var offeredSubscriptions = new OfferedSubscriptions
        {
            SubscriptionId = new Guid("5fb7097c-335c-4d07-b4fd-000004e2d28c"),
            SubscriptionName = "TestContainers"
        };
        var activeSubscriber = new Subscriber
        {
            FirstName = "TesterFirst",
            LastName = "TesterLast",
            PhoneNumber = "555-555-1111"
        };
        var subscriberOne = new Subscriber
        {
            FirstName = "TesterFirst",
            LastName = "TesterLast",
            PhoneNumber = "555-555-1111"
        };
        var subscriberTwo = new Subscriber
        {
            FirstName = "SubscriberFirst",
            LastName = "SubscriberLast",
            PhoneNumber = "333-555-1111"
        };

        _checkInContext.Subscribers.Add(subscriberOne);
        _checkInContext.Subscribers.Add(subscriberTwo);
        _checkInContext.OfferedSubscriptions.Add(offeredSubscriptions);
        _checkInContext.ActiveSubscriptions.Add(new ActiveSubscriptions(Guid.NewGuid(), activeSubscriber.SubscriberId,
            offeredSubscriptions.SubscriptionId, DateTime.Now, "555-555-1111")
        {
            PhoneNumber = "555-555-1111"
        });
        await _checkInContext.SaveChangesAsync();
    }
}