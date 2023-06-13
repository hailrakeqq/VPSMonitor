using System.Diagnostics;
using Renci.SshNet;

namespace VPSMonitor.Core.Application;

public static class CommandExecutor
{
    public static string ExecuteCommand(SshClient sshClient, string command)
    {
        var bashCommand = sshClient.CreateCommand(command);

        return bashCommand.Execute();
    }
}