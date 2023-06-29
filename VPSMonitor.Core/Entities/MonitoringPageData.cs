using VPSMonitor.Core.Entities;

namespace VPSMonitor.API.Entities;

public class MonitoringPageData
{
    public SystemInfo SystemInfo { get; set; }
    public string[] CpuUsageInfo { get; set; }
    public string RamUsageInfo { get; set; }
    public string DiscpartUsageInfo { get; set; }
    public NetworkInfo NetworkInfo { get; set; }
}