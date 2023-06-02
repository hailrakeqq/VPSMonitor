using Microsoft.AspNetCore.Mvc;
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
   public async Task<IActionResult> ExecuteCommand([FromBody]Test test)//(string host, string sshId, string command)
   {
      if (string.IsNullOrWhiteSpace(test.Host) || string.IsNullOrWhiteSpace(test.Command))
      {
         return BadRequest("Invalid input parameters.");
      }

      try
      {
         // var sshKey = await _sshService.GetSshKeyById(sshId);

         //var sshClient = _sshService.Connect(host, sshKey.Username, sshKey.Ssh);
         
         var sshClient = _sshService.Connect(test.Host, test.Username, test.Rsa);
         var response = await _sshService.ExecuteCommandAsync(sshClient, test.Command);
         
         //var response = await _sshService.ExecuteCommandAsync(sshClient, command);
         _sshService.Disconnect(sshClient);

         return Ok(response);
      }
      catch (Exception ex)
      {
         return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
      }
      
      
      // if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(command))
      // {
      //    return BadRequest("Invalid input parameters.");
      // }
      //
      // try
      // {
      //    var sshKey = await _sshService.GetSshKeyById(sshId);
      //
      //    var sshClient = _sshService.Connect(host, sshKey.Username, sshKey.Ssh);
      //
      //    var response = await _sshService.ExecuteCommandAsync(sshClient, command);
      //    _sshService.Disconnect(sshClient);
      //
      //    return Ok(response);
      // }
      // catch (Exception ex)
      // {
      //    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
      // }
   }
}