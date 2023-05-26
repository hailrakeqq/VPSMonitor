namespace VPSMonitor.API.Entities;

public class LoginResponse
{
    public string? Id { get; set; }
    public string? Email { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
}