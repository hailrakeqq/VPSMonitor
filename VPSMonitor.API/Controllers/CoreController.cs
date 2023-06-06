using Microsoft.AspNetCore.Mvc;
using VPSMonitor.API.Entities;
using VPSMonitor.API.Repository;
using VPSMonitor.Core;

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

   [HttpPost]
   [Route("GetResourcesUsageInfo")]
   public async Task<IActionResult> GetResourcesUsageInfo([FromBody] SshRequest request)
   {
      using (var sshClient = _sshService.Connect(request.Host, request.Username, request.Password))
      {
         var commands = new List<Task<string>>
         {
            _sshService.ExecuteCommandAsync(sshClient, "mpstat -P ALL -o JSON"),
            _sshService.ExecuteCommandAsync(sshClient, "free -h"),
            _sshService.ExecuteCommandAsync(sshClient, "df -h"),
            _sshService.ExecuteCommandAsync(sshClient, "uptime")
         };

         await Task.WhenAll(commands);

         var responseTasks = new List<Task<string>>(4);
         responseTasks.Add(Task.Run(() => Parser.mpstatCommandParse(commands[0].Result)));
         responseTasks.Add(Task.Run(() => Parser.freeCommandParse(commands[1].Result)));
         responseTasks.Add(Task.Run(() => Parser.dfCommandParse(commands[2].Result)));
         responseTasks.Add(commands[3]);

         await Task.WhenAll(responseTasks);
         
         var response = responseTasks.Select(t => t.Result).ToList();

         return Ok(response);
      }
   }
}