using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using VPSMonitor.API.Entities;
using VPSMonitor.Core.Entities;

namespace VPSMonitor.Core;

public static class Parser
{
    public static SystemInfo ParseSystemInfoData(List<string> outputCommands)
    {
        return new SystemInfo()
        {
            Hostname = outputCommands[0],
            OS = outputCommands[1].Split("\"")[1],
            Kernel = outputCommands[2],
            CpuArchitecture = outputCommands[3],
            DateTime = outputCommands[4]
        };
    }

    public static string[] mpstatCommandParse(string data)
    {
        var jsonObject = JObject.Parse(data);
        JArray cpuLoadArray = (JArray)jsonObject["sysstat"]["hosts"][0]["statistics"][0]["cpu-load"];
        var idleValues = cpuLoadArray.Select(item => (double)item["idle"]).ToArray();
        string[] result = new string[idleValues.Length];

        for (int i = 0; i < idleValues.Length; i++)
        {
            if (i == 0)
            {
                result[i] = $"ALL:    {Math.Round(100 - idleValues[i], 2)}%";
                continue;
            }
            result[i] = $"{i - 1}:    {Math.Round(100 - idleValues[i], 2)}%";
        }

        return result;
    }

    public static List<string> permissionParse(string permissionValue)
    {
        List<string> parsedPermission = new List<string>();
        /* example:
            d rwx r-x---
            |rwx| - user permissions
            r - read 
            w - write
            x - execute
            - - no permission 
        */
        for (int i = 1; i <= 3; i++)
        {
            switch (permissionValue[i])
            {
                case 'x':
                    parsedPermission.Add("Execute");
                    continue;
                case 'w':
                    parsedPermission.Add("Write");
                    continue;
                case 'r':
                    parsedPermission.Add("Read");
                    continue;
            }
        }

        return parsedPermission;
    }

    public static List<LinuxUser> parseGetUserCommand(string usernameAndHomeDirectoryPathOutput, string userIdOutput)
    {
        var linesOfUsernameAndHomeDirectory = usernameAndHomeDirectoryPathOutput.Split("\n");
        var linesOfUserId = userIdOutput.Split("\n");
        var users = new List<LinuxUser>();

        for (int i = 0; i < linesOfUserId.Length; i++)
        {
            string[] fieldsOfOfUsernameAndHomeDirectory = linesOfUsernameAndHomeDirectory[i].Split(" ");

            if (fieldsOfOfUsernameAndHomeDirectory[0] != "" && linesOfUserId[i] != "")
            {
                string userId = linesOfUserId[i].Split(':')[1];
                var user = new LinuxUser()
                {
                    Id = userId,
                    Username = fieldsOfOfUsernameAndHomeDirectory[2],
                    Permissions = Parser.permissionParse(fieldsOfOfUsernameAndHomeDirectory[0]),
                    HomeDirectoryPath = fieldsOfOfUsernameAndHomeDirectory.Where(x => x.StartsWith("/")).FirstOrDefault()
                };

                users.Add(user);
            }
        }
        return users;
    }

    public static NetworkInfo ParseNetworkInfo(string ipAddressCommandOutput, string gatewayCommandOutput, string gatewayIpV6CommandOutput)
    {
        string ipAddress = ExtractIpAddress(ipAddressCommandOutput);
        string ipAddressIpV6 = ExtractIpAddressIpv6(ipAddressCommandOutput);
        string[] parsedIpV6 = ipAddressIpV6.Split('/');
        return new NetworkInfo()
        {
            IpV4NetworkInfo = new IpV4NetworkInfo()
            {
                IpAddress = ipAddress,
                Gateway = ExtractGateway(gatewayCommandOutput),
                Netmask = ExtractNetmask(ipAddressCommandOutput, ipAddress)
            },
            IpV6NetworkInfo = new IpV6NetworkInfo()
            {
                IpAddress = parsedIpV6[0],
                Gateway = ExtractGatewayIpv6(gatewayIpV6CommandOutput),
                Netmask = $"/{ipAddressIpV6.Split('/')[1]}"
            }
        };
    }

    private static string ExtractIpAddress(string ipAddressCommandOutput)
    {
        MatchCollection ipAddressMatches = Regex.Matches(ipAddressCommandOutput, @"inet (\d+\.\d+\.\d+\.\d+)");

        foreach (Match match in ipAddressMatches)
        {
            string address = match.Groups[1].Value;
            if (!IsLocalAddress(address))
                return address;
        }

        return "";
    }

    private static string ExtractNetmask(string ipAddressCommandOutput, string ipAddress)
    {
        Match netmaskMatch = Regex.Match(ipAddressCommandOutput, $@"inet {ipAddress}/(?<Netmask>\d+)");

        if (netmaskMatch.Success)
        {
            int integerNetmask = Convert.ToInt32(netmaskMatch.Groups["Netmask"].Value);
            string netmask = ConvertPrefixLengthToMaskV4(integerNetmask) + $"/{integerNetmask}";
            return netmask;
        }

        return "";
    }

    private static string ExtractGateway(string commandOutput)
    {
        Match match = Regex.Match(commandOutput, @"default via (\d+\.\d+\.\d+\.\d+)");
        return match.Success ? match.Groups[1].Value : "";
    }

    private static string ExtractIpAddressIpv6(string ipAddressCommandOutput)
    {
        Match match = Regex.Match(ipAddressCommandOutput, @"inet6 ([\w:]+/\d+) scope global");
        return match.Success ? match.Groups[1].Value : "";
    }

    private static string ExtractGatewayIpv6(string commandOutput)
    {
        Match match = Regex.Match(commandOutput, @"default via ([\da-fA-F:]+)");
        return match.Success ? match.Groups[1].Value : "";
    }

    private static bool IsLocalAddress(string ipAddress)
    {
        return ipAddress.StartsWith("127.") || ipAddress.StartsWith("192.168.") ? true : false;
    }

    private static string ConvertPrefixLengthToMaskV4(int prefixLength)
    {
        if (prefixLength < 0 || prefixLength > 32)
        {
            throw new ArgumentException("Invalid prefix length. The value must be between 0 and 32.");
        }
        int mask = (int)(uint.MaxValue << (32 - prefixLength));

        byte[] octets = BitConverter.GetBytes(mask);

        Array.Reverse(octets);

        return string.Join(".", octets);
    }
}