using System.IO.Compression;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Renci.SshNet;
using VPSMonitor.API.Entities;
using VPSMonitor.API.Repository;

namespace VPSMonitor.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class SftpController : Controller
{
    private readonly ISftpRepository _sftpService;
    private readonly ISshRepository _sshService;
    public SftpController(ISftpRepository sftpService, ISshRepository sshService)
    {
        _sftpService = sftpService;
        _sshService = sshService;
    }

    [HttpPost("list")]
    public IActionResult ListAllFilesAndFolders([FromBody] SftpRequest request)
    {
        using var client = _sftpService.Connect(request.Host, request.Username, request.Password);
        var filesAndFolders = _sftpService.GetAllFilesAndFolders(client, request.DirectoryPath);

        if (filesAndFolders != null)
            return Ok(filesAndFolders);

        return NoContent();
    }

    [HttpPost("upload")]
    public IActionResult UploadFile(IFormCollection form)
    {
        var formData = HttpContext.Request.Form;
        var files = formData.Files;
        string host = formData["host"];
        string username = formData["username"];
        string password = formData["password"];

        using var client = _sftpService.Connect(host, username, password);
        foreach (var file in files)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file selected for upload.");

            string directoryPath = $"{formData["remoteFilePath"]}/{file.FileName}";

            using var stream = file.OpenReadStream();
            _sftpService.UploadItem(client, stream, directoryPath);
        }
        return Ok();
    }

    [HttpPost("download")]
    public async Task<FileContentResult> DownloadFile([FromBody] SftpRequest sftpRequest)
    {
        using var client = _sftpService.Connect(sftpRequest.Host, sftpRequest.Username, sftpRequest.Password);
        using var stream = new MemoryStream();
        if (sftpRequest.SelectedFiles.Length == 1 && Path.HasExtension(sftpRequest.SelectedFiles[0]))
        {
            var file = await _sftpService.DownloadFileAsync(client, sftpRequest.SelectedFiles[0], stream);
            return File(file, "application/octet-stream");
        }

        var zipArchive = await _sftpService.CreateZipArchiveAsync(sftpRequest, client, stream);
        return File(zipArchive, "application/zip");
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateItem([FromBody] SftpRequest sftpRequest)
    {
        using var client = _sftpService.Connect(sftpRequest.Host, sftpRequest.Username, sftpRequest.Password);
        if (sftpRequest.NewItemType == "directory")
        {
            _sftpService.CreateFolder(client, sftpRequest.DirectoryPath, sftpRequest.NewItemName);
            return Ok();
        }

        _sftpService.CreateFile(client, sftpRequest.DirectoryPath, sftpRequest.NewItemName);
        return Ok();
    }

    [HttpPut("rename")]
    public IActionResult RenameFileOrFolder([FromBody] SftpRequest sftpRequest)
    {
        using var client = _sftpService.Connect(sftpRequest.Host, sftpRequest.Username, sftpRequest.Password);
        _sftpService.RenameItem(client, sftpRequest.ItemPath, sftpRequest.NewItemName);
        return Ok();
    }

    [HttpPut("copy")]
    public IActionResult CopyFileOrFolder([FromBody] SftpRequest sftpRequest)
    {
        using var client = _sshService.Connect(sftpRequest.Host, sftpRequest.Username, sftpRequest.Password);
            _sftpService.CopyItem(client, sftpRequest.SourcePath, sftpRequest.DestinationPath);
        return Ok();
    }

    [HttpPut("move")]
    public IActionResult MoveFileOrFolder([FromBody] SftpRequest sftpRequest)
    {
        using var client = _sshService.Connect(sftpRequest.Host, sftpRequest.Username, sftpRequest.Password);
            _sftpService.MoveItem(client, sftpRequest.SourcePath, sftpRequest.DestinationPath);
        return Ok();
    }

    [HttpDelete("delete")]
    public IActionResult DeleteFilesOrFolder([FromBody] SftpRequest sftpRequest)
    {
        using (var client = _sftpService.Connect(sftpRequest.Host, sftpRequest.Username, sftpRequest.Password))
            _sftpService.DeleteItem(client, sftpRequest.SelectedFiles);
        return Ok();
    }

    [HttpGet("exists")]
    public IActionResult FileExists(string path)
    {
        return Ok();
    }

    [HttpPut("permissions")]
    public IActionResult ChangePermissions(string path, string permissions)
    {
        return Ok();
    }
}