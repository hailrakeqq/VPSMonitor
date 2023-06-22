using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VPSMonitor.API.Entities;
using VPSMonitor.API.Repository;
using VPSMonitor.Core;

namespace VPSMonitor.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CoreUserCrudController : Controller
{
    private readonly ISshRepository _sshService;

    public CoreUserCrudController(ISshRepository sshRepository)
    {
        _sshService = sshRepository;
    }
    
    [HttpPost]
    [Route("GetUsers")]
    public async Task<IActionResult> GetUserInfo([FromBody] SshRequest request)
    {
        using (var sshClient = _sshService.Connect(request.Host, request.Username, request.Password))
        {
            string result = await _sshService.ExecuteCommandAsync(sshClient, "ls -ld /home/*/");
            var parsedValue = Parser.parseGetUserCommand(result);
            return Ok(parsedValue);
        }
    }

    [HttpPost]
    [Route("CreateUser")]
    public async Task<IActionResult> CreateLinuxUser([FromBody] SshRequestCoreForUserCrud sshRequest)
    {
        using (var sshClient = _sshService.Connect(sshRequest.HostAddress, sshRequest.HostUsername, sshRequest.HostPassword))
        {
            string username = sshRequest.UserUsername;
            
            //Create user and home directory for created user
            await _sshService.ExecuteCommandAsync(sshClient, $"useradd -m {username}");
            //Grant rights to the user
            await _sshService.ExecuteCommandAsync(sshClient, $"chmod -R {username}:{username} /home/{username}");
            return Ok();
        }
    }

    [HttpPut]
    [Route("ChangeUserData")]
    public async Task<IActionResult> ChangeLinuxUserData([FromBody] SshRequestCoreForUserCrud sshRequest)
    {

        return Ok();
    }

    [HttpDelete]
    [Route("DeleteUser")]
    public async Task<IActionResult> DeleteLinuxUser([FromBody] SshRequestCoreForUserCrud sshRequest)
    {
        using (var sshClient = _sshService.Connect(sshRequest.HostAddress, sshRequest.HostUsername, sshRequest.HostPassword))
        {
            //Delete user and user home directory 
            await _sshService.ExecuteCommandAsync(sshClient, $"userdel -r {sshRequest.UserUsername}");

            return Ok();
        }
    }
}