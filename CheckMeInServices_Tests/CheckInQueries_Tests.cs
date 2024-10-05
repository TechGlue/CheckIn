using CheckMeInService.Data;
using CheckMeInService.Models;
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
    public void CreateNewCheckIn_GivenValidSubscriptionAndFutureReminderHours_ReturnsTrue()
    {
        // Arrange
        Subscriber newSubscriber = new Subscriber(Guid.NewGuid(), "first", "last", "111-111-1111");
        OfferedSubscriptions newSubscription =
            new OfferedSubscriptions(Guid.NewGuid(), "TestAddNewMemberSubscriptionFalse");

        ActiveSubscriptions activeSubscription = new ActiveSubscriptions(Guid.NewGuid(), newSubscriber.SubscriberId,
            newSubscription.SubscriptionId, DateTime.Now, newSubscriber.PhoneNumber);

        _dbContext.Subscribers.Add(newSubscriber);
        _dbContext.OfferedSubscriptions.Add(newSubscription);
        _dbContext.ActiveSubscriptions.Add(activeSubscription);
        _dbContext.SaveChanges();

        int futureReminderHours = 1;

        // Act
        bool output = _checkInQueries.CreateNewCheckIn(activeSubscription, futureReminderHours);
        CheckIn? checkIn =
            _dbContext.CheckIn.FirstOrDefault(x =>
                x != null && x.ActiveSubscriptionId == activeSubscription.ActiveSubscriptionId);

        // Assert
        Assert.True(output);
        Assert.NotNull(checkIn);
        Assert.Equal(0, checkIn.CurrentStreak);
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
            newSubscription.SubscriptionId, DateTime.Now, newSubscriber.PhoneNumber);

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
        Subscriber newSubscriber = new Subscriber(Guid.NewGuid(), "RemoveCheckInFirst", "RemoveCheckInLast", "333-333-3333");
        OfferedSubscriptions newSubscription =
            new OfferedSubscriptions(Guid.NewGuid(), "RemoveCheckIn");

        ActiveSubscriptions activeSubscription = new ActiveSubscriptions(Guid.NewGuid(), newSubscriber.SubscriberId,
            newSubscription.SubscriptionId, DateTime.Now, newSubscriber.PhoneNumber);

        CheckIn newCheckIn = new CheckIn(Guid.NewGuid(), activeSubscription.ActiveSubscriptionId, DateTime.Now,
            DateTime.Now, 0);
        
        _dbContext.Subscribers.Add(newSubscriber);
        _dbContext.OfferedSubscriptions.Add(newSubscription);
        _dbContext.ActiveSubscriptions.Add(activeSubscription);
        _dbContext.CheckIn.Add(newCheckIn);
        _dbContext.SaveChanges();

        // Act 
        bool output = _checkInQueries.RemoveCheckIn(activeSubscription);
        CheckIn? checkIns = _dbContext.CheckIn.FirstOrDefault(x => x != null && x.CheckInId == newCheckIn.CheckInId);

        // Assert
        Assert.True(output);
        Assert.Null(checkIns);
    }

    [Fact]
    public void RemoveCheckIn_GivenInvalidActiveSubscription_ReturnFalse()
    {
        // Arrange
        Subscriber newSubscriber = new Subscriber(Guid.NewGuid(), "RemoveCheckInFirst", "RemoveCheckInLast", "333-333-3333");
        OfferedSubscriptions newSubscription =
            new OfferedSubscriptions(Guid.NewGuid(), "RemoveCheckIn");

        ActiveSubscriptions activeSubscription = new ActiveSubscriptions(Guid.NewGuid(), newSubscriber.SubscriberId,
            newSubscription.SubscriptionId, DateTime.Now, newSubscriber.PhoneNumber);
        
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