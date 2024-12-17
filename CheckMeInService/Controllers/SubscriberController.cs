using CheckMeInService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CheckMeInService.Controllers;

public class SubscriberController: BaseController
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
}