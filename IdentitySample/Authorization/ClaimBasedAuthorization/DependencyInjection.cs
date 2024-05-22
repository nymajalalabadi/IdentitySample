using IdentitySample.Authorization.ClaimBasedAuthorization.Utilities.MvcNamesUtilities;
using IdentitySample.Authorization.ClaimBasedAuthorization.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace IdentitySample.Authorization.ClaimBasedAuthorization
{
    public static class DependencyInjection
    {
        public static void AddClaimBasedAuthorization(this IServiceCollection service)
        {
            service.AddHttpContextAccessor();
            service.AddSingleton<IClaimBasedAuthorizationUtilities, ClaimBasedAuthorizationUtilities>();
            service.AddSingleton<IMvcUtilities, MvcUtilities>();
            service.AddScoped<IAuthorizationHandler, ClaimBasedAuthorizationHandler>();
            service.AddAuthorization(option =>
            {
                option.AddPolicy("ClaimBasedAuthorization", policy =>
                    policy.Requirements.Add(new ClaimBasedAuthorizationRequirement()));
            });
        }
    }

}
