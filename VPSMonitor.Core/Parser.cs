using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

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

    public static string mpstatCommandParse(string data)
    {
        var jsonObject = JObject.Parse(data);
        var cpuLoad = jsonObject["sysstat"]
                                ["hosts"][0]
                                ["statistics"][0]
                                ["cpu-load"][0]["idle"].Value<double>();

        return $"{Math.Round(100 - cpuLoad, 2).ToString()}%";
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
}