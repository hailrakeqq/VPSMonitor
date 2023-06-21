using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VPSMonitor.API.Entities;
using VPSMonitor.API.Repository;
using VPSMonitor.Core;

namespace VPSMonitor.API.Controllers;

[ApiController]
[Authorize]
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
         var sshClient = _sshService.Connect(request.Host, request.Username, request.Password);

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
   [Route("GetSystemInfo")]
   public async Task<IActionResult> GetSystemInfo([FromBody] SshRequest request)
   {
      using (var sshClient = _sshService.Connect(request.Host, request.Username, request.Password))
      {
         string[] commands =
         {
            "hostname",
            "cat /etc/os-release | grep -e '^PRETTY_NAME='",
            "uname -r", "uname -m", "date"
         };

         var commandTasks = commands.Select(command => _sshService.ExecuteCommandAsync(sshClient, command)).ToList();
         await Task.WhenAll(commandTasks);

         var commandResults = commandTasks.Select(task => task.Result).ToList();

         return Ok(new SystemInfo()
         {
            Hostname = commandResults[0],
            OS = commandResults[1].Split("\"")[1],
            Kernel = commandResults[2],
            CpuArchitecture = commandResults[3],
            DateTime = commandResults[4]
         });
      }
   }

   [HttpPost]
   [Route("GetCpuInfo")]
   public async Task<IActionResult> GetCpuUsageInfo([FromBody] SshRequest request)
   {
      using (var sshClient = _sshService.Connect(request.Host, request.Username, request.Password))
      {
         string result = await _sshService.ExecuteCommandAsync(sshClient, "mpstat -P ALL -o JSON");
         return Ok(Parser.mpstatCommandParse(result));
      }
   }

   [HttpPost]
   [Route("GetRamInfo")]
   public async Task<IActionResult> GetRamUsageInfo([FromBody] SshRequest request)
   {
      using (var sshClient = _sshService.Connect(request.Host, request.Username, request.Password))
      {
         string result = await _sshService.ExecuteCommandAsync(sshClient, "free -h");
         return Ok(result);
      }
   }
}