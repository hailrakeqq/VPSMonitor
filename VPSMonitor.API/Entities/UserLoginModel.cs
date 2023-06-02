using System.ComponentModel.DataAnnotations;

namespace VPSMonitor.API.Entities;

public class UserLoginModel
{
    [Required] public string? Email { get; set; }
    [Required] public string? Password { get; set; }
}

public class UserChangeDataModel
{
    public string? Email { get; set; }
    public string? Password { get; set; }
    [Required] public string? ConfirmPassword { get; set; }
}