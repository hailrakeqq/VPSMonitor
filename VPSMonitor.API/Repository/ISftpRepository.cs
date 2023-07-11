using Renci.SshNet;
using Renci.SshNet.Sftp;
using VPSMonitor.API.Entities;

namespace VPSMonitor.API.Repository;

public interface ISftpRepository
{
    SftpClient Connect(string host, string username, string password);
    void Disconnect(SftpClient sftpClient);
    List<SftpFile> GetAllFilesAndFolders(SftpClient sftpClient, string remoteDirectory);
    Task<byte[]> CreateZipArchiveAsync(SftpRequest sftpRequest, SftpClient client, Stream stream);
    Task<byte[]> DownloadFileAsync(SftpClient client, string file, Stream stream);
    void CreateFolder(SftpClient sftpClient, string remoteDirectory, string folderName);
    void CreateFile(SftpClient sftpClient, string remoteDirectory, string fileName);
    void UploadItem(SftpClient sftpClient, Stream stream, string remoteFilePath);
    void RenameItem(SftpClient sftpClient, string currentPath, string newName);
    void CopyItem(SshClient sshClient, string sourcePath, string destinationPath);
    void MoveItem(SshClient sshClient, string sourcePath, string destinationPath);
    void DeleteItem(SftpClient client, string[] itemsToDelete);
    bool IsFileExists(SftpClient sftpClient, string path);
    void ChangePermissions(SftpClient sftpClient, string path, string permissions);
}