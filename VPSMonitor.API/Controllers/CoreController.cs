using Microsoft.AspNetCore.Mvc;
using VPSMonitor.API.Entities;
using VPSMonitor.API.Repository;

namespace VPSMonitor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoreController : Controller
{
   private readonly ISshRepository _sshService;

   public CoreController(ISshRepository sshService)
   {
      _sshService = sshService;
   }

   [HttpPost]
   [Route("ExecuteCommand")]
   public async Task<IActionResult> ExecuteCommand([FromBody] SshRequest request)
   {
      if (string.IsNullOrWhiteSpace(request.Host) || string.IsNullOrWhiteSpace(request.Command))
      {
         return BadRequest("Invalid input parameters.");
      }

      try
      {
         var sshClient = _sshService.Connect(request.Host, request.Username,request.Password);

         var response = await _sshService.ExecuteCommandAsync(sshClient, request.Command);
         _sshService.Disconnect(sshClient);

         return Ok(response);
      }
      catch (Exception ex)
      {
         return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
      }
   }
}