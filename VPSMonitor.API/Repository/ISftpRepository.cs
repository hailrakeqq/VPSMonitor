using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace VPSMonitor.API.Repository;

public interface ISftpRepository
{
    SftpClient Connect(string host, string username, string password);
    void Disconnect(SftpClient sftpClient);
    List<SftpFile> ListAllFilesAndFolders(SftpClient sftpClient, string remoteDirectory);
    void CreateFolder(SftpClient sftpClient, string remoteDirectory, string folderName);
    void UploadFile(SftpClient sftpClient, System.IO.Stream stream, string remoteFilePath);
    void DownloadFile(SftpClient sftpClient, string remoteFilePath, string localFilePath);
    void RenameFileOrFolder(SftpClient sftpClient, string currentPath, string newName);
    void CopyFileOrFolder(SftpClient sftpClient, string sourcePath, string destinationPath);
    void MoveFileOrFolder(SftpClient sftpClient, string sourcePath, string destinationPath);
    void DeleteFileOrFolder(SftpClient sftpClient, string path);
    bool FileExists(SftpClient sftpClient, string path);
    void ChangePermissions(SftpClient sftpClient, string path, string permissions);
}