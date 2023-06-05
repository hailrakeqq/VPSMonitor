using Microsoft.AspNetCore.Authentication;
using VPSMonitor.API.Entities;
using VPSMonitor.API.Repository;
using VPSMonitor.API.Services;

namespace VPSMonitor.API.Properties;

public static class DIContainer
{
    public static IServiceCollection AddTokenService(this IServiceCollection services)
    {
        return services.AddTransient<ITokenService, TokenService>();
    }

    public static IServiceCollection AddLoginResponse(this IServiceCollection services)
    {
        return services.AddSingleton<LoginResponse>();
    }
    
    public static IServiceCollection AddUserService(this IServiceCollection services)
    {
        return services.AddTransient<IUserRepository, UserService>();
    }

    public static IServiceCollection AddSSHService(this IServiceCollection services)
    {
        return services.AddSingleton<ISshRepository ,SshService>();
    }
}