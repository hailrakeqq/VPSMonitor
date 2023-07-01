using System.Text.RegularExpressions;
using VPSMonitor.Core.Entities;

namespace VPSMonitor.Core
{
    public static class Parser
    {
        public static string GetOSName(string osReleaseOutput)
        {
            var match = Regex.Match(osReleaseOutput, @"PRETTY_NAME=\""(.*?)\""");
            return match.Success ? match.Groups[1].Value : "";
        }

        public static string[] MpstatCommandParse(string data)
        {
            var lines = data.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var cpuLoads = new List<string>();

            for (int i = 0; i < lines.Length; i++)
            {
                var fields = lines[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (fields[0] == "Linux" || fields[2] == "CPU")
                    continue;

                if (i == 2)
                {
                    cpuLoads.Add($"ALL: {Math.Round((100 - double.Parse(fields[12])), 2)}%");
                    continue;
                }

                else if (fields.Length >= 12)
                    cpuLoads.Add($"{i - 3}: {Math.Round((100 - double.Parse(fields[12])), 2)}%");
            }

            return cpuLoads.ToArray();
        }

        /// <summary>
        /// example:
        /// d rwx r-x---
        ///  |rwx| - user permissions
        /// r - read
        /// w - write
        /// x - execute
        /// - - no permission
        /// </summary>
        /// <param name="permissionValue"></param>
        /// <returns>
        /// List of user permissions:
        /// example: ["Read", "Write", "Execute"]
        /// </returns>
        public static List<string> permissionParse(string permissionValue)
        {
            List<string> parsedPermission = new List<string>();
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

        public static async Task<List<LinuxUser>> ParseGetUserCommandAsync(string usernameAndHomeDirectoryPathOutput, string userIdOutput)
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
                        Id = Convert.ToInt32(userId),
                        Username = fieldsOfOfUsernameAndHomeDirectory[2],
                        Permissions = permissionParse(fieldsOfOfUsernameAndHomeDirectory[0]),
                        HomeDirectoryPath = fieldsOfOfUsernameAndHomeDirectory.FirstOrDefault(x => x.StartsWith("/"))
                    };

                    users.Add(user);
                }
            }
            return users;
        }

        public static async Task<NetworkInfo> ParseNetworkInfoAsync(string ipAddressCommandOutput, string gatewayCommandOutput, string gatewayIpV6CommandOutput)
        {
            string ipAddress = await Task.Run(() => ExtractIpAddress(ipAddressCommandOutput));
            string ipAddressIpV6 = await Task.Run(() => ExtractIpAddressIpv6(ipAddressCommandOutput));
            string[] parsedIpV6 = ipAddressIpV6.Split('/');

            return new NetworkInfo()
            {
                IpV4NetworkInfo = new IpV4NetworkInfo(ipAddress, await Task.Run(() => ExtractGateway(gatewayCommandOutput)), await Task.Run(() => ExtractNetmask(ipAddressCommandOutput, ipAddress))),
                IpV6NetworkInfo = new IpV6NetworkInfo(parsedIpV6[0], await Task.Run(() => ExtractGatewayIpv6(gatewayIpV6CommandOutput)), $"/{parsedIpV6[1]}")
            };
        }

        private static string ExtractIpAddress(string ipAddressCommandOutput)
        {
            MatchCollection ipAddressMatches = Regex.Matches(ipAddressCommandOutput, @"inet (\d+\.\d+\.\d+\.\d+)");

            foreach (Match match in ipAddressMatches)
            {
                string address = match.Groups[1].Value;
                if (!IsLocalAddress(match.Groups[1].Value))
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
            return ipAddress.StartsWith("127.") || ipAddress.StartsWith("192.168.");
        }

        private static string ConvertPrefixLengthToMaskV4(int prefixLength)
        {
            if (prefixLength < 0 || prefixLength > 32)
            {
                throw new ArgumentException("Invalid prefix length. The value must be between 0 and 32.");
            }

            uint mask = (uint)(uint.MaxValue << (32 - prefixLength));
            byte[] octets = BitConverter.GetBytes(mask);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(octets);
            }

            return string.Join(".", octets);
        }
    }
}