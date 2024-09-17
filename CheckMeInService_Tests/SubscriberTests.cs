using CheckMeInService.Models;

namespace CheckMeInService_Tests;

[TestClass]
public class SubscriberTests
{
    [TestMethod]
    public void AddNewSubscription_GivenValidSubscriptionName_RegisterSubscriptionForUser ()
    {
        //Arrange
        Subscriber newSubscriber = new Subscriber("Luis", "999-999-999");
        
        // Act
        newSubscriber.AddNewSubscription("Excercise");

        // Assert
        Assert.IsNotNull(newSubscriber.ReturnSubscriberId);
        Assert.IsNotNull(newSubscriber.Subscriptions);
        Assert.AreEqual(1, newSubscriber.Subscriptions.Count);
    }
    
    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void AddNewSubscription_GivenValidSubscriptionName_RegisterSubscriptionForUserThrowException()
    {
        //Arrange
        Subscriber newSubscriber = new Subscriber("Luis", "999-999-999");
        
        
        // Act
        newSubscriber.AddNewSubscription("NotExcercise");

        // Assert
        Assert.IsNotNull(newSubscriber.ReturnSubscriberId);
        Assert.IsNotNull(newSubscriber.Subscriptions);
    }
}