namespace VPSMonitor.API.Entities;

public class SystemInfo
{
    public string Hostname { get; set; }
    public string OS { get; set; }
    public string Kernel { get; set; }
    public string CpuArchitecture { get; set; }
    public string DateTime { get; set; }
}