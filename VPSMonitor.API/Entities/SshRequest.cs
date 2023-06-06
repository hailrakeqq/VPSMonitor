namespace VPSMonitor.API.Entities;

public class SshRequest
{
    public string Host { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string? Command { get; set; }
}