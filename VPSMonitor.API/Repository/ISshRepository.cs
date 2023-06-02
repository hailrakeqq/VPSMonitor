using Renci.SshNet;
using VPSMonitor.API.Entities;

namespace VPSMonitor.API.Repository;

public interface ISshRepository
{
    SshClient Connect(string host, string username, string sshKey);
    void Disconnect(SshClient sshClient);
    Task<string> ExecuteCommandAsync(SshClient sshClient, string command);
    Task<List<SshKey>> GetUserSshKeys(CurrentUser currentUser);
    Task<SshKey> GetSshKeyById(string id);
    Task Create(SshKey sshKey);
    Task Update(SshKey newSshKey);
    Task Delete(string id);
}