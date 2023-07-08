namespace VPSMonitor.API.Entities;

public class SftpRequest
{
    public string Host { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string DirectoryPath { get; set; }
    public string LocalFilePath { get; set; }
    public string[] SelectedFiles { get; set; }
    public string newItemName { get; set; }
    public string newItemType { get; set; }
}