using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using VPSMonitor.API.Entities;

namespace VPSMonitor.API.Services;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    AuthRefreshToken GenerateRefreshToken(string userId);
    AuthRefreshToken GetRefreshTokenDocumentById(string id);
    Task<bool> UpdateRefreshTokenByUserId(AuthRefreshToken refreshTokenDocument,string id);
    Task CreateRefreshTokenDocumentAsync(AuthRefreshToken refreshTokenDocument);
}

public sealed class TokenService : ITokenService
{
    private readonly IConfiguration _config;
    private readonly ApplicationDbContext _context;

    public TokenService(IConfiguration config, ApplicationDbContext context)
    {
        _config = config;
        _context = context;
    }

    public string GenerateAccessToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id!),
            new Claim(ClaimTypes.Email, user.Email!),
        };

        var token = new JwtSecurityToken(_config["Jwt:Issuer"],
            _config["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddMinutes(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public AuthRefreshToken GenerateRefreshToken(string userId)
    {
        var userRefreshTokenDocument = _context.refreshtokens.FirstOrDefault(u => u.UserId == userId); 
        
        if (userRefreshTokenDocument != null)
        {
            userRefreshTokenDocument.RefreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            return userRefreshTokenDocument;
        }
        
        return null;
    }

    public AuthRefreshToken GetRefreshTokenDocumentById(string userId)
    {
        var refreshTokenDocument = _context.refreshtokens.FirstOrDefault(t => t.UserId == userId);
        
        return refreshTokenDocument != null ? refreshTokenDocument : null!;
    }

    public async Task<bool> UpdateRefreshTokenByUserId(AuthRefreshToken newRefreshToken,string userId)
    {
        var existingRefreshToken = await _context.refreshtokens.FirstOrDefaultAsync(t => t.UserId == userId);

        if (existingRefreshToken != null)
        {
            existingRefreshToken.RefreshToken = newRefreshToken.RefreshToken;
            existingRefreshToken.TokenCreated = newRefreshToken.TokenCreated;
            existingRefreshToken.TokenExpires = newRefreshToken.TokenExpires;

            await _context.SaveChangesAsync();

            return true;
        }

        return false;
    }

    public async Task CreateRefreshTokenDocumentAsync(AuthRefreshToken refreshTokenDocument)
    {
        await _context.refreshtokens.AddAsync(refreshTokenDocument);
    }
}