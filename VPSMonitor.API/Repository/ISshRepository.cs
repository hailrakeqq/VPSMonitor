using Renci.SshNet;
using VPSMonitor.API.Entities;

namespace VPSMonitor.API.Repository;

public interface ISshRepository
{
    SshClient Connect(string host, string username, string password);
    void Disconnect(SshClient sshClient);
    Task<string> ExecuteCommandAsync(SshClient sshClient, string command);
}