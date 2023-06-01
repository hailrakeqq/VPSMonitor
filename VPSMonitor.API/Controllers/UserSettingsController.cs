using Microsoft.AspNetCore.Mvc;

namespace VPSMonitor.API.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class UserSettingsController : Controller
{
    [HttpGet]
    [Route("test")]
    public IActionResult Test()
    {
        return Ok("Hello");
    }
}