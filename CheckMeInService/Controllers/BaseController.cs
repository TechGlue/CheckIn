using Microsoft.AspNetCore.Mvc;

namespace CheckMeInService.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public abstract class BaseController: Controller
{
    
}