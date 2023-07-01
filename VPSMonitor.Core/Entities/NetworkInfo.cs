namespace VPSMonitor.Core.Entities;

public class NetworkInfo
{
    public IpV4NetworkInfo IpV4NetworkInfo { get; set; }
    public IpV6NetworkInfo IpV6NetworkInfo { get; set; }
}

public class IpV4NetworkInfo : NetworkInfoBase
{
    public IpV4NetworkInfo(string ipAddress, string netmask, string gateway) : base(ipAddress, netmask, gateway)
    {
    }
}

public class IpV6NetworkInfo : NetworkInfoBase
{
    public IpV6NetworkInfo(string ipAddress, string netmask, string gateway) : base(ipAddress, netmask, gateway)
    {
    }
}


public class NetworkInfoBase
{
    public string IpAddress { get; set; }
    public string Netmask { get; set; }
    public string Gateway { get; set; }

    public NetworkInfoBase(string ipAddress, string netmask, string gateway)
    {
        IpAddress = ipAddress;
        Netmask = netmask;
        Gateway = gateway;
    }
}
