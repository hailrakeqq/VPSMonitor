namespace VPSMonitor.Core.Entities;

public class LinuxUser
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string HomeDirectoryPath { get; set; }
    public List<string> Permissions { get; set; }
}