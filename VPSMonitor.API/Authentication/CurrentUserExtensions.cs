using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using VPSMonitor.API.Entities;

namespace VPSMonitor.API.Authentication;

public static class CurrentUserExtensions
{
    public static IServiceCollection AddCurrentUser(this IServiceCollection services)
    {
        services.AddScoped<CurrentUser>();
        services.AddScoped<IClaimsTransformation, ClaimsTransformation>();
        return services;
    }

    private sealed class ClaimsTransformation : IClaimsTransformation
    {
        private readonly CurrentUser _currentUser;


        public ClaimsTransformation(CurrentUser currentUser)
        {
            _currentUser = currentUser;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            _currentUser.Principal = principal;

            var loginProvider = principal.FindFirstValue("provider");

            return principal;
        }
    }
}