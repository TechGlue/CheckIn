using CheckMeInService.Data;
using CheckMeInService.Models;
using Microsoft.EntityFrameworkCore;
using Testcontainers.SqlEdge;

namespace CheckMeInServices_Tests;

public class AzureSqlHandler_Tests : IAsyncLifetime
{
    private readonly SqlEdgeContainer _container;
    private SubscribersContext _dbContext;
    private AzureSqlHandler azureSqlHandler;

    public AzureSqlHandler_Tests()
    {
        _container = new SqlEdgeBuilder()
            .WithImage("mcr.microsoft.com/azure-sql-edge:1.0.7")
            .WithExposedPort(1433)
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
        bool output = azureSqlHandler.AddNewSubscriber(newSubscriber);

        var users = await _dbContext.Subscribers.ToListAsync();

        // Assert
        Assert.True(output);
        Assert.Equal(3, users.Count);
        Assert.True(users.Exists(x => x.FirstName == newSubscriber.FirstName));
    }

    [Fact]
    public void AddNewSubscriber_InvalidConnection_ReturnsFalse()
    {
        // Arrange
        AzureSqlHandler failingConnection = new AzureSqlHandler("BadConnection");
        Subscriber newSubscriber = new Subscriber(Guid.NewGuid(), "AddNewFirst", "AddNewLast", "555-555-3555");

        // Act
        bool output = failingConnection.AddNewSubscriber(newSubscriber);

        // Assert
        Assert.False(output);
    }

    [Fact]
    public void GetSubscription_ValidSubscriptionName_ReturnsSubscription()
    {
        // Arrange
        _dbContext.OfferedSubscriptions.Add(new OfferedSubscriptions(Guid.NewGuid(), "TestSubscription"));
        _dbContext.SaveChanges();

        // Act
        OfferedSubscriptions? validSubscription = azureSqlHandler.GetSubscription("TestSubscription");
        
        // Assert
        Assert.NotNull(validSubscription);
        Assert.Equal("TestSubscription", validSubscription.SubscriptionName);
    }

    [Fact]
    public void GetSubscription_InvalidSubscriptionName_ReturnsNullSubscription()
    {
        // Arrange - Act  
       OfferedSubscriptions? invalidSubscription = azureSqlHandler.GetSubscription("InvalidSubscription"); 
        
        // Assert
        Assert.Null(invalidSubscription);
    }

    [Fact]
    public void VerifySubscriberExists_ValidSubscriber_ReturnsTrue()
    {
        // Arrange
        Subscriber newSubscriber = new Subscriber(Guid.NewGuid(), "TesterFirst", "TesterLast", "555-555-1111");
        
        // Act
        bool output = azureSqlHandler.VerifySubscriberExists(newSubscriber);
        
        // Assert
        Assert.True(output);
    }
    
    [Fact]
    public void VerifySubscriberExists_InvalidSubscriber_ReturnsFalse()
    {
        // Arrange
        Subscriber newSubscriber = new Subscriber(Guid.NewGuid(), "TesterFirst", "TesterLast", "341-535-2345");
        
        // Act
        bool output = azureSqlHandler.VerifySubscriberExists(newSubscriber);
        
        // Assert
        Assert.False(output);
    }

    [Fact]
    public void CheckForExistingSubscription_ValidSubscriberAndSubscription_ReturnsFalse()
    {
        // Arrange
        Subscriber newSubscriber = new Subscriber(Guid.NewGuid(), "TesterFirst", "TesterLast", "341-535-2345");
        OfferedSubscriptions newSubscription = new OfferedSubscriptions(Guid.NewGuid(), "TestSubscription");
        
        _dbContext.OfferedSubscriptions.Add(newSubscription);
        _dbContext.Subscribers.Add(newSubscriber);
        _dbContext.ActiveSubscriptions.Add(new ActiveSubscriptions(Guid.NewGuid(), newSubscriber.SubscriberId, newSubscription.SubscriptionId, DateTime.Now, newSubscriber.PhoneNumber));
        _dbContext.SaveChanges();
        
        // Act
        bool validSubscription = azureSqlHandler.CheckForExistingSubscription(newSubscriber, newSubscription);

        // Assert  
        Assert.True(validSubscription);
    }

