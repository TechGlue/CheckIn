using CheckMeInService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CheckMeInService.Controllers;

// Todo:  
// write tests for all of the factory patterns 
// Refactor controllers into smaller pieces 
// Testing for the add subscription and remove subscription endpoints 
// Remove subscription  
// No need for update as once enrolled there isn't much to change

public class SubscribersController : BaseController
{
    private readonly CheckMeInContext _checkMeInContext;
    private readonly ILogger<SubscribersController> _logger;

    public SubscribersController(CheckMeInContext checkMeInContext, ILogger<SubscribersController> logger)
    {
        _checkMeInContext = checkMeInContext;
        _logger = logger;
    }

    // one off endpoint for testing authentication 
    [HttpGet]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public IActionResult TestSubscribers()
    {
        _logger.LogWarning("Subscribers API is up");
        return Ok("Subscribers Controllers is working");
    }

    // double check if we want to keep this 
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Subscriber), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSubscriberById(Guid id)
    {
        var subscriber = await _checkMeInContext.Subscribers.SingleOrDefaultAsync(x => x.SubscriberId == id);

        if (subscriber == null)
        {
            return NotFound("Invalid id provided");
        }

        var subscriberResponse = SubscriberToGetSubscriberResponse(subscriber);
        return Ok(subscriberResponse);
    }

    [HttpPost()]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateActiveSubscription([FromQuery] string firstName, [FromQuery] string lastName,
        [FromQuery] string phoneNumber,
        [FromQuery] string subscriptionName)
    {
        _logger.LogInformation("Creating a new subscription for: {PhoneNumber}", phoneNumber);
        // Grab subscription 
        OfferedSubscriptions? offeredSubscription =
            await _checkMeInContext.OfferedSubscriptions.SingleOrDefaultAsync( x=> x.SubscriptionName == "Exercise");

        if (offeredSubscription is null)
        {
            _logger.LogWarning($"Given subscription is not available {subscriptionName}", subscriptionName);
            return NotFound("Subscription is currently not being offered.");
        }
        
        Subscriber? subscriber = await _checkMeInContext.Subscribers.SingleOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
        

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
        
        
        // check if subscriber has an active subscription before creating a new one 
        ActiveSubscriptions? newSubscriptions = await _checkMeInContext.ActiveSubscriptions.SingleOrDefaultAsync(x => x.SubscriptionId == offeredSubscription.SubscriptionId && x.SubscriberId == subscriber.SubscriberId);;

        if (newSubscriptions is not null)
        {
            _logger.LogWarning($"{firstName} is already enrolled into {subscriptionName}", subscriber.FirstName, subscriptionName);
            return NotFound($"{subscriber.FirstName} is already enrolled into {offeredSubscription.SubscriptionName}");
        }
        
        // go and create the subscription
        newSubscriptions = new ActiveSubscriptions()
        {
            ActiveSubscriptionId = Guid.NewGuid(),
            SubscriberId = subscriber.SubscriberId,
            SubscriptionId = offeredSubscription.SubscriptionId,
            SubscriptionStartDate = DateTime.Now,
            PhoneNumber = subscriber.PhoneNumber
        };

        _checkMeInContext.ActiveSubscriptions.Add(newSubscriptions);
        await _checkMeInContext.SaveChangesAsync();

        return Ok();
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