using Microsoft.EntityFrameworkCore;
using Renci.SshNet;
using VPSMonitor.API.Entities;
using VPSMonitor.API.Repository;
using VPSMonitor.Core.Infrastructure;

namespace VPSMonitor.API.Services;

public class SshService : ISshRepository
{
    private readonly ApplicationDbContext _context;

    public SshService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public SshClient Connect(string host, string username, string password)
    {
        return SshConnectionContext.Connect(host, username, password);
    }
    
    public void Disconnect(SshClient sshClient)
    {
        SshConnectionContext.Disconect(sshClient);
    }
    
    public async Task<string> ExecuteCommandAsync(SshClient sshClient, string command)
    {
        var commandResult = await Task.Run(() => sshClient.RunCommand(command));
        return commandResult.Result;
    }
}
