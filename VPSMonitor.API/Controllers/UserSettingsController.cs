using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VPSMonitor.API.Entities;
using VPSMonitor.API.Repository;
using VPSMonitor.API.Services;

namespace VPSMonitor.API.Controllers;

[Authorize]
[ApiController]
[Route("/api/[controller]")]
public class UserSettingsController : Controller
{
    public readonly IUserRepository _userRepository;

    public UserSettingsController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    [HttpPut]
    [Route("ChangeUserData/{id:Guid}")]
    public async Task<IActionResult> ChangeUserData([FromRoute] string id, [FromBody] UserChangeDataModel newUserData)
    {
        var existingUser = await _userRepository.GetItemById(id);

        if (existingUser != null)
        {
            if (existingUser.Password == Toolchain.GenerateHash(newUserData.ConfirmPassword))
            {
                if (!string.IsNullOrEmpty(newUserData.Email))
                {
                    var userWithNewEmail = await _userRepository.GetUserByEmail(newUserData.Email);
                    if (userWithNewEmail != null)
                        return Conflict("User with this email already exist.");
                    
                    existingUser.Email = newUserData.Email;
                }

                if (!string.IsNullOrEmpty(newUserData.Password))
                {
                    if (existingUser.Password == Toolchain.GenerateHash(newUserData.Password))
                        return Conflict("You cannot set the password you had before.");
                    existingUser.Password = Toolchain.GenerateHash(newUserData.Password);
                }
                
                _userRepository.Update(existingUser);

                return Ok("Your user data has been changed successfully.");
            }
            
            return Unauthorized("You entered the wrong password.");
        }

        return NotFound();
    }
    
    [HttpDelete]
    [Route("DeleteAccount/{id:Guid}")]
    public async Task<IActionResult> DeleteUserAccount([FromRoute] string userId, [FromBody] string confirmPassword)
    {
        var user = await _userRepository.GetItemById(userId);
        if (user != null && Toolchain.GenerateHash(confirmPassword) == user.Password)
        {
            await _userRepository.Delete(user.Id);
            return Ok("User account was seccessfully deleted.");
        }
        
        return NotFound();
    }
}