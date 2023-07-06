using Renci.SshNet;
using Renci.SshNet.Sftp;
using VPSMonitor.API.Entities;

namespace VPSMonitor.API.Repository;

public interface ISftpRepository
{
    SftpClient Connect(string host, string username, string password);
    void Disconnect(SftpClient sftpClient);
    List<SftpFile> GetAllFilesAndFolders(SftpClient sftpClient, string remoteDirectory);
    Task<byte[]> CreateZipArchive(SftpRequest sftpRequest, SftpClient client, Stream stream);
    Task<byte[]> DownloadFile(SftpClient client, string file, Stream stream);
    void CreateFolder(SftpClient sftpClient, string remoteDirectory, string folderName);
    void UploadFile(SftpClient sftpClient, Stream stream, string remoteFilePath);
    void RenameFileOrFolder(SftpClient sftpClient, string currentPath, string newName);
    void CopyFileOrFolder(SftpClient sftpClient, string sourcePath, string destinationPath);
    void MoveFileOrFolder(SftpClient sftpClient, string sourcePath, string destinationPath);
    void DeleteFileOrFolder(SftpClient sftpClient, string path);
    bool FileExists(SftpClient sftpClient, string path);
    void ChangePermissions(SftpClient sftpClient, string path, string permissions);
}