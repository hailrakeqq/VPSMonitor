using System.Security.Claims;

namespace VPSMonitor.API.Entities;

public class CurrentUser
{
    public User? User { get; set; }
    public ClaimsPrincipal Principal { get; set; } = default!;

    public string Id => Principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
    public string Email => Principal.FindFirstValue(ClaimTypes.Email)!;
    public string Username => Principal.FindFirstValue(ClaimTypes.Name)!;
    public string Role => Principal.FindFirstValue(ClaimTypes.Role)!;
}