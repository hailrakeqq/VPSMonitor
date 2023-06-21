using Microsoft.AspNetCore.Mvc;
using VPSMonitor.API.Entities;
using VPSMonitor.API.Repository;
using VPSMonitor.API.Services;

namespace VPSMonitor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : Controller
{
    private readonly IUserRepository _userService;
    private readonly ITokenService _tokenService;
    public AuthController(IUserRepository userService, ITokenService tokenService)
    {
        _userService = userService;
        _tokenService = tokenService;
    }
    
    [HttpPost]
    [Route("Registration")]
    public async Task<IActionResult> CreateUser([FromBody] User user)
    {
        //TODO: Give better name for variable
        var isUserExist = await _userService.GetUserByEmail(user.Email);

        if (isUserExist == null)
        {
            user = new User()
            {
                Id = Guid.NewGuid().ToString(),
                Email = user.Email.ToLower(),
                Password = Toolchain.GenerateHash(user.Password)
            };
            var userRefreshToken = new AuthRefreshToken
            {
                Id = Guid.NewGuid().ToString(),
                UserId = user.Id
            };

            await _userService.Create(user);
            await _tokenService.CreateRefreshTokenDocumentAsync(userRefreshToken);
            
            _userService.Save();

            return Ok(user);
        }

        return Conflict("user already exist.");
    }
    
    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login([FromBody] UserLoginModel userLoginModel)
    {
        var currentUser = await _userService.ValidateUserLoginModel(userLoginModel);

        if (currentUser == null)
            return Unauthorized(currentUser);

        var newRefreshToken = _tokenService.GenerateRefreshToken(currentUser.Id!);
        await _tokenService.UpdateRefreshTokenByUserId(newRefreshToken, currentUser.Id!);
        var loginResponse = new LoginResponse
        {
            Id = currentUser.Id,
            Email = currentUser.Email,
            AccessToken = _tokenService.GenerateAccessToken(currentUser),
            RefreshToken = newRefreshToken.RefreshToken
        };
         
        return Ok(loginResponse);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromHeader] string userId, [FromHeader] string refreshToken)
    {
        var user = await _userService.GetItemById(userId);
        var userRefreshTokenDocument = _tokenService.GetRefreshTokenDocumentById(user.Id!);

        if (userRefreshTokenDocument.RefreshToken != refreshToken && userRefreshTokenDocument.TokenExpires < DateTime.UtcNow)
            return Unauthorized("Invalid refresh token.");

        var newAccessToken = _tokenService.GenerateAccessToken(user);
        return Ok(newAccessToken);
    }
}