using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VPSMonitor.API.Entities;
using VPSMonitor.API.Repository;

namespace VPSMonitor.API.Controllers;

[Authorize]
[ApiController]
[Route("/api/[controller]")]
public class SshController : Controller
{
    private readonly IUserRepository _userRepository;
    private readonly ISshRepository _sshRepository;

    public SshController(IUserRepository userRepository, ISshRepository sshRepository)
    {
        _userRepository = userRepository;
        _sshRepository = sshRepository;
    }

    [HttpGet]
    [Route("GetUserSshKeys")]
    public async Task<IActionResult> GetUserSshKeys(CurrentUser currentUser)
    {
        var sshCollection = await _sshRepository.GetUserSshKeys(currentUser);
        return Ok(sshCollection);
    }

    [HttpGet]
    [Route("GetSshKey/{id:Guid}")]
    public async Task<IActionResult> GetSshKeyById([FromRoute] string id, CurrentUser currentUser)
    {
        var sshKey = await _sshRepository.GetSshKeyById(id);
        if (sshKey.UserId == currentUser.Id)
            return Ok(sshKey);
        
        return Forbid();
    }
    
    
    [HttpPost]
    [Route("AddSshKey/{id:Guid}")]
    public async Task<IActionResult> AddSshKeyToUserAccount([FromRoute] string id, [FromBody] SshKey sshKeyObject)
    {
        var user = await _userRepository.GetItemById(id);
        if (user != null)
        {
            sshKeyObject.Id = Guid.NewGuid().ToString();
            sshKeyObject.UserId = id;

            _sshRepository.Create(sshKeyObject);
            return Ok("SSH Key Was Successfully Added.");
        }
        
        return NotFound();
    }

    [HttpPut]
    [Route("ChangeSshKey/{id:Guid}")]
    public async Task<IActionResult> ChangeSshKey([FromRoute] string id, 
                                                  [FromBody] SshKey updatedSshKey,
                                                  CurrentUser currentUser)
    {
        var sshKey = await _sshRepository.GetSshKeyById(id);
        if (sshKey.UserId == currentUser.Id)
        {
            await _sshRepository.Update(updatedSshKey);
        }
        return Forbid();
    }

    [HttpDelete]
    [Route("Delete/{id:Guid}")]
    public async Task<IActionResult> DeleteSshKey([FromRoute] string id, CurrentUser currentUser)
    {
        var sshKey = await _sshRepository.GetSshKeyById(id);
        if (sshKey.UserId == currentUser.Id)
            await _sshRepository.Delete(id);
        
        return Forbid();
    }
}