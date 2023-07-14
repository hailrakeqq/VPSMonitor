using System.IO.Compression;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using VPSMonitor.API.Entities;
using VPSMonitor.API.Repository;
using VPSMonitor.Core.Infrastructure;

namespace VPSMonitor.API.Services;

public class SftpService : ISftpRepository
{
    #region sftp connection functio
    public SftpClient Connect(string host, string username, string password)
    {
        return SftpConnectionContext.Connect(host, username, password);
    }

    public void Disconnect(SftpClient sftpClient)
    {
        SftpConnectionContext.Disconnect(sftpClient);
    }

    #endregion

    #region Download files and directory

    public async Task<byte[]> DownloadFileAsync(SftpClient client, string file, Stream stream)
    {
        await Task.Run(() => client.DownloadFile(file, stream));

        stream.Position = 0;

        byte[] fileBytes = ((MemoryStream)stream).ToArray();
        return fileBytes;
    }

    public async Task<byte[]> CreateZipArchiveAsync(SftpRequest sftpRequest, SftpClient client, Stream stream)
    {
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        string zipPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".zip");

        try
        {
            await DownloadFilesAsync(sftpRequest.SelectedFiles, client, tempDir);

            using (FileStream zipStream = new FileStream(zipPath, FileMode.Create))
            using (ZipArchive zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Create))
            {
                string[] files = Directory.GetFiles(tempDir, "*", SearchOption.AllDirectories);

                foreach (string file in files)
                {
                    string relativePath = Path.GetRelativePath(tempDir, file);
                    zipArchive.CreateEntryFromFile(file, relativePath, CompressionLevel.Optimal);
                }
            }

            stream.Position = 0;

            using (FileStream zipFileStream = File.OpenRead(zipPath))
            {
                byte[] zipBytes = new byte[zipFileStream.Length];
                await zipFileStream.ReadAsync(zipBytes.AsMemory(0, zipBytes.Length));
                return zipBytes;
            }
        }
        finally
        {
            Cleanup(tempDir, zipPath);
        }
    }

    private async Task DownloadFilesAsync(string[] files, SftpClient client, string destinationDirectory)
    {
        foreach (var file in files)
        {
            string itemName = Path.GetFileName(file);
            string filePath = Path.Combine(destinationDirectory, itemName);

            if (!Path.HasExtension(file))
            {
                string[] splitedFile = file.Split('/');
                string directoryName = splitedFile[splitedFile.Length - 1];
                string subDirectory = Path.Combine(destinationDirectory, directoryName);
                Directory.CreateDirectory(subDirectory);
                await DownloadFilesRecursivelyAsync(client, file, subDirectory);
            }
            else
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                    client.DownloadFile(file, fileStream);
            }
        }
    }

    private async Task DownloadFilesRecursivelyAsync(SftpClient client, string remotePath, string localPath)
    {
        Directory.CreateDirectory(localPath);
        var remoteFiles = client.ListDirectory(remotePath);

        foreach (var remoteFile in remoteFiles)
        {
            if (remoteFile.FullName[remoteFile.FullName.Length - 1] == '.')
                continue;

            string itemName = remoteFile.Name;
            string itemRemotePath = remoteFile.FullName;
            string itemLocalPath = Path.Combine(localPath, itemName);

            if (remoteFile.IsDirectory)
                await DownloadFilesRecursivelyAsync(client, itemRemotePath, itemLocalPath);
            else
            {
                using (var fileStream = new FileStream(itemLocalPath, FileMode.Create))
                    client.DownloadFile(itemRemotePath, fileStream);
            }
        }
    }

    private static void Cleanup(string tempDir, string zipPath)
    {
        Directory.Delete(tempDir, true);
        System.IO.File.Delete(zipPath);
    }

    #endregion

    #region Default filesystem function 

    public List<SftpFile> GetAllFilesAndFolders(SftpClient sftpClient, string remoteDirectory)
    {
        SftpFileAttributes attributes = sftpClient.GetAttributes(remoteDirectory);
        if (attributes.IsDirectory)
        {
            var directory = sftpClient.ListDirectory(remoteDirectory).ToList();
            if (directory.Any())
                return directory;
        }

        return null;
    }

    public void CreateFolder(SftpClient sftpClient, string remoteDirectory, string folderName)
    {
        sftpClient.CreateDirectory($"{remoteDirectory}/{folderName}");
    }

    public void CreateFile(SftpClient sftpClient, string remoteDirectory, string fileName)
    {
        sftpClient.Create($"{remoteDirectory}/{fileName}");
    }

    public void UploadItem(SftpClient sftpClient, Stream stream, string remoteFilePath)
    {
        sftpClient.UploadFile(stream, remoteFilePath);
    }

    public void RenameItem(SftpClient sftpClient, string currentPath, string newName)
    {
        sftpClient.RenameFile(currentPath, newName);
    }

    public void CopyItem(SshClient sshClient, string sourcePath, string destinationPath)
    {
        sshClient.RunCommand($"cp {sourcePath} {destinationPath}");
    }
    public void MoveItem(SshClient sshClient, string sourcePath, string destinationPath)
    {
        sshClient.RunCommand($"mv {sourcePath} {destinationPath}");
    }

    #region Delete File Or Folder
    public void DeleteItem(SftpClient client, string[] itemsToDelete)
    {
        foreach (var item in itemsToDelete)
        {
            if (!IsFileExists(client, item))
                continue;

            if (Path.HasExtension(item))
            {
                client.Delete(item);
                continue;
            }

            DeleteDirectoryRecursive(client, item);
        }
    }

    private void DeleteDirectoryRecursive(SftpClient client, string directory)
    {
        var directoryContents = client.ListDirectory(directory);

        foreach (var entry in directoryContents)
        {
            if (entry.Name == "." || entry.Name == "..")
                continue;

            var entryPath = Path.Combine(directory, entry.Name);

            if (entry.IsDirectory)
                DeleteDirectoryRecursive(client, entryPath);
            else
                client.DeleteFile(entryPath);
        }

        client.DeleteDirectory(directory);
    }

    #endregion

    public bool IsFileExists(SftpClient sftpClient, string path)
    {
        return sftpClient.Exists(path);
    }

    public void ChangePermissions(SftpClient sftpClient, string path, string permissions)
    {
        var permissionsValue = Convert.ToInt32(permissions, 8);
        sftpClient.ChangePermissions(path, (short)permissionsValue);
    }

    #endregion  
}
