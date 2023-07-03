using Renci.SshNet;

namespace VPSMonitor.Core.Infrastructure;

public static class SshConnectionContext
{
    public static SshClient Connect(string host, string username, string password)
    {
        var connectionInfo =
            new ConnectionInfo(host, username, new PasswordAuthenticationMethod(username, password));
        var sshClient = new SshClient(connectionInfo);

        sshClient.Connect();
        return sshClient;
    }

    public static void Disconnect(SshClient sshClient)
    {
        sshClient.Disconnect();
        sshClient.Dispose();
    }
}