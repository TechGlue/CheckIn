using Microsoft.AspNetCore.Mvc;

namespace CheckMeInService.Controllers;

[ApiController]
[Route("v1/[controller]")]
[Produces("application/json")]
public abstract class BaseController: Controller
{
    
}