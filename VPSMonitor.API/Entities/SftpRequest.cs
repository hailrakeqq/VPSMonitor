namespace VPSMonitor.API.Entities;

public class SftpRequest
{
    public string Host { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string DirectoryPath { get; set; }
    public string[] SelectedFiles { get; set; }
    public string SourcePath { get; set; }
    public string DestinationPath { get; set; }
    public string ItemPath { get; set; }
    public string NewItemName { get; set; }
    public string NewItemType { get; set; }
}