using System.Text;
using Renci.SshNet;

namespace VPSMonitor.Core.Infrastructure;

public static class SshConnectionContext
{
    public static SshClient Connect(string host, string username, string sshKey)
    {
        var sshKeyFile = new PrivateKeyFile(new MemoryStream(Encoding.ASCII.GetBytes(sshKey)));
        var connectionInfo =
            new ConnectionInfo(host, username, new PrivateKeyAuthenticationMethod(username, sshKeyFile));
        var sshClient = new SshClient(connectionInfo);
        
        sshClient.Connect();
        return sshClient;
    }

    public static void Disconect(SshClient sshClient)
    {
        if(sshClient.IsConnected)
            sshClient.Disconnect();
        
        sshClient.Dispose();
    }
}