    [Fact]
    public void CheckForExistingSubscription_InvalidSubscriberAndSubscription_ReturnsFalse()
    {
        // Arrange
        Subscriber newSubscriber = new Subscriber(Guid.NewGuid(), "TesterFirstInvalid", "TesterLastInvalid", "555-222-1111");
        OfferedSubscriptions newSubscription = new OfferedSubscriptions(Guid.NewGuid(), "TestSubscriptionInvalid");
        
        // Act
        bool validSubscription = azureSqlHandler.CheckForExistingSubscription(newSubscriber, newSubscription);

        // Assert - CheckForExistingSubscription should return true if the subscription is not NULL 
        Assert.False(validSubscription);
    }

    [Fact]
    public void AddNewMemberSubscription_ValidSubscriberAndSubscription_ReturnsTrue()
    {
        // Arrange
        Subscriber newSubscriber = new Subscriber(Guid.NewGuid(), "TestAddNewSubscriptionFirst", "TestAddNewSubscriptionLast", "333-555-1111");
        OfferedSubscriptions newSubscription = new OfferedSubscriptions(Guid.NewGuid(), "TestAddNewMemberSubscription");
        
        _dbContext.OfferedSubscriptions.Add(newSubscription);
        _dbContext.Subscribers.Add(newSubscriber);
        
        // Act
        bool validSubscription = azureSqlHandler.AddNewMemberSubscription(newSubscriber, newSubscription);
        ActiveSubscriptions? activeSubscriptions = _dbContext.ActiveSubscriptions.FirstOrDefault(x => x.SubscriberId == newSubscriber.SubscriberId);
        
        // Assert
        Assert.True(validSubscription);
        Assert.NotNull(activeSubscriptions);
    }
    
    [Fact]
    public void AddNewMemberSubscription_SubscriptionAlreadyExists_ReturnsFalse()
    {
        // Arrange
        Subscriber newSubscriber = new Subscriber(Guid.NewGuid(), "TestAddNewSubscriptionFirst", "TestAddNewSubscriptionLastFalse", "111-111-1111");
        OfferedSubscriptions newSubscription = new OfferedSubscriptions(Guid.NewGuid(), "TestAddNewMemberSubscriptionFalse");
        
        _dbContext.OfferedSubscriptions.Add(newSubscription);
        _dbContext.Subscribers.Add(newSubscriber);
        
        // Act
        _ = azureSqlHandler.AddNewMemberSubscription(newSubscriber, newSubscription);
        bool existingSubscription = azureSqlHandler.AddNewMemberSubscription(newSubscriber, newSubscription);
        
        // Assert
        Assert.False(existingSubscription);
    }

    [Fact]
    public void FetchExistingSubscriber_ExistingPhoneNumber_ReturnsValidSubscriber()
    {
        // Arrange 
        string existingPhoneNumber = "555-555-1111";

        // Act 
        Subscriber? subscriber = azureSqlHandler.FetchExistingSubscriber(existingPhoneNumber);

        // Assert
        Assert.NotNull(subscriber);
    }
    
    [Fact]
    public void FetchExistingSubscriber_NonExistingPhoneNumber_ReturnsNullSubscriber()
    {
        // Arrange 
        string nonexistentPhoneNumber = "000-000-0000";

        // Act 
        Subscriber? subscriber = azureSqlHandler.FetchExistingSubscriber(nonexistentPhoneNumber);

        // Assert
        Assert.Null(subscriber);
    }

    // Setup DB container
    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        _dbContext = new SubscribersContext(_container.GetConnectionString());

        azureSqlHandler = new AzureSqlHandler(_container.GetConnectionString());

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