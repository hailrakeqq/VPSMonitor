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
    
    public SshClient Connect(string host, string username, string sshKey)
    {
        return SshConnectionContext.Connect(host, username, sshKey);
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

    public async Task<List<SshKey>> GetUserSshKeys(CurrentUser currentUser)
    {
        return _context.sshkeys.Where(u => u.UserId == currentUser.Id).ToList();
    }

    public async Task<SshKey> GetSshKeyById(string id)
    {
        return await _context.sshkeys.FirstOrDefaultAsync(ssh => ssh.Id == id);
    }

    public async Task Create(SshKey sshKey)
    {
        await _context.sshkeys.AddAsync(sshKey);
        Save();
    }

    public async Task Update(SshKey newSshKey)
    {
        var existedSshKey = await GetSshKeyById(newSshKey.Id);
        if (existedSshKey != null)
        {
            existedSshKey.Ssh = newSshKey.Ssh;
            existedSshKey.Username = newSshKey.Username;
            
            Save();
        }

    }

    public async Task Delete(string id)
    {
        var sshKey = await _context.sshkeys.FirstOrDefaultAsync(sshKey => sshKey.Id == id);
        
        if (sshKey != null)
            _context.sshkeys.Remove(sshKey);

        Save();
    }

    private void Save()
    {
        _context.SaveChangesAsync();
    } 
}
