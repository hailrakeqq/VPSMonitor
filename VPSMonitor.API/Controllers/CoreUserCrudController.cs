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
            string usersAndTheirHomeDirectory = await _sshService.ExecuteCommandAsync(sshClient, "ls -ld /home/*/");
            string usersId = await _sshService.ExecuteCommandAsync(sshClient, "stat -c \"% U:% u\" /home/*/");

            var parsedValue = await Parser.ParseGetUserCommandAsync(usersAndTheirHomeDirectory, usersId);
            return Ok(parsedValue);
        }
    }

    /// <summary>
    /// this function create linux user and home directory;
    /// if users list is equals 1 => create user with 1001 id because it can create new user with root id(root id == 1000)
    /// </summary>
    /// <param name="sshRequest"></param>
    /// <returns>Ok result</returns>
    [HttpPost]
    [Route("CreateUser")]
    public async Task<IActionResult> CreateLinuxUser([FromBody] SshRequestCoreForUserCrud sshRequest)
    {
        using (var sshClient = _sshService.Connect(sshRequest.HostAddress, sshRequest.HostUsername, sshRequest.HostPassword))
        {
            string username = sshRequest.UserUsername;

            await _sshService.ExecuteCommandAsync(sshClient, $"sudo adduser --disabled-password --gecos '' {username}");
            await _sshService.ExecuteCommandAsync(sshClient, $"echo '{username}:{sshRequest.UserPassword}' | sudo chpasswd");

            return Ok();
        }
    }

    [HttpPut]
    [Route("ChangeUserData")]
    public async Task<IActionResult> ChangeLinuxUserData([FromBody] SshRequestCoreForUserCrud sshRequest)
    {
        return Ok();
    }

    /// <summary>
    /// Delete user and user home directory 
    /// </summary>
    /// <param name="sshRequest"></param>
    /// <returns>ok result</returns>
    [HttpDelete]
    [Route("DeleteUser")]
    public async Task<IActionResult> DeleteLinuxUser([FromBody] SshRequestCoreForUserCrud sshRequest)
    {
        using (var sshClient = _sshService.Connect(sshRequest.HostAddress, sshRequest.HostUsername, sshRequest.HostPassword))
        {
            await _sshService.ExecuteCommandAsync(sshClient, $"userdel -r {sshRequest.UserUsername}");

            return Ok();
        }
    }
}