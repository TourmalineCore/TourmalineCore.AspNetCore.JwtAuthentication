using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Example.Net7._0.BaseAuthentication.Controllers;

[ApiController]
[Route("[controller]")]
public class ExampleController : ControllerBase
{
    [HttpGet]
    [Authorize]
    public IActionResult Get()
    {
        return Ok("You are authenticated!");
    }

    [HttpGet("public")]
    public IActionResult GetPublic()
    {
        return Ok("This endpoint is public");
    }
}