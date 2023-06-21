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
    public async Task<IActionResult> CreateLinuxUser([FromBody] SshRequest sshRequest)
    {
        
        return Ok();
    }

    [HttpPut]
    [Route("ChangeUserData")]
    public async Task<IActionResult> ChangeLinuxUserData()
    {

        return Ok();
    }

    [HttpDelete]
    [Route("DeleteUser")]
    public async Task<IActionResult> DeleteLinuxUser()
    {
        return Ok();
    }
}