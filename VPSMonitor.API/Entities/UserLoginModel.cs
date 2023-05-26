using System.ComponentModel.DataAnnotations;

namespace VPSMonitor.API.Entities;

public class UserLoginModel
{
    [Required] public string? Email { get; set; }

    [Required] public string? Password { get; set; }
}