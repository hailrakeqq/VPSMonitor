using System.Security.Cryptography;
using System.Text;

namespace VPSMonitor.API;

public static class Toolchain
{
    public static string GenerateHash(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();

            return hash;
        }
    }
}