using CheckMeInService.Data;
using CheckMeInService.Models;
using Microsoft.EntityFrameworkCore;
using Testcontainers.SqlEdge;

namespace CheckMeInServices_Tests;

public class CheckInQueries_Tests : IAsyncLifetime
{
    private readonly SqlEdgeContainer _container;
    private CheckMeInContext _dbContext;
    private CheckInQueries _checkInQueries;

    public CheckInQueries_Tests()
    {
        _container = new SqlEdgeBuilder()
            .WithImage("mcr.microsoft.com/azure-sql-edge:1.0.7")
            .WithExposedPort(new Random().Next(49152, 65535))
            .WithCleanUp(true)
            .Build();
    }
    
    [Fact]
    public void LogCheckIn_CheckInEarly_ReturnsFalse()
    {
        // Arrange
        ActiveSubscriptions activeSubscription = new ActiveSubscriptions(Guid.NewGuid(),
            Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, "444-444-4444")
        {
            PhoneNumber = "444-444-4444"
        };

        CheckIn newCheckIn = new CheckIn
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
        bool output = _checkInQueries.LogCheckIn(activeSubscription); 

        // Assert
        Assert.False(output);
    }

    [Fact]
    public void LogCheckIn_GivenInvalidSubscription_CheckInNotFoundReturnsFalse()
    {
        // Arrange
        ActiveSubscriptions activeSubscription = new ActiveSubscriptions(Guid.NewGuid(),
            Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, "444-444-4444")
        {
            PhoneNumber = "444-444-4444"
        };
        _dbContext.Add(activeSubscription);

        // Act
        bool output = _checkInQueries.LogCheckIn(activeSubscription);

        // Assert
        Assert.False(output);
    }

    [Fact]
    public void LogCheckIn_GivenValidSubscription_ReturnsTrue()
    {
        // Arrange
        ActiveSubscriptions activeSubscription = new ActiveSubscriptions(Guid.NewGuid(),
            Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, "444-444-4444")
        {
            PhoneNumber = "444-444-4444"
        };

        CheckIn newCheckIn = new CheckIn
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
        bool output = _checkInQueries.LogCheckIn(activeSubscription); 

        CheckIn? checkIn = _dbContext.CheckIn.SingleOrDefault(x =>
            x != null && x.ActiveSubscriptionId == newCheckIn.ActiveSubscriptionId);

        // Assert
        Assert.True(output);
        Assert.NotNull(checkIn);
    }

    [Fact]
    public void CreateNewCheckIn_GivenValidSubscriptionAndFutureReminderHours_ReturnsTrue()
    {
        // Arrange
        Subscriber newSubscriber = new Subscriber(Guid.NewGuid(), "first", "last", "111-111-1111");
        OfferedSubscriptions newSubscription =
            new OfferedSubscriptions(Guid.NewGuid(), "TestAddNewMemberSubscriptionFalse");

        ActiveSubscriptions activeSubscription = new ActiveSubscriptions(Guid.NewGuid(), newSubscriber.SubscriberId,
            newSubscription.SubscriptionId, DateTime.Now, newSubscriber.PhoneNumber)
        {
            PhoneNumber = newSubscriber.PhoneNumber
        };

        _dbContext.Subscribers.Add(newSubscriber);
        _dbContext.OfferedSubscriptions.Add(newSubscription);
        _dbContext.ActiveSubscriptions.Add(activeSubscription);
        _dbContext.SaveChanges();

        int futureReminderHours = 1;

        // Act
        bool output = _checkInQueries.CreateNewCheckIn(activeSubscription, futureReminderHours);
        CheckIn? checkIn =
            _dbContext.CheckIn.SingleOrDefault(x =>
                x != null && x.ActiveSubscriptionId == activeSubscription.ActiveSubscriptionId);

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
        Subscriber newSubscriber = new Subscriber(Guid.NewGuid(), "first", "last", "222-111-1111");
        OfferedSubscriptions newSubscription =
            new OfferedSubscriptions(Guid.NewGuid(), "TestAddNewMemberSubscriptionFalse");

        ActiveSubscriptions activeSubscription = new ActiveSubscriptions(Guid.NewGuid(), newSubscriber.SubscriberId,
            newSubscription.SubscriptionId, DateTime.Now, newSubscriber.PhoneNumber)
        {
            PhoneNumber = newSubscriber.PhoneNumber
        };

        _dbContext.Subscribers.Add(newSubscriber);
        _dbContext.OfferedSubscriptions.Add(newSubscription);
        _dbContext.ActiveSubscriptions.Add(activeSubscription);
        _dbContext.SaveChanges();

        int futureReminderHours = 1;

        // Act 
        _ = _checkInQueries.CreateNewCheckIn(activeSubscription, futureReminderHours);
        bool output = _checkInQueries.CreateNewCheckIn(activeSubscription, futureReminderHours);

        // Assert
        Assert.False(output);
    }

