using CheckMeInService.Models;

namespace CheckMeInService_Tests;

[TestClass]
public class SubscriberTests
{
    [TestMethod]
    public void AddNewSubscription_GivenValidSubscriptionName_RegisterSubscriptionForUser()
    {
        //Arrange
        Subscriber newSubscriber = new Subscriber("Luis", "999-999-999");

        // Act
        newSubscriber.AddNewSubscription("Exercise");

        // Assert
        Assert.IsNotNull(newSubscriber.ReturnSubscriberId);
        Assert.IsNotNull(newSubscriber.Subscriptions);
        Assert.AreEqual(1, newSubscriber.Subscriptions.Count);
    }

    [TestMethod]
    public void RemoveExistingSubscription_GivenValidSubscriptionName_RegisterSubscriptionForUser()
    {
        //Arrange
        Subscriber newSubscriber = new Subscriber("Luis", "999-999-999");

        // Act
        newSubscriber.AddNewSubscription("Exercise");
        newSubscriber.RemoveExistingSubscription("Exercise");

        // Assert
        Assert.IsNotNull(newSubscriber.ReturnSubscriberId);
        Assert.IsNotNull(newSubscriber.Subscriptions);
        Assert.AreEqual(0, newSubscriber.Subscriptions.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void RemoveExistingSubscription_GivenInvalidSubscriptionName_RemoveExistingSubscriptionThrowsException()
    {
        //Arrange
        Subscriber newSubscriber = new Subscriber("Luis", "999-999-999");


        // Act
        newSubscriber.AddNewSubscription("NotExercise");

        // Assert
        Assert.IsNotNull(newSubscriber.ReturnSubscriberId);
        Assert.IsNotNull(newSubscriber.Subscriptions);
        Assert.AreEqual(0, newSubscriber.Subscriptions.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void AddNewSubscription_GivenValidSubscriptionName_RegisterSubscriptionForUserThrowsException()
    {
        //Arrange
        Subscriber newSubscriber = new Subscriber("Luis", "999-999-999");


        // Act
        newSubscriber.AddNewSubscription("NotExercise");

        // Assert
        Assert.IsNotNull(newSubscriber.ReturnSubscriberId);
        Assert.IsNotNull(newSubscriber.Subscriptions);
        Assert.AreEqual(0, newSubscriber.Subscriptions.Count);
    }


    // Start adding some tests to the subscriber that will add a sample payload from the db
}