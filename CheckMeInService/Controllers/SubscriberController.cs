using CheckMeInService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CheckMeInService.Controllers;

public class SubscriberController : BaseController
{
    private readonly CheckMeInContext _checkMeInContext;
    private readonly ILogger<SubscriberController> _logger;

    public SubscriberController(CheckMeInContext checkMeInContext, ILogger<SubscriberController> logger)
    {
        _checkMeInContext = checkMeInContext;
        _logger = logger;
    }

    // one off endpoint for testing authentication 
    [HttpGet]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public IActionResult TestSubscribers()
    {
        return Ok("Subscribers Controllers is working");
    }

    // double check if we want to keep this 
    [HttpGet("id")]
    [ProducesResponseType(typeof(Subscriber), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSubscriberById(Guid id)
    {
        var subscriber = await _checkMeInContext.Subscribers.SingleOrDefaultAsync(x => x.SubscriberId == id);

        if (subscriber == null)
        {
            return NotFound();
        }

        var subscriberResponse = SubscriberToGetSubscriberResponse(subscriber);
        return Ok(subscriberResponse);
    }

    [HttpPost("{firstName}, {lastName}, {phoneNumber}, {subscriptionName}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateActiveSubscription(string firstName, string lastName, string phoneNumber,
        string subscriptionName)
    {
        _logger.LogInformation("Creating a new subscription for: {PhoneNumber}", phoneNumber);

        // Grab subscription 
        OfferedSubscriptions? offeredSubscription =
            await _checkMeInContext.OfferedSubscriptions.FindAsync(subscriptionName);

        if (offeredSubscription is null)
        {
            _logger.LogWarning($"Given subscription is not available {subscriptionName}", subscriptionName);
            return NotFound("Subscription is currently not being offered.");
        }

        Subscriber? subscriber = await _checkMeInContext.Subscribers.FindAsync(phoneNumber);

        if (subscriber is null)
        {
            _logger.LogWarning("Subscriber with PhoneNumber: {PhoneNumber} was not found. Creating a new subscriber",
                phoneNumber);

            subscriber = new Subscriber()
            {
                FirstName = firstName, LastName = lastName, PhoneNumber = phoneNumber
            };
            _checkMeInContext.Subscribers.Add(subscriber);
            await _checkMeInContext.SaveChangesAsync();
        }

        // go and create the subscription
        ActiveSubscriptions newSubscriptions = new ActiveSubscriptions()
        {
            ActiveSubscriptionId = Guid.NewGuid(),
            SubscriberId = subscriber.SubscriberId,
            SubscriptionId = offeredSubscription.SubscriptionId,
            SubscriptionStartDate = DateTime.Now,
            PhoneNumber = subscriber.PhoneNumber
        };

        _checkMeInContext.ActiveSubscriptions.Add(newSubscriptions);
        await _checkMeInContext.SaveChangesAsync();

        return Ok(newSubscriptions);
    }


    private static Subscriber SubscriberToGetSubscriberResponse(Subscriber subscriber)
    {
        return new Subscriber()
        {
            SubscriberId = subscriber.SubscriberId, FirstName = subscriber.FirstName, LastName = subscriber.LastName,
            PhoneNumber = subscriber.PhoneNumber
        };
    }
}