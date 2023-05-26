namespace VPSMonitor.API.Entities;

public class AuthRefreshToken
{
    public string? Id { get; set; }
    public string? UserId { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime TokenCreated { get; set; } = DateTime.UtcNow;
    public DateTime TokenExpires { get; set; } = DateTime.UtcNow.AddDays(7);
}