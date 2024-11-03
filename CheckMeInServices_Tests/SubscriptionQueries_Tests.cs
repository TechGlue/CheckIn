using CheckMeInService.Data;
using CheckMeInService.Models;
using Microsoft.EntityFrameworkCore;
using Testcontainers.SqlEdge;

namespace CheckMeInServices_Tests;

public class SubscriptionQueriesTests : IAsyncLifetime
{
    private readonly SqlEdgeContainer _container;
    private CheckMeInContext _dbContext;
    private SubscriptionQueries _subscriptionQueries;

    public SubscriptionQueriesTests()
    {
        _container = new SqlEdgeBuilder()
            .WithImage("mcr.microsoft.com/azure-sql-edge:1.0.7")
            .WithExposedPort(new Random().Next(49152, 65535))
            .WithCleanUp(true)
            .Build();
    }

    // Setup DB container
    [Fact]
    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        _dbContext = new CheckMeInContext(_container.GetConnectionString());

        _subscriptionQueries = new SubscriptionQueries(_container.GetConnectionString());

        await _dbContext.Database.EnsureCreatedAsync();

        await SeedTestDataSubscribers();
    }

    public Task DisposeAsync()
    {
        return _container.DisposeAsync().AsTask();
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
        var newSubscriber = new Subscriber
        {
            FirstName = "AddNewFirst",
            LastName = "AddNewLast",
            PhoneNumber = "555-555-3555"
        };

        // Act
        var output = _subscriptionQueries.AddNewSubscriber(newSubscriber);

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
        var newSubscription = new OfferedSubscriptions
        {
            SubscriptionName = "TestSubscription"
        };
        _dbContext.OfferedSubscriptions.Add(newSubscription);
        _dbContext.SaveChanges();

        // Act
        var validSubscription = _subscriptionQueries.GetSubscription("TestSubscription");

        // Assert
        Assert.NotNull(validSubscription);
        Assert.Equal("TestSubscription", validSubscription.SubscriptionName);
    }

    [Fact]
    public void GetSubscription_InvalidSubscriptionName_ReturnsNullSubscription()
    {
        // Arrange - Act  
        var invalidSubscription = _subscriptionQueries.GetSubscription("InvalidSubscription");

        // Assert
        Assert.Null(invalidSubscription);
    }

    [Fact]
    public void VerifySubscriberExists_ValidSubscriber_ReturnsTrue()
    {
        // Arrange
        var newSubscriber = new Subscriber
        {
            FirstName = "TesterFirst",
            LastName = "TesterLast",
            PhoneNumber = "555-555-1111"
        };

        // Act
        var output = _subscriptionQueries.VerifySubscriberExists(newSubscriber);

        // Assert
        Assert.True(output);
    }

    [Fact]
    public void VerifySubscriberExists_InvalidSubscriber_ReturnsFalse()
    {
        // Arrange
        var newSubscriber = new Subscriber
        {
            FirstName = "TesterFirst",
            LastName = "TesterLast",
            PhoneNumber = "341-535-2345"
        };

        // Act
        var output = _subscriptionQueries.VerifySubscriberExists(newSubscriber);

        // Assert
        Assert.False(output);
    }

    [Fact]
    public void CheckForExistingSubscription_ValidSubscriberAndSubscription_ReturnsFalse()
    {
        // Arrange
        var newSubscriber = new Subscriber
        {
            FirstName = "TesterFirst",
            LastName = "TesterLast",
            PhoneNumber = "555-555-1111"
        };
        var newSubscription = new OfferedSubscriptions
        {
            SubscriptionId = Guid.NewGuid(), SubscriptionName = "TestSubscription"
        };

        // Act
        _dbContext.OfferedSubscriptions.Add(newSubscription);
        _dbContext.Subscribers.Add(newSubscriber);
        _dbContext.ActiveSubscriptions.Add(new ActiveSubscriptions(Guid.NewGuid(), newSubscriber.SubscriberId,
            newSubscription.SubscriptionId, DateTime.Now, newSubscriber.PhoneNumber)
        {
            PhoneNumber = newSubscriber.PhoneNumber
        });
        _dbContext.SaveChanges();
        var validSubscription = _subscriptionQueries.CheckForExistingSubscription(newSubscriber, newSubscription);

        // Assert  
        Assert.True(validSubscription);
    }


    [Fact]
    public void CheckForExistingSubscription_InvalidSubscriberAndSubscription_ReturnsFalse()
    {
        // Arrange
        var newSubscriber =
            new Subscriber
            {
                FirstName = "TesterFirstInvalid",
                LastName = "TesterLastInvalid",
                PhoneNumber = "555-222-1111"
            };
        var newSubscription = new OfferedSubscriptions
        {
            SubscriptionName = "TestSubscriptionInvalid"
        };

        // Act
        var validSubscription = _subscriptionQueries.CheckForExistingSubscription(newSubscriber, newSubscription);

        // Assert - CheckForExistingSubscription should return true if the subscription is not NULL 
        Assert.False(validSubscription);
    }

    [Fact]
    public void AddNewMemberSubscription_ValidSubscriberAndSubscription_ReturnsTrue()
    {
        // Arrange
        var newSubscriber = new Subscriber
        {
            FirstName = "TestAddNewSubscriptionFirst",
            LastName = "TestAddNewSubscriptionLast",
            PhoneNumber = "333-555-1111"
        };
        var newSubscription = new OfferedSubscriptions
        {
            SubscriptionName = "TestAddNewMemberSubscription"
        };

        // Act
        _dbContext.OfferedSubscriptions.Add(newSubscription);
        _dbContext.Subscribers.Add(newSubscriber);
        _dbContext.SaveChanges();
        var validSubscription = _subscriptionQueries.AddNewMemberSubscription(newSubscriber, newSubscription);
        var activeSubscriptions =
            _dbContext.ActiveSubscriptions.SingleOrDefault(x => x.SubscriberId == newSubscriber.SubscriberId);

        // Assert
        Assert.True(validSubscription);
        Assert.NotNull(activeSubscriptions);
    }

    [Fact]
    public void AddNewMemberSubscription_SubscriptionAlreadyExists_ReturnsFalse()
    {
        // Arrange
        var newSubscriber = new Subscriber
        {
            FirstName = "TestAddNewSubscriptionFirst",
            LastName = "TestAddNewSubscriptionLastFalse",
            PhoneNumber = "111-111-1111"
        };
        var newSubscription =
            new OfferedSubscriptions
            {
                SubscriptionName = "TestAddNewMemberSubscriptionFalse"
            };

        // Act
        _dbContext.OfferedSubscriptions.Add(newSubscription);
        _dbContext.Subscribers.Add(newSubscriber);
        _dbContext.SaveChanges();
        _ = _subscriptionQueries.AddNewMemberSubscription(newSubscriber, newSubscription);
        var existingSubscription = _subscriptionQueries.AddNewMemberSubscription(newSubscriber, newSubscription);

        // Assert
        Assert.False(existingSubscription);
    }

    [Fact]
    public void FetchExistingSubscriber_ExistingPhoneNumber_ReturnsValidSubscriber()
    {
        // Arrange 
        var existingPhoneNumber = "555-555-1111";

        // Act 
        var subscriber = _subscriptionQueries.FetchExistingSubscriber(existingPhoneNumber);

        // Assert
        Assert.NotNull(subscriber);
    }

    [Fact]
    public void FetchExistingSubscriber_NonExistingPhoneNumber_ReturnsNullSubscriber()
    {
        // Arrange 
        var nonexistentPhoneNumber = "000-000-0000";

        // Act 
        var subscriber = _subscriptionQueries.FetchExistingSubscriber(nonexistentPhoneNumber);

        // Assert
        Assert.Null(subscriber);
    }


    [Fact]
    public void RemoveActiveSubscription_InValidSubscriberAndSubscription_ReturnsFalse()
    {
        // Arrange
        var newSubscriber = new Subscriber
        {
            FirstName = "first",
            LastName = "last",
            PhoneNumber = "222-222-2222"
        };
        var newSubscription = new OfferedSubscriptions
        {
            SubscriptionName = "RemoveActiveSubscription"
        };

        // Act
        var output = _subscriptionQueries.RemoveActiveSubscription(newSubscriber, newSubscription);

        // Assert
        Assert.False(output);
    }

    [Fact]
    public void RemoveActiveSubscription_ValidSubscriberAndSubscription_ReturnsTrue()
    {
        // Arrange
        var newSubscriber = new Subscriber
        {
            FirstName = "first",
            LastName = "last",
            PhoneNumber = "111-111-1111"
        };
        var newSubscription = new OfferedSubscriptions
        {
            SubscriptionName = "RemoveActiveSubscription"
        };
        var activeSubscription = new ActiveSubscriptions(Guid.NewGuid(), newSubscriber.SubscriberId,
            newSubscription.SubscriptionId, DateTime.Now, newSubscriber.PhoneNumber)
        {
            PhoneNumber = newSubscriber.PhoneNumber
        };

        // Act
        _dbContext.Subscribers.Add(newSubscriber);
        _dbContext.OfferedSubscriptions.Add(newSubscription);
        _dbContext.ActiveSubscriptions.Add(activeSubscription);
        _dbContext.SaveChanges();
        var validSubscription = _subscriptionQueries.RemoveActiveSubscription(newSubscriber, newSubscription);

        // Assert
        Assert.True(validSubscription);
    }

    [Fact]
    public void GetActiveSubscription_ValidPhoneNumber_ReturnsActiveSubscription()
    {
        // Arrange - Act
        var activeSubscriptions = _subscriptionQueries.GetActiveSubscriptions("555-555-1111", "TestContainers");

        // Assert
        Assert.NotNull(activeSubscriptions);
        Assert.Equal("555-555-1111", activeSubscriptions.PhoneNumber);
    }

    [Fact]
    public void GetActiveSubscription_InvalidPhoneNumber_ReturnsNull()
    {
        // Arrange - Act
        var activeSubscriptions = _subscriptionQueries.GetActiveSubscriptions("NotAPhoneNumber", "Not a subscription");

        // Assert
        Assert.Null(activeSubscriptions);
    }

    [Fact]
    public void RemoveActiveSubscription_ActiveSubscription_ReturnsTrue()
    {
        // Arrange
        var newSubscriber = new Subscriber
        {
            FirstName = "TestAddNewSubscriptionFirst",
            LastName = "TestAddNewSubscriptionLast",
            PhoneNumber = "333-555-1111"
        };
        var newSubscription = new OfferedSubscriptions
        {
            SubscriptionName = "TestAddNewMemberSubscription"
        };

        var dummySubscription =
            new ActiveSubscriptions(Guid.NewGuid(), newSubscriber.SubscriberId, newSubscription.SubscriptionId,
                DateTime.Now, newSubscriber.PhoneNumber)
            {
                PhoneNumber = newSubscriber.PhoneNumber
            };

        // Act
        _dbContext.OfferedSubscriptions.Add(newSubscription);
        _dbContext.Subscribers.Add(newSubscriber);
        _dbContext.ActiveSubscriptions.Add(dummySubscription);
        _dbContext.SaveChanges();

        // Act
        var output = _subscriptionQueries.RemoveActiveSubscription(dummySubscription);

        // Assert
        Assert.True(output);
    }

    [Fact]
    public void RemoveActiveSubscription_InvalidActiveSubscription_ReturnsFalse()
    {
        // Arrange
        var dummySubscription =
            new ActiveSubscriptions
            {
                PhoneNumber = "NotANumber"
            };

        // Act
        var output = _subscriptionQueries.RemoveActiveSubscription(dummySubscription);

        // Assert
        Assert.False(output);
    }

    private async Task SeedTestDataSubscribers()
    {
        var offeredSubscriptions = new OfferedSubscriptions
        {
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

        _dbContext.Subscribers.Add(subscriberOne);
        _dbContext.Subscribers.Add(subscriberTwo);
        _dbContext.OfferedSubscriptions.Add(offeredSubscriptions);
        _dbContext.ActiveSubscriptions.Add(new ActiveSubscriptions(Guid.NewGuid(), activeSubscriber.SubscriberId,
            offeredSubscriptions.SubscriptionId, DateTime.Now, "555-555-1111")
        {
            PhoneNumber = "555-555-1111"
        });
        await _dbContext.SaveChangesAsync();
    }
}