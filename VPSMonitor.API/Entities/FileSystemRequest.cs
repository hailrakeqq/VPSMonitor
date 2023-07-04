namespace VPSMonitor.API.Entities;

public class FileSystemRequest
{
    public string Host { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string DirectoryPath { get; set; }

    public IFormFileCollection Files { get; set; }

}