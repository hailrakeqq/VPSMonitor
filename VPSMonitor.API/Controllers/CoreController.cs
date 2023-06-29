using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VPSMonitor.API.Entities;
using VPSMonitor.API.Repository;
using VPSMonitor.Core;
using VPSMonitor.Core.Entities;

namespace VPSMonitor.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CoreController : Controller
{
    private readonly ISshRepository _sshService;

    public CoreController(ISshRepository sshService)
    {
        _sshService = sshService;
    }

    [HttpPost]
    [Route("ExecuteCommand")]
    public async Task<IActionResult> ExecuteCommand([FromBody] SshRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Host) || string.IsNullOrWhiteSpace(request.Command))
        {
            return BadRequest("Invalid input parameters.");
        }

        try
        {
            var sshClient = _sshService.Connect(request.Host, request.Username, request.Password);

            var response = await _sshService.ExecuteCommandAsync(sshClient, request.Command);
            _sshService.Disconnect(sshClient);

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
        }
    }

    [HttpPost]
    [Route("GetAllDataForMonitoringPage")]
    public async Task<IActionResult> GetDataForMonitoringPage([FromBody] SshRequest request)
    {
        using (var sshClient = _sshService.Connect(request.Host, request.Username, request.Password))
        {
            string[] commands =
            {
                "hostname",
                "cat /etc/os-release | grep -e '^PRETTY_NAME='",
                "uname -r",
                "uname -m",
                "date",
                "mpstat -P ALL -o JSON",
                "free -h",
                "df -h",
                "ip addr show",
                "ip route show default",
                "ip -6 route | grep ^default"
            };

            // string command = string.Join(";", commands);
            // var commandResult = await _sshService.ExecuteCommandAsync(sshClient, command);
            // var commandResults = commandResult.Split('\n');

            var commandTasks = commands.Select(command => Task.Run(() => _sshService.ExecuteCommandAsync(sshClient, command))).ToList();
            await Task.WhenAll(commandTasks);

            var commandResults = commandTasks.Select(task => task.Result).ToList();

            var systemInfo = new SystemInfo()
            {
                Hostname = commandResults[0],
                OS = commandResults[1].Split("\"")[1],
                Kernel = commandResults[2],
                CpuArchitecture = commandResults[3],
                DateTime = commandResults[4]
            };

            var cpuUsageInfo = Parser.mpstatCommandParse(commandResults[5]);
            var ramUsageInfo = commandResults[6];
            var discpartUsageInfo = commandResults[7];
            var networkInfo = Parser.ParseNetworkInfo(commandResults[8], commandResults[9], commandResults[10]);

            var result = new MonitoringPageData()
            {
                SystemInfo = systemInfo,
                CpuUsageInfo = cpuUsageInfo,
                RamUsageInfo = ramUsageInfo,
                DiscpartUsageInfo = discpartUsageInfo,
                NetworkInfo = networkInfo
            };

            return Ok(result);
        }
    }

    [HttpPost]
    [Route("GetSystemInfo")]
    public async Task<IActionResult> GetSystemInfo([FromBody] SshRequest request)
    {
        using (var sshClient = _sshService.Connect(request.Host, request.Username, request.Password))
        {
            string[] commands =
            {
               "hostname",
               "cat /etc/os-release | grep -e '^PRETTY_NAME='",
               "uname -r",
               "uname -m",
               "date",
            };

            var commandTasks = commands.Select(command => _sshService.ExecuteCommandAsync(sshClient, command)).ToList();
            await Task.WhenAll(commandTasks);

            var commandResults = commandTasks.Select(task => task.Result).ToList();

            return Ok(new SystemInfo()
            {
                Hostname = commandResults[0],
                OS = commandResults[1].Split("\"")[1],
                Kernel = commandResults[2],
                CpuArchitecture = commandResults[3],
                DateTime = commandResults[4]
            });
        }
    }

    [HttpPost]
    [Route("GetCpuInfo")]
    public async Task<IActionResult> GetCpuUsageInfo([FromBody] SshRequest request)
    {
        using (var sshClient = _sshService.Connect(request.Host, request.Username, request.Password))
        {
            string result = await _sshService.ExecuteCommandAsync(sshClient, "mpstat -P ALL -o JSON");
            return Ok(Parser.mpstatCommandParse(result));
        }
    }

    [HttpPost]
    [Route("GetRamInfo")]
    public async Task<IActionResult> GetRamUsageInfo([FromBody] SshRequest request)
    {
        using (var sshClient = _sshService.Connect(request.Host, request.Username, request.Password))
        {
            string result = await _sshService.ExecuteCommandAsync(sshClient, "free -h");
            return Ok(result);
        }
    }

    [HttpPost]
    [Route("GetNetworkInfo")]
    public async Task<IActionResult> GetNetworkInfo([FromBody] SshRequest request)
    {
        using (var sshClient = _sshService.Connect(request.Host, request.Username, request.Password))
        {
            var ipAddrCommandOutput = await _sshService.ExecuteCommandAsync(sshClient, "ip addr show");
            var gatewayCommandOutput = await _sshService.ExecuteCommandAsync(sshClient, "ip route show default");
            var gatewayIpV6CommandOutput = await _sshService.ExecuteCommandAsync(sshClient, "ip -6 route | grep ^default");
            var result = Parser.ParseNetworkInfo(ipAddrCommandOutput, gatewayCommandOutput, gatewayIpV6CommandOutput);
            return Ok(result);
        }
    }
}