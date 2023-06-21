using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using VPSMonitor.Core.Entities;

namespace VPSMonitor.Core;

public static class Parser
{
    public static string freeCommandParse(string command)
    {
        //example string: 
        //total used free shared buff / cache available Mem: 1.9Gi 327Mi 611Mi 16Mi 1.0Gi 1.4Gi Swap: 975Mi 89Mi 886Mi 
        string[] result = command.Split(": ")[1].Split('i');
        return $"{result[1].Trim()} / {result[0].Trim()}";
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

    public static string dfCommandParse(string command)
    {
        string[] lines = command.Trim().Split('\n');

        foreach (string line in lines)
        {
            string[] columns = line.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            Regex rootDirectoryRegex = new Regex(@"^/\s*$");
            if (rootDirectoryRegex.IsMatch(columns[columns.Length - 1]))
            {
                return $"{columns[2]} / {columns[1]}";
            }
        }

        return "Can't find root dir";
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

    public static List<LinuxUser> parseGetUserCommand(string input)
    {
        var lines = input.Split("\n");
        var users = new List<LinuxUser>();
        foreach (var line in lines)
        {
            string[] fields = line.Split(" ");
            if (fields[0] != "")
            {
                var user = new LinuxUser()
                {
                    username = fields[2],
                    permissions = Parser.permissionParse(fields[0])
                };

                int number;
                if (Int32.TryParse(fields[2], out number))
                {
                    user.username = "root";
                }
               
                users.Add(user);
            }
        }

        return users;
    }
}