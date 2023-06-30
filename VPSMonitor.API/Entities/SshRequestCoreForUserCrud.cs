namespace VPSMonitor.API.Entities;

public class SshRequestCoreForUserCrud
{
    public string HostAddress { get; set; }
    public string HostUsername { get; set; }
    public string HostPassword { get; set; }
    public string UserUsername { get; set; }
    public string UserPassword { get; set; }
    public string UserConfirmPassword { get; set; }
}