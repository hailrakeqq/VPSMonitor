namespace VPSMonitor.Core.Entities;

public class LinuxUser
{
    public string username { get; set; }
    public List<string> permissions { get; set; }
}