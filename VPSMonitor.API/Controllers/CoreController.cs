using Microsoft.AspNetCore.Mvc;
using VPSMonitor.API.Services;
using VPSMonitor.Core.Application;
using VPSMonitor.Core.Infrastructure;

namespace VPSMonitor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoreController : Controller
{
   private readonly SSHService _sshService;
   public CoreController(SSHService sshService)
   {
      _sshService = sshService;
   }
   
   [HttpPost]
   [Route("ExecuteCommand")]
   public async Task<IActionResult> ExecuteCommand(string host, string username, string password, string command)
   {
      if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(command))
      {
         return BadRequest("Invalid input parameters.");
      }
      try
      {
         var sshService = new SSHService();
         var sshClient = _sshService.Connect(host, username, password);

         var response = await sshService.ExecuteCommandAsync(sshClient, command);
         sshService.Disconnect(sshClient);

         return Ok(response);
      }
      catch (Exception ex)
      {
         return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
      }
   }
}