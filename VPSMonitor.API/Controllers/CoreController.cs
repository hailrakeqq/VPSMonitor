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
   [Route("GetSystemInfo")]
   public async Task<IActionResult> GetSystemInfo([FromBody] SshRequest request)
   {
      using (var sshClient = _sshService.Connect(request.Host, request.Username, request.Password))
      {
         string[] commands = { "hostname",
            "cat /etc/os-release | grep -e '^PRETTY_NAME='",
            "uname -r", "uname -m", "date"};
         var commandResult = new List<string>(commands.Length);
         
         foreach (var command in commands)
         {
            commandResult.Add(await _sshService.ExecuteCommandAsync(sshClient, command));
         }

         return Ok(new SystemInfo()
         {
            Hostname = commandResult[0],
            OS = commandResult[1].Split("\"")[1],
            Kernel = commandResult[2],
            CpuArchitecture = commandResult[3],
            DateTime = commandResult[4]
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

   [HttpPost]
   [Route("GetUsers")]
   public async Task<IActionResult> GetUserInfo([FromBody] SshRequest request)
   {
      using (var sshClient = _sshService.Connect(request.Host, request.Username, request.Password))
      {
         string result = await _sshService.ExecuteCommandAsync(sshClient, "ls -ld /home/*/");
         Console.WriteLine("original string: " + result);
         var lines = result.Split("\n");
         var users = new List<LinuxUser>();
         foreach (var line in lines)
         {
            Console.WriteLine("string after \"n\" split: " + line);
            
            string[] fields = line.Split(" ");
            if (fields[0] != "")
            {
               var user = new LinuxUser()
               {
                  username = fields[2],
                  permissions = Parser.permissionParse(fields[0])
               };

               int number;
               if (Int32.TryParse(fields[2], out number))
               {
                  user.username = "root";
               }
               
               users.Add(user);
            }
         }
         return Ok(users);
      }
   }
}