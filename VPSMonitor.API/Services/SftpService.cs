using Renci.SshNet;
using Renci.SshNet.Sftp;
using VPSMonitor.API.Repository;
using VPSMonitor.Core.Infrastructure;

namespace VPSMonitor.API.Services;

public class SftpService : ISftpRepository
{
    public SftpClient Connect(string host, string username, string password)
    {
        return SftpConnectionContext.Connect(host, username, password);
    }

    public void Disconnect(SftpClient sftpClient)
    {
        SftpConnectionContext.Disconnect(sftpClient);
    }

    public List<SftpFile> GetAllFilesAndFolders(SftpClient sftpClient, string remoteDirectory)
    {
        return sftpClient.ListDirectory(remoteDirectory).ToList();
    }

    public void CreateFolder(SftpClient sftpClient, string remoteDirectory, string folderName)
    {
        sftpClient.CreateDirectory($"{remoteDirectory}/{folderName}");
    }

    public void UploadFile(SftpClient sftpClient, System.IO.Stream stream, string remoteFilePath)
    {
        sftpClient.UploadFile(stream, remoteFilePath);
    }

    public void DownloadFile(SftpClient sftpClient, string remoteFilePath, string localFilePath)
    {
        using (var fileStream = File.Create(localFilePath))
        {
            sftpClient.DownloadFile(remoteFilePath, fileStream);
        }
    }

    public void RenameFileOrFolder(SftpClient sftpClient, string currentPath, string newName)
    {
        sftpClient.RenameFile(currentPath, newName);
    }

    public void CopyFileOrFolder(SftpClient sftpClient, string sourcePath, string destinationPath)
    {
        throw new NotImplementedException();
    }

    public void MoveFileOrFolder(SftpClient sftpClient, string sourcePath, string destinationPath)
    {
        throw new NotImplementedException();
    }

    public void DeleteFileOrFolder(SftpClient sftpClient, string path)
    {
        sftpClient.Delete(path);
    }

    public bool FileExists(SftpClient sftpClient, string path)
    {
        return sftpClient.Exists(path);
    }

    public void ChangePermissions(SftpClient sftpClient, string path, string permissions)
    {
        var permissionsValue = Convert.ToInt32(permissions, 8);
        sftpClient.ChangePermissions(path, (short)permissionsValue);
    }
}