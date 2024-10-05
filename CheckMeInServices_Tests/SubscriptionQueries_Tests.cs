using CheckMeInService.Data;
using CheckMeInService.Models;
using Microsoft.EntityFrameworkCore;
using Testcontainers.SqlEdge;

namespace CheckMeInServices_Tests;

public class SubscriptionQueries_Tests : IAsyncLifetime
{
    private readonly SqlEdgeContainer _container;
    private CheckMeInContext _dbContext;
    private SubscriptionQueries _subscriptionQueries;

    public SubscriptionQueries_Tests()
    {
        _container = new SqlEdgeBuilder()
            .WithImage("mcr.microsoft.com/azure-sql-edge:1.0.7")
            .WithExposedPort(new Random().Next(49152, 65535))
            .WithCleanUp(true)
            .Build();
    }

    [Fact]
    public async void IntroTest_AzureSqlHandler_TestSubscribers()
    {
        // Arrange - Act
        var users = await _dbContext.Subscribers.ToListAsync();

        // Assert
        Assert.Equal(2, users.Count);
    }


    [Fact]
    public async void AddNewSubscriber_ValidSubscriber_ReturnsTrue()
    {
        // Arrange
        Subscriber newSubscriber = new Subscriber(Guid.NewGuid(), "AddNewFirst", "AddNewLast", "555-555-3555");

        // Act
        bool output = _subscriptionQueries.AddNewSubscriber(newSubscriber);

        var users = await _dbContext.Subscribers.ToListAsync();

        // Assert
        Assert.True(output);
        Assert.Equal(3, users.Count);
        Assert.True(users.Exists(x => x.FirstName == newSubscriber.FirstName));
    }

    [Fact]
    public void GetSubscription_ValidSubscriptionName_ReturnsSubscription()
    {
        // Arrange
        _dbContext.OfferedSubscriptions.Add(new OfferedSubscriptions(Guid.NewGuid(), "TestSubscription"));
        _dbContext.SaveChanges();

        // Act
        OfferedSubscriptions? validSubscription = _subscriptionQueries.GetSubscription("TestSubscription");

        // Assert
        Assert.NotNull(validSubscription);
        Assert.Equal("TestSubscription", validSubscription.SubscriptionName);
    }

    [Fact]
    public void GetSubscription_InvalidSubscriptionName_ReturnsNullSubscription()
    {
        // Arrange - Act  
        OfferedSubscriptions? invalidSubscription = _subscriptionQueries.GetSubscription("InvalidSubscription");

        // Assert
        Assert.Null(invalidSubscription);
    }

    [Fact]
    public void VerifySubscriberExists_ValidSubscriber_ReturnsTrue()
    {
        // Arrange
        Subscriber newSubscriber = new Subscriber(Guid.NewGuid(), "TesterFirst", "TesterLast", "555-555-1111");

        // Act
        bool output = _subscriptionQueries.VerifySubscriberExists(newSubscriber);

        // Assert
        Assert.True(output);
    }

    [Fact]
    public void VerifySubscriberExists_InvalidSubscriber_ReturnsFalse()
    {
        // Arrange
        Subscriber newSubscriber = new Subscriber(Guid.NewGuid(), "TesterFirst", "TesterLast", "341-535-2345");

        // Act
        bool output = _subscriptionQueries.VerifySubscriberExists(newSubscriber);

        // Assert
        Assert.False(output);
    }

    [Fact]
    public void CheckForExistingSubscription_ValidSubscriberAndSubscription_ReturnsFalse()
    {
        // Arrange
        Subscriber newSubscriber = new Subscriber(Guid.NewGuid(), "TesterFirst", "TesterLast", "341-535-2345");
        OfferedSubscriptions newSubscription = new OfferedSubscriptions(Guid.NewGuid(), "TestSubscription");


        // Act
        _dbContext.OfferedSubscriptions.Add(newSubscription);
        _dbContext.Subscribers.Add(newSubscriber);
        _dbContext.ActiveSubscriptions.Add(new ActiveSubscriptions(Guid.NewGuid(), newSubscriber.SubscriberId,
            newSubscription.SubscriptionId, DateTime.Now, newSubscriber.PhoneNumber));
        _dbContext.SaveChanges();
        bool validSubscription = _subscriptionQueries.CheckForExistingSubscription(newSubscriber, newSubscription);

        // Assert  
        Assert.True(validSubscription);
    }


    [Fact]
    public void CheckForExistingSubscription_InvalidSubscriberAndSubscription_ReturnsFalse()
    {
        // Arrange
        Subscriber newSubscriber =
            new Subscriber(Guid.NewGuid(), "TesterFirstInvalid", "TesterLastInvalid", "555-222-1111");
        OfferedSubscriptions newSubscription = new OfferedSubscriptions(Guid.NewGuid(), "TestSubscriptionInvalid");

        // Act
        bool validSubscription = _subscriptionQueries.CheckForExistingSubscription(newSubscriber, newSubscription);

        // Assert - CheckForExistingSubscription should return true if the subscription is not NULL 
        Assert.False(validSubscription);
    }

