using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public IActionResult ListAllFilesAndFolders([FromBody] FileSystemRequest request)
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
}



// [HttpGet("download")]
// public async Task<IActionResult> DownloadFile(string remoteFilePath, string localFilePath)
// {
//     _sftpService.DownloadFile(remoteFilePath, localFilePath);
//     return Ok();
// }

// [HttpPut("rename")]
// public IActionResult RenameFileOrFolder(string currentPath, string newName)
// {
//     _sftpService.RenameFileOrFolder(currentPath, newName);
//     return Ok();
// }

// [HttpPost("copy")]
// public IActionResult CopyFileOrFolder(string sourcePath, string destinationPath)
// {
//     _sftpService.CopyFileOrFolder(sourcePath, destinationPath);
//     return Ok();
// }

// [HttpPost("move")]
// public IActionResult MoveFileOrFolder(string sourcePath, string destinationPath)
// {
//     _sftpService.MoveFileOrFolder(sourcePath, destinationPath);
//     return Ok();
// }

// [HttpDelete("delete")]
// public IActionResult DeleteFileOrFolder(string path)
// {
//     _sftpService.DeleteFileOrFolder(path);
//     return Ok();
// }

// [HttpGet("exists")]
// public IActionResult FileExists(string path)
// {
//     bool exists = _sftpService.FileExists(path);
//     return Ok(exists);
// }

// [HttpPut("permissions")]
// public IActionResult ChangePermissions(string path, string permissions)
// {
//     _sftpService.ChangePermissions(path, permissions);
//     return Ok();
// }