    [Fact]
    public void RemoveCheckIn_GivenValidActiveSubscription_ReturnTrue()
    {
        // Arrange 
        Subscriber newSubscriber =
            new Subscriber(Guid.NewGuid(), "RemoveCheckInFirst", "RemoveCheckInLast", "333-333-3333");
        OfferedSubscriptions newSubscription =
            new OfferedSubscriptions(Guid.NewGuid(), "RemoveCheckIn");

        ActiveSubscriptions activeSubscription = new ActiveSubscriptions(Guid.NewGuid(), newSubscriber.SubscriberId,
            newSubscription.SubscriptionId, DateTime.Now, newSubscriber.PhoneNumber)
        {
            PhoneNumber = newSubscriber.PhoneNumber
        };

        CheckIn newCheckIn = new CheckIn(Guid.NewGuid(), activeSubscription.ActiveSubscriptionId, DateTime.Now,
            DateTime.Now, 0);

        _dbContext.Subscribers.Add(newSubscriber);
        _dbContext.OfferedSubscriptions.Add(newSubscription);
        _dbContext.ActiveSubscriptions.Add(activeSubscription);
        _dbContext.CheckIn.Add(newCheckIn);
        _dbContext.SaveChanges();

        // Act 
        bool output = _checkInQueries.RemoveCheckIn(activeSubscription);
        CheckIn? checkIns = _dbContext.CheckIn.SingleOrDefault(x => x != null && x.CheckInId == newCheckIn.CheckInId);

        // Assert
        Assert.True(output);
        Assert.Null(checkIns);
    }

    [Fact]
    public void RemoveCheckIn_GivenInvalidActiveSubscription_ReturnFalse()
    {
        // Arrange
        Subscriber newSubscriber =
            new Subscriber(Guid.NewGuid(), "RemoveCheckInFirst", "RemoveCheckInLast", "333-333-3333");
        OfferedSubscriptions newSubscription =
            new OfferedSubscriptions(Guid.NewGuid(), "RemoveCheckIn");

        ActiveSubscriptions activeSubscription = new ActiveSubscriptions(Guid.NewGuid(), newSubscriber.SubscriberId,
            newSubscription.SubscriptionId, DateTime.Now, newSubscriber.PhoneNumber)
        {
            PhoneNumber = newSubscriber.PhoneNumber
        };

        _dbContext.Subscribers.Add(newSubscriber);
        _dbContext.OfferedSubscriptions.Add(newSubscription);
        _dbContext.ActiveSubscriptions.Add(activeSubscription);
        _dbContext.SaveChanges();

        // Act 
        bool output = _checkInQueries.RemoveCheckIn(activeSubscription);

        // Assert
        Assert.False(output);
    }

    [Fact]
    public void FetchAllActiveCheckIns_ReturnsListOfCheckIns()
    {
        // Arrange 
        // Fake check ins not going to bind to anything
        CheckIn dummyCheckInOne = new CheckIn(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, DateTime.Now, 0);
        CheckIn dummyCheckInTwo = new CheckIn(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, DateTime.Now, 0);
        CheckIn dummyCheckInThree = new CheckIn(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, DateTime.Now, 0);

        _dbContext.CheckIn.Add(dummyCheckInOne);
        _dbContext.CheckIn.Add(dummyCheckInTwo);
        _dbContext.CheckIn.Add(dummyCheckInThree);
        _dbContext.SaveChanges();

        // Act 
        List<CheckIn?> output = _checkInQueries.FetchAllActiveCheckIns();

        // Assert
        Assert.Equal(3, output.Count);
        Assert.NotNull(output);
    }

    [Fact]
    public void FetchAllActiveCheckIns_ReturnsEmptyList()
    {
        // Arrange - Act 
        List<CheckIn?> output = _checkInQueries.FetchAllActiveCheckIns();

        // Assert
        Assert.Empty(output);
    }

    [Fact]
    public void TestMethod_UnmanagedCode()
    {
        // This is an example of unmanged code think of an array where you manually have to set up the space in memory
        int[] array = new int[5];

        // Vs. managed code where the memory is managed for you. This is a list that is going to dynamically grow with the amount of data you insert into it. 
        List<int> list = new List<int>();
    }


    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        _dbContext = new CheckMeInContext(_container.GetConnectionString());
        _checkInQueries = new CheckInQueries(_container.GetConnectionString());

        await _dbContext.Database.EnsureCreatedAsync();
    }

    public Task DisposeAsync() =>
        _container.DisposeAsync().AsTask();
}