    [Fact]
    public void AddNewMemberSubscription_ValidSubscriberAndSubscription_ReturnsTrue()
    {
        // Arrange
        Subscriber newSubscriber = new Subscriber(Guid.NewGuid(), "TestAddNewSubscriptionFirst",
            "TestAddNewSubscriptionLast", "333-555-1111");
        OfferedSubscriptions newSubscription = new OfferedSubscriptions(Guid.NewGuid(), "TestAddNewMemberSubscription");

        // Act
        _dbContext.OfferedSubscriptions.Add(newSubscription);
        _dbContext.Subscribers.Add(newSubscriber);
        _dbContext.SaveChanges();
        bool validSubscription = _subscriptionQueries.AddNewMemberSubscription(newSubscriber, newSubscription);
        ActiveSubscriptions? activeSubscriptions =
            _dbContext.ActiveSubscriptions.FirstOrDefault(x => x.SubscriberId == newSubscriber.SubscriberId);

        // Assert
        Assert.True(validSubscription);
        Assert.NotNull(activeSubscriptions);
    }

    [Fact]
    public void AddNewMemberSubscription_SubscriptionAlreadyExists_ReturnsFalse()
    {
        // Arrange
        Subscriber newSubscriber = new Subscriber(Guid.NewGuid(), "TestAddNewSubscriptionFirst",
            "TestAddNewSubscriptionLastFalse", "111-111-1111");
        OfferedSubscriptions newSubscription =
            new OfferedSubscriptions(Guid.NewGuid(), "TestAddNewMemberSubscriptionFalse");

        // Act
        _dbContext.OfferedSubscriptions.Add(newSubscription);
        _dbContext.Subscribers.Add(newSubscriber);
        _dbContext.SaveChanges();
        _ = _subscriptionQueries.AddNewMemberSubscription(newSubscriber, newSubscription);
        bool existingSubscription = _subscriptionQueries.AddNewMemberSubscription(newSubscriber, newSubscription);

        // Assert
        Assert.False(existingSubscription);
    }

    [Fact]
    public void FetchExistingSubscriber_ExistingPhoneNumber_ReturnsValidSubscriber()
    {
        // Arrange 
        string existingPhoneNumber = "555-555-1111";

        // Act 
        Subscriber? subscriber = _subscriptionQueries.FetchExistingSubscriber(existingPhoneNumber);

        // Assert
        Assert.NotNull(subscriber);
    }

    [Fact]
    public void FetchExistingSubscriber_NonExistingPhoneNumber_ReturnsNullSubscriber()
    {
        // Arrange 
        string nonexistentPhoneNumber = "000-000-0000";

        // Act 
        Subscriber? subscriber = _subscriptionQueries.FetchExistingSubscriber(nonexistentPhoneNumber);

        // Assert
        Assert.Null(subscriber);
    }


    [Fact]
    public void RemoveActiveSubscription_InValidSubscriberAndSubscription_ReturnsFalse()
    {
        // Arrange
        Subscriber newSubscriber = new Subscriber(Guid.NewGuid(), "first", "last", "222-222-2222");
        OfferedSubscriptions newSubscription = new OfferedSubscriptions(Guid.NewGuid(), "RemoveActiveSubscription");

        // Act
        bool output = _subscriptionQueries.RemoveActiveSubscription(newSubscriber, newSubscription);

        // Assert
        Assert.False(output);
    }

    [Fact]
    public void RemoveActiveSubscription_ValidSubscriberAndSubscription_ReturnsTrue()
    {
        // Arrange
        Subscriber newSubscriber = new Subscriber(Guid.NewGuid(), "first", "last", "111-111-1111");
        OfferedSubscriptions newSubscription = new OfferedSubscriptions(Guid.NewGuid(), "RemoveActiveSubscription");
        ActiveSubscriptions activeSubscription = new ActiveSubscriptions(Guid.NewGuid(), newSubscriber.SubscriberId,
            newSubscription.SubscriptionId, DateTime.Now, newSubscriber.PhoneNumber);

        // Act
        _dbContext.Subscribers.Add(newSubscriber);
        _dbContext.OfferedSubscriptions.Add(newSubscription);
        _dbContext.ActiveSubscriptions.Add(activeSubscription);
        _dbContext.SaveChanges();
        bool validSubscription = _subscriptionQueries.RemoveActiveSubscription(newSubscriber, newSubscription);

        // Assert
        Assert.True(validSubscription);
    }

    [Fact]
    // Setup DB container
    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        _dbContext = new CheckMeInContext(_container.GetConnectionString());

        _subscriptionQueries = new SubscriptionQueries(_container.GetConnectionString());

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