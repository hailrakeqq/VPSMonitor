using Renci.SshNet;

namespace VPSMonitor.Core.Infrastructure;

public class VpsConnectionContext
{
    public SshClient Connect(string host, string username, string password)
    {
        var connectionInfo =
            new ConnectionInfo(host, username, new PasswordAuthenticationMethod(username, password));
        var sshClient = new SshClient(connectionInfo);
        
        sshClient.Connect();
        return sshClient;
    }

    public void Disconect(SshClient sshClient)
    {
        if(sshClient.IsConnected)
            sshClient.Disconnect();
        
        sshClient.Dispose();
    }
}