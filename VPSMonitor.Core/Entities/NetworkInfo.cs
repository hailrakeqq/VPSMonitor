namespace VPSMonitor.Core.Entities;

public class NetworkInfo
{
    public IpV4NetworkInfo IpV4NetworkInfo { get; set; }
    public IpV6NetworkInfo IpV6NetworkInfo { get; set; }
}

public class IpV4NetworkInfo : NetworkInfoBase
{ }

public class IpV6NetworkInfo : NetworkInfoBase
{ }

public class NetworkInfoBase
{
    public string IpAddress { get; set; }
    public string Netmask { get; set; }
    public string Gateway { get; set; }
}
