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

    public SftpController(ISftpRepository sftpService)
    {
        _sftpService = sftpService;
    }

    [HttpPost("list")]
    public IActionResult ListAllFilesAndFolders([FromBody] SftpRequest request)
    {
        using (var client = _sftpService.Connect(request.Host, request.Username, request.Password))
        {
            var filesAndFolders = _sftpService.GetAllFilesAndFolders(client, request.DirectoryPath);
            return Ok(filesAndFolders);
        }
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormCollection form)
    {
        var formData = HttpContext.Request.Form;
        var files = formData.Files;
        string host = formData["host"];
        string username = formData["username"];
        string password = formData["password"];

        using (var client = _sftpService.Connect(host, username, password))
        {
            foreach (var file in files)
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file selected for upload.");

                string directoryPath = $"{formData["remoteFilePath"]}/{file.FileName}";

                using (var stream = file.OpenReadStream())
                {
                    _sftpService.UploadFile(client, stream, directoryPath);
                }
            }
            return Ok();
        }
    }

    [HttpPost("download")]
    public async Task<FileContentResult> DownloadFile([FromBody] SftpRequest sftpRequest)
    {
        using (var client = _sftpService.Connect(sftpRequest.Host, sftpRequest.Username, sftpRequest.Password))
        using (var stream = new MemoryStream())
        {
            if (sftpRequest.SelectedFiles.Length == 1 && Path.HasExtension(sftpRequest.SelectedFiles[0]))
            {
                var file = await _sftpService.DownloadFile(client, sftpRequest.SelectedFiles[0], stream);
                return File(file, "application/octet-stream");
            }

            var zipArchive = await _sftpService.CreateZipArchive(sftpRequest, client, stream);
            return File(zipArchive, "application/zip");
        }
    }

    [HttpPut("rename")]
    public IActionResult RenameFileOrFolder(string currentPath, string newName)
    {
        return Ok();
    }

    [HttpPost("copy")]
    public IActionResult CopyFileOrFolder(string sourcePath, string destinationPath)
    {
        return Ok();
    }

    [HttpPost("move")]
    public IActionResult MoveFileOrFolder(string sourcePath, string destinationPath)
    {
        return Ok();
    }

    [HttpDelete("delete")]
    public IActionResult DeleteFileOrFolder(string path)
    {
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