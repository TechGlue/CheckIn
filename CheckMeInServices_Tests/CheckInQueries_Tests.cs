using CheckMeInService.Data;
using CheckMeInService.Models;
using Testcontainers.SqlEdge;

namespace CheckMeInServices_Tests;

public class CheckInQueriesTests : IAsyncLifetime
{
    private readonly SqlEdgeContainer _container;
    private CheckInQueries _checkInQueries;
    private CheckMeInContext _dbContext;

    public CheckInQueriesTests()
    {
        _container = new SqlEdgeBuilder()
            .WithImage("mcr.microsoft.com/azure-sql-edge:1.0.7")
            .WithExposedPort(new Random().Next(49152, 65535))
            .WithCleanUp(true)
            .Build();
    }


    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        _dbContext = new CheckMeInContext(_container.GetConnectionString());
        _checkInQueries = new CheckInQueries(_container.GetConnectionString());

        await _dbContext.Database.EnsureCreatedAsync();
    }

    public Task DisposeAsync()
    {
        return _container.DisposeAsync().AsTask();
    }

    [Fact]
    public void LogCheckIn_CheckInEarly_ReturnsFalse()
    {
        // Arrange
        var activeSubscription = new ActiveSubscriptions(Guid.NewGuid(),
            Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, "444-444-4444")
        {
            PhoneNumber = "444-444-4444"
        };

        var newCheckIn = new CheckIn
        {
            ActiveSubscriptionId = activeSubscription.ActiveSubscriptionId,
            LastCheckInDate = DateTime.Now.AddHours(10),
            FutureCheckInDate = DateTime.Now.AddHours(24),
            TotalCheckIns = 0
        };

        _dbContext.ActiveSubscriptions.Add(activeSubscription);
        _dbContext.CheckIn.Add(newCheckIn);
        _dbContext.SaveChanges();

        // Act 
        var output = _checkInQueries.LogCheckIn(activeSubscription);

        // Assert
        Assert.False(output);
    }

    [Fact]
    public void LogCheckIn_GivenInvalidSubscription_CheckInNotFoundReturnsFalse()
    {
        // Arrange
        var activeSubscription = new ActiveSubscriptions(Guid.NewGuid(),
            Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, "444-444-4444")
        {
            PhoneNumber = "444-444-4444"
        };
        _dbContext.Add(activeSubscription);

        // Act
        var output = _checkInQueries.LogCheckIn(activeSubscription);

        // Assert
        Assert.False(output);
    }

    [Fact]
    public void LogCheckIn_GivenValidSubscription_ReturnsTrue()
    {
        // Arrange
        var activeSubscription = new ActiveSubscriptions(Guid.NewGuid(),
            Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, "444-444-4444")
        {
            PhoneNumber = "444-444-4444"
        };

        var newCheckIn = new CheckIn
        {
            ActiveSubscriptionId = activeSubscription.ActiveSubscriptionId,
            LastCheckInDate = DateTime.Now.AddHours(25),
            FutureCheckInDate = DateTime.Now.AddHours(24),
            TotalCheckIns = 0
        };

        _dbContext.ActiveSubscriptions.Add(activeSubscription);
        _dbContext.CheckIn.Add(newCheckIn);
        _dbContext.SaveChanges();

        // Act 
        var output = _checkInQueries.LogCheckIn(activeSubscription);

        var checkIn = _dbContext.CheckIn.SingleOrDefault(x =>
            x.ActiveSubscriptionId == newCheckIn.ActiveSubscriptionId);

        // Assert
        Assert.True(output);
        Assert.NotNull(checkIn);
    }

    [Fact]
    public void CreateNewCheckIn_GivenValidSubscriptionAndFutureReminderHours_ReturnsTrue()
    {
        // Arrange
        var newSubscriber = new Subscriber
        {
            SubscriberId = Guid.NewGuid(), FirstName = "first", LastName = "last", PhoneNumber = "111-111-1111"
        };

        var newSubscription = new OfferedSubscriptions
        {
            SubscriptionId = Guid.NewGuid(), SubscriptionName = "TestAddNewMemberSubscriptionFalse"
        };

        var activeSubscription = new ActiveSubscriptions
        {
            ActiveSubscriptionId = Guid.NewGuid(), SubscriberId = newSubscriber.SubscriberId,
            PhoneNumber = newSubscriber.PhoneNumber, SubscriptionId = newSubscription.SubscriptionId,
            SubscriptionStartDate = DateTime.Now
        };

        _dbContext.Subscribers.Add(newSubscriber);
        _dbContext.OfferedSubscriptions.Add(newSubscription);
        _dbContext.ActiveSubscriptions.Add(activeSubscription);
        _dbContext.SaveChanges();

        var futureReminderHours = 1;

        // Act
        var output = _checkInQueries.CreateNewCheckIn(activeSubscription, futureReminderHours);
        var checkIn =
            _dbContext.CheckIn.SingleOrDefault(x =>
                x.ActiveSubscriptionId == activeSubscription.ActiveSubscriptionId);

        // Assert
        Assert.True(output);
        Assert.NotNull(checkIn);
        Assert.Equal(0, checkIn.TotalCheckIns);
        Assert.Equal(activeSubscription.SubscriptionStartDate, checkIn.LastCheckInDate);
        Assert.Equal(activeSubscription.SubscriptionStartDate.AddHours(futureReminderHours), checkIn.FutureCheckInDate);
    }

    [Fact]
    public void CreateNewCheckIn_GivenExistingSubscriptionAndFutureReminderHours_ReturnsFalse()
    {
        // Arrange
        var newSubscriber = new Subscriber
        {
            FirstName = "first",
            LastName = "last",
            PhoneNumber = "222-111-1111"
        };
        var newSubscription =
            new OfferedSubscriptions
            {
                SubscriptionName = "TestAddNewMemberSubscriptionFalse"
            };

        var activeSubscription = new ActiveSubscriptions(Guid.NewGuid(), newSubscriber.SubscriberId,
            newSubscription.SubscriptionId, DateTime.Now, newSubscriber.PhoneNumber)
        {
            PhoneNumber = newSubscriber.PhoneNumber
        };

        _dbContext.Subscribers.Add(newSubscriber);
        _dbContext.OfferedSubscriptions.Add(newSubscription);
        _dbContext.ActiveSubscriptions.Add(activeSubscription);
        _dbContext.SaveChanges();

        var futureReminderHours = 1;

        // Act 
        _ = _checkInQueries.CreateNewCheckIn(activeSubscription, futureReminderHours);
        var output = _checkInQueries.CreateNewCheckIn(activeSubscription, futureReminderHours);

        // Assert
        Assert.False(output);
    }

    [Fact]
    public void RemoveCheckIn_GivenValidActiveSubscription_ReturnTrue()
    {
        // Arrange 
        var newSubscriber =
            new Subscriber(Guid.NewGuid(), "RemoveCheckInFirst", "RemoveCheckInLast", "333-333-3333")
            {
                FirstName = "RemoveCheckInFirst",
                LastName = "RemoveCheckInLast",
                PhoneNumber = "333-333-3333"
            };
        var newSubscription =
            new OfferedSubscriptions(Guid.NewGuid(), "RemoveCheckIn")
            {
                SubscriptionName = "RemoveCheckIn"
            };

        var activeSubscription = new ActiveSubscriptions(Guid.NewGuid(), newSubscriber.SubscriberId,
            newSubscription.SubscriptionId, DateTime.Now, newSubscriber.PhoneNumber)
        {
            PhoneNumber = newSubscriber.PhoneNumber
        };

        var newCheckIn = new CheckIn(Guid.NewGuid(), activeSubscription.ActiveSubscriptionId, DateTime.Now,
            DateTime.Now, 0);

        _dbContext.Subscribers.Add(newSubscriber);
        _dbContext.OfferedSubscriptions.Add(newSubscription);
        _dbContext.ActiveSubscriptions.Add(activeSubscription);
        _dbContext.CheckIn.Add(newCheckIn);
        _dbContext.SaveChanges();

        // Act 
        var output = _checkInQueries.RemoveCheckIn(activeSubscription);
        var checkIns = _dbContext.CheckIn.SingleOrDefault(x => x.CheckInId == newCheckIn.CheckInId);

        // Assert
        Assert.True(output);
        Assert.Null(checkIns);
    }

    [Fact]
    public void RemoveCheckIn_GivenInvalidActiveSubscription_ReturnFalse()
    {
        // Arrange
        var newSubscriber =
            new Subscriber
            {
                FirstName = "RemoveCheckInFirst",
                LastName = "RemoveCheckInLast",
                PhoneNumber = "333-333-3333"
            };
        var newSubscription =
            new OfferedSubscriptions
            {
                SubscriptionName = "RemoveCheckIn"
            };

        var activeSubscription = new ActiveSubscriptions(Guid.NewGuid(), newSubscriber.SubscriberId,
            newSubscription.SubscriptionId, DateTime.Now, newSubscriber.PhoneNumber)
        {
            PhoneNumber = newSubscriber.PhoneNumber
        };

        _dbContext.Subscribers.Add(newSubscriber);
        _dbContext.OfferedSubscriptions.Add(newSubscription);
        _dbContext.ActiveSubscriptions.Add(activeSubscription);
        _dbContext.SaveChanges();

        // Act 
        var output = _checkInQueries.RemoveCheckIn(activeSubscription);

        // Assert
        Assert.False(output);
    }

    [Fact]
    public void FetchAllActiveCheckIns_ReturnsListOfCheckIns()
    {
        // Arrange 
        // Fake check ins not going to bind to anything
        var dummyCheckInOne = new CheckIn(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, DateTime.Now, 0);
        var dummyCheckInTwo = new CheckIn(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, DateTime.Now, 0);
        var dummyCheckInThree = new CheckIn(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, DateTime.Now, 0);

        _dbContext.CheckIn.Add(dummyCheckInOne);
        _dbContext.CheckIn.Add(dummyCheckInTwo);
        _dbContext.CheckIn.Add(dummyCheckInThree);
        _dbContext.SaveChanges();

        // Act 
        var output = _checkInQueries.FetchAllActiveCheckIns();

        // Assert
        Assert.Equal(3, output.Count);
        Assert.NotNull(output);
    }

    [Fact]
    public void FetchAllActiveCheckIns_ReturnsEmptyList()
    {
        // Arrange - Act 
        var output = _checkInQueries.FetchAllActiveCheckIns();

        // Assert
        Assert.Empty(output);
    }
}