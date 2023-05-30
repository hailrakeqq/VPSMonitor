using Microsoft.AspNetCore.Mvc;
using VPSMonitor.Core.Application;
using VPSMonitor.Core.Infrastructure;

namespace VPSMonitor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoreController : Controller
{
   [HttpPost]
   [Route("ExecuteCommand")]
   public async Task<IActionResult> ExecuteCommand(string host, string username, string password, string command)
   {
      var sshContext = new VpsConnectionContext();
      var sshClient = sshContext.Connect(host, username, password);

      var response = CommandExecutor.ExecuteCommand(sshClient, command);
      sshContext.Disconect(sshClient);
      return Ok(response);
   }
}