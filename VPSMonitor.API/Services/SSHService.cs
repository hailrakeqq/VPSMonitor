using Renci.SshNet;
using VPSMonitor.Core.Infrastructure;

namespace VPSMonitor.API.Services;

public class SSHService
{
    public SshClient Connect(string host, string username, string password)
    {
        //TODO: Зробити конект через SSH ключі
        return SSHConnectionContext.Connect(host, username, password);
    }
    
    public async Task<string> ExecuteCommandAsync(SshClient sshClient, string command)
    {
        var commandResult = await Task.Run(() => sshClient.RunCommand(command));
        return commandResult.Result;
    }

    public void Disconnect(SshClient sshClient)
    {
        SSHConnectionContext.Disconect(sshClient);
    }
}