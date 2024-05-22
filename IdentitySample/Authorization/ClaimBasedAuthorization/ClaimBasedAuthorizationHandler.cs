using IdentitySample.Authorization.ClaimBasedAuthorization.Utilities;
using IdentitySample.Models;
using IdentitySample.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace IdentitySample.Authorization.ClaimBasedAuthorization
{
    public class ClaimBasedAuthorizationHandler : AuthorizationHandler<ClaimBasedAuthorizationRequirement>
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IClaimBasedAuthorizationUtilities _utilities;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClaimBasedAuthorizationHandler(SignInManager<ApplicationUser> signInManager, IClaimBasedAuthorizationUtilities utilities, IHttpContextAccessor httpContextAccessor)
        {
            _signInManager = signInManager;
            _utilities = utilities;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ClaimBasedAuthorizationRequirement requirement)
        {
            var claimToAuthorize = _utilities.GetClaimToAuthorize(_httpContextAccessor.HttpContext);

            if (string.IsNullOrWhiteSpace(claimToAuthorize))
            {
                context.Succeed(requirement);

                return Task.CompletedTask;
            }

            if (!_signInManager.IsSignedIn(context.User))
            {
                return Task.CompletedTask;
            }

            if (context.User.HasClaim(ClaimStore.UserAccess, claimToAuthorize))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
