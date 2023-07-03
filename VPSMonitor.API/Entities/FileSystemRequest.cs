namespace VPSMonitor.API.Entities;

public class FileSystemRequest
{
    public string Host { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string UserHomeDirectoryPath { get; set; }